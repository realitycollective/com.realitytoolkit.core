// Copyright (c) Reality Collective. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using RealityCollective.Extensions;
using RealityCollective.ServiceFramework.Definitions;
using RealityCollective.ServiceFramework.Modules;
using RealityCollective.ServiceFramework.Services;
using RealityToolkit.Definitions.Physics;
using RealityToolkit.EventDatum.Input;
using RealityToolkit.Extensions;
using RealityToolkit.Input.Definitions;
using RealityToolkit.Input.Interactions.Interactors;
using RealityToolkit.Input.Interfaces;
using RealityToolkit.Utilities;
using RealityToolkit.Utilities.Physics;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEvents = UnityEngine.EventSystems;

namespace RealityToolkit.Input.Modules
{
    /// <summary>
    /// The focus provider handles the focused objects per input source.
    /// </summary>
    /// <remarks>There are convenience properties for getting only Gaze Pointer if needed.</remarks>
    [System.Runtime.InteropServices.Guid("249D4D78-CADD-45BA-9438-DB9FC2509213")]
    public class FocusProvider : BaseServiceModule, IFocusProvider
    {
        /// <inheritdoc />
        public FocusProvider(string name, uint priority, BaseProfile profile, IInputService parentService) : base(name, priority, profile, parentService)
        {
            inputService = parentService;

            if (!ServiceManager.Instance.TryGetServiceProfile<IInputService, InputServiceProfile>(out var inputServiceProfile))
            {
                throw new Exception($"Unable to start {name}! An {nameof(InputServiceProfile)} is required for this feature.");
            }

            focusLayerMasks = inputServiceProfile.PointersProfile.PointerRaycastLayerMasks;
            globalPointingExtent = inputServiceProfile.PointersProfile.PointingExtent;
            debugPointingRayColors = inputServiceProfile.PointersProfile.DebugPointingRayColors;
            Raycaster.DebugEnabled = inputServiceProfile.PointersProfile.DrawDebugPointingRays;
        }

        private readonly HashSet<PointerData> pointers = new HashSet<PointerData>();
        private readonly HashSet<GameObject> pendingOverallFocusEnterSet = new HashSet<GameObject>();
        private readonly Dictionary<GameObject, int> pendingOverallFocusExitSet = new Dictionary<GameObject, int>();
        private readonly List<PointerData> pendingPointerSpecificFocusChange = new List<PointerData>();
        private readonly PointerHitResult physicsHitResult = new PointerHitResult();
        private readonly PointerHitResult graphicsHitResult = new PointerHitResult();
        private readonly Color[] debugPointingRayColors;
        private RenderTexture uiRaycastCameraTargetTexture;
        private bool didCreateUIRaycastCamera;

        private IInputService inputService = null;

        protected IInputService InputService
            => inputService ?? (inputService = ServiceManager.Instance.GetService<IInputService>());

        #region IFocusProvider Properties

        /// <inheritdoc />
        public override string Name => "Focus Provider";

        /// <inheritdoc />
        public override uint Priority => 2;

        private readonly float globalPointingExtent;

        /// <inheritdoc />
        float IFocusProvider.GlobalPointingExtent => globalPointingExtent;

        private readonly LayerMask[] focusLayerMasks;

        /// <inheritdoc />
        public LayerMask[] GlobalPointerRaycastLayerMasks => focusLayerMasks;

        private Camera uiRaycastCamera = null;

        /// <inheritdoc />
        public Camera UIRaycastCamera
        {
            get
            {
                if (uiRaycastCamera == null)
                {
                    EnsureUiRaycastCameraSetup();
                }

                return uiRaycastCamera;
            }
            private set => uiRaycastCamera = value;
        }

        #endregion IFocusProvider Properties

        /// <summary>
        /// GazeProvider is a little special, so we keep track of it even if it's not a registered pointer. For the sake
        /// of StabilizationPlaneModifier and potentially other components that care where the user's looking, we need
        /// to do a gaze raycast even if gaze isn't used for focus.
        /// </summary>
        private PointerData gazeProviderPointingData;

        /// <summary>
        /// Cached <see cref="Vector3"/> reference to the new raycast position.
        /// </summary>
        /// <remarks>Only used to update UI raycast results.</remarks>
        private Vector3 newUiRaycastPosition = Vector3.zero;

        [Serializable]
        private class PointerData : IPointerResult, IEquatable<PointerData>
        {
            private const int IGNORE_RAYCAST_LAYER = 2;

            public readonly IInteractor Pointer;

            private FocusDetails focusDetails;

            /// <inheritdoc />
            public Vector3 StartPoint { get; private set; }

            /// <inheritdoc />
            public Vector3 EndPoint => focusDetails.EndPoint;

            /// <inheritdoc />
            public Vector3 EndPointLocalSpace => focusDetails.EndPointLocalSpace;

            /// <inheritdoc />
            public GameObject CurrentPointerTarget { get; private set; }

            private GameObject syncedPointerTarget;

            /// <inheritdoc />
            public GameObject PreviousPointerTarget { get; private set; }

            /// <inheritdoc />
            public GameObject LastHitObject => focusDetails.HitObject;

            /// <inheritdoc />
            public int RayStepIndex { get; private set; }

            /// <inheritdoc />
            public float RayDistance => focusDetails.RayDistance;

            /// <inheritdoc />
            public Vector3 Normal => focusDetails.Normal;

            /// <inheritdoc />
            public Vector3 NormalLocalSpace => focusDetails.NormalLocalSpace;

            /// <inheritdoc />
            public Vector3 Direction { get; private set; }

            /// <inheritdoc />
            public RaycastHit LastRaycastHit => focusDetails.LastRaycastHit;

            /// <inheritdoc />
            public UnityEvents.RaycastResult LastGraphicsRaycastResult => focusDetails.LastGraphicsRaycastResult;

            /// <inheritdoc />
            public Vector3 GrabPointLocalSpace { get; private set; }

            /// <inheritdoc />
            public Vector3 GrabPoint { get; private set; }

            /// <summary>
            /// The graphic input event data used for raycasting uGUI elements.
            /// </summary>
            public GraphicInputEventData GraphicEventData
            {
                get
                {
                    if (graphicData == null)
                    {
                        graphicData = new GraphicInputEventData(UnityEvents.EventSystem.current);
                        graphicData.Clear();
                    }

                    Debug.Assert(graphicData != null);

                    return graphicData;
                }
            }

            private GraphicInputEventData graphicData;
            private int prevPhysicsLayer;
            private Vector3 lastPosition;

            /// <summary>
            /// Constructor.
            /// </summary>
            /// <param name="pointer"></param>
            public PointerData(IInteractor pointer)
            {
                focusDetails = new FocusDetails();
                Pointer = pointer;
            }

            public void UpdateHit(PointerHitResult hitResult, GameObject syncTarget)
            {
                focusDetails.LastRaycastHit = hitResult.RaycastHit;
                focusDetails.LastGraphicsRaycastResult = hitResult.GraphicsRaycastResult;

                if (hitResult.RayStepIndex >= 0)
                {
                    RayStepIndex = hitResult.RayStepIndex;
                    StartPoint = hitResult.Ray.Origin;

                    focusDetails.RayDistance = hitResult.RayDistance;
                    focusDetails.EndPoint = hitResult.HitPointOnObject;
                    focusDetails.Normal = hitResult.HitNormalOnObject;
                }
                else
                {
                    // If we don't have a valid ray cast, use the whole pointer ray.focusDetails.EndPoint
                    var firstStep = Pointer.Rays[0];
                    var finalStep = Pointer.Rays[Pointer.Rays.Length - 1];
                    RayStepIndex = 0;

                    StartPoint = firstStep.Origin;

                    var rayDistance = 0.0f;

                    for (int i = 0; i < Pointer.Rays.Length; i++)
                    {
                        rayDistance += Pointer.Rays[i].Length;
                    }

                    focusDetails.RayDistance = rayDistance;
                    focusDetails.EndPoint = finalStep.Terminus;
                    focusDetails.Normal = -finalStep.Direction;
                }

                Direction = focusDetails.EndPoint - lastPosition;
                lastPosition = focusDetails.EndPoint;

                focusDetails.HitObject = hitResult.HitObject;

                if (syncTarget != null)
                {
                    if (syncedPointerTarget == null && CurrentPointerTarget != null && CurrentPointerTarget == syncTarget)
                    {
                        Debug.Assert(CurrentPointerTarget != null, "No Sync Target Set!");

                        syncedPointerTarget = CurrentPointerTarget;

                        prevPhysicsLayer = CurrentPointerTarget.layer;
                        Debug.Assert(prevPhysicsLayer != IGNORE_RAYCAST_LAYER, $"Failed to get a valid raycast layer for {syncedPointerTarget.name}: {LayerMask.LayerToName(prevPhysicsLayer)}");
                        CurrentPointerTarget.SetLayerRecursively(IGNORE_RAYCAST_LAYER);

                        var grabPoint = Pointer.OverrideGrabPoint ?? focusDetails.EndPoint;

                        if (grabPoint == Vector3.zero)
                        {
                            GrabPoint = CurrentPointerTarget.transform.TransformPoint(grabPoint);
                            GrabPointLocalSpace = CurrentPointerTarget.transform.InverseTransformPoint(GrabPoint);
                        }
                        else
                        {
                            GrabPoint = grabPoint;
                            GrabPointLocalSpace = CurrentPointerTarget.transform.InverseTransformPoint(GrabPoint);
                        }
                    }
                    else if (syncTarget != CurrentPointerTarget)
                    {
                        GetCurrentTarget();
                    }

                    if (syncedPointerTarget != null)
                    {
                        GrabPoint = CurrentPointerTarget.transform.TransformPoint(GrabPointLocalSpace);
                        GrabPointLocalSpace = CurrentPointerTarget.transform.InverseTransformPoint(GrabPoint);

                        // Visualize the relevant points and their relation
                        if (Application.isEditor && Raycaster.DebugEnabled)
                        {
                            DebugUtilities.DrawPoint(GrabPoint, Color.red);
                            DebugUtilities.DrawPoint(focusDetails.EndPoint, Color.yellow);

                            Debug.DrawLine(focusDetails.EndPoint, GrabPoint, Color.magenta);

                            var currentPosition = CurrentPointerTarget.transform.position;
                            var targetPosition = (focusDetails.EndPoint + currentPosition) - GrabPoint;

                            Debug.DrawLine(GrabPoint, currentPosition, Color.magenta);
                            Debug.DrawLine(currentPosition, GrabPoint, Color.magenta);
                            DebugUtilities.DrawPoint(currentPosition, Color.cyan);
                            DebugUtilities.DrawPoint(targetPosition, Color.blue);

                            Debug.DrawLine(targetPosition, currentPosition, Color.blue);
                        }
                    }
                }
                else
                {
                    GetCurrentTarget();
                }

                void GetCurrentTarget()
                {
                    if (syncedPointerTarget != null)
                    {
                        syncedPointerTarget.SetLayerRecursively(prevPhysicsLayer);
                        syncedPointerTarget = null;
                    }

                    PreviousPointerTarget = CurrentPointerTarget;
                    CurrentPointerTarget = focusDetails.HitObject;
                    Pointer.OverrideGrabPoint = null;
                    GrabPoint = Vector3.zero;
                    GrabPointLocalSpace = Vector3.zero;
                }

                if (CurrentPointerTarget != null)
                {
                    focusDetails.EndPointLocalSpace = CurrentPointerTarget.transform.InverseTransformPoint(focusDetails.EndPoint);
                    focusDetails.NormalLocalSpace = CurrentPointerTarget.transform.InverseTransformDirection(focusDetails.Normal);
                }
                else
                {
                    focusDetails.EndPointLocalSpace = Vector3.zero;
                    focusDetails.NormalLocalSpace = Vector3.zero;
                }
            }

            /// <summary>
            /// Update focus information while focus is locked. If the object is moving,
            /// this updates the hit point to its new world transform.
            /// </summary>
            public void UpdateFocusLockedHit()
            {
                if (focusDetails.HitObject != null)
                {
                    // In case the focused object is moving, we need to update the focus point based on the object's new transform.
                    focusDetails.EndPoint = focusDetails.HitObject.transform.TransformPoint(focusDetails.EndPointLocalSpace);
                    focusDetails.Normal = focusDetails.HitObject.transform.TransformDirection(focusDetails.NormalLocalSpace);

                    focusDetails.EndPointLocalSpace = focusDetails.HitObject.transform.InverseTransformPoint(focusDetails.EndPoint);
                    focusDetails.NormalLocalSpace = focusDetails.HitObject.transform.InverseTransformDirection(focusDetails.Normal);
                }

                StartPoint = Pointer.Rays[0].Origin;

                for (int i = 0; i < Pointer.Rays.Length; i++)
                {
                    // TODO: figure out how reliable this is. Should focusDetails.RayDistance be updated?
                    if (Pointer.Rays[i].Contains(focusDetails.EndPoint))
                    {
                        RayStepIndex = i;
                        break;
                    }
                }
            }

            /// <summary>
            /// Reset the currently focused object data.
            /// </summary>
            /// <param name="clearPreviousObject">Optional flag to choose not to clear the previous object.</param>
            public void ResetFocusedObjects(bool clearPreviousObject = true)
            {
                PreviousPointerTarget = clearPreviousObject ? null : CurrentPointerTarget;

                focusDetails.EndPoint = focusDetails.EndPoint;
                focusDetails.EndPointLocalSpace = focusDetails.EndPointLocalSpace;
                focusDetails.Normal = focusDetails.Normal;
                focusDetails.NormalLocalSpace = focusDetails.NormalLocalSpace;
                focusDetails.HitObject = null;
            }

            /// <inheritdoc />
            public bool Equals(PointerData other)
            {
                if (other is null) { return false; }
                if (ReferenceEquals(this, other)) { return true; }
                return Pointer.PointerId == other.Pointer.PointerId;
            }

            /// <inheritdoc />
            public override bool Equals(object obj)
            {
                if (obj is null) { return false; }
                if (ReferenceEquals(this, obj)) { return true; }
                return obj is PointerData pointer && Equals(pointer);
            }

            /// <inheritdoc />
            public override int GetHashCode() => Pointer != null ? Pointer.GetHashCode() : 0;
        }

        /// <summary>
        /// Helper class for storing intermediate hit results. Should be applied to the PointerData once all
        /// possible hits of a pointer have been processed.
        /// </summary>
        private class PointerHitResult
        {
            public RaycastHit RaycastHit;
            public UnityEvents.RaycastResult GraphicsRaycastResult;

            public GameObject HitObject;
            public Vector3 HitPointOnObject;
            public Vector3 HitNormalOnObject;

            public RayStep Ray;
            public int RayStepIndex = -1;
            public float RayDistance;

            /// <summary>
            /// Clears the pointer result.
            /// </summary>
            public void Clear()
            {
                RaycastHit = default;
                GraphicsRaycastResult = default;

                HitObject = null;
                HitPointOnObject = Vector3.zero;
                HitNormalOnObject = Vector3.zero;

                Ray = default;
                RayStepIndex = -1;
                RayDistance = 0.0f;
            }

            /// <summary>
            /// Set hit focus information from a closest-collider-to pointer check.
            /// </summary>
            public void Set(GameObject hitObject, Vector3 hitPointOnObject, Vector4 hitNormalOnObject, RayStep ray, int rayStepIndex, float rayDistance)
            {
                RaycastHit = default;
                GraphicsRaycastResult = default;

                HitObject = hitObject;
                HitPointOnObject = hitPointOnObject;
                HitNormalOnObject = hitNormalOnObject;

                Ray = ray;
                RayStepIndex = rayStepIndex;
                RayDistance = rayDistance;
            }

            /// <summary>
            /// Set hit focus information from a physics raycast.
            /// </summary>
            public void Set(RaycastHit hit, RayStep ray, int rayStepIndex, float rayDistance)
            {
                RaycastHit = hit;
                GraphicsRaycastResult = default;

                HitObject = hit.transform.gameObject;
                HitPointOnObject = hit.point;
                HitNormalOnObject = hit.normal;

                Ray = ray;
                RayStepIndex = rayStepIndex;
                RayDistance = rayDistance;
            }

            /// <summary>
            /// Set hit information from a canvas raycast.
            /// </summary>
            public void Set(UnityEvents.RaycastResult result, Vector3 hitPointOnObject, Vector4 hitNormalOnObject, RayStep ray, int rayStepIndex, float rayDistance)
            {
                RaycastHit = default;
                GraphicsRaycastResult = result;

                HitObject = result.gameObject;
                HitPointOnObject = hitPointOnObject;
                HitNormalOnObject = hitNormalOnObject;

                Ray = ray;
                RayStepIndex = rayStepIndex;
                RayDistance = rayDistance;
            }
        }

        #region IService Implementation

        /// <inheritdoc />
        public override void Initialize()
        {
            base.Initialize();

            foreach (var inputSource in InputService.DetectedInputSources)
            {
                RegisterPointers(inputSource);
            }
        }

        /// <inheritdoc />
        public override void Update()
        {
            base.Update();

            UpdatePointers();
            UpdateFocusedObjects();
        }

        /// <inheritdoc />
        public override void Destroy()
        {
            base.Destroy();
            CleanUpUiRaycastCamera();
        }

        #endregion IService Implementation

        #region Focus Details by IPointer

        /// <inheritdoc />
        public GameObject GetFocusedObject(IInteractor pointingSource)
        {
            if (pointingSource == null)
            {
                Debug.LogError("No Pointer passed to get focused object");
                return null;
            }

            return !TryGetFocusDetails(pointingSource, out var focusDetails) ? null : focusDetails.CurrentPointerTarget;
        }

        /// <inheritdoc />
        public bool TryGetFocusDetails(IInteractor pointer, out IPointerResult focusDetails)
        {
            if (TryGetPointerData(pointer, out var pointerData))
            {
                focusDetails = pointerData;
                return true;
            }

            focusDetails = default;
            return false;
        }

        /// <inheritdoc />
        public bool TryGetSpecificPointerGraphicEventData(IInteractor pointer, out GraphicInputEventData graphicInputEventData)
        {
            if (TryGetPointerData(pointer, out var pointerData))
            {
                Debug.Assert(pointerData.GraphicEventData != null);
                graphicInputEventData = pointerData.GraphicEventData;
                graphicInputEventData.selectedObject = pointerData.GraphicEventData.pointerCurrentRaycast.gameObject;
                return true;
            }

            graphicInputEventData = null;
            return false;
        }
        #endregion Focus Details by IPointer

        #region Utilities

        /// <inheritdoc />
        public uint GenerateNewPointerId()
        {
            var newId = (uint)UnityEngine.Random.Range(1, int.MaxValue);

            foreach (var pointerData in pointers)
            {
                if (pointerData.Pointer.PointerId == newId)
                {
                    return GenerateNewPointerId();
                }
            }

            return newId;
        }

        /// <summary>
        /// Utility for validating the UIRaycastCamera.
        /// </summary>
        /// <returns>The UIRaycastCamera</returns>
        private void EnsureUiRaycastCameraSetup()
        {
            const string uiRayCastCameraName = "UIRaycastCamera";

            if (Camera.main.IsNull())
            {
                // The main camera is not available yet, so we cannot init the raycat camera
                // at this time. The get-accessor of the UIRaycastCamera property will ensure
                // it is set up at a later time when accessed.
                return;
            }

            GameObject cameraObject;

            var existingUiRaycastCameraObject = GameObject.Find(uiRayCastCameraName);
            if (existingUiRaycastCameraObject != null)
            {
                cameraObject = existingUiRaycastCameraObject;
            }
            else
            {
                cameraObject = new GameObject { name = uiRayCastCameraName };
                cameraObject.transform.SetParent(Camera.main.transform, false);
                didCreateUIRaycastCamera = true;
            }

            uiRaycastCamera = cameraObject.EnsureComponent<Camera>();
            uiRaycastCamera.enabled = false;
            uiRaycastCamera.clearFlags = CameraClearFlags.Color;
            uiRaycastCamera.backgroundColor = new Color(0, 0, 0, 1);
            uiRaycastCamera.orthographic = true;
            uiRaycastCamera.orthographicSize = 0.5f;
            uiRaycastCamera.nearClipPlane = 0.0f;
            uiRaycastCamera.farClipPlane = 1000f;
            uiRaycastCamera.rect = new Rect(0, 0, 1, 1);
            uiRaycastCamera.depth = 0;
            uiRaycastCamera.renderingPath = RenderingPath.UsePlayerSettings;
            uiRaycastCamera.useOcclusionCulling = false;
            uiRaycastCamera.allowHDR = false;
            uiRaycastCamera.allowMSAA = false;
            uiRaycastCamera.allowDynamicResolution = false;
            uiRaycastCamera.targetDisplay = 0;
            uiRaycastCamera.stereoTargetEye = StereoTargetEyeMask.Both;
            uiRaycastCamera.cullingMask = Camera.main.cullingMask;

            if (uiRaycastCameraTargetTexture == null)
            {
                // Set target texture to specific pixel size so that drag thresholds are treated the same regardless of underlying
                // device display resolution.
                uiRaycastCameraTargetTexture = new RenderTexture(128, 128, 0);
            }

            uiRaycastCamera.targetTexture = uiRaycastCameraTargetTexture;
        }

        private void CleanUpUiRaycastCamera()
        {
            if (uiRaycastCameraTargetTexture != null)
            {
                uiRaycastCameraTargetTexture.Destroy();
            }

            uiRaycastCameraTargetTexture = null;

            if (didCreateUIRaycastCamera && UIRaycastCamera.gameObject.IsNotNull())
            {
                UIRaycastCamera.gameObject.Destroy();
            }

            UIRaycastCamera = null;
        }

        /// <inheritdoc />
        public bool IsPointerRegistered(IInteractor pointer)
        {
            Debug.Assert(pointer.PointerId != 0, $"{pointer} does not have a valid pointer id!");
            return TryGetPointerData(pointer, out _);
        }

        /// <inheritdoc />
        public bool RegisterPointer(IInteractor pointer)
        {
            Debug.Assert(pointer.PointerId != 0, $"{pointer} does not have a valid pointer id!");

            if (IsPointerRegistered(pointer)) { return false; }

            var pointerData = new PointerData(pointer);
            pointers.Add(pointerData);
            // Initialize the pointer result
            UpdatePointer(pointerData);
            return true;
        }

        private void RegisterPointers(IInputSource inputSource)
        {
            // If our input source does not have any pointers, then skip.
            if (inputSource.Pointers == null) { return; }

            for (int i = 0; i < inputSource.Pointers.Length; i++)
            {
                var pointer = inputSource.Pointers[i];
                RegisterPointer(pointer);

                // Special Registration for Gaze
                if (InputService.GazeProvider != null &&
                    inputSource.SourceId == InputService.GazeProvider.GazeInputSource.SourceId &&
                    gazeProviderPointingData == null)
                {
                    gazeProviderPointingData = new PointerData(pointer);
                }
            }
        }

        /// <inheritdoc />
        public bool UnregisterPointer(IInteractor pointer)
        {
            Debug.Assert(pointer.PointerId != 0, $"{pointer} does not have a valid pointer id!");

            if (!TryGetPointerData(pointer, out var pointerData)) { return false; }

            // Raise focus events if needed.
            if (pointerData.CurrentPointerTarget != null)
            {
                var unfocusedObject = pointerData.CurrentPointerTarget;
                var objectIsStillFocusedByOtherPointer = false;

                foreach (var otherPointer in pointers)
                {
                    if (otherPointer.Pointer.PointerId != pointer.PointerId &&
                        otherPointer.CurrentPointerTarget == unfocusedObject)
                    {
                        objectIsStillFocusedByOtherPointer = true;
                        break;
                    }
                }

                if (!objectIsStillFocusedByOtherPointer)
                {
                    // Policy: only raise focus exit if no other pointers are still focusing the object
                    InputService.RaiseFocusExit(pointer, unfocusedObject);
                }

                InputService.RaisePreFocusChanged(pointer, unfocusedObject, null);
            }

            pointers.Remove(pointerData);
            return true;
        }

        /// <summary>
        /// Returns the registered PointerData for the provided pointing input source.
        /// </summary>
        /// <param name="pointer">the pointer who's data we're looking for</param>
        /// <param name="data">The data associated to the pointer</param>
        /// <returns>Pointer Data if the pointing source is registered.</returns>
        private bool TryGetPointerData(IInteractor pointer, out PointerData data)
        {
            foreach (var pointerData in pointers)
            {
                if (pointerData.Pointer.PointerId == pointer.PointerId)
                {
                    data = pointerData;
                    return true;
                }
            }

            data = null;
            return false;
        }

        private void UpdatePointers()
        {
            int pointerCount = 0;

            foreach (var pointer in pointers)
            {
                UpdatePointer(pointer);

                if (Application.isEditor && Raycaster.DebugEnabled)
                {
                    Color debugPointingRayColor;

                    if (debugPointingRayColors != null &&
                        debugPointingRayColors.Length > 0)
                    {
                        debugPointingRayColor = debugPointingRayColors[pointerCount++ % debugPointingRayColors.Length];
                    }
                    else
                    {
                        debugPointingRayColor = Color.green;
                    }

                    Debug.DrawRay(pointer.StartPoint, (pointer.EndPoint - pointer.StartPoint), debugPointingRayColor);
                }
            }
        }

        private void UpdatePointer(PointerData pointer)
        {
            if (pointer.Pointer.IsFarInteractor)
            {
                UpdateFarPointer(pointer);
                return;
            }

            UpdateNearPointer(pointer);
        }

        private void UpdateNearPointer(PointerData pointer)
        {

        }

        private void UpdateFarPointer(PointerData pointer)
        {
            // Call the pointer's OnPreRaycast function
            // This will give it a chance to prepare itself for raycasts
            // eg, by building its Rays array
            pointer.Pointer.OnPreRaycast();

            // If pointer interaction isn't enabled, clear its result object and return
            if (!pointer.Pointer.IsInteractionEnabled)
            {
                // Don't clear the previous focused object since we still want to trigger FocusExit events
                pointer.ResetFocusedObjects(false);

                // Only set the result if it's null
                // Otherwise we'd get incorrect data.
                if (pointer.Pointer.Result == null)
                {
                    pointer.Pointer.Result = pointer;
                }
            }
            else
            {
                // If the pointer is locked, keep the focused object the same.
                // This will ensure that we execute events on those objects
                // even if the pointer isn't pointing at them.
                // We don't want to update focused locked hits if we're syncing the pointer's target position.
                if (pointer.Pointer.IsFocusLocked && pointer.Pointer.SyncedTarget == null)
                {
                    pointer.UpdateFocusLockedHit();
                }
                else
                {
                    // Otherwise, continue
                    var prioritizedLayerMasks = (pointer.Pointer.PointerRaycastLayerMasksOverride ?? GlobalPointerRaycastLayerMasks);

                    physicsHitResult.Clear();

                    // Perform raycast to determine focused object
                    RaycastPhysics(pointer.Pointer, prioritizedLayerMasks, physicsHitResult);
                    var currentHitResult = physicsHitResult;

                    // If we have a unity event system, perform graphics raycasts as well to support Unity UI interactions
                    if (UnityEvents.EventSystem.current != null)
                    {
                        graphicsHitResult.Clear();
                        // NOTE: We need to do this AFTER RaycastPhysics so we use the current hit point to perform the correct 2D UI Raycast.
                        RaycastGraphics(pointer.Pointer, pointer.GraphicEventData, prioritizedLayerMasks, graphicsHitResult);

                        currentHitResult = GetPrioritizedHitResult(currentHitResult, graphicsHitResult, prioritizedLayerMasks);
                    }

                    // Apply the hit result only now so changes in the current target are detected only once per frame.
                    pointer.UpdateHit(currentHitResult, pointer.Pointer.SyncedTarget);
                }

                // Set the pointer's result last
                pointer.Pointer.Result = pointer;
            }

            Debug.Assert(pointer.Pointer.Result != null);

            // Call the pointer's OnPostRaycast function
            // This will give it a chance to respond to raycast results
            // eg by updating its appearance
            pointer.Pointer.OnPostRaycast();
        }

        #region Physics Raycasting

        private static PointerHitResult GetPrioritizedHitResult(PointerHitResult hit1, PointerHitResult hit2, LayerMask[] prioritizedLayerMasks)
        {
            if (hit1.HitObject != null && hit2.HitObject != null)
            {
                // Check layer prioritization.
                if (prioritizedLayerMasks.Length > 1)
                {
                    // Get the index in the prioritized layer masks
                    int layerIndex1 = hit1.HitObject.layer.FindLayerListIndex(prioritizedLayerMasks);
                    int layerIndex2 = hit2.HitObject.layer.FindLayerListIndex(prioritizedLayerMasks);

                    if (layerIndex1 != layerIndex2)
                    {
                        return (layerIndex1 < layerIndex2) ? hit1 : hit2;
                    }
                }

                // Check which hit is closer.
                return hit1.RayDistance < hit2.RayDistance ? hit1 : hit2;
            }

            return hit1.HitObject != null ? hit1 : hit2;
        }

        /// <summary>
        /// Perform a Unity physics Raycast to determine which scene objects with a collider is currently being gazed at, if any.
        /// </summary>
        /// <param name="pointer"></param>
        /// <param name="prioritizedLayerMasks"></param>
        /// <param name="hitResult"></param>
        private static void RaycastPhysics(IInteractor pointer, LayerMask[] prioritizedLayerMasks, PointerHitResult hitResult)
        {
            float rayStartDistance = 0;
            var pointerRays = pointer.Rays;

            if (pointerRays == null)
            {
                Debug.LogError($"No valid rays for {pointer.PointerName} pointer.");
                return;
            }

            if (pointerRays.Length <= 0)
            {
                Debug.LogError($"No valid rays for {pointer.PointerName} pointer");
                return;
            }

            // Check raycast for each step in the pointing source
            for (int i = 0; i < pointerRays.Length; i++)
            {
                switch (pointer.RaycastMode)
                {
                    case RaycastMode.Simple:
                        if (Raycaster.RaycastSimplePhysicsStep(pointerRays[i], prioritizedLayerMasks, out var simplePhysicsHit))
                        {
                            // Set the pointer source's origin ray to this step
                            UpdatePointerRayOnHit(pointerRays, simplePhysicsHit, i, rayStartDistance, hitResult);
                            return;
                        }
                        break;
                    case RaycastMode.Box:
                        // TODO box raycast mode
                        Debug.LogWarning("Box Raycasting Mode not supported for pointers.");
                        break;
                    case RaycastMode.Sphere:
                        if (Raycaster.RaycastSpherePhysicsStep(pointerRays[i], pointer.SphereCastRadius, prioritizedLayerMasks, out var spherePhysicsHit))
                        {
                            // Set the pointer source's origin ray to this step
                            UpdatePointerRayOnHit(pointerRays, spherePhysicsHit, i, rayStartDistance, hitResult);
                            return;
                        }
                        break;
                    // TODO Sphere Overlap
                    default:
                        Debug.LogError($"Invalid raycast mode {pointer.RaycastMode} for {pointer.PointerName} pointer.");
                        break;
                }

                rayStartDistance += pointer.Rays[i].Length;
            }
        }

        private static void UpdatePointerRayOnHit(RayStep[] raySteps, RaycastHit physicsHit, int hitRayIndex, float rayStartDistance, PointerHitResult hitResult)
        {
            var origin = raySteps[hitRayIndex].Origin;
            var terminus = physicsHit.point;
            raySteps[hitRayIndex].UpdateRayStep(ref origin, ref terminus);
            hitResult.Set(physicsHit, raySteps[hitRayIndex], hitRayIndex, rayStartDistance + physicsHit.distance);
        }

        #endregion Physics Raycasting

        #region uGUI Graphics Raycasting

        /// <summary>
        /// Perform a Unity Graphics Raycast to determine which uGUI element is currently being gazed at, if any.
        /// </summary>
        /// <param name="pointer"></param>
        /// <param name="graphicEventData"></param>
        /// <param name="prioritizedLayerMasks"></param>
        /// <param name="hitResult"></param>
        private void RaycastGraphics(IInteractor pointer, UnityEvents.PointerEventData graphicEventData, LayerMask[] prioritizedLayerMasks, PointerHitResult hitResult)
        {
            Debug.Assert(UIRaycastCamera != null, "Missing UIRaycastCamera!");
            Debug.Assert(UIRaycastCamera.nearClipPlane == 0, "Near plane must be zero for raycast distances to be correct");

            if (pointer.Rays == null || pointer.Rays.Length <= 0)
            {
                Debug.LogError($"No valid rays for {pointer.PointerName} pointer.");
                return;
            }

            // Cast rays for every step until we score a hit
            var totalDistance = 0.0f;
            for (int i = 0; i < pointer.Rays.Length; i++)
            {

                UnityEvents.RaycastResult raycastResult;
                if (RaycastGraphicsStep(graphicEventData, pointer.Rays[i], prioritizedLayerMasks, out raycastResult) &&
                        raycastResult.isValid &&
                        raycastResult.distance < pointer.Rays[i].Length &&
                        raycastResult.module != null &&
                        raycastResult.module.eventCamera == UIRaycastCamera)
                {
                    totalDistance += raycastResult.distance;

                    newUiRaycastPosition.x = raycastResult.screenPosition.x;
                    newUiRaycastPosition.y = raycastResult.screenPosition.y;
                    newUiRaycastPosition.z = raycastResult.distance;

                    Vector3 worldPos = UIRaycastCamera.ScreenToWorldPoint(newUiRaycastPosition);
                    Vector3 normal = -raycastResult.gameObject.transform.forward;

                    hitResult.Set(raycastResult, worldPos, normal, pointer.Rays[i], i, totalDistance);
                    return;
                }

                totalDistance += pointer.Rays[i].Length;
            }
        }

        /// <summary>
        /// Raycasts each graphic <see cref="RayStep"/>
        /// </summary>
        /// <param name="graphicEventData"></param>
        /// <param name="step"></param>
        /// <param name="prioritizedLayerMasks"></param>
        /// <param name="uiRaycastResult"></param>
        private bool RaycastGraphicsStep(UnityEvents.PointerEventData graphicEventData, RayStep step, LayerMask[] prioritizedLayerMasks, out UnityEvents.RaycastResult uiRaycastResult)
        {
            uiRaycastResult = default;
            var currentEventSystem = UnityEvents.EventSystem.current;

            if (currentEventSystem == null)
            {
                Debug.LogError("Current Event System is Invalid!");
                return false;
            }

            if (step.Direction == Vector3.zero)
            {
                Debug.LogError("RayStep Direction is Invalid!");
                return false;
            }

            // Move the uiRaycast camera to the current pointer's position.
            UIRaycastCamera.transform.position = step.Origin;
            UIRaycastCamera.transform.rotation = Quaternion.LookRotation(step.Direction, Vector3.up);

            // We always raycast from the center of the camera.
            var newPosition = graphicRaycastMultiplier;
            newPosition.x *= UIRaycastCamera.pixelWidth;
            newPosition.y *= UIRaycastCamera.pixelHeight;
            graphicEventData.position = newPosition;

            // Graphics raycast
            uiRaycastResult = currentEventSystem.Raycast(graphicEventData, prioritizedLayerMasks);
            graphicEventData.pointerCurrentRaycast = uiRaycastResult;

            return UIRaycastCamera.gameObject != null;
        }

        private readonly Vector2 graphicRaycastMultiplier = new Vector2(0.5f, 0.5f);

        #endregion uGUI Graphics Raycasting

        /// <summary>
        /// Raises the Focus Events to the Input Manger if needed.
        /// </summary>
        private void UpdateFocusedObjects()
        {
            Debug.Assert(pendingPointerSpecificFocusChange.Count == 0);
            Debug.Assert(pendingOverallFocusExitSet.Count == 0);
            Debug.Assert(pendingOverallFocusEnterSet.Count == 0);

            // NOTE: We compute the set of events to send before sending the first event
            //       just in case someone responds to the event by adding/removing a
            //       pointer which would change the structures we're iterating over.

            foreach (var pointer in pointers)
            {
                if (pointer.PreviousPointerTarget != pointer.CurrentPointerTarget)
                {
                    pendingPointerSpecificFocusChange.Add(pointer);

                    // Initially, we assume all pointer-specific focus changes will
                    // also result in an overall focus change...

                    if (pointer.PreviousPointerTarget != null)
                    {
                        int numExits;
                        if (pendingOverallFocusExitSet.TryGetValue(pointer.PreviousPointerTarget, out numExits))
                        {
                            pendingOverallFocusExitSet[pointer.PreviousPointerTarget] = numExits + 1;
                        }
                        else
                        {
                            pendingOverallFocusExitSet.Add(pointer.PreviousPointerTarget, 1);
                        }
                    }

                    if (pointer.CurrentPointerTarget != null)
                    {
                        pendingOverallFocusEnterSet.Add(pointer.CurrentPointerTarget);
                    }
                }
            }

            // Early out if there have been no focus changes
            if (pendingPointerSpecificFocusChange.Count == 0)
            {
                return;
            }

            // ... but now we trim out objects whose overall focus was maintained the same by a different pointer:

            foreach (var pointer in pointers)
            {
                if (pointer.CurrentPointerTarget != null)
                {
                    pendingOverallFocusExitSet.Remove(pointer.CurrentPointerTarget);
                }
                pendingOverallFocusEnterSet.Remove(pointer.PreviousPointerTarget);
            }

            // Now we raise the events:
            for (int iChange = 0; iChange < pendingPointerSpecificFocusChange.Count; iChange++)
            {
                PointerData change = pendingPointerSpecificFocusChange[iChange];
                GameObject pendingUnfocusObject = change.PreviousPointerTarget;
                GameObject pendingFocusObject = change.CurrentPointerTarget;

                InputService?.RaisePreFocusChanged(change.Pointer, pendingUnfocusObject, pendingFocusObject);

                if (pendingUnfocusObject != null && pendingOverallFocusExitSet.TryGetValue(pendingUnfocusObject, out int numExits))
                {
                    if (numExits > 1)
                    {
                        pendingOverallFocusExitSet[pendingUnfocusObject] = numExits - 1;
                    }
                    else
                    {
                        InputService?.RaiseFocusExit(change.Pointer, pendingUnfocusObject);
                        pendingOverallFocusExitSet.Remove(pendingUnfocusObject);
                    }
                }

                if (pendingOverallFocusEnterSet.Contains(pendingFocusObject))
                {
                    InputService?.RaiseFocusEnter(change.Pointer, pendingFocusObject);
                    pendingOverallFocusEnterSet.Remove(pendingFocusObject);
                }

                InputService?.RaiseFocusChanged(change.Pointer, pendingUnfocusObject, pendingFocusObject);
            }

            Debug.Assert(pendingOverallFocusExitSet.Count == 0);
            Debug.Assert(pendingOverallFocusEnterSet.Count == 0);
            pendingPointerSpecificFocusChange.Clear();
        }

        #endregion Accessors

        #region ISourceState Implementation

        /// <inheritdoc />
        public void OnSourceDetected(SourceStateEventData eventData)
        {
            RegisterPointers(eventData.InputSource);
        }

        /// <inheritdoc />
        public void OnSourceLost(SourceStateEventData eventData)
        {
            // If the input source does not have pointers, then skip.
            if (eventData.InputSource.Pointers == null) { return; }

            for (var i = 0; i < eventData.InputSource.Pointers.Length; i++)
            {
                // Special unregistration for Gaze
                if (gazeProviderPointingData != null && eventData.InputSource.Pointers[i].PointerId == gazeProviderPointingData.Pointer.PointerId)
                {
                    // If the source lost is the gaze input source, then reset it.
                    if (eventData.InputSource.SourceId == InputService.GazeProvider.GazeInputSource.SourceId)
                    {
                        gazeProviderPointingData.ResetFocusedObjects();
                        gazeProviderPointingData = null;
                    }
                    // Otherwise, don't unregister the gaze pointer, since the gaze input source is still active.
                    else
                    {
                        continue;
                    }
                }

                UnregisterPointer(eventData.InputSource.Pointers[i]);
            }
        }

        #endregion ISourceState Implementation
    }
}