// Copyright (c) Reality Collective. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;
using XRTK.Definitions.Controllers;
using XRTK.Definitions.Devices;
using XRTK.Definitions.Utilities;
using XRTK.Extensions;
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

        private const string pinchInputName = "Pinch";
        private const string pointInputName = "Point";
        private const string gripInputName = "Grip";
        private const string gripPressInputName = "Grip Press";
        private const string gripPoseInputName = "Grip Pose";
        private const string indexFingerPoseInputName = "Index Finger Pose";
        private const string spatialPointerPoseInputName = "Spatial Pointer Pose";

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

        /// <inheritdoc />
        public override MixedRealityInteractionMapping[] DefaultLeftHandedInteractions => DefaultInteractions;

        /// <inheritdoc />
        public override MixedRealityInteractionMapping[] DefaultRightHandedInteractions => DefaultInteractions;

        /// <inheritdoc />
        protected override void UpdateInteractionMappings()
        {
            Debug.Assert(Interactions != null && Interactions.Length > 0, $"Interaction mappings must be defined for {GetType().Name} - {ControllerHandedness}.");

            if (!TryGetInputDevice(out var inputDevice))
            {
                Debug.LogError($"Cannot find input device for {GetType().Name} - {ControllerHandedness}");
                return;
            }

            for (var i = 0; i < Interactions.Length; i++)
            {
                var interactionMapping = Interactions[i];
                switch (interactionMapping.InputType)
                {
                    case DeviceInputType.Trigger:
                        UpdateSingleAxisInteractionMapping(interactionMapping, inputDevice);
                        break;
                    case DeviceInputType.ButtonPress:
                        UpdateDigitalInteractionMapping(interactionMapping, inputDevice);
                        break;
                    case DeviceInputType.SpatialGrip:
                        UpdateSpatialGripPoseMapping(interactionMapping);
                        break;
                    case DeviceInputType.SpatialPointer:
                        UpdateSpatialPointerPoseMapping(interactionMapping);
                        break;
                    default:
                        Debug.LogError($"Input {interactionMapping.InputType} is not handled for controller {GetType().Name} - {ControllerHandedness}.");
                        break;
                }

                interactionMapping.RaiseInputAction(InputSource, ControllerHandedness);
            }
        }
    }
}
