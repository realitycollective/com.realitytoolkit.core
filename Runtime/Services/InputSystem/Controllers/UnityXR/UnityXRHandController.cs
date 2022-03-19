// Copyright (c) Reality Collective. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using XRTK.Definitions.Controllers;
using XRTK.Definitions.Devices;
using XRTK.Definitions.Utilities;
using XRTK.Interfaces.InputSystem.Providers.Controllers;
using XRTK.Interfaces.InputSystem.Providers.Controllers.Hands;

namespace XRTK.Services.InputSystem.Controllers.UnityXR
{
    /// <summary>
    /// A base for hand controllers powered by Unity's XR Plugin Management module.
    /// </summary>
    public abstract class UnityXRHandController : UnityXRController
    {
        /// <inheritdoc />
        public UnityXRHandController() { }

        /// <inheritdoc />
        public UnityXRHandController(IMixedRealityControllerDataProvider controllerDataProvider, TrackingState trackingState, Handedness controllerHandedness, MixedRealityControllerMappingProfile controllerMappingProfile)
            : base(controllerDataProvider, trackingState, controllerHandedness, controllerMappingProfile) { }

        private const string pinchInputName = "Pinch";
        private const string pointInputName = "Point";
        private const string gripInputName = "Grip";
        private const string gripPressInputName = "Grip Press";
        private const string gripPoseInputName = "Grip Pose";
        private const string indexFingerPoseInputName = "Index Finger Pose";
        private const string spatialPointerPoseInputName = "Spatial Pointer Pose";

        protected IUnityXRHandJointDataProvider handJointDataProvider;
        protected IUnityXRHandMeshDataProvider handMeshDataProvider;

        /// <inheritdoc />
        public override MixedRealityInteractionMapping[] DefaultInteractions { get; } =
        {
            new MixedRealityInteractionMapping(pinchInputName, AxisType.Digital, pinchInputName, DeviceInputType.ButtonPress),
            new MixedRealityInteractionMapping(pointInputName, AxisType.Digital, pointInputName, DeviceInputType.ButtonPress),
            new MixedRealityInteractionMapping(gripInputName, AxisType.SingleAxis, gripInputName, DeviceInputType.Trigger),
            new MixedRealityInteractionMapping(gripPressInputName, AxisType.Digital, gripPressInputName, DeviceInputType.ButtonPress),
            new MixedRealityInteractionMapping(gripPoseInputName, AxisType.SixDof, gripPoseInputName, DeviceInputType.SpatialGrip),
            new MixedRealityInteractionMapping(indexFingerPoseInputName, AxisType.SixDof, indexFingerPoseInputName, DeviceInputType.IndexFinger),
            new MixedRealityInteractionMapping(spatialPointerPoseInputName, AxisType.SixDof, spatialPointerPoseInputName, DeviceInputType.SpatialPointer)
        };
    }
}
