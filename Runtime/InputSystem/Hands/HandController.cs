// Copyright (c) Reality Collective. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using RealityCollective.Definitions.Utilities;
using RealityCollective.ServiceFramework.Services;
using RealityToolkit.CameraSystem.Interfaces;
using RealityToolkit.Definitions.Controllers;
using RealityToolkit.Definitions.Devices;
using RealityToolkit.Definitions.Utilities;
using RealityToolkit.InputSystem.Controllers.UnityInput;
using RealityToolkit.InputSystem.Extensions;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

namespace RealityToolkit.InputSystem.Hands
{
    /// <summary>
    /// Default controller implementation for <see cref="IHandController"/>.
    /// </summary>
    [System.Runtime.InteropServices.Guid("B18A9A6C-E5FD-40AE-89E9-9822415EC62B")]
    public class HandController : UnityXRController, IHandController
    {
        /// <inheritdoc />
        public HandController() { }

        /// <inheritdoc />
        public HandController(IHandControllerServiceModule serviceModule, TrackingState trackingState, Handedness controllerHandedness, MixedRealityControllerMappingProfile controllerMappingProfile)
            : base(serviceModule, trackingState, controllerHandedness, controllerMappingProfile)
        {
            trackedHandJointPoseProvider = new TrackedHandJointPoseProvider();

            if (!ServiceManager.Instance.TryGetService<IMixedRealityCameraSystem>(out var cameraSystem))
            {
                Debug.LogError($"The {GetType().Name} requires a valid {nameof(IMixedRealityCameraSystem)} to work.");
                return;
            }

            cameraRig = cameraSystem.MainCameraRig;
            handBoundsLOD = serviceModule.BoundsMode;
        }

        private const string pinchPressInputName = "Pinch";
        private const string pointInputName = "Point";
        private const string gripInputName = "Grip";
        private const string gripPressInputName = "Grip Press";
        private const string gripPoseInputName = "Grip Pose";
        private const string indexFingerPoseInputName = "Index Finger Pose";
        private const string spatialPointerPoseInputName = "Spatial Pointer Pose";
        private const int poseFrameBufferSize = 5;

        private readonly HandBoundsLOD handBoundsLOD;
        private MixedRealityPose[] jointPoses = new MixedRealityPose[Enum.GetNames(typeof(TrackedHandJoint)).Length];
        private Dictionary<TrackedHandJoint, MixedRealityPose> jointPosesDict = new Dictionary<TrackedHandJoint, MixedRealityPose>();
        private readonly HandBoundsProvider boundsProvider = new HandBoundsProvider();
        protected ITrackedHandJointPoseProvider trackedHandJointPoseProvider;
        protected IMixedRealityCameraRig cameraRig;

        /// <inheritdoc />
        public bool IsPinching { get; private set; }

        /// <inheritdoc />
        public float PinchStrength { get; private set; }

        /// <inheritdoc />
        public bool IsPointing { get; private set; }

        /// <inheritdoc />
        public bool IsGripping { get; private set; }

        /// <inheritdoc />
        public float GripStrength { get; private set; }

        /// <inheritdoc />
        public override MixedRealityInteractionMapping[] DefaultInteractions { get; } =
        {
            new MixedRealityInteractionMapping(pinchPressInputName, AxisType.Digital, pinchPressInputName, DeviceInputType.ButtonPress),
            new MixedRealityInteractionMapping(pointInputName, AxisType.Digital, pointInputName, DeviceInputType.ButtonPress),
            new MixedRealityInteractionMapping(gripInputName, AxisType.SingleAxis, gripInputName, DeviceInputType.Trigger),
            new MixedRealityInteractionMapping(gripPressInputName, AxisType.Digital, gripPressInputName, DeviceInputType.ButtonPress),
            new MixedRealityInteractionMapping(gripPoseInputName, AxisType.SixDof, gripPoseInputName, DeviceInputType.SpatialGrip),
            new MixedRealityInteractionMapping(indexFingerPoseInputName, AxisType.SixDof, indexFingerPoseInputName, DeviceInputType.IndexFinger),
            new MixedRealityInteractionMapping(spatialPointerPoseInputName, AxisType.SixDof, spatialPointerPoseInputName, DeviceInputType.SpatialPointer)
        };

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

            if (TrackingState == TrackingState.Tracked)
            {
                UpdateHandJoints();
                UpdateIsPinching();
                UpdatePinchStrength();
                UpdateIsPointing();
                UpdateIsGripping();
                UpdateGripStrength();
                UpdateControllerPose();
                UpdateSpatialPointerPose();
                UpdateSpatialGripPose();
                UpdateBounds();
            }

            UpdateInteractionMappings();
        }

        /// <inheritdoc />
        public bool TryGetBounds(TrackedHandBounds handBounds, out Bounds[] newBounds) => boundsProvider.Bounds.TryGetValue(handBounds, out newBounds);

        /// <inheritdoc />
        public bool TryGetJointPose(TrackedHandJoint joint, out MixedRealityPose pose, Space relativeTo = Space.Self)
        {
            if (relativeTo == Space.Self)
            {
                // Return joint pose relative to hand root.
                return jointPosesDict.TryGetValue(joint, out pose);
            }

            if (jointPosesDict.TryGetValue(joint, out var localPose))
            {
                pose = new MixedRealityPose
                {
                    // Combine root pose with local joint pose.
                    Position = Pose.Position + Pose.Rotation * localPose.Position,
                    Rotation = Pose.Rotation * localPose.Rotation
                };

                // Translate to world space.
                pose.Position = cameraRig.RigTransform.TransformPoint(pose.Position);
                pose.Rotation = cameraRig.RigTransform.rotation * pose.Rotation;

                return true;
            }

            pose = MixedRealityPose.ZeroIdentity;
            return false;
        }

        #region Internal State Updates

        /// <summary>
        /// Updates the controller's bounds calculations.
        /// </summary>
        protected virtual void UpdateBounds()
        {
            boundsProvider.Update(handBoundsLOD, ref jointPosesDict);
        }

        /// <summary>
        /// Updates the <see cref="IHandController.IsPinching"/> value.
        /// </summary>
        protected virtual void UpdateIsPinching()
        {
            var thumbTipPose = jointPoses[(int)TrackedHandJoint.ThumbTip];
            var indexTipPose = jointPoses[(int)TrackedHandJoint.IndexTip];

            const float thumbIndexPinchDistanceThreshold = 0.0004f;
            IsPinching = (thumbTipPose.Position - indexTipPose.Position).sqrMagnitude < thumbIndexPinchDistanceThreshold;
        }

        /// <summary>
        /// Updates the <see cref="IHandController.PinchStrength"/> value.
        /// </summary>
        protected virtual void UpdatePinchStrength()
        {
            var thumbTipPose = jointPoses[(int)TrackedHandJoint.ThumbTip];
            var indexTipPose = jointPoses[(int)TrackedHandJoint.IndexTip];

            const float fullPinchThreshold = 0.0004f;
            const float noPinchThreshold = 0.0025f;
            const float pinchStrengthDistance = noPinchThreshold - fullPinchThreshold;

            var distanceSquareMagnitude = (thumbTipPose.Position - indexTipPose.Position).sqrMagnitude - fullPinchThreshold;
            PinchStrength = 1 - Mathf.Clamp(distanceSquareMagnitude / pinchStrengthDistance, 0f, 1f);
        }

        /// <summary>
        /// Updates the <see cref="IHandController.IsPointing"/> value.
        /// </summary>
        protected virtual void UpdateIsPointing()
        {
            if (TryGetJointPose(TrackedHandJoint.Palm, out var localPalmPose))
            {
                IsPointing = false;
                return;
            }

            var worldPalmPose = new MixedRealityPose
            {
                Position = localPalmPose.Position,
                Rotation = cameraRig.RigTransform.rotation * localPalmPose.Rotation
            };

            // We check if the palm forward is roughly in line with the camera lookAt.
            const float isPointingDotProductThreshold = .1f;
            var projectedPalmUp = Vector3.ProjectOnPlane(-worldPalmPose.Up, cameraRig.CameraTransform.up);
            IsPointing = Vector3.Dot(cameraRig.CameraTransform.forward, projectedPalmUp) > isPointingDotProductThreshold;
        }

        /// <summary>
        /// Updates the <see cref="IHandController.IsGripping"/> value.
        /// </summary>
        protected virtual void UpdateIsGripping()
        {
            if (InputDevice.TryGetFeatureValue(CommonUsages.gripButton, out var isGripping))
            {
                IsGripping = isGripping;
                return;
            }

            IsGripping = false;
        }

        /// <summary>
        /// Updates the <see cref="IHandController.GripStrength"/> value.
        /// </summary>
        protected virtual void UpdateGripStrength()
        {
            if (InputDevice.TryGetFeatureValue(CommonUsages.grip, out var gripStrength))
            {
                GripStrength = gripStrength;
                return;
            }

            GripStrength = 0f;
        }

        /// <inheritdoc />
        protected override void UpdateControllerPose()
        {
            if (TrackingState != TrackingState.Tracked)
            {
                IsPositionAvailable = false;
                IsPositionApproximate = false;
                IsRotationAvailable = false;
                return;
            }

            IsPositionAvailable = TryGetJointPose(TrackedHandJoint.Wrist, out var wristPose);
            IsRotationAvailable = IsPositionAvailable;
            IsPositionApproximate = false;

            if (wristPose != Pose)
            {
                Pose = wristPose;

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
        /// Updates the controller's hand joint information.
        /// </summary>
        protected virtual void UpdateHandJoints()
        {
            Debug.Assert(trackedHandJointPoseProvider != null, $"{GetType().Name} has no {nameof(ITrackedHandJointPoseProvider)} to work with.");
            trackedHandJointPoseProvider.UpdateHandJoints(InputDevice, ref jointPoses, ref jointPosesDict);
        }

        /// <inheritdoc/>
        protected override void UpdateSpatialPointerPose()
        {
            var palmPose = jointPosesDict[TrackedHandJoint.Palm];
            var wristPose = jointPosesDict[TrackedHandJoint.Wrist];

            palmPose.Rotation = Quaternion.Inverse(palmPose.Rotation) * palmPose.Rotation;

            var thumbProximalPose = jointPoses[(int)TrackedHandJoint.ThumbProximal];
            var indexDistalPose = jointPoses[(int)TrackedHandJoint.IndexDistal];
            var pointerPosition = Vector3.Lerp(thumbProximalPose.Position, indexDistalPose.Position, .5f);

            var forward = wristPose.Forward;
            forward.y = palmPose.Forward.y;
            var pointerEndPosition = pointerPosition + forward;
            var pointerDirection = (pointerEndPosition - pointerPosition).normalized;
            var pointerRotation = Quaternion.LookRotation(pointerDirection);

            pointerRotation = cameraRig.CameraTransform.rotation * pointerRotation;
            SpatialPointerPose = new MixedRealityPose(pointerPosition, pointerRotation);
        }

        #endregion

        #region Interaction Mappings

        /// <inheritdoc />
        protected override void UpdateSingleAxisInteractionMapping(MixedRealityInteractionMapping interactionMapping)
        {
            Debug.Assert(interactionMapping.AxisType == AxisType.SingleAxis);
            switch (interactionMapping.InputName)
            {
                case gripInputName: UpdateGripStrengthInteractionMapping(interactionMapping); break;
            }

            interactionMapping.RaiseInputAction(InputSource, ControllerHandedness);
        }

        /// <inheritdoc />
        protected override void UpdateSixDofInteractionMapping(MixedRealityInteractionMapping interactionMapping)
        {
            Debug.Assert(interactionMapping.AxisType == AxisType.SixDof);
            switch (interactionMapping.InputName)
            {
                case indexFingerPoseInputName: UpdateIndexFingerPoseInteractionMapping(interactionMapping); break;
            }
        }

        /// <inheritdoc />
        protected override void UpdateDigitalInteractionMapping(MixedRealityInteractionMapping interactionMapping)
        {
            Debug.Assert(interactionMapping.AxisType == AxisType.Digital);
            switch (interactionMapping.InputName)
            {
                case pointInputName: UpdateIsPointingInteractionMapping(interactionMapping); break;
                case pinchPressInputName: UpdateIsPinchingInteractionMapping(interactionMapping); break;
                case gripPressInputName: UpdateIsGrippingInteractionMapping(interactionMapping); break;
            }
        }

        /// <summary>
        /// Updates the <see cref="IHandController"/>'s is pointing interaciton mapping.
        /// </summary>
        /// <param name="interactionMapping"><see cref="MixedRealityInteractionMapping"/>.</param>
        protected virtual void UpdateIsPointingInteractionMapping(MixedRealityInteractionMapping interactionMapping)
        {
            Debug.Assert(string.Equals(interactionMapping.InputName, pointInputName));
            interactionMapping.BoolData = IsPointing;
        }

        /// <summary>
        /// Updates the <see cref="IHandController"/>'s is pinching interaciton mapping.
        /// </summary>
        /// <param name="interactionMapping"><see cref="MixedRealityInteractionMapping"/>.</param>
        protected virtual void UpdateIsPinchingInteractionMapping(MixedRealityInteractionMapping interactionMapping)
        {
            Debug.Assert(string.Equals(interactionMapping.InputName, pinchPressInputName));
            interactionMapping.BoolData = IsPinching;
        }

        /// <summary>
        /// Updates the <see cref="IHandController"/>'s is gripping interaciton mapping.
        /// </summary>
        /// <param name="interactionMapping"><see cref="MixedRealityInteractionMapping"/>.</param>
        protected virtual void UpdateIsGrippingInteractionMapping(MixedRealityInteractionMapping interactionMapping)
        {
            Debug.Assert(string.Equals(interactionMapping.InputName, gripPressInputName));
            interactionMapping.BoolData = IsGripping;
        }

        /// <summary>
        /// Updates the <see cref="IHandController"/>'s grip strength interaciton mapping.
        /// </summary>
        /// <param name="interactionMapping"><see cref="MixedRealityInteractionMapping"/>.</param>
        protected virtual void UpdateGripStrengthInteractionMapping(MixedRealityInteractionMapping interactionMapping)
        {
            Debug.Assert(string.Equals(interactionMapping.InputName, gripInputName));
            interactionMapping.FloatData = GripStrength;
        }

        /// <summary>
        /// Updates the <see cref="IHandController"/>'s index finger pose interaciton mapping.
        /// </summary>
        /// <param name="interactionMapping"><see cref="MixedRealityInteractionMapping"/>.</param>
        protected virtual void UpdateIndexFingerPoseInteractionMapping(MixedRealityInteractionMapping interactionMapping)
        {
            Debug.Assert(string.Equals(interactionMapping.InputName, indexFingerPoseInputName));

            if (TryGetJointPose(TrackedHandJoint.IndexTip, out var indexTipPose))
            {
                interactionMapping.PoseData = indexTipPose;
            }
        }

        #endregion
    }
}
