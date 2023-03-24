// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using RealityCollective.Definitions.Utilities;
using RealityCollective.ServiceFramework.Services;
using RealityToolkit.CameraService.Interfaces;
using RealityToolkit.Definitions.Controllers.Hands;
using RealityToolkit.Definitions.Devices;
using RealityToolkit.InputSystem.Interfaces.Controllers.Hands;
using RealityToolkit.Utilities;
using System.Collections.Generic;
using UnityEngine;

namespace RealityToolkit.InputSystem.Controllers.Hands
{
    /// <summary>
    /// The hand data post processor updates <see cref="HandData"/> provided
    /// by a platform and enriches it with potentially missing information.
    /// </summary>
    public sealed class HandDataPostProcessor : IHandDataPostProcessor
    {
        /// <summary>
        /// Creates a new instance of the hand data post processor.
        /// </summary>
        /// <param name="trackedPoses">Pose recognizer instance to use for pose recognition.</param>
        /// <param name="isGrippingThreshold">Threshold in range [0, 1] that defines when a hand is considered to be grabing.</param>
        public HandDataPostProcessor(IReadOnlyList<HandControllerPoseProfile> trackedPoses, float isGrippingThreshold)
        {
            TrackedPoseProcessor = new HandTrackedPosePostProcessor(trackedPoses);
            GripPostProcessor = new HandGripPostProcessor(isGrippingThreshold);
        }

        private const float IS_POINTING_DOTP_THRESHOLD = .1f;
        private const float TWO_CENTIMETER_SQUARE_MAGNITUDE = 0.0004f;
        private const float FIVE_CENTIMETER_SQUARE_MAGNITUDE = 0.0025f;
        private const float PINCH_STRENGTH_DISTANCE = FIVE_CENTIMETER_SQUARE_MAGNITUDE - TWO_CENTIMETER_SQUARE_MAGNITUDE;

        private static ICameraService cameraSystem = null;

        private static ICameraService CameraSystem
            => cameraSystem ?? (cameraSystem = ServiceManager.Instance.GetService<ICameraService>());

        private static Camera playerCamera = null;

        private static Camera PlayerCamera
        {
            get
            {
                if (playerCamera == null)
                {
                    playerCamera = CameraSystem != null ? CameraSystem.CameraRig.PlayerCamera : Camera.main;
                }

                return playerCamera;
            }
        }

        /// <summary>
        /// Processor instance used for pose recognition.
        /// </summary>
        private HandTrackedPosePostProcessor TrackedPoseProcessor { get; }

        /// <summary>
        /// Grip post processor instance used for grip estimation.
        /// </summary>
        private HandGripPostProcessor GripPostProcessor { get; }

        /// <summary>
        /// Is <see cref="HandData.PointerPose"/> provided by the platform?
        /// </summary>
        public bool PlatformProvidesPointerPose { get; set; }

        /// <summary>
        /// Is <see cref="HandData.IsPinching"/> provided by the platform?
        /// </summary>
        public bool PlatformProvidesIsPinching { get; set; }

        /// <summary>
        /// Is <see cref="HandData.PinchStrength"/> provided by the platform?
        /// </summary>
        public bool PlatformProvidesPinchStrength { get; set; }

        /// <summary>
        /// Is <see cref="HandData.IsPointing"/> provided by the platform?
        /// </summary>
        public bool PlatformProvidesIsPointing { get; set; }

        /// <inheritdoc />
        public HandData PostProcess(Handedness handedness, HandData handData)
        {
            handData = UpdateIsPinchingAndStrength(handData);
            handData = UpdateIsPointing(handData);
            handData = UpdatePointerPose(handData);
            handData = GripPostProcessor.PostProcess(handedness, handData);
            handData = TrackedPoseProcessor.PostProcess(handedness, handData);

            return handData;
        }

        /// <summary>
        /// Updates <see cref="HandData.IsPinching"/> and <see cref="HandData.PinchStrength"/>
        /// if the platform did not provide it.
        /// </summary>
        /// <param name="handData">The hand data to update <see cref="HandData.IsPinching"/> and <see cref="HandData.PinchStrength"/> for.</param>
        private HandData UpdateIsPinchingAndStrength(HandData handData)
        {
            if (handData.TrackingState == TrackingState.Tracked)
            {
                var thumbTipPose = handData.Joints[(int)TrackedHandJoint.ThumbTip];
                var indexTipPose = handData.Joints[(int)TrackedHandJoint.IndexTip];

                if (!PlatformProvidesIsPinching)
                {
                    handData.IsPinching = (thumbTipPose.position - indexTipPose.position).sqrMagnitude < TWO_CENTIMETER_SQUARE_MAGNITUDE;
                }

                if (!PlatformProvidesPinchStrength)
                {
                    var distanceSquareMagnitude = (thumbTipPose.position - indexTipPose.position).sqrMagnitude - TWO_CENTIMETER_SQUARE_MAGNITUDE;
                    handData.PinchStrength = 1 - Mathf.Clamp(distanceSquareMagnitude / PINCH_STRENGTH_DISTANCE, 0f, 1f);
                }
            }
            else
            {
                handData.IsPinching = false;
                handData.PinchStrength = 0f;
            }

            return handData;
        }

        /// <summary>
        /// Updates <see cref="HandData.IsPointing"/> if the platform did not provide it.
        /// </summary>
        /// <param name="handData">The hand data to update <see cref="HandData.IsPointing"/> for.</param>
        private HandData UpdateIsPointing(HandData handData)
        {
            if (handData.TrackingState == TrackingState.Tracked && !PlatformProvidesIsPointing)
            {
                var rigTransform = CameraSystem != null
                    ? CameraSystem.CameraRig.RigTransform
                    : Camera.main.transform.parent;
                var localPalmPose = handData.Joints[(int)TrackedHandJoint.Palm];
                var worldPalmPose = new Pose
                {
                    position = handData.RootPose.position + handData.RootPose.rotation * localPalmPose.position,
                    rotation = rigTransform.rotation * handData.RootPose.rotation * localPalmPose.rotation
                };

                // We check if the palm forward is roughly in line with the camera lookAt.
                var projectedPalmUp = Vector3.ProjectOnPlane(-worldPalmPose.up, PlayerCamera.transform.up);
                handData.IsPointing = Vector3.Dot(PlayerCamera.transform.forward, projectedPalmUp) > IS_POINTING_DOTP_THRESHOLD;
            }
            else
            {
                handData.IsPointing = false;
            }

            return handData;
        }

        /// <summary>
        /// Updates <see cref="HandData.PointerPose"/> if the platform did not provide it.
        /// </summary>
        /// <param name="handData">The hand data to update <see cref="HandData.PointerPose"/> for.</param>
        private HandData UpdatePointerPose(HandData handData)
        {
            if (handData.TrackingState == TrackingState.Tracked && !PlatformProvidesPointerPose)
            {
                var palmPose = handData.Joints[(int)TrackedHandJoint.Palm];
                palmPose.rotation = Quaternion.Inverse(palmPose.rotation) * palmPose.rotation;

                var thumbProximalPose = handData.Joints[(int)TrackedHandJoint.ThumbProximal];
                var indexDistalPose = handData.Joints[(int)TrackedHandJoint.IndexDistal];
                var pointerPosition = handData.RootPose.position + Vector3.Lerp(thumbProximalPose.position, indexDistalPose.position, .5f);
                var pointerEndPosition = pointerPosition + palmPose.forward * 10f;
                var pointerDirection = pointerEndPosition - pointerPosition;
                var pointerRotation = Quaternion.LookRotation(pointerDirection, PlayerCamera.transform.up) * handData.RootPose.rotation;

                pointerRotation = PlayerCamera.transform.rotation * pointerRotation;
                handData.PointerPose = new Pose(pointerPosition, pointerRotation);
            }

            return handData;
        }
    }
}
