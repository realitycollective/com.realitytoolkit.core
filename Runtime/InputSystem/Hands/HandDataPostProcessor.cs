// Copyright (c) Reality Collective. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using RealityCollective.ServiceFramework.Services;
using RealityToolkit.CameraSystem.Interfaces;
using RealityToolkit.Definitions.Utilities;
using RealityToolkit.Utilities;
using UnityEngine;

namespace RealityToolkit.InputSystem.Hands
{
    /// <summary>
    /// The hand data post processor updates <see cref="HandData"/> provided
    /// by a platform and enriches it with potentially missing information.
    /// </summary>
    public sealed class HandDataPostProcessor : BaseHandPostProcessor
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="handController">The <see cref="IHandController"/> to post process <see cref="HandData"/> for.</param>
        /// <param name="handControllerSettings">Configuration to use when post processing information for the <see cref="IHandController"/>.</param>
        public HandDataPostProcessor(IHandController handController, HandControllerSettings handControllerSettings)
            : base(handController, handControllerSettings) { }

        private const float IS_POINTING_DOTP_THRESHOLD = .1f;
        private const float TWO_CENTIMETER_SQUARE_MAGNITUDE = 0.0004f;
        private const float FIVE_CENTIMETER_SQUARE_MAGNITUDE = 0.0025f;
        private const float PINCH_STRENGTH_DISTANCE = FIVE_CENTIMETER_SQUARE_MAGNITUDE - TWO_CENTIMETER_SQUARE_MAGNITUDE;

        private static IMixedRealityCameraSystem cameraSystem = null;

        private static IMixedRealityCameraSystem CameraSystem
            => cameraSystem ?? (cameraSystem = ServiceManager.Instance.GetService<IMixedRealityCameraSystem>());

        private static Camera playerCamera = null;

        private static Camera PlayerCamera
        {
            get
            {
                if (playerCamera == null)
                {
                    playerCamera = CameraSystem != null ? CameraSystem.MainCameraRig.PlayerCamera : CameraCache.Main;
                }

                return playerCamera;
            }
        }

        /// <inheritdoc />
        public override HandData PostProcess(HandData handData)
        {
            handData = UpdateIsPinchingAndStrength(handData);
            handData = UpdateIsPointing(handData);
            handData = UpdatePointerPose(handData);

            return handData;
        }

        /// <summary>
        /// Updates <see cref="HandData.IsPinching"/> and <see cref="HandData.PinchStrength"/>
        /// if the platform did not provide it.
        /// </summary>
        /// <param name="handData">The hand data to update <see cref="HandData.IsPinching"/> and <see cref="HandData.PinchStrength"/> for.</param>
        private HandData UpdateIsPinchingAndStrength(HandData handData)
        {
            var thumbTipPose = handData.Joints[(int)TrackedHandJoint.ThumbTip];
            var indexTipPose = handData.Joints[(int)TrackedHandJoint.IndexTip];

            handData.IsPinching = (thumbTipPose.Position - indexTipPose.Position).sqrMagnitude < TWO_CENTIMETER_SQUARE_MAGNITUDE;

            var distanceSquareMagnitude = (thumbTipPose.Position - indexTipPose.Position).sqrMagnitude - TWO_CENTIMETER_SQUARE_MAGNITUDE;
            handData.PinchStrength = 1 - Mathf.Clamp(distanceSquareMagnitude / PINCH_STRENGTH_DISTANCE, 0f, 1f);

            return handData;
        }

        /// <summary>
        /// Updates <see cref="HandData.IsPointing"/> if the platform did not provide it.
        /// </summary>
        /// <param name="handData">The hand data to update <see cref="HandData.IsPointing"/> for.</param>
        private HandData UpdateIsPointing(HandData handData)
        {
            var rigTransform = CameraSystem != null
                ? CameraSystem.MainCameraRig.RigTransform
                : CameraCache.Main.transform.parent;
            var localPalmPose = handData.Joints[(int)TrackedHandJoint.Palm];
            var worldPalmPose = new MixedRealityPose
            {
                Position = localPalmPose.Position,
                Rotation = rigTransform.rotation * localPalmPose.Rotation
            };

            // We check if the palm forward is roughly in line with the camera lookAt.
            var projectedPalmUp = Vector3.ProjectOnPlane(-worldPalmPose.Up, PlayerCamera.transform.up);
            handData.IsPointing = Vector3.Dot(PlayerCamera.transform.forward, projectedPalmUp) > IS_POINTING_DOTP_THRESHOLD;

            return handData;
        }

        /// <summary>
        /// Updates <see cref="HandData.PointerPose"/> if the platform did not provide it.
        /// </summary>
        /// <param name="handData">The hand data to update <see cref="HandData.PointerPose"/> for.</param>
        private HandData UpdatePointerPose(HandData handData)
        {
            var palmPose = handData.JointsDict[TrackedHandJoint.Palm];
            var wristPose = handData.JointsDict[TrackedHandJoint.Wrist];

            palmPose.Rotation = Quaternion.Inverse(palmPose.Rotation) * palmPose.Rotation;

            var thumbProximalPose = handData.Joints[(int)TrackedHandJoint.ThumbProximal];
            var indexDistalPose = handData.Joints[(int)TrackedHandJoint.IndexDistal];
            var pointerPosition = Vector3.Lerp(thumbProximalPose.Position, indexDistalPose.Position, .5f);

            var forward = wristPose.Forward;
            forward.y = palmPose.Forward.y;
            var pointerEndPosition = pointerPosition + forward;
            var pointerDirection = (pointerEndPosition - pointerPosition).normalized;
            var pointerRotation = Quaternion.LookRotation(pointerDirection);

            pointerRotation = PlayerCamera.transform.rotation * pointerRotation;
            handData.PointerPose = new MixedRealityPose(pointerPosition, pointerRotation);

            return handData;
        }
    }
}
