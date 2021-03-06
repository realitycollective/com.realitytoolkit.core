// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using RealityToolkit.Definitions.Controllers;
using RealityToolkit.Definitions.Devices;
using RealityCollective.Definitions.Utilities;
using RealityToolkit.Interfaces.InputSystem.Providers.Controllers;
using UnityEngine;

namespace RealityToolkit.Services.InputSystem.Controllers.OpenVR
{
    [System.Runtime.InteropServices.Guid("24A7E9CA-43F0-487A-9E2B-609CCEDF2756")]
    public class OculusRemoteOpenVRController : GenericOpenVRController
    {
        /// <inheritdoc />
        public OculusRemoteOpenVRController() { }

        /// <inheritdoc />
        public OculusRemoteOpenVRController(IMixedRealityControllerDataProvider controllerDataProvider, TrackingState trackingState, Handedness controllerHandedness, MixedRealityControllerMappingProfile controllerMappingProfile)
            : base(controllerDataProvider, trackingState, controllerHandedness, controllerMappingProfile)
        {
        }

        /// <inheritdoc />
        public override MixedRealityInteractionMapping[] DefaultInteractions => new[]
        {
            new MixedRealityInteractionMapping("D-Pad Position", AxisType.DualAxis, DeviceInputType.DirectionalPad, ControllerMappingLibrary.AXIS_5, ControllerMappingLibrary.AXIS_6),
            new MixedRealityInteractionMapping("Button.One", AxisType.Digital, DeviceInputType.ButtonPress, KeyCode.JoystickButton0),
            new MixedRealityInteractionMapping("Button.Two", AxisType.Digital, DeviceInputType.ButtonPress, KeyCode.JoystickButton1),
        };
    }
}
