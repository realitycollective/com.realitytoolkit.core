// Copyright (c) Reality Collective. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using RealityCollective.Definitions.Utilities;
using RealityToolkit.Definitions.Controllers;
using RealityToolkit.Definitions.Controllers.Hands;
using RealityToolkit.Definitions.Devices;
using RealityToolkit.Definitions.InputSystem;
using RealityToolkit.Definitions.Utilities;
using RealityToolkit.Extensions;
using RealityToolkit.Interfaces.CameraSystem;
using RealityToolkit.Interfaces.InputSystem;
using RealityToolkit.Interfaces.InputSystem.Controllers.Hands;
using RealityToolkit.Interfaces.InputSystem.Providers.Controllers;
using RealityToolkit.Interfaces.InputSystem.Providers.Controllers.Hands;
using RealityToolkit.Services.InputSystem.Controllers.Hands;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace RealityToolkit.Services.InputSystem.Controllers.UnityXR
{
    /// <summary>
    /// A base for hand controllers powered by Unity's XR Plugin Management module.
    /// </summary>
    public abstract class UnityXRHandController : UnityXRController, IHandController
    {
        /// <inheritdoc />
        public UnityXRHandController() { }

        /// <inheritdoc />
        public UnityXRHandController(IMixedRealityControllerDataProvider controllerDataProvider, TrackingState trackingState, Handedness controllerHandedness, MixedRealityControllerMappingProfile controllerMappingProfile)
            : base(controllerDataProvider, trackingState, controllerHandedness, controllerMappingProfile)
        {
            if (!MixedRealityToolkit.TryGetSystemProfile<IMixedRealityInputSystem, MixedRealityInputSystemProfile>(out var inputSystemProfile))
            {
                Debug.LogError($"The {nameof(UnityXRHandController)} requires a valid {nameof(MixedRealityInputSystemProfile)} to work.");
                return;
            }

            handJointDataProvider = new UnityXRHandJointDataProvider();
            handRenderingMode = inputSystemProfile.HandControllerSettings.RenderingMode;
            jointPoses = new MixedRealityPose[Enum.GetNames(typeof(XRHandJoint)).Length - 1];
            jointPosesDict = new Dictionary<XRHandJoint, MixedRealityPose>();

            postProcessors = new IHandDataPostProcessor[]
            {
                new HandDataPostProcessor(this, inputSystemProfile.HandControllerSettings),
                new HandGripPostProcessor(this, inputSystemProfile.HandControllerSettings),
                new HandTrackedPosePostProcessor(this, inputSystemProfile.HandControllerSettings),
                new HandBoundsPostProcessor(this, inputSystemProfile.HandControllerSettings)
            };

            if (!MixedRealityToolkit.TryGetService<IMixedRealityCameraSystem>(out var cameraSystem))
            {
                cameraRigTransform = Camera.main.transform.parent;
            }
            else
            {
                cameraRigTransform = cameraSystem.MainCameraRig.RigTransform;
            }
        }

        private const string pinchPressInputName = "Pinch";
        private const string pointInputName = "Point";
        private const string gripInputName = "Grip";
        private const string gripPressInputName = "Grip Press";
        private const string gripPoseInputName = "Grip Pose";
        private const string indexFingerPoseInputName = "Index Finger Pose";
        private const string spatialPointerPoseInputName = "Spatial Pointer Pose";
        private const int poseFrameBufferSize = 5;

        private HandData handData;
        private MixedRealityPose[] jointPoses;
        private Dictionary<XRHandJoint, MixedRealityPose> jointPosesDict;
        protected IUnityXRHandJointDataProvider handJointDataProvider;
        protected IUnityXRHandMeshDataProvider handMeshDataProvider;
        protected Transform cameraRigTransform;
        private readonly HandRenderingMode handRenderingMode;
        private readonly IHandDataPostProcessor[] postProcessors;
        private readonly Queue<bool> isPinchingBuffer = new Queue<bool>(poseFrameBufferSize);
        private readonly Queue<bool> isGrippingBuffer = new Queue<bool>(poseFrameBufferSize);
        private readonly Queue<bool> isPointingBuffer = new Queue<bool>(poseFrameBufferSize);

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
        public string TrackedPoseId { get; private set; }

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
                Debug.LogError($"Cannot find input device for {GetType().Name} - {Handedness}");
                return;
            }

            InputDevice = inputDevice;
            UpdateTrackingState();

            if (TrackingState != TrackingState.Tracked)
            {
                UpdateInteractionMappings();
                return;
            }

            UpdateHandJoints();

            for (var i = 0; i < postProcessors.Length; i++)
            {
                handData = postProcessors[i].PostProcess(handData);
            }

            // Some platforms may not have a hand mesh provider so only if one is available in the platform
            // implementation and hand mesh rendering is requested, we update mesh data.
            if (handMeshDataProvider != null && handRenderingMode == HandRenderingMode.Mesh)
            {
                UpdateHandMesh();
            }

            UpdateControllerPose();
            UpdateSpatialPointerPose();
            UpdateSpatialGripPose();
            UpdateInteractionMappings();
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

            IsPositionAvailable = TryGetJointPose(XRHandJoint.Wrist, out var wristPose);
            IsRotationAvailable = IsPositionAvailable;
            IsPositionApproximate = false;

            if (wristPose != Pose)
            {
                Pose = wristPose;
                InputSystem?.RaiseSourcePoseChanged(InputSource, this, Pose);
            }
        }

        /// <summary>
        /// Updates the controller's hand joint information.
        /// </summary>
        protected virtual void UpdateHandJoints()
        {
            Debug.Assert(handJointDataProvider != null, $"{GetType().Name} has no {nameof(IUnityXRHandJointDataProvider)} to work with.");
            handJointDataProvider.UpdateHandJoints(InputDevice, ref jointPoses, ref jointPosesDict);
            handData = new HandData(jointPoses);
        }

        /// <summary>
        /// Updates the controller's hand mesh information.
        /// </summary>
        protected virtual void UpdateHandMesh()
        {
            Debug.Assert(handMeshDataProvider != null, $"{GetType().Name} has no {nameof(IUnityXRHandMeshDataProvider)} to work with.");
            handData.Mesh = handMeshDataProvider.UpdateHandMesh(InputDevice);
        }

        /// <inheritdoc />
        protected override void UpdateTrackingState()
        {
            // This is a workaround until the tracking state has been implemented by Unity
            // for OpenXR hands.
            TrackingState = TrackingState.Tracked;
        }

        /// <inheritdoc />
        public bool TryGetBounds(TrackedHandBounds handBounds, out Bounds[] newBounds) => handData.Bounds.TryGetValue(handBounds, out newBounds);

        /// <inheritdoc />
        public bool TryGetJointPose(XRHandJoint joint, out MixedRealityPose pose, Space relativeTo = Space.Self)
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
                pose.Position = cameraRigTransform.TransformPoint(pose.Position);
                pose.Rotation = cameraRigTransform.rotation * pose.Rotation;

                return true;
            }

            pose = MixedRealityPose.ZeroIdentity;
            return false;
        }

        /// <inheritdoc />
        public bool TryGetHandMeshData(out HandMeshData handMeshData)
        {
            if (!handData.Mesh.IsEmpty)
            {
                handMeshData = handData.Mesh;
                return true;
            }

            handMeshData = HandMeshData.Empty;
            return false;
        }

        /// <inheritdoc />
        public bool TryGetCurlStrength(HandFinger handFinger, out float curlStrength)
        {
            if (handData.FingerCurlStrengths == null)
            {
                curlStrength = 0f;
                return false;
            }

            curlStrength = handData.FingerCurlStrengths[(int)handFinger];
            return true;
        }

        #region Interaction Mappings

        /// <inheritdoc />
        protected override void UpdateSingleAxisInteractionMapping(MixedRealityInteractionMapping interactionMapping)
        {
            Debug.Assert(interactionMapping.AxisType == AxisType.SingleAxis);
            switch (interactionMapping.InputName)
            {
                case gripInputName: UpdateGripStrengthInteractionMapping(interactionMapping); break;
            }

            interactionMapping.RaiseInputAction(InputSource, Handedness);
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

        private void UpdateIsPointingInteractionMapping(MixedRealityInteractionMapping interactionMapping)
        {
            Debug.Assert(string.Equals(interactionMapping.InputName, pointInputName));

            if (TrackingState == TrackingState.Tracked)
            {
                var isPointingThisFrame = handData.IsPointing;
                if (isPointingBuffer.Count < poseFrameBufferSize)
                {
                    isPointingBuffer.Enqueue(isPointingThisFrame);
                    IsPointing = false;
                }
                else
                {
                    isPointingBuffer.Dequeue();
                    isPointingBuffer.Enqueue(isPointingThisFrame);

                    isPointingThisFrame = true;
                    for (int i = 0; i < isPointingBuffer.Count; i++)
                    {
                        var value = isPointingBuffer.Dequeue();

                        if (!value)
                        {
                            isPointingThisFrame = false;
                        }

                        isPointingBuffer.Enqueue(value);
                    }

                    IsPointing = isPointingThisFrame;
                }
            }
            else
            {
                isPointingBuffer.Clear();
                IsPointing = false;
            }

            interactionMapping.BoolData = IsPointing;
        }

        private void UpdateIsPinchingInteractionMapping(MixedRealityInteractionMapping interactionMapping)
        {
            Debug.Assert(string.Equals(interactionMapping.InputName, pinchPressInputName));

            if (TrackingState == TrackingState.Tracked)
            {
                var isPinchingThisFrame = handData.IsPinching;
                if (isPinchingBuffer.Count < poseFrameBufferSize)
                {
                    isPinchingBuffer.Enqueue(isPinchingThisFrame);
                    IsPinching = false;
                }
                else
                {
                    isPinchingBuffer.Dequeue();
                    isPinchingBuffer.Enqueue(isPinchingThisFrame);

                    isPinchingThisFrame = true;
                    for (int i = 0; i < isPinchingBuffer.Count; i++)
                    {
                        var value = isPinchingBuffer.Dequeue();

                        if (!value)
                        {
                            isPinchingThisFrame = false;
                        }

                        isPinchingBuffer.Enqueue(value);
                    }

                    IsPinching = isPinchingThisFrame;
                }
            }
            else
            {
                isPinchingBuffer.Clear();
                IsPinching = false;
            }

            interactionMapping.BoolData = IsPinching;
        }

        private void UpdateIsGrippingInteractionMapping(MixedRealityInteractionMapping interactionMapping)
        {
            Debug.Assert(string.Equals(interactionMapping.InputName, gripPressInputName));

            if (TrackingState == TrackingState.Tracked)
            {
                var isGrippingThisFrame = handData.IsGripping;
                if (isGrippingBuffer.Count < poseFrameBufferSize)
                {
                    isGrippingBuffer.Enqueue(isGrippingThisFrame);
                    IsGripping = false;
                }
                else
                {
                    isGrippingBuffer.Dequeue();
                    isGrippingBuffer.Enqueue(isGrippingThisFrame);

                    isGrippingThisFrame = true;
                    for (int i = 0; i < isGrippingBuffer.Count; i++)
                    {
                        var value = isGrippingBuffer.Dequeue();

                        if (!value)
                        {
                            isGrippingThisFrame = false;
                        }

                        isGrippingBuffer.Enqueue(value);
                    }

                    IsGripping = isGrippingThisFrame;
                }
            }
            else
            {
                isGrippingBuffer.Clear();
                IsGripping = false;
            }

            interactionMapping.BoolData = IsGripping;
        }

        private void UpdateGripStrengthInteractionMapping(MixedRealityInteractionMapping interactionMapping)
        {
            Debug.Assert(string.Equals(interactionMapping.InputName, gripInputName));
            interactionMapping.FloatData = GripStrength;
        }

        private void UpdateIndexFingerPoseInteractionMapping(MixedRealityInteractionMapping interactionMapping)
        {
            Debug.Assert(string.Equals(interactionMapping.InputName, indexFingerPoseInputName));

            if (TryGetJointPose(XRHandJoint.IndexTip, out var indexTipPose))
            {
                interactionMapping.PoseData = indexTipPose;
            }
        }

        #endregion
    }
}
