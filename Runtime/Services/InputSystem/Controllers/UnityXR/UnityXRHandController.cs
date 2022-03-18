// Copyright (c) Reality Collective. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using XRTK.Definitions.Controllers;
using XRTK.Definitions.Devices;
using XRTK.Definitions.Utilities;
using XRTK.Interfaces.InputSystem.Providers.Controllers;

namespace XRTK.Services.InputSystem.Controllers.UnityXR
{
    /// <summary>
    /// A hand controller powered by Unity's XR Plugin Management module.
    /// </summary>
    [System.Runtime.InteropServices.Guid("dac6f2b1-5375-40ac-a033-7f73f0a39e1d")]
    public class UnityXRHandController : UnityXRController
    {
        /// <inheritdoc />
        public UnityXRHandController() { }

        /// <inheritdoc />
        public UnityXRHandController(IMixedRealityControllerDataProvider controllerDataProvider, TrackingState trackingState, Handedness controllerHandedness, MixedRealityControllerMappingProfile controllerMappingProfile)
            : base(controllerDataProvider, trackingState, controllerHandedness, controllerMappingProfile) { }

        /// <inheritdoc />
        public override MixedRealityInteractionMapping[] DefaultInteractions { get; } =
        {
            // 6 DoF pose of the spatial pointer ("far interaction pointer").
            new MixedRealityInteractionMapping("Spatial Pointer Pose", AxisType.SixDof, DeviceInputType.SpatialPointer),
            // Select / pinch button press / release.
            new MixedRealityInteractionMapping("Select", AxisType.Digital, DeviceInputType.Select),
            // Hand in pointing pose yes/no?
            new MixedRealityInteractionMapping("Point", AxisType.Digital, DeviceInputType.ButtonPress),
            // Grip / grab button press / release.
            new MixedRealityInteractionMapping("Grip", AxisType.Digital, DeviceInputType.TriggerPress),
            // 6 DoF grip pose ("Where to put things when grabbing something?")
            new MixedRealityInteractionMapping("Grip Pose", AxisType.SixDof, DeviceInputType.SpatialGrip),
            // 6 DoF index finger tip pose (mainly for "near interaction pointer").
            new MixedRealityInteractionMapping("Index Finger Pose", AxisType.SixDof, DeviceInputType.IndexFinger)
        };

        /// <inheritdoc />
        public override MixedRealityInteractionMapping[] DefaultLeftHandedInteractions => DefaultInteractions;

        /// <inheritdoc />
        public override MixedRealityInteractionMapping[] DefaultRightHandedInteractions => DefaultInteractions;

        /// <inheritdoc />
        protected override void UpdateInteractionMappings()
        {
            // TODO: Implement mappings.
        }
    }
}
