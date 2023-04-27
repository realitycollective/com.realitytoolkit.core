// Copyright (c) Reality Collective. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using RealityCollective.Definitions.Utilities;
using RealityToolkit.Definitions.Controllers;
using RealityToolkit.Definitions.Devices;
using RealityToolkit.Input.Interfaces.Modules;
using RealityToolkit.Input.Processors;
using System.Collections.Generic;
using UnityEngine;

namespace RealityToolkit.Input.Controllers.UnityInput
{
    /// <summary>
    /// Manages the mouse using unity input system.
    /// </summary>
    [System.Runtime.InteropServices.Guid("A80E17F6-8221-49C3-BC4B-CAB495C91D6C")]
    public class MouseController : BaseController
    {
        /// <inheritdoc />
        public MouseController() { }

        /// <inheritdoc />
        public MouseController(IControllerServiceModule controllerDataProvider, TrackingState trackingState, Handedness controllerHandedness, ControllerMappingProfile controllerMappingProfile)
            : base(controllerDataProvider, trackingState, controllerHandedness, controllerMappingProfile)
        {
        }

        /// <inheritdoc />
        public override InteractionMapping[] DefaultInteractions
        {
            get
            {
                var singleAxisProcessor = ScriptableObject.CreateInstance<InvertDualAxisProcessor>();
                singleAxisProcessor.InvertX = true;
                var dualAxisProcessor = ScriptableObject.CreateInstance<InvertDualAxisProcessor>();
                dualAxisProcessor.InvertX = true;
                dualAxisProcessor.InvertY = true;
                return new[]
                {
                    new InteractionMapping("Spatial Mouse Position", AxisType.SixDof, DeviceInputType.SpatialPointer),
                    new InteractionMapping("Mouse Position", AxisType.DualAxis, DeviceInputType.PointerPosition, ControllerMappingLibrary.MouseY, ControllerMappingLibrary.MouseX, new List<InputProcessor>{ singleAxisProcessor }),
                    new InteractionMapping("Mouse Scroll Position", AxisType.DualAxis, DeviceInputType.Scroll, ControllerMappingLibrary.MouseScroll, ControllerMappingLibrary.MouseScroll, new List<InputProcessor>{ dualAxisProcessor }),
                    new InteractionMapping("Left Mouse Button", AxisType.Digital, DeviceInputType.ButtonPress, KeyCode.Mouse0),
                    new InteractionMapping("Right Mouse Button", AxisType.Digital, DeviceInputType.ButtonPress, KeyCode.Mouse1),
                    new InteractionMapping("Mouse Button 2", AxisType.Digital, DeviceInputType.ButtonPress, KeyCode.Mouse2),
                    new InteractionMapping("Mouse Button 3", AxisType.Digital, DeviceInputType.ButtonPress, KeyCode.Mouse3),
                    new InteractionMapping("Mouse Button 4", AxisType.Digital, DeviceInputType.ButtonPress, KeyCode.Mouse4),
                    new InteractionMapping("Mouse Button 5", AxisType.Digital, DeviceInputType.ButtonPress, KeyCode.Mouse5),
                    new InteractionMapping("Mouse Button 6", AxisType.Digital, DeviceInputType.ButtonPress, KeyCode.Mouse6),
                };
            }
        }

        public static bool IsInGameWindow => UnityEngine.Input.mousePresent &&
                                             (UnityEngine.Input.mousePosition.x > 0 ||
                                              UnityEngine.Input.mousePosition.y > 0 ||
                                              UnityEngine.Input.mousePosition.x < Screen.width ||
                                              UnityEngine.Input.mousePosition.y < Screen.height);

        private Vector2 mousePosition;

        /// <summary>
        /// Update controller.
        /// </summary>
        public void Update()
        {
            base.UpdateController();

            // Bail early if our mouse isn't in our game window.
            if (!IsInGameWindow)
            {
                return;
            }

            if (InputSource.Pointers[0].BaseCursor != null)
            {
                Pose = new Pose(InputSource.Pointers[0].BaseCursor.Position, InputSource.Pointers[0].BaseCursor.Rotation);
            }

            mousePosition.x = UnityEngine.Input.GetAxis(Interactions[1].AxisCodeX);
            mousePosition.y = UnityEngine.Input.GetAxis(Interactions[1].AxisCodeY);

            InputService?.RaiseSourcePositionChanged(InputSource, this, mousePosition);
            InputService?.RaiseSourcePoseChanged(InputSource, this, Pose);

            for (int i = 0; i < Interactions.Length; i++)
            {
                if (Interactions[i].InputType == DeviceInputType.SpatialPointer)
                {
                    Interactions[i].PoseData = Pose;

                    // If our value was updated, raise it.
                    if (Interactions[i].Updated)
                    {
                        InputService?.RaisePoseInputChanged(InputSource, Interactions[i].InputAction, Interactions[i].PoseData);
                    }
                }

                if (Interactions[i].InputType == DeviceInputType.PointerPosition)
                {
                    Interactions[i].Vector2Data = mousePosition;

                    // If our value was updated, raise it.
                    if (Interactions[i].Updated)
                    {
                        InputService?.RaisePositionInputChanged(InputSource, Interactions[i].InputAction, Interactions[i].Vector2Data);
                    }
                }

                if (Interactions[i].InputType == DeviceInputType.Scroll)
                {
                    Interactions[i].Vector2Data = UnityEngine.Input.mouseScrollDelta;

                    // If our value was updated, raise it.
                    if (Interactions[i].Updated)
                    {
                        InputService?.RaisePositionInputChanged(InputSource, Interactions[i].InputAction, Interactions[i].Vector2Data);
                    }
                }

                if (Interactions[i].AxisType == AxisType.Digital)
                {
                    var keyButton = UnityEngine.Input.GetKey(Interactions[i].KeyCode);

                    // Update the interaction data source
                    Interactions[i].BoolData = keyButton;

                    // If our value changed raise it.
                    if (Interactions[i].ControlActivated)
                    {
                        // Raise input system Event if it enabled
                        if (Interactions[i].BoolData)
                        {
                            InputService?.RaiseOnInputDown(InputSource, ControllerHandedness, Interactions[i].InputAction);
                        }
                        else
                        {
                            InputService?.RaiseOnInputUp(InputSource, ControllerHandedness, Interactions[i].InputAction);
                        }
                    }

                    // If our value was updated, raise it.
                    if (Interactions[i].Updated)
                    {
                        InputService?.RaiseOnInputPressed(InputSource, ControllerHandedness, Interactions[i].InputAction);
                    }
                }
            }
        }
    }
}
