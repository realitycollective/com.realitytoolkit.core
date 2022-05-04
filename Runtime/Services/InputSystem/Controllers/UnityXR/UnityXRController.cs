// Copyright (c) Reality Collective. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using RealityToolkit.Definitions.Controllers;
using RealityToolkit.Definitions.Devices;
using RealityToolkit.Definitions.Utilities;
using RealityToolkit.Extensions;
using RealityToolkit.Interfaces.InputSystem.Providers.Controllers;
using RealityToolkit.Services.InputSystem.Controllers;

namespace RealityToolkit.Services.InputSystem.Controllers.UnityXR
{
    /// <summary>
    /// Abstract base type for all controllers based on Unity's XR Plugin Management module.
    /// </summary>
    public abstract class UnityXRController : BaseController
    {
        /// <inheritdoc />
        public UnityXRController() { }

        /// <inheritdoc />
        public UnityXRController(IMixedRealityControllerDataProvider controllerDataProvider, TrackingState trackingState, Handedness controllerHandedness, MixedRealityControllerMappingProfile controllerMappingProfile)
            : base(controllerDataProvider, trackingState, controllerHandedness, controllerMappingProfile) { }

        /// <summary>
        /// This dictionary contains <see cref="AxisType.Digital"/> mappings to their respective <see cref="InputFeatureUsage"/> equivalent
        /// used to read the buttons state.
        /// </summary>
        protected virtual IReadOnlyDictionary<string, InputFeatureUsage<bool>> DigitalInputFeatureUsageMap { get; set; } = new Dictionary<string, InputFeatureUsage<bool>>();

        /// <summary>
        /// This dictionary contains <see cref="AxisType.SingleAxis"/> mappings to their respective <see cref="InputFeatureUsage"/> equivalent
        /// used to read the buttons state.
        /// </summary>
        protected virtual IReadOnlyDictionary<string, InputFeatureUsage<float>> SingleAxisInputFeatureUsageMap { get; set; } = new Dictionary<string, InputFeatureUsage<float>>();

        /// <summary>
        /// This dictionary contains <see cref="AxisType.DualAxis"/> mappings to their respective <see cref="InputFeatureUsage"/> equivalent
        /// used to read the buttons state.
        /// </summary>
        protected virtual IReadOnlyDictionary<string, InputFeatureUsage<Vector2>> DualAxisInputFeatureUsageMap { get; set; } = new Dictionary<string, InputFeatureUsage<Vector2>>();

        /// <summary>
        /// The controller's grip pose in world space.
        /// </summary>
        protected MixedRealityPose SpatialGripPose { get; set; }

        /// <summary>
        /// The controller's pointer pose in world space.
        /// </summary>
        protected MixedRealityPose SpatialPointerPose { get; set; }

        /// <summary>
        /// The input device this controller represents.
        /// </summary>
        protected InputDevice InputDevice { get; set; }

        /// <inheritdoc />
        public override void UpdateController()
        {
            if (!Enabled)
            {
                return;
            }

            if (!TryGetInputDevice(out var inputDevice))
            {
                Debug.LogError($"Cannot find input device for {GetType().Name} - {ControllerHandedness}");
                return;
            }

            InputDevice = inputDevice;

            UpdateTrackingState();
            UpdateControllerPose();
            UpdateSpatialPointerPose();
            UpdateSpatialGripPose();
            UpdateInteractionMappings();
        }

        /// <summary>
        /// Reads controller input and updates mappings.
        /// </summary>
        protected virtual void UpdateInteractionMappings()
        {
            Debug.Assert(Interactions != null && Interactions.Length > 0, $"Interaction mappings must be defined for {GetType().Name} - {ControllerHandedness}.");

            for (var i = 0; i < Interactions.Length; i++)
            {
                var interactionMapping = Interactions[i];
                switch (interactionMapping.InputType)
                {
                    case DeviceInputType.Trigger:
                        UpdateSingleAxisInteractionMapping(interactionMapping, InputDevice);
                        break;
                    case DeviceInputType.ButtonPress:
                        UpdateDigitalInteractionMapping(interactionMapping, InputDevice);
                        break;
                    case DeviceInputType.ThumbStick:
                        UpdateDualAxisInteractionMapping(interactionMapping, InputDevice);
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

        /// <summary>
        /// Updates the controller's <see cref="AxisType.Digital"/> mappings.
        /// </summary>
        /// <param name="interactionMapping">The <see cref="MixedRealityInteractionMapping"/> to update.</param>
        /// <param name="inputDevice">The <see cref="InputDevice"/> data is read from.</param>
        protected virtual void UpdateDigitalInteractionMapping(MixedRealityInteractionMapping interactionMapping, InputDevice inputDevice)
        {
            Debug.Assert(interactionMapping.AxisType == AxisType.Digital);

            if (!DigitalInputFeatureUsageMap.ContainsKey(interactionMapping.InputName))
            {
                Debug.LogError($"Interaction mapping {interactionMapping.InputName} is not handled for controller {GetType().Name} - {ControllerHandedness}.");
                return;
            }

            interactionMapping.BoolData = inputDevice.TryGetFeatureValue(DigitalInputFeatureUsageMap[interactionMapping.InputName], out bool value) && value;
        }

        /// <summary>
        /// Updates the controller's <see cref="AxisType.SingleAxis"/> mappings.
        /// </summary>
        /// <param name="interactionMapping">The <see cref="MixedRealityInteractionMapping"/> to update.</param>
        /// <param name="inputDevice">The <see cref="InputDevice"/> data is read from.</param>
        protected virtual void UpdateSingleAxisInteractionMapping(MixedRealityInteractionMapping interactionMapping, InputDevice inputDevice)
        {
            Debug.Assert(interactionMapping.AxisType == AxisType.SingleAxis);

            if (!SingleAxisInputFeatureUsageMap.ContainsKey(interactionMapping.InputName))
            {
                Debug.LogError($"Interaction mapping {interactionMapping.InputName} is not handled for controller {GetType().Name} - {ControllerHandedness}.");
                return;
            }

            if (inputDevice.TryGetFeatureValue(SingleAxisInputFeatureUsageMap[interactionMapping.InputName], out float value))
            {
                interactionMapping.FloatData = value;
            }
        }

        /// <summary>
        /// Updates the controller's <see cref="AxisType.DualAxis"/> mappings.
        /// </summary>
        /// <param name="interactionMapping">The <see cref="MixedRealityInteractionMapping"/> to update.</param>
        /// <param name="inputDevice">The <see cref="InputDevice"/> data is read from.</param>
        protected virtual void UpdateDualAxisInteractionMapping(MixedRealityInteractionMapping interactionMapping, InputDevice inputDevice)
        {
            Debug.Assert(interactionMapping.AxisType == AxisType.DualAxis);

            if (!DualAxisInputFeatureUsageMap.ContainsKey(interactionMapping.InputName))
            {
                Debug.LogError($"Interaction mapping {interactionMapping.InputName} is not handled for controller {GetType().Name} - {ControllerHandedness}.");
                return;
            }

            if (inputDevice.TryGetFeatureValue(DualAxisInputFeatureUsageMap[interactionMapping.InputName], out Vector2 value))
            {
                interactionMapping.Vector2Data = value;
            }
        }

        /// <summary>
        /// Updates the controller's <see cref="TrackingState"/>.
        /// </summary>
        protected virtual void UpdateTrackingState()
        {
            var currentTrackingState = TrackingState;
            if (InputDevice.TryGetFeatureValue(CommonUsages.isTracked, out var isTracked))
            {
                TrackingState = isTracked ? TrackingState.Tracked : TrackingState.NotTracked;
            }

            if (TrackingState != currentTrackingState)
            {
                InputSystem?.RaiseSourceTrackingStateChanged(InputSource, this, TrackingState);
            }
        }

        /// <summary>
        /// Updates the controller's pose.
        /// </summary>
        protected virtual void UpdateControllerPose()
        {
            if (TrackingState != TrackingState.Tracked)
            {
                IsPositionAvailable = false;
                IsPositionApproximate = false;
                IsRotationAvailable = false;
                return;
            }

            IsPositionAvailable = InputDevice.TryGetFeatureValue(CommonUsages.devicePosition, out var position);
            IsRotationAvailable = InputDevice.TryGetFeatureValue(CommonUsages.deviceRotation, out var rotation);
            IsPositionApproximate = false;

            var updatedControllerPose = new MixedRealityPose(position, rotation);
            if (updatedControllerPose != ControllerPose)
            {
                ControllerPose = updatedControllerPose;
                InputSystem?.RaiseSourcePoseChanged(InputSource, this, ControllerPose);
            }
        }

        /// <summary>
        /// Updates the controller's spatial pointer pose.
        /// </summary>
        protected virtual void UpdateSpatialPointerPose()
        {
            SpatialPointerPose = ControllerPose;
        }

        /// <summary>
        /// Updates the spatial pointer pose interaction mapping value.
        /// </summary>
        /// <param name="interactionMapping">The spatial pointer pose mapping.</param>
        protected void UpdateSpatialPointerPoseMapping(MixedRealityInteractionMapping interactionMapping)
        {
            Debug.Assert(interactionMapping.AxisType == AxisType.SixDof);
            interactionMapping.PoseData = SpatialPointerPose;
        }

        /// <summary>
        /// Updates the controller's grip pose.
        /// </summary>
        protected virtual void UpdateSpatialGripPose()
        {
            SpatialGripPose = ControllerPose;
        }

        /// <summary>
        /// Updates the grip pose interaction mapping value.
        /// </summary>
        /// <param name="interactionMapping">The grip pose mapping.</param>
        protected void UpdateSpatialGripPoseMapping(MixedRealityInteractionMapping interactionMapping)
        {
            Debug.Assert(interactionMapping.AxisType == AxisType.SixDof);
            interactionMapping.PoseData = SpatialGripPose;
        }

        /// <summary>
        /// Gets the input device for this controller.
        /// </summary>
        /// <param name="inputDevice"><see cref="InputDevice"/> providing controller data.</param>
        /// <returns><c>True</c>, if device found.</returns>
        protected bool TryGetInputDevice(out InputDevice inputDevice)
        {
            switch (ControllerHandedness)
            {
                case Handedness.Left:
                    inputDevice = InputDevices.GetDeviceAtXRNode(XRNode.LeftHand);
                    return inputDevice != default;
                case Handedness.Right:
                    inputDevice = InputDevices.GetDeviceAtXRNode(XRNode.RightHand);
                    return inputDevice != default;
                case Handedness.None:
                case Handedness.Both:
                case Handedness.Any:
                    inputDevice = InputDevices.GetDeviceAtXRNode(XRNode.GameController);
                    return inputDevice != default;
                case Handedness.Other:
                default:
                    inputDevice = default;
                    return false;
            }
        }
    }
}
