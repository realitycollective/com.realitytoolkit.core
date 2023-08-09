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
    /// Xbox Controller using Unity Input System
    /// </summary>
    [System.Runtime.InteropServices.Guid("71E70C1B-9F77-4B69-BFC7-974905BB7702")]
    public class XboxController : GenericJoystickController
    {
        /// <inheritdoc />
        public XboxController() { }

        /// <inheritdoc />
        public XboxController(IControllerServiceModule controllerDataProvider, TrackingState trackingState, Handedness controllerHandedness, ControllerProfile controllerMappingProfile)
            : base(controllerDataProvider, trackingState, controllerHandedness, controllerMappingProfile)
        {
        }

        /// <inheritdoc />
        public override InteractionMapping[] DefaultInteractions
        {
            get
            {
                var dualAxisProcessor = ScriptableObject.CreateInstance<InvertDualAxisProcessor>();
                dualAxisProcessor.InvertY = true;
                return new[]
                {
                    new InteractionMapping("Left Thumbstick", AxisType.DualAxis, DeviceInputType.ThumbStick, ControllerMappingLibrary.AXIS_1, ControllerMappingLibrary.AXIS_2, new List<InputProcessor>{ dualAxisProcessor }),
                    new InteractionMapping("Left Thumbstick Click", AxisType.Digital, DeviceInputType.ButtonPress, KeyCode.JoystickButton8),
                    new InteractionMapping("Right Thumbstick", AxisType.DualAxis, DeviceInputType.ThumbStick, ControllerMappingLibrary.AXIS_4, ControllerMappingLibrary.AXIS_5, new List<InputProcessor>{ dualAxisProcessor }),
                    new InteractionMapping("Right Thumbstick Click", AxisType.Digital, DeviceInputType.ButtonPress, KeyCode.JoystickButton9),
                    new InteractionMapping("D-Pad", AxisType.DualAxis, DeviceInputType.DirectionalPad, ControllerMappingLibrary.AXIS_6, ControllerMappingLibrary.AXIS_7, new List<InputProcessor>{ dualAxisProcessor }),
                    new InteractionMapping("Shared Trigger", AxisType.SingleAxis, DeviceInputType.Trigger, ControllerMappingLibrary.AXIS_3),
                    new InteractionMapping("Left Trigger", AxisType.SingleAxis, DeviceInputType.Trigger, ControllerMappingLibrary.AXIS_9),
                    new InteractionMapping("Right Trigger", AxisType.SingleAxis, DeviceInputType.Trigger, ControllerMappingLibrary.AXIS_10),
                    new InteractionMapping("View", AxisType.Digital, DeviceInputType.ButtonPress, KeyCode.JoystickButton6),
                    new InteractionMapping("Menu", AxisType.Digital, DeviceInputType.ButtonPress, KeyCode.JoystickButton7),
                    new InteractionMapping("Left Bumper", AxisType.Digital, DeviceInputType.ButtonPress, KeyCode.JoystickButton4),
                    new InteractionMapping("Right Bumper", AxisType.Digital, DeviceInputType.ButtonPress, KeyCode.JoystickButton5),
                    new InteractionMapping("A", AxisType.Digital, DeviceInputType.ButtonPress, KeyCode.JoystickButton0),
                    new InteractionMapping("B", AxisType.Digital, DeviceInputType.ButtonPress, KeyCode.JoystickButton1),
                    new InteractionMapping("X", AxisType.Digital, DeviceInputType.ButtonPress, KeyCode.JoystickButton2),
                    new InteractionMapping("Y", AxisType.Digital, DeviceInputType.ButtonPress, KeyCode.JoystickButton3),
                };
            }
        }
    }
}