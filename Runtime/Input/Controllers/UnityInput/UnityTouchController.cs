﻿// Copyright (c) Reality Collective. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using RealityCollective.Definitions.Utilities;
using RealityToolkit.Definitions.Controllers;
using RealityToolkit.Definitions.Devices;
using RealityToolkit.Input.Interfaces.Modules;
using UnityEngine;

namespace RealityToolkit.Input.Controllers.UnityInput
{
    [System.Runtime.InteropServices.Guid("98F97EDA-4418-4B4B-88E9-E4F1F0734E4E")]
    public class UnityTouchController : BaseController
    {
        /// <inheritdoc />
        public UnityTouchController() { }

        /// <inheritdoc />
        public UnityTouchController(IControllerServiceModule controllerDataProvider, TrackingState trackingState, Handedness controllerHandedness, ControllerProfile controllerMappingProfile)
            : base(controllerDataProvider, trackingState, controllerHandedness, controllerMappingProfile)
        {
        }

        private const float K_CONTACT_EPSILON = 30.0f;

        /// <summary>
        /// Time in seconds to determine if the contact registers as a tap or a hold
        /// </summary>
        public float MaxTapContactTime { get; set; } = 0.5f;

        /// <summary>
        /// The threshold a finger must move before starting a manipulation gesture.
        /// </summary>
        public float ManipulationThreshold { get; set; } = 5f;

        /// <summary>
        /// Current Touch Data for the Controller.
        /// </summary>
        public Touch TouchData { get; internal set; }

        /// <summary>
        /// Current Screen point ray for the Touch.
        /// </summary>
        public Ray ScreenPointRay { get; internal set; }

        /// <summary>
        /// The current lifetime of the Touch.
        /// </summary>
        public float Lifetime { get; private set; } = 0.0f;

        /// <inheritdoc />
        public override InteractionMapping[] DefaultInteractions { get; } =
        {
            new InteractionMapping("Touch Pointer Delta", AxisType.DualAxis, DeviceInputType.PointerPosition),
            new InteractionMapping("Touch Pointer Position", AxisType.SixDof, DeviceInputType.SpatialPointer),
            new InteractionMapping("Touch Press", AxisType.Digital, DeviceInputType.PointerClick),
            new InteractionMapping("Touch Hold", AxisType.Digital, DeviceInputType.ButtonPress),
            new InteractionMapping("Touch Drag", AxisType.DualAxis, DeviceInputType.Touchpad)
        };

        private bool isTouched;
        private bool isHolding;
        private bool isManipulating;

        /// <summary>
        /// Start the touch.
        /// </summary>
        public void StartTouch()
        {
            InputService?.RaisePointerDown(InputSource.Pointers[0], Interactions[2].InputAction);
            isTouched = true;
            InputService?.RaiseGestureStarted(this, Interactions[4].InputAction);
            isHolding = true;
        }

        /// <summary>
        /// Update the touch data.
        /// </summary>
        public void Update()
        {
            UpdateController();

            if (!isTouched) { return; }

            Lifetime += Time.deltaTime;

            if (TouchData.phase == TouchPhase.Moved)
            {
                Interactions[0].Vector2Data = TouchData.deltaPosition;

                // If our value was updated, raise it.
                if (Interactions[0].Updated)
                {
                    InputService?.RaisePositionInputChanged(InputSource, Interactions[0].InputAction, TouchData.deltaPosition);
                }

                if (InputSource.Pointers[0].BaseCursor != null)
                {
                    Pose = new Pose(InputSource.Pointers[0].BaseCursor.Position, InputSource.Pointers[0].BaseCursor.Rotation);
                }

                InputService?.RaiseSourcePoseChanged(InputSource, this, Pose);

                Interactions[1].PoseData = Pose;

                // If our value was updated, raise it.
                if (Interactions[1].Updated)
                {
                    InputService?.RaisePoseInputChanged(InputSource, Interactions[1].InputAction, Pose);
                }

                if (!isManipulating)
                {
                    if (Mathf.Abs(TouchData.deltaPosition.x) > ManipulationThreshold ||
                        Mathf.Abs(TouchData.deltaPosition.y) > ManipulationThreshold)
                    {
                        InputService?.RaiseGestureCanceled(this, Interactions[4].InputAction);
                        isHolding = false;

                        InputService?.RaiseGestureStarted(this, Interactions[5].InputAction);
                        isManipulating = true;
                    }
                }
                else
                {
                    InputService?.RaiseGestureUpdated(this, Interactions[5].InputAction, TouchData.deltaPosition);
                }
            }
        }

        /// <summary>
        /// End the touch.
        /// </summary>
        public void EndTouch()
        {
            if (TouchData.phase == TouchPhase.Ended)
            {
                if (Lifetime < K_CONTACT_EPSILON)
                {
                    if (isHolding)
                    {
                        InputService?.RaiseGestureCanceled(this, Interactions[4].InputAction);
                        isHolding = false;
                    }

                    if (isManipulating)
                    {
                        InputService?.RaiseGestureCanceled(this, Interactions[5].InputAction);
                        isManipulating = false;
                    }
                }
                else if (Lifetime < MaxTapContactTime)
                {
                    if (isHolding)
                    {
                        InputService?.RaiseGestureCanceled(this, Interactions[4].InputAction);
                        isHolding = false;
                    }

                    if (isManipulating)
                    {
                        InputService?.RaiseGestureCanceled(this, Interactions[5].InputAction);
                        isManipulating = false;
                    }

                    InputService?.RaisePointerClicked(InputSource.Pointers[0], Interactions[2].InputAction);
                }

                if (isHolding)
                {
                    InputService?.RaiseGestureCompleted(this, Interactions[4].InputAction);
                    isHolding = false;
                }

                if (isManipulating)
                {
                    InputService?.RaiseGestureCompleted(this, Interactions[5].InputAction, TouchData.deltaPosition);
                    isManipulating = false;
                }
            }

            if (isHolding)
            {
                InputService?.RaiseGestureCompleted(this, Interactions[4].InputAction);
                isHolding = false;
            }

            Debug.Assert(!isHolding);

            if (isManipulating)
            {
                InputService?.RaiseGestureCompleted(this, Interactions[5].InputAction, TouchData.deltaPosition);
                isManipulating = false;
            }

            Debug.Assert(!isManipulating);

            InputService?.RaisePointerUp(InputSource.Pointers[0], Interactions[2].InputAction);

            Lifetime = 0.0f;
            isTouched = false;
            Interactions[0].Vector2Data = Vector2.zero;
            Interactions[1].PoseData = Pose.identity;
        }
    }
}