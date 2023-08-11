// Copyright (c) Reality Collective. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using RealityCollective.Extensions;
using RealityCollective.ServiceFramework.Services;
using RealityToolkit.EventDatum.Input;
using RealityToolkit.Input.Definitions;
using RealityToolkit.Input.InputSources;
using RealityToolkit.Input.Interactors;
using RealityToolkit.Input.Interfaces;
using RealityToolkit.Input.Interfaces.Controllers;
using RealityToolkit.Input.Interfaces.Handlers;
using RealityToolkit.Utilities.Physics;
using System;
using UnityEngine;

namespace RealityToolkit.Input.Modules
{
    /// <summary>
    /// This class provides Gaze as an Input Source so users can interact with objects using their head.
    /// </summary>
    [DisallowMultipleComponent]
    [System.Runtime.InteropServices.Guid("BED2C9A4-48C5-49D2-BCA4-3D351153BA75")]
    public class GazeProvider : MonoBehaviour, IGazeProvider, IInputHandler
    {
        private const float VelocityThreshold = 0.1f;

        private const float MovementThreshold = 0.01f;

        private bool sourceRaised = false;

        [SerializeField]
        [Tooltip("Maximum distance at which the gaze can hit a GameObject.")]
        private float maxGazeCollisionDistance = 10.0f;

        /// <summary>
        /// The Physics layers, in prioritized order, that are used to determine the GazeTarget when raycasting.
        /// <example>
        /// <para>Allow the cursor to hit SR, but first prioritize any DefaultRaycastLayers (potentially behind SR)</para>
        /// <code language="csharp"><![CDATA[
        /// int sr = LayerMask.GetMask("SR");
        /// int nonSR = Physics.DefaultRaycastLayers &amp; ~sr;
        /// GazeProvider.Instance.RaycastLayerMasks = new LayerMask[] { nonSR, sr };
        /// ]]></code>
        /// </example>
        /// </summary>
        [SerializeField]
        [Tooltip("The Physics layers, in prioritized order, that are used to determine the GazeTarget when raycasting.")]
        private LayerMask[] raycastLayerMasks = { Physics.DefaultRaycastLayers };

        /// <summary>
        /// Current stabilization method, used to smooth out the gaze ray data.
        /// If left null, no stabilization will be performed.
        /// </summary>
        [SerializeField]
        [Tooltip("Stabilizer, if any, used to smooth out the gaze ray data.")]
        private GenericStabilizer stabilizer = null;

        /// <summary>
        /// Transform that should be used as the source of the gaze position and rotation.
        /// Defaults to the main camera.
        /// </summary>
        [SerializeField]
        [Tooltip("Transform that should be used to represent the gaze position and rotation. Defaults to CameraCache.Main")]
        private Transform gazeTransform = null;

        [SerializeField]
        [Range(0.01f, 1f)]
        [Tooltip("Minimum head velocity threshold")]
        private float minHeadVelocityThreshold = 0.5f;

        [SerializeField]
        [Range(0.1f, 5f)]
        [Tooltip("Maximum head velocity threshold")]
        private float maxHeadVelocityThreshold = 2f;

        /// <inheritdoc />
        public bool Enabled
        {
            get => enabled;
            set => enabled = value;
        }

        /// <inheritdoc />
        public IInputSource GazeInputSource
        {
            get
            {
                if (gazeInputSource == null)
                {
                    gazeInputSource = new BaseGenericInputSource("Gaze");
                    gazePointer.SetGazeInputSourceParent(gazeInputSource);
                }

                return gazeInputSource;
            }
        }

        private BaseGenericInputSource gazeInputSource;

        /// <inheritdoc />
        public IInteractor GazePointer => gazePointer ?? InitializeGazePointer();
        private InternalGazePointer gazePointer = null;

        /// <inheritdoc />
        public ICursor GazeCursor => GazePointer.BaseCursor;

        /// <inheritdoc />
        public GameObject GazeTarget { get; private set; }

        /// <inheritdoc />
        public RaycastHit HitInfo { get; private set; }

        /// <inheritdoc />
        public Vector3 HitPosition { get; private set; }

        /// <inheritdoc />
        public Vector3 HitNormal { get; private set; }

        /// <inheritdoc />
        public Vector3 GazeOrigin => GazePointer.Rays[0].Origin;

        /// <inheritdoc />
        public Vector3 GazeDirection => GazePointer.Rays[0].Direction;

        /// <inheritdoc />
        public Vector3 HeadVelocity { get; private set; }

        /// <inheritdoc />
        public Vector3 HeadMovementDirection { get; private set; }

        /// <inheritdoc />
        public GameObject GameObject
        {
            get
            {
                try
                {
                    return gameObject;
                }
                catch
                {
                    return null;
                }
            }
        }

        private float lastHitDistance = 2.0f;

        private bool delayInitialization = true;

        private Vector3 lastHeadPosition = Vector3.zero;

        private IInputService inputService = null;

        protected IInputService InputService
            => inputService ?? (inputService = ServiceManager.Instance.GetService<IInputService>());

        #region InternalGazePointer Class

        private class InternalGazePointer : GenericPointer
        {
            private readonly Transform gazeTransform;
            private readonly BaseRayStabilizer stabilizer;
            private readonly GazeProvider gazeProvider;

            public InternalGazePointer(GazeProvider gazeProvider, string pointerName, IInputSource inputSourceParent, LayerMask[] raycastLayerMasks, float pointerExtent, Transform gazeTransform, BaseRayStabilizer stabilizer)
                    : base(pointerName, inputSourceParent)
            {
                this.gazeProvider = gazeProvider;
                PointerRaycastLayerMasksOverride = raycastLayerMasks;
                this.pointerExtent = pointerExtent;
                this.gazeTransform = gazeTransform;
                this.stabilizer = stabilizer;
                IsInteractionEnabled = true;
            }

            #region IPointer Implementation

            /// <inheritdoc />
            public override IController Controller { get; set; }

            /// <inheritdoc />
            public override IInputSource InputSource { get; protected set; }

            private float pointerExtent;

            /// <inheritdoc />
            public override float PointerExtent
            {
                get => pointerExtent;
                set => pointerExtent = value;
            }

            /// <summary>
            /// Only for use when initializing Gaze Pointer on startup.
            /// </summary>
            /// <param name="gazeInputSource"></param>
            internal void SetGazeInputSourceParent(IInputSource gazeInputSource)
            {
                InputSource = gazeInputSource;
            }

            /// <inheritdoc />
            public override void OnPreRaycast()
            {
                Vector3 newGazeOrigin = gazeTransform.position;
                Vector3 newGazeNormal = gazeTransform.forward;

                // Update gaze info from stabilizer
                if (stabilizer != null)
                {
                    stabilizer.UpdateStability(gazeTransform.localPosition, gazeTransform.localRotation * Vector3.forward);
                    var transformParent = gazeTransform.parent;

                    if (transformParent != null)
                    {
                        newGazeOrigin = transformParent.TransformPoint(stabilizer.StablePosition);
                        newGazeNormal = transformParent.TransformDirection(stabilizer.StableRay.direction);
                    }
                    else
                    {
                        newGazeOrigin = gazeTransform.TransformPoint(stabilizer.StablePosition);
                        newGazeNormal = gazeTransform.TransformDirection(stabilizer.StableRay.direction);
                    }
                }

                Vector3 endPoint = newGazeOrigin + (newGazeNormal * pointerExtent);
                Rays[0].UpdateRayStep(ref newGazeOrigin, ref endPoint);

                gazeProvider.HitPosition = Rays[0].Origin + (gazeProvider.lastHitDistance * Rays[0].Direction);
            }

            public override void OnPostRaycast()
            {
                gazeProvider.HitInfo = Result.LastRaycastHit;
                gazeProvider.GazeTarget = Result.CurrentPointerTarget;

                if (Result.CurrentPointerTarget != null)
                {
                    gazeProvider.lastHitDistance = (Result.EndPoint - Rays[0].Origin).magnitude;
                    gazeProvider.HitPosition = Rays[0].Origin + (gazeProvider.lastHitDistance * Rays[0].Direction);
                    gazeProvider.HitNormal = Result.Normal;
                }
            }

            public override bool TryGetPointerPosition(out Vector3 position)
            {
                position = gazeTransform.position;
                return true;
            }

            public override bool TryGetPointingRay(out Ray pointingRay)
            {
                pointingRay = new Ray(gazeProvider.GazeOrigin, gazeProvider.GazeDirection);
                return true;
            }

            public override bool TryGetPointerRotation(out Quaternion rotation)
            {
                rotation = Quaternion.identity;
                return false;
            }

            #endregion IPointer Implementation
        }

        #endregion InternalGazePointer Class

        #region MonoBehaviour Implementation

        private bool lateInitialize = true;

        private void OnValidate()
        {
            Debug.Assert(minHeadVelocityThreshold < maxHeadVelocityThreshold, "Minimum head velocity threshold should be less than the maximum velocity threshold.");
        }

        protected virtual void OnEnable()
        {
            if (!lateInitialize &&
                ServiceManager.Instance.IsInitialized)
            {
                InputService?.Register(gameObject);
            }

            if (!delayInitialization)
            {
                // The first time we call OnEnable we skip this.
                RaiseSourceDetected();
            }
        }

        protected virtual async void Start()
        {
            if (lateInitialize)
            {
                try
                {
                    inputService = await ServiceManager.Instance.GetServiceAsync<IInputService>();
                }
                catch (Exception e)
                {
                    Debug.LogError(e);
                    return;
                }

                // We've been destroyed during the await.
                if (this == null) { return; }

                lateInitialize = false;
                InputService.Register(gameObject);

                if (GazePointer.BaseCursor != null)
                {
                    GazePointer.BaseCursor.IsVisible = true;
                }

                if (delayInitialization)
                {
                    delayInitialization = false;
                    RaiseSourceDetected();
                }
            }
        }

        private void Update()
        {
            if (Raycaster.DebugEnabled && gazeTransform != null)
            {
                Debug.DrawRay(GazeOrigin, (HitPosition - GazeOrigin), Color.white);
            }
        }

        private void LateUpdate()
        {
            if (gazeTransform == null) { return; }

            // Update head velocity.
            Vector3 headPosition = GazeOrigin;
            Vector3 headDelta = headPosition - lastHeadPosition;

            if (headDelta.sqrMagnitude < MovementThreshold * MovementThreshold)
            {
                headDelta = Vector3.zero;
            }

            if (Time.fixedDeltaTime > 0)
            {
                float velocityAdjustmentRate = 3f * Time.fixedDeltaTime;
                HeadVelocity = HeadVelocity * (1f - velocityAdjustmentRate) + headDelta * velocityAdjustmentRate / Time.fixedDeltaTime;

                if (HeadVelocity.sqrMagnitude < VelocityThreshold * VelocityThreshold)
                {
                    HeadVelocity = Vector3.zero;
                }
            }

            // Update Head Movement Direction
            float multiplier = Mathf.Clamp01(Mathf.InverseLerp(minHeadVelocityThreshold, maxHeadVelocityThreshold, HeadVelocity.magnitude));

            Vector3 newHeadMoveDirection = Vector3.Lerp(headPosition, HeadVelocity, multiplier).normalized;
            lastHeadPosition = headPosition;
            float directionAdjustmentRate = Mathf.Clamp01(5f * Time.fixedDeltaTime);

            HeadMovementDirection = Vector3.Slerp(HeadMovementDirection, newHeadMoveDirection, directionAdjustmentRate);

            if (Raycaster.DebugEnabled && gazeTransform != null)
            {
                Debug.DrawLine(lastHeadPosition, lastHeadPosition + HeadMovementDirection * 10f, Color.Lerp(Color.red, Color.green, multiplier));
                Debug.DrawLine(lastHeadPosition, lastHeadPosition + HeadVelocity, Color.yellow);
            }
        }

        protected virtual void OnDisable()
        {
            InputService?.Unregister(gameObject);

            if (GazePointer.BaseCursor != null)
            {
                GazePointer.BaseCursor.IsVisible = false;
            }

            if (sourceRaised)
            {
                InputService?.RaiseSourceLost(GazeInputSource);
            }
        }

        protected void OnDestroy()
        {
            InputService?.Unregister(gameObject);

            if (GazePointer != null && !GazeCursor.Equals(null) && GazeCursor.GameObjectReference.IsNotNull())
            {
                GazeCursor.GameObjectReference.Destroy();
            }
        }

        #endregion MonoBehaviour Implementation

        #region IInputHandler Implementation

        /// <inheritdoc />
        void IInputHandler.OnInputUp(InputEventData eventData)
        {
            for (int i = 0; i < eventData.InputSource.Pointers.Length; i++)
            {
                if (eventData.InputSource.Pointers[i].PointerId == GazePointer.PointerId)
                {
                    InputService.RaisePointerUp(GazePointer, eventData.InputAction);
                    InputService.RaisePointerClicked(GazePointer, eventData.InputAction);
                    return;
                }
            }
        }

        /// <inheritdoc />
        void IInputHandler.OnInputDown(InputEventData eventData)
        {
            for (int i = 0; i < eventData.InputSource.Pointers.Length; i++)
            {
                if (eventData.InputSource.Pointers[i].PointerId == GazePointer.PointerId)
                {
                    InputService.RaisePointerDown(GazePointer, eventData.InputAction, eventData.InputSource);
                    return;
                }
            }
        }

        #endregion IInputHandler Implementation

        #region Utilities

        private IInteractor InitializeGazePointer()
        {
            if (InputService == null) { return null; }

            if (gazeTransform == null)
            {
                gazeTransform = Camera.main.transform;
            }

            if (gazeTransform == null)
            {
                Debug.LogError($"No {nameof(gazeTransform)} to raycast from!");
                return null;
            }

            if (gazeTransform.parent == null)
            {
                Debug.LogError($"{nameof(gazeTransform)}:{gazeTransform.name} must have a parent!");
                return null;
            }

            gazePointer = new InternalGazePointer(this, "Gaze Pointer", null, raycastLayerMasks, maxGazeCollisionDistance, gazeTransform, stabilizer);

            if (GazeCursor == null &&
                ServiceManager.Instance.TryGetServiceProfile<IInputService, InputServiceProfile>(out var inputServiceProfile) &&
                inputServiceProfile.GazeCursorPrefab != null)
            {
                var cursor = Instantiate(inputServiceProfile.GazeCursorPrefab, gazeTransform.parent);
                SetGazeCursor(cursor);
            }

            return gazePointer;
        }

        private async void RaiseSourceDetected()
        {
            try
            {
                inputService = await ServiceManager.Instance.GetServiceAsync<IInputService>();
            }
            catch (Exception e)
            {
                Debug.LogError(e);
                return;
            }

            // We've been destroyed during the await.
            if (this == null) { return; }

            InputService.RaiseSourceDetected(GazeInputSource);

            if (GazePointer.BaseCursor != null)
            {
                GazePointer.BaseCursor.IsVisible = true;
            }

            sourceRaised = true;
        }

        /// <summary>
        /// Set the gaze cursor.
        /// </summary>
        public void SetGazeCursor(GameObject cursor)
        {
            Debug.Assert(cursor != null);
            cursor.transform.parent = transform.parent;
            GazePointer.BaseCursor = cursor.GetComponent<ICursor>();
            Debug.Assert(GazePointer.BaseCursor != null, "Failed to load cursor");
            GazePointer.BaseCursor.SetVisibilityOnSourceDetected = false;
            GazePointer.BaseCursor.Pointer = GazePointer;
        }

        #endregion Utilities
    }
}