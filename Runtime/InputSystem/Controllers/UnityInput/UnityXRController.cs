// Copyright (c) Reality Collective. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using RealityCollective.Definitions.Utilities;
using RealityToolkit.Definitions.Controllers;
using RealityToolkit.Definitions.Devices;
using RealityToolkit.Definitions.Utilities;
using RealityToolkit.InputSystem.Extensions;
using RealityToolkit.InputSystem.Interfaces.Modules;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

namespace RealityToolkit.InputSystem.Controllers.UnityInput
{
    /// <summary>
    /// Abstract base type for all controllers based on Unity's XR Plugin Management module.
    /// </summary>
    public abstract class UnityXRController : BaseController
    {
        /// <inheritdoc />
        public UnityXRController() { }

        /// <inheritdoc />
        public UnityXRController(IMixedRealityControllerServiceModule controllerDataProvider, TrackingState trackingState, Handedness controllerHandedness, MixedRealityControllerMappingProfile controllerMappingProfile)
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
                switch (interactionMapping.AxisType)
                {
                    case AxisType.SingleAxis:
                        UpdateSingleAxisInteractionMapping(interactionMapping);
                        break;
                    case AxisType.Digital:
                        UpdateDigitalInteractionMapping(interactionMapping);
                        break;
                    case AxisType.DualAxis:
                        UpdateDualAxisInteractionMapping(interactionMapping);
                        break;
                    case AxisType.SixDof:
                        {
                            if (interactionMapping.InputType == DeviceInputType.SpatialGrip)
                            {
                                UpdateSpatialGripPoseMapping(interactionMapping);
                            }
                            else if (interactionMapping.InputType == DeviceInputType.SpatialPointer)
                            {
                                UpdateSpatialPointerPoseMapping(interactionMapping);
                            }
                            else
                            {
                                UpdateSixDofInteractionMapping(interactionMapping);
                            }
                        }
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
        protected virtual void UpdateDigitalInteractionMapping(MixedRealityInteractionMapping interactionMapping)
        {
            Debug.Assert(interactionMapping.AxisType == AxisType.Digital);

            if (!DigitalInputFeatureUsageMap.ContainsKey(interactionMapping.InputName))
            {
                Debug.LogError($"Interaction mapping {interactionMapping.InputName} is not handled for controller {GetType().Name} - {ControllerHandedness}.");
                return;
            }

            interactionMapping.BoolData = InputDevice.TryGetFeatureValue(DigitalInputFeatureUsageMap[interactionMapping.InputName], out bool value) && value;
        }

        /// <summary>
        /// Updates the controller's <see cref="AxisType.SixDof"/> mappings.
        /// </summary>
        /// <param name="interactionMapping">The <see cref="MixedRealityInteractionMapping"/> to update.</param>
        protected virtual void UpdateSixDofInteractionMapping(MixedRealityInteractionMapping interactionMapping)
        {
            Debug.Assert(interactionMapping.AxisType == AxisType.SixDof);
        }

        /// <summary>
        /// Updates the controller's <see cref="AxisType.SingleAxis"/> mappings.
        /// </summary>
        /// <param name="interactionMapping">The <see cref="MixedRealityInteractionMapping"/> to update.</param>
        protected virtual void UpdateSingleAxisInteractionMapping(MixedRealityInteractionMapping interactionMapping)
        {
            Debug.Assert(interactionMapping.AxisType == AxisType.SingleAxis);

            if (!SingleAxisInputFeatureUsageMap.ContainsKey(interactionMapping.InputName))
            {
                Debug.LogError($"Interaction mapping {interactionMapping.InputName} is not handled for controller {GetType().Name} - {ControllerHandedness}.");
                return;
            }

            if (InputDevice.TryGetFeatureValue(SingleAxisInputFeatureUsageMap[interactionMapping.InputName], out float value))
            {
                interactionMapping.FloatData = value;
            }
        }

        /// <summary>
        /// Updates the controller's <see cref="AxisType.DualAxis"/> mappings.
        /// </summary>
        /// <param name="interactionMapping">The <see cref="MixedRealityInteractionMapping"/> to update.</param>
        protected virtual void UpdateDualAxisInteractionMapping(MixedRealityInteractionMapping interactionMapping)
        {
            Debug.Assert(interactionMapping.AxisType == AxisType.DualAxis);

            if (!DualAxisInputFeatureUsageMap.ContainsKey(interactionMapping.InputName))
            {
                Debug.LogError($"Interaction mapping {interactionMapping.InputName} is not handled for controller {GetType().Name} - {ControllerHandedness}.");
                return;
            }

            if (InputDevice.TryGetFeatureValue(DualAxisInputFeatureUsageMap[interactionMapping.InputName], out Vector2 value))
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
            else if (InputDevice.TryGetFeatureValue(CommonUsages.trackingState, out var inputTrackingState))
            {
                TrackingState = inputTrackingState == InputTrackingState.None ? TrackingState.NotTracked : TrackingState.Tracked;
            }
            else
            {
                // Fallback to assume we are tracked as long as this controller is being updated.
                TrackingState = TrackingState.Tracked;
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
            if (updatedControllerPose != Pose)
            {
                Pose = updatedControllerPose;

                if (IsPositionAvailable && IsRotationAvailable)
                {
                    InputSystem?.RaiseSourcePoseChanged(InputSource, this, Pose);
                }
                else if (IsPositionAvailable && !IsRotationAvailable)
                {
                    InputSystem?.RaiseSourcePositionChanged(InputSource, this, Pose.Position);
                }
                else if (!IsPositionAvailable && IsRotationAvailable)
                {
                    InputSystem?.RaiseSourceRotationChanged(InputSource, this, Pose.Rotation);
                }
            }
        }

        /// <summary>
        /// Updates the controller's spatial pointer pose.
        /// </summary>
        protected virtual void UpdateSpatialPointerPose()
        {
            SpatialPointerPose = Pose;
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
            SpatialGripPose = Pose;
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
