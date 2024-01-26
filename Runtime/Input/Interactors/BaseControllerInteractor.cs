// Copyright (c) Reality Collective. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using RealityCollective.Extensions;
using RealityToolkit.Definitions.Physics;
using RealityToolkit.EventDatum.Input;
using RealityToolkit.Input.Controllers;
using RealityToolkit.Input.Definitions;
using RealityToolkit.Input.Interfaces;
using RealityToolkit.Input.Interfaces.Handlers;
using RealityToolkit.Interfaces.Physics;
using RealityToolkit.Utilities.Physics;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEvents = UnityEngine.EventSystems;

namespace RealityToolkit.Input.Interactors
{
    /// <summary>
    /// Abstract base implementation for new <see cref="IControllerInteractor"/>s.
    /// </summary>
    [DisallowMultipleComponent]
    public abstract class BaseControllerInteractor : ControllerPoseSynchronizer, IControllerInteractor
    {
        [SerializeField]
        protected LayerMask uiLayerMask = -1;

        [SerializeField]
        [Tooltip("Source transform for raycast origin - leave null to use default transform")]
        protected Transform raycastOrigin = null;

        [SerializeField]
        [Tooltip("The hold action that will enable the raise the input event for this pointer.")]
        protected InputAction activeHoldAction = InputAction.None;

        [SerializeField]
        [Tooltip("The action that will enable the raise the input event for this pointer.")]
        protected InputAction pointerAction = InputAction.None;

        [SerializeField]
        [Tooltip("The action that will enable the raise the input grab event for this pointer.")]
        protected InputAction grabAction = InputAction.None;

        [SerializeField]
        [Tooltip("Does the interaction require hold?")]
        protected bool requiresHoldAction = false;

        [SerializeField]
        [Tooltip("Enables the pointer ray when the pointer is started.")]
        protected bool enablePointerOnStart = false;

        [Min(0.01f)]
        [SerializeField]
        protected float defaultPointerExtent = 10f;

        [SerializeField]
        protected bool overrideGlobalPointerExtent = false;

        [SerializeField]
        [Range(0f, 360f)]
        [Tooltip("The Y orientation of the pointer - used for rotation and navigation")]
        protected float pointerOrientation = 0f;

        [SerializeField]
        private GameObject cursorPrefab = null;

        [SerializeField]
        private bool disableCursorOnStart = false;

        protected bool DisableCursorOnStart => disableCursorOnStart;

        [SerializeField]
        private bool setCursorVisibilityOnSourceDetected = false;

        private GameObject cursorInstance = null;
        private Vector3 lastPointerPosition = Vector3.zero;
        private readonly List<InputAction> inputDownActions = new List<InputAction>();
        private readonly Dictionary<uint, List<GameObject>> inputDownTrackingDictionary = new Dictionary<uint, List<GameObject>>();

        /// <inheritdoc/>
        public bool IsOverUI => Result != null &&
                    Result.CurrentTarget.IsNotNull() &&
                    uiLayerMask == (uiLayerMask | (1 << Result.CurrentTarget.layer));

        /// <inheritdoc />
        public abstract bool IsFarInteractor { get; }

        /// <inheritdoc />
        public string PointerName => gameObject.name;

        /// <summary>
        /// True if select is pressed right now
        /// </summary>
        protected bool IsSelectPressed { get; set; } = false;

        /// <summary>
        /// <c>true</c>, if any <see cref="InputAction"/> is down on this <see cref="IInteractor"/>.
        /// </summary>
        protected bool IsInputDown => inputDownActions.Count > 0;

        /// <summary>
        /// True if select has been pressed once since this component was enabled
        /// </summary>
        protected bool HasSelectPressedOnce { get; set; } = false;

        /// <summary>
        /// Gets or sets whether this pointer is currently pressed and hold.
        /// </summary>
        protected bool IsHoldPressed { get; set; } = false;

        /// <summary>
        /// True if grab is pressed right now
        /// </summary>
        protected bool IsGrabPressed { get; set; } = false;

        /// <summary>
        /// True if grab is pressed right now
        /// </summary>
        protected bool IsDragging { get; set; } = false;

        /// <inheritdoc />
        public IInputSource InputSource { get; protected set; }

        /// <inheritdoc />
        public ICursor BaseCursor { get; set; }

        private ICursorModifier cursorModifier = null;

        /// <inheritdoc />
        public ICursorModifier CursorModifier
        {
            get
            {
                if (cursorModifier != null &&
                    cursorModifier.HostTransform != null &&
                   !cursorModifier.HostTransform.gameObject.activeInHierarchy)
                {
                    cursorModifier = null;
                }

                return cursorModifier;
            }
            set => cursorModifier = value;
        }

        /// <inheritdoc />
        public virtual bool IsInteractionEnabled
        {
            get
            {
                if (DirectPrivilege)
                {
                    return false;
                }

                if (IsTeleportRequestActive)
                {
                    return false;
                }

                if (requiresHoldAction && IsHoldPressed)
                {
                    return true;
                }

                if (IsSelectPressed || IsGrabPressed)
                {
                    return true;
                }

                return HasSelectPressedOnce || enablePointerOnStart;
            }
        }

        private bool isFocusLocked = false;

        /// <inheritdoc />
        public bool IsFocusLocked
        {
            get
            {
                if (isFocusLocked &&
                    syncedTarget == null)
                {
                    isFocusLocked = false;
                }

                if (syncedTarget != null)
                {
                    if (syncedTarget.activeInHierarchy)
                    {
                        isFocusLocked = true;
                    }
                    else
                    {
                        isFocusLocked = false;
                        syncedTarget = null;
                    }
                }

                return isFocusLocked;
            }
            set
            {
                if (value && syncedTarget == null)
                {
                    if (Result.CurrentTarget != null)
                    {
                        syncedTarget = Result.CurrentTarget;
                    }
                    else
                    {
                        Debug.LogWarning("No Sync Target to lock onto!");
                        return;
                    }
                }

                if (!value && syncedTarget != null)
                {
                    syncedTarget = null;
                }

                isFocusLocked = value;
            }
        }

        /// <inheritdoc />
        public GameObject SyncedTarget
        {
            get => syncedTarget = IsFocusLocked ? syncedTarget : null;
            set
            {
                isFocusLocked = value != null;
                syncedTarget = value;
            }
        }

        private GameObject syncedTarget = null;

        /// <inheritdoc />
        public Vector3? OverrideGrabPoint { get; set; } = null;

        private float pointerExtent;
        /// <inheritdoc />
        public float PointerExtent
        {
            get
            {
                if (overrideGlobalPointerExtent)
                {
                    if (InputService?.FocusProvider != null)
                    {
                        return InputService.FocusProvider.GlobalPointingExtent;
                    }
                }

                if (pointerExtent.Equals(0f))
                {
                    pointerExtent = defaultPointerExtent;
                }

                Debug.Assert(pointerExtent > 0f);
                return pointerExtent;
            }
            set
            {
                pointerExtent = value;
                Debug.Assert(pointerExtent > 0f, "Cannot set the pointer extent to 0. Resetting to the default pointer extent");
                overrideGlobalPointerExtent = false;
            }
        }

        /// <inheritdoc />
        public float DefaultPointerExtent => defaultPointerExtent;

        /// <inheritdoc />
        public RayStep[] Rays { get; protected set; } = { new RayStep(Vector3.zero, Vector3.forward) };

        /// <inheritdoc />
        public LayerMask[] PointerRaycastLayerMasksOverride { get; set; } = null;

        /// <inheritdoc />
        public IFocusHandler FocusHandler { get; set; }

        /// <inheritdoc />
        public IInteractorResult Result { get; set; }

        /// <inheritdoc />
        public IBaseRayStabilizer RayStabilizer { get; set; } = new GenericStabilizer();

        /// <inheritdoc />
        public RaycastMode RaycastMode { get; set; } = RaycastMode.Simple;

        /// <inheritdoc />
        public float SphereCastRadius { get; set; } = 0.1f;

        /// <inheritdoc />
        public virtual float PointerOrientation
        {
            get => pointerOrientation + (raycastOrigin != null ? raycastOrigin.eulerAngles.y : transform.eulerAngles.y);
            set => pointerOrientation = value < 0
                        ? Mathf.Clamp(value, -360f, 0f)
                        : Mathf.Clamp(value, 0f, 360f);
        }

        /// <summary>
        /// Source <see cref="Transform"/> for the raycast origin.
        /// </summary>
        public Transform RaycastOrigin
        {
            get => raycastOrigin == null ? transform : raycastOrigin;
            protected set => raycastOrigin = value;
        }

        /// <summary>
        /// The forward direction of the targeting ray
        /// </summary>
        public virtual Vector3 PointerDirection => raycastOrigin != null ? raycastOrigin.forward : transform.forward;

        /// <inheritdoc/>
        public bool IsTeleportRequestActive { get; set; } = false;

        private uint pointerId;
        /// <inheritdoc />
        public uint PointerId
        {
            get
            {
                if (pointerId == 0)
                {
                    pointerId = InputService.FocusProvider.GenerateNewPointerId();
                }

                return pointerId;
            }
        }

        /// <inheritdoc/>
        public override IController Controller
        {
            get => base.Controller;
            set
            {
                base.Controller = value;
                InputSource = base.Controller.InputSource;
            }
        }

        /// <inheritdoc/>
        public GameObject GameObject => gameObject;

        /// <inheritdoc />
        public virtual bool DirectPrivilege { get; set; }

        /// <summary>
        /// Set a new cursor for this <see cref="IInteractor"/>
        /// </summary>
        /// <remarks>This <see cref="GameObject"/> must have a <see cref="ICursor"/> attached to it.</remarks>
        /// <param name="newCursor">The new cursor</param>
        public virtual void SetCursor(GameObject newCursor = null)
        {
            if (cursorInstance != null)
            {
                cursorInstance.Destroy();
                cursorInstance = newCursor;
            }

            if (cursorInstance == null && cursorPrefab != null)
            {
                cursorInstance = Instantiate(cursorPrefab, transform);
            }

            if (cursorInstance != null)
            {
                cursorInstance.name = $"{Handedness}_{name}_Cursor";
                BaseCursor = cursorInstance.GetComponent<ICursor>();

                if (BaseCursor != null)
                {
                    BaseCursor.Pointer = this;
                    BaseCursor.SetVisibilityOnSourceDetected = setCursorVisibilityOnSourceDetected;
                    BaseCursor.IsVisible = !disableCursorOnStart;
                }
                else
                {
                    Debug.LogError($"No ICursor component found on {cursorInstance.name}", this);
                }
            }
        }

        /// <inheritdoc/>
        protected override void Start()
        {
            base.Start();
            SetCursor();
        }

        /// <inheritdoc/>
        protected override void OnEnable()
        {
            base.OnEnable();
            InputService.Add(this);
        }

        /// <inheritdoc/>
        protected override void OnDisable()
        {
            if (IsSelectPressed || IsGrabPressed)
            {
                InputService.RaisePointerUp(this, pointerAction);
            }

            base.OnDisable();

            IsHoldPressed = false;
            IsSelectPressed = false;
            IsGrabPressed = false;
            HasSelectPressedOnce = false;

            if (BaseCursor != null)
            {
                BaseCursor.IsVisible = false;
            }

            if (InputService != null)
            {
                InputService.Remove(this);
            }
        }

        /// <inheritdoc />
        public virtual void OnPreRaycast() { }

        /// <inheritdoc />
        public virtual void OnPostRaycast()
        {
            if (grabAction != InputAction.None)
            {
                if (IsGrabPressed)
                {
                    DragHandler(grabAction);
                }
            }
            else
            {
                if (IsSelectPressed)
                {
                    DragHandler(pointerAction);

                }
            }
        }

        /// <inheritdoc />
        public override void OnSourceLost(SourceStateEventData eventData)
        {
            base.OnSourceLost(eventData);

            if (eventData.SourceId == InputSource.SourceId)
            {
                inputDownActions.Clear();

                if (requiresHoldAction)
                {
                    IsHoldPressed = false;
                }

                if (IsSelectPressed)
                {
                    OnRaisePointerUp(pointerAction);
                }

                if (IsGrabPressed)
                {
                    OnRaisePointerUp(grabAction);
                }

                IsSelectPressed = false;
                IsGrabPressed = false;
            }
        }

        /// <inheritdoc />
        public override void OnInputUp(InputEventData eventData)
        {
            base.OnInputUp(eventData);

            if (eventData.SourceId == InputSource.SourceId)
            {
                inputDownActions.SafeRemoveListItem(eventData.InputAction);

                if (requiresHoldAction && eventData.InputAction == activeHoldAction)
                {
                    IsHoldPressed = false;
                }

                if (grabAction != InputAction.None &&
                    eventData.InputAction == grabAction)
                {
                    IsGrabPressed = false;

                    OnRaisePointerClicked(grabAction);
                    OnRaisePointerUp(grabAction);
                }

                if (eventData.InputAction == pointerAction)
                {
                    IsSelectPressed = false;
                    OnRaisePointerClicked(pointerAction);
                    OnRaisePointerUp(pointerAction);
                }
            }
        }

        /// <inheritdoc />
        public override void OnInputDown(InputEventData eventData)
        {
            base.OnInputDown(eventData);

            if (eventData.SourceId == InputSource.SourceId)
            {
                inputDownActions.EnsureListItem(eventData.InputAction);

                if (requiresHoldAction && eventData.InputAction == activeHoldAction)
                {
                    IsHoldPressed = true;
                }

                if (grabAction != InputAction.None &&
                    eventData.InputAction == grabAction)
                {
                    IsGrabPressed = true;
                    OnRaisePointerDown(grabAction);
                }

                if (eventData.InputAction == pointerAction)
                {
                    IsSelectPressed = true;
                    HasSelectPressedOnce = true;
                    OnRaisePointerDown(pointerAction);
                }
            }
        }

        private void DragHandler(InputAction action)
        {
            if (IsDragging)
            {
                var currentPointerPosition = PointerPosition;
                var delta = currentPointerPosition - lastPointerPosition;
                InputService.RaisePointerDrag(this, action, delta);
                lastPointerPosition = currentPointerPosition;
            }
            else
            {
                IsDragging = true;
                var currentPointerPosition = PointerPosition;
                InputService.RaisePointerDragBegin(this, action, currentPointerPosition);
                lastPointerPosition = currentPointerPosition;
            }
        }

        /// <inheritdoc />
        public virtual bool TryGetPointerPosition(out Vector3 position)
        {
            position = raycastOrigin != null ? raycastOrigin.position : transform.position;
            return true;
        }

        private Vector3 PointerPosition
        {
            get
            {
                if (TryGetPointerPosition(out var pos))
                {
                    return pos;
                }
                return Vector3.zero;
            }
        }

        /// <inheritdoc />
        public virtual bool TryGetPointingRay(out Ray pointingRay)
        {
            TryGetPointerPosition(out var pointerPosition);
            pointingRay = pointerRay;
            pointingRay.origin = pointerPosition;
            pointingRay.direction = PointerDirection;
            return true;
        }

        private readonly Ray pointerRay = new Ray();

        /// <inheritdoc />
        public virtual bool TryGetPointerRotation(out Quaternion rotation)
        {
            var pointerRotation = raycastOrigin != null ? raycastOrigin.eulerAngles : transform.eulerAngles;
            rotation = Quaternion.Euler(pointerRotation.x, PointerOrientation, pointerRotation.z);
            return true;
        }

        /// <summary>
        /// Invoked just before the <see cref="IInteractor"/> is going to
        /// raise <see cref="InputService.RaisePointerClicked(IInteractor, InputAction, IInputSource)"/>. Use this hook
        /// in inheriting implementations to add onw logic, if needed.
        /// </summary>
        /// <param name="inputAction">The <see cref="InputAction"/> about to be raised.</param>
        protected virtual void OnRaisePointerClicked(InputAction inputAction)
        {
            if (Result.CurrentTarget.IsNotNull() && IsInteractionEnabled)
            {
                InputService.RaisePointerClicked(this, inputAction);
            }
        }

        /// <summary>
        /// Invoked just before the <see cref="IInteractor"/> is going to
        /// raise <see cref="InputService.RaisePointerUp(IInteractor, InputAction, IInputSource)"/>. Use this hook
        /// in inheriting implementations to add onw logic, if needed.
        /// </summary>
        /// <param name="inputAction">The <see cref="InputAction"/> about to be raised.</param>
        protected virtual void OnRaisePointerUp(InputAction inputAction)
        {
            ResolveInputDown(inputAction);
            InputService.RaisePointerUp(this, inputAction);
        }

        /// <summary>
        /// Invoked just before the <see cref="IInteractor"/> is going to
        /// raise <see cref="InputService.RaisePointerDown(IInteractor, InputAction, IInputSource)"/>. Use this hook
        /// in inheriting implementations to add onw logic, if needed.
        /// </summary>
        /// <param name="inputAction">The <see cref="InputAction"/> about to be raised.</param>
        protected virtual void OnRaisePointerDown(InputAction inputAction)
        {
            if (Result.CurrentTarget.IsNotNull() && IsInteractionEnabled)
            {
                TrackInputDown(inputAction, Result.CurrentTarget);
                InputService.RaisePointerDown(this, inputAction);
            }
        }

        /// <summary>
        /// Records the input down on the <paramref name="target"/> with <paramref name="inputAction"/>,
        /// so that we can ensure input up is raised on the <paramref name="target"/> even without focus.
        /// </summary>
        /// <param name="inputAction">The <see cref="InputAction"/>.</param>
        /// <param name="target">The input down target <see cref="UnityEngine.GameObject"/>.</param>
        protected void TrackInputDown(InputAction inputAction, GameObject target)
        {
            if (inputDownTrackingDictionary.TryGetValue(inputAction.Id, out var targets))
            {
                targets.EnsureListItem(target);
                return;
            }

            inputDownTrackingDictionary.Add(inputAction.Id, new List<GameObject> { target });
        }

        /// <summary>
        /// Raises input up for <paramref name="inputAction"/> on any previously tracked
        /// <see cref="UnityEngine.GameObject"/>s using <see cref="TrackInputDown(InputAction, GameObject)"/>.
        /// </summary>
        /// <param name="inputAction">The <see cref="InputAction"/> to resolve.</param>
        protected void ResolveInputDown(InputAction inputAction)
        {
            if (!inputDownTrackingDictionary.TryGetValue(inputAction.Id, out var targets))
            {
                return;
            }

            for (var i = 0; i < targets.Count; i++)
            {
                var target = targets[i];
                var handlers = target.GetComponents<IPointerHandler>();

                if (handlers != null)
                {
                    var eventData = new PointerEventData(UnityEvents.EventSystem.current);
                    eventData.Initialize(this, inputAction);

                    for (var j = 0; j < handlers.Length; j++)
                    {
                        handlers[j].OnPointerUp(eventData);
                    }
                }
            }

            inputDownTrackingDictionary.Remove(inputAction.Id);
        }

        /// <inheritdoc />
        bool IEqualityComparer.Equals(object left, object right)
        {
            return left.Equals(right);
        }

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            if (obj is null) { return false; }
            if (this == null) { return false; }
            if (ReferenceEquals(this, obj)) { return true; }
            return obj.GetType() == GetType() && Equals((IInteractor)obj);
        }

        private bool Equals(IInteractor other)
        {
            return other != null && PointerId == other.PointerId && string.Equals(PointerName, other.PointerName);
        }

        /// <inheritdoc />
        int IEqualityComparer.GetHashCode(object obj)
        {
            return obj.GetHashCode();
        }

        /// <inheritdoc />
        public override int GetHashCode() => HashCode.Combine(PointerId, PointerName);
    }
}