﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using RealityToolkit.Definitions.InputSystem;
using RealityToolkit.Definitions.Physics;
using RealityToolkit.EventDatum.Input;
using RealityToolkit.Interfaces.InputSystem;
using RealityToolkit.Services.InputSystem.Listeners;
using RealityToolkit.Utilities.UX.Pointers;
using UnityEngine;

namespace RealityToolkit.Utilities.UX.Cursors
{
    /// <summary>
    /// Object that represents a cursor in 3D space controlled by gaze.
    /// </summary>
    public class BaseCursor : InputSystemGlobalListener, IMixedRealityCursor
    {
        /// <summary>
        /// The current <see cref="CursorStateEnum"/> of the cursor.
        /// </summary>
        public CursorStateEnum CursorState { get; private set; } = CursorStateEnum.None;

        [SerializeField]
        [Tooltip("The distance from the hit surface to place the cursor")]
        private float surfaceCursorDistance = 0.02f;

        /// <summary>
        /// The distance from the hit surface to place the cursor.
        /// </summary>
        public float SurfaceCursorDistance
        {
            get => surfaceCursorDistance;
            set => surfaceCursorDistance = value;
        }

        [Header("Motion")]

        [SerializeField]
        [Tooltip("When lerping, use unscaled time. This is useful for games that have a pause mechanism or otherwise adjust the game timescale.")]
        private bool useUnscaledTime = true;

        /// <summary>
        /// When lerping, use unscaled time. This is useful for games that have a pause mechanism or otherwise adjust the game timescale.
        /// </summary>
        public bool UseUnscaledTime
        {
            get => useUnscaledTime;
            set => useUnscaledTime = value;
        }

        [SerializeField]
        [Tooltip("Blend value for surface normal to user facing lerp")]
        private float positionLerpTime = 0.01f;

        /// <summary>
        /// Blend value for surface normal to user facing lerp.
        /// </summary>
        public float PositionLerpTime
        {
            get => positionLerpTime;
            set => positionLerpTime = value;
        }

        [SerializeField]
        [Tooltip("Blend value for surface normal to user facing lerp")]
        private float scaleLerpTime = 0.01f;

        /// <summary>
        /// Blend value for surface normal to user facing lerp.
        /// </summary>
        public float ScaleLerpTime
        {
            get => scaleLerpTime;
            set => scaleLerpTime = value;
        }

        [SerializeField]
        [Tooltip("Blend value for surface normal to user facing lerp")]
        private float rotationLerpTime = 0.01f;

        /// <summary>
        /// Blend value for surface normal to user facing lerp.
        /// </summary>
        public float RotationLerpTime
        {
            get => rotationLerpTime;
            set => rotationLerpTime = value;
        }

        [Range(0, 1)]
        [SerializeField]
        [Tooltip("Blend value for surface normal to user facing lerp")]
        private float lookRotationBlend = 0.5f;

        /// <summary>
        /// Blend value for surface normal to user facing lerp.
        /// </summary>
        public float LookRotationBlend
        {
            get => lookRotationBlend;
            set => lookRotationBlend = value;
        }

        [Header("Transform References")]

        [SerializeField]
        [Tooltip("Visual that is displayed when cursor is active normally")]
        protected Transform PrimaryCursorVisual = null;

        protected bool IsSourceDetected => visibleSourcesCount > 0;

        protected bool IsPointerDown = false;

        protected GameObject TargetedObject = null;

        private uint visibleSourcesCount = 0;

        private Vector3 targetPosition;
        private Vector3 targetScale;
        private Quaternion targetRotation;

        #region IMixedRealityCursor Implementation

        /// <inheritdoc />
        public virtual IMixedRealityPointer Pointer
        {
            get => pointer;
            set
            {
                pointer = value;
                pointer.BaseCursor = this;
                RegisterManagers();
            }
        }

        private IMixedRealityPointer pointer;

        /// <inheritdoc />
        public virtual Vector3 Position => transform.position;

        /// <inheritdoc />
        public virtual Quaternion Rotation => transform.rotation;

        /// <inheritdoc />
        public virtual Vector3 LocalScale => transform.localScale;

        /// <inheritdoc />
        public virtual void SetVisibility(bool visible)
        {
            if (PrimaryCursorVisual != null &&
                PrimaryCursorVisual.gameObject.activeInHierarchy != visible)
            {
                PrimaryCursorVisual.gameObject.SetActive(visible);
            }
        }

        /// <inheritdoc />
        public bool IsVisible => PrimaryCursorVisual != null ? PrimaryCursorVisual.gameObject.activeInHierarchy : gameObject.activeInHierarchy;

        /// <inheritdoc />
        public bool SetVisibilityOnSourceDetected { get; set; } = false;

        /// <inheritdoc />
        public GameObject GameObjectReference => gameObject;

        #endregion IMixedRealityCursor Implementation

        #region IMixedRealitySourceStateHandler Implementation

        /// <inheritdoc />
        public virtual void OnSourceDetected(SourceStateEventData eventData)
        {
            if (eventData.Controller != null)
            {
                for (int i = 0; i < eventData.InputSource.Pointers.Length; i++)
                {
                    // If a source is detected that's using this cursor's pointer, we increment the count to set the cursor state properly.
                    if (eventData.InputSource.Pointers[i].PointerId == Pointer.PointerId)
                    {
                        visibleSourcesCount++;

                        if (SetVisibilityOnSourceDetected && visibleSourcesCount == 1)
                        {
                            SetVisibility(true);
                        }

                        return;
                    }
                }
            }
        }

        /// <inheritdoc />
        public virtual void OnSourceLost(SourceStateEventData eventData)
        {
            if (eventData.Controller != null)
            {
                for (int i = 0; i < eventData.InputSource.Pointers.Length; i++)
                {
                    // If a source is lost that's using this cursor's pointer, we decrement the count to set the cursor state properly.
                    if (eventData.InputSource.Pointers[i].PointerId == Pointer.PointerId)
                    {
                        var basePointer = eventData.InputSource.Pointers[i] as BaseControllerPointer;

                        if (basePointer != null &&
                            basePointer.DestroyOnSourceLost)
                        {
                            IsPointerDown = false;
                            Destroy(gameObject);
                            return;
                        }

                        visibleSourcesCount--;
                    }
                }
            }

            if (!IsSourceDetected)
            {
                IsPointerDown = false;

                if (SetVisibilityOnSourceDetected)
                {
                    SetVisibility(false);
                }
            }
        }

        #endregion IMixedRealitySourceStateHandler Implementation

        #region IMixedRealityFocusChangedHandler Implementation

        /// <inheritdoc />
        public virtual void OnBeforeFocusChange(FocusEventData eventData)
        {
            if (Pointer.PointerId == eventData.Pointer.PointerId)
            {
                TargetedObject = eventData.NewFocusedObject;
            }
        }

        /// <inheritdoc />
        public virtual void OnFocusChanged(FocusEventData eventData) { }

        #endregion IMixedRealityFocusChangedHandler Implementation

        #region IMixedRealityPointerHandler Implementation

        /// <inheritdoc />
        public virtual void OnPointerDown(MixedRealityPointerEventData eventData)
        {
            foreach (var sourcePointer in eventData.InputSource.Pointers)
            {
                if (sourcePointer.PointerId == Pointer.PointerId)
                {
                    IsPointerDown = true;
                }
            }
        }

        /// <inheritdoc />
        public virtual void OnPointerClicked(MixedRealityPointerEventData eventData) { }

        /// <inheritdoc />
        public virtual void OnPointerUp(MixedRealityPointerEventData eventData)
        {
            foreach (var sourcePointer in eventData.InputSource.Pointers)
            {
                if (sourcePointer.PointerId == Pointer.PointerId)
                {
                    IsPointerDown = false;
                }
            }
        }

        #endregion IMixedRealityPointerHandler Implementation

        #region MonoBehaviour Implementation

        private void LateUpdate()
        {
            UpdateCursorState();
            UpdateCursorTransform();
        }

        protected override void OnEnable()
        {
            // We don't call base.OnEnable because we handle registering the global listener a bit differently.
            OnCursorStateChange(CursorStateEnum.None);
        }

        protected override void OnDisable()
        {
            // We don't call base.OnDisable because we handle un-registering the global listener a bit differently.
            TargetedObject = null;
            visibleSourcesCount = 0;
            OnCursorStateChange(CursorStateEnum.Contextual);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            UnregisterManagers();
        }

        #endregion MonoBehaviour Implementation

        /// <summary>
        /// Register to events from the managers the cursor needs.
        /// </summary>
        protected virtual void RegisterManagers()
        {
            // Register the cursor as a listener, so that it can always get input events it cares about
            InputSystem.Register(gameObject);

            // Setup the cursor to be able to respond to input being globally enabled / disabled
            if (InputSystem.IsInputEnabled)
            {
                OnInputEnabled();
            }
            else
            {
                OnInputDisabled();
            }

            InputSystem.InputEnabled += OnInputEnabled;
            InputSystem.InputDisabled += OnInputDisabled;
        }

        /// <summary>
        /// Unregister from events from the managers the cursor needs.
        /// </summary>
        protected virtual void UnregisterManagers()
        {
            if (InputSystem != null)
            {
                InputSystem.InputEnabled -= OnInputEnabled;
                InputSystem.InputDisabled -= OnInputDisabled;
                InputSystem.Unregister(gameObject);
            }
        }

        /// <summary>
        /// Update the cursor's transform
        /// </summary>
        protected virtual void UpdateCursorTransform()
        {
            if (Pointer == null)
            {
                Debug.LogError($"[BaseCursor.{name}] No Pointer has been assigned!");
                return;
            }

            if (!InputSystem.FocusProvider.TryGetFocusDetails(Pointer, out var focusDetails))
            {
                if (InputSystem.FocusProvider.IsPointerRegistered(Pointer))
                {
                    Debug.LogError($"{name}: Unable to get focus details for {pointer.GetType().Name}!");
                }

                return;
            }

            var newTargetedObject = InputSystem.FocusProvider.GetFocusedObject(Pointer);
            Vector3 lookForward;

            // Normalize scale on before update
            targetScale = Vector3.one;

            // If no game object is hit, put the cursor at the default distance
            if (newTargetedObject == null)
            {
                TargetedObject = null;
                targetPosition = focusDetails.EndPoint;
                lookForward = -RayStep.GetDirectionByDistance(Pointer.Rays, focusDetails.RayDistance);
                targetRotation = lookForward.magnitude > 0f ? Quaternion.LookRotation(lookForward, Vector3.up) : transform.rotation;
            }
            else
            {
                // Update currently targeted object
                TargetedObject = newTargetedObject;

                if (Pointer.CursorModifier != null)
                {
                    Pointer.CursorModifier.GetModifiedTransform(this, out targetPosition, out targetRotation, out targetScale);
                }
                else
                {
                    // If no modifier is on the target, just use the hit result to set cursor position
                    // Get the look forward by using distance between pointer origin and target position
                    // (This may not be strictly accurate for extremely wobbly pointers, but it should produce usable results)
                    var distanceToTarget = Vector3.Distance(Pointer.Rays[0].Origin, focusDetails.EndPoint);
                    lookForward = -RayStep.GetDirectionByDistance(Pointer.Rays, distanceToTarget);
                    targetPosition = focusDetails.EndPoint + (lookForward * surfaceCursorDistance);
                    var lookRotation = Vector3.Slerp(focusDetails.Normal, lookForward, lookRotationBlend);
                    targetRotation = Quaternion.LookRotation(lookRotation == Vector3.zero ? lookForward : lookRotation, Vector3.up);
                }
            }

            float deltaTime = useUnscaledTime
                ? Time.unscaledDeltaTime
                : Time.deltaTime;

            // Use the lerp times to blend the position to the target position
            var cachedTransform = transform;

            if (Pointer.IsFocusLocked && focusDetails.CurrentPointerTarget != null)
            {
                if (Pointer.SyncedTarget == null)
                {
                    cachedTransform.position = focusDetails.EndPoint;
                }
                else
                {
                    cachedTransform.position = focusDetails.GrabPoint;
                }
            }
            else
            {
                cachedTransform.position = Vector3.Slerp(cachedTransform.position, targetPosition, deltaTime / positionLerpTime);
                cachedTransform.localScale = Vector3.Slerp(cachedTransform.localScale, targetScale, deltaTime / scaleLerpTime);
                cachedTransform.rotation = Quaternion.Slerp(cachedTransform.rotation, targetRotation, deltaTime / rotationLerpTime);
            }
        }

        /// <summary>
        /// Disable input and set to contextual to override input
        /// </summary>
        public virtual void OnInputDisabled()
        {
            // Reset visible hands on disable
            visibleSourcesCount = 0;

            OnCursorStateChange(CursorStateEnum.Contextual);
        }

        /// <summary>
        /// Enable input and set to none to reset cursor
        /// </summary>
        public virtual void OnInputEnabled()
        {
            OnCursorStateChange(CursorStateEnum.None);
        }

        /// <summary>
        /// Internal update to check for cursor state changes
        /// </summary>
        private void UpdateCursorState()
        {
            CursorStateEnum newState = CheckCursorState();

            if (CursorState != newState)
            {
                OnCursorStateChange(newState);
            }
        }

        /// <summary>
        /// Virtual function for checking state changes.
        /// </summary>
        public virtual CursorStateEnum CheckCursorState()
        {
            if (CursorState != CursorStateEnum.Contextual)
            {
                if (IsPointerDown)
                {
                    return CursorStateEnum.Select;
                }

                if (CursorState == CursorStateEnum.Select)
                {
                    return CursorStateEnum.Release;
                }

                if (IsSourceDetected)
                {
                    return TargetedObject != null ? CursorStateEnum.InteractHover : CursorStateEnum.Interact;
                }

                return TargetedObject != null ? CursorStateEnum.ObserveHover : CursorStateEnum.Observe;
            }

            return CursorStateEnum.Contextual;
        }

        /// <summary>
        /// Change the cursor state to the new state.  Override in cursor implementations.
        /// </summary>
        /// <param name="state"></param>
        public virtual void OnCursorStateChange(CursorStateEnum state)
        {
            CursorState = state;
        }
    }
}
