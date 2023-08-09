// Copyright (c) Reality Collective. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using RealityCollective.Extensions;
using RealityToolkit.EventDatum.Input;
using RealityToolkit.Input.Definitions;
using RealityToolkit.Input.Interfaces;
using RealityToolkit.Input.Interfaces.Controllers;
using UnityEngine;

namespace RealityToolkit.Input.Interactions.Interactors
{
    /// <summary>
    /// Abstract base implementation for new <see cref="IControllerInteractor"/>s.
    /// </summary>
    [DisallowMultipleComponent]
    public abstract class BaseControllerInteractor : BaseInteractor, IControllerInteractor
    {
        [SerializeField]
        private GameObject cursorPrefab = null;

        [SerializeField]
        private bool disableCursorOnStart = false;

        protected bool DisableCursorOnStart => disableCursorOnStart;

        [SerializeField]
        private bool setCursorVisibilityOnSourceDetected = false;

        private GameObject cursorInstance = null;

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

        /// <inheritdoc />
        public override void OnSourceLost(SourceStateEventData eventData)
        {
            base.OnSourceLost(eventData);

            if (eventData.SourceId == InputSource.SourceId)
            {
                if (requiresHoldAction)
                {
                    IsHoldPressed = false;
                }

                if (IsSelectPressed)
                {
                    InputService.RaisePointerUp(this, pointerAction);
                }

                if (IsGrabPressed)
                {
                    InputService.RaisePointerUp(this, grabAction);
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
                if (requiresHoldAction && eventData.InputAction == activeHoldAction)
                {
                    IsHoldPressed = false;
                }

                if (grabAction != InputAction.None &&
                    eventData.InputAction == grabAction)
                {
                    IsGrabPressed = false;

                    InputService.RaisePointerClicked(this, grabAction);
                    InputService.RaisePointerUp(this, grabAction);
                }

                if (eventData.InputAction == pointerAction)
                {
                    IsSelectPressed = false;
                    InputService.RaisePointerClicked(this, pointerAction);
                    InputService.RaisePointerUp(this, pointerAction);
                }
            }
        }

        /// <inheritdoc />
        public override void OnInputDown(InputEventData eventData)
        {
            base.OnInputDown(eventData);

            if (eventData.SourceId == InputSource.SourceId)
            {
                if (requiresHoldAction && eventData.InputAction == activeHoldAction)
                {
                    IsHoldPressed = true;
                }

                if (grabAction != InputAction.None &&
                    eventData.InputAction == grabAction)
                {
                    IsGrabPressed = true;

                    InputService.RaisePointerDown(this, grabAction);
                }

                if (eventData.InputAction == pointerAction)
                {
                    IsSelectPressed = true;
                    HasSelectPressedOnce = true;
                    InputService.RaisePointerDown(this, pointerAction);
                }
            }
        }
    }
}