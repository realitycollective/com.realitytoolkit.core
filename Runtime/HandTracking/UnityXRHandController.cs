// Copyright (c) Reality Collective. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using RealityCollective.Definitions.Utilities;
using RealityToolkit.Definitions.Controllers;
using RealityToolkit.Definitions.Controllers.Hands;
using RealityToolkit.Definitions.Devices;
using RealityToolkit.Definitions.InputSystem;
using RealityToolkit.Definitions.Utilities;
using RealityToolkit.Interfaces.CameraSystem;
using RealityToolkit.Interfaces.InputSystem;
using RealityToolkit.Interfaces.InputSystem.Controllers.Hands;
using RealityToolkit.Interfaces.InputSystem.Providers.Controllers;
using RealityToolkit.Interfaces.InputSystem.Providers.Controllers.Hands;
using RealityToolkit.Services.InputSystem.Controllers.Hands;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

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
            if (!MixedRealityToolkit.TryGetService(out cameraSystem))
            {
                Debug.LogError($"The {nameof(UnityXRHandController)} requires the {nameof(IMixedRealityCameraSystem)} to work.");
                return;
            }

            if (!MixedRealityToolkit.TryGetService<IMixedRealityInputSystem>(out _))
            {
                Debug.LogError($"The {nameof(UnityXRHandController)} requires the {nameof(IMixedRealityInputSystem)} to work.");
                return;
            }

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
        }

        private const string pinchPressInputName = "Pinch";
        private const string pointInputName = "Point";
        private const string gripInputName = "Grip";
        private const string gripPressInputName = "Grip Press";
        private const string gripPoseInputName = "Grip Pose";
        private const string indexFingerPoseInputName = "Index Finger Pose";
        private const string spatialPointerPoseInputName = "Spatial Pointer Pose";

        private HandData handData;
        private MixedRealityPose[] jointPoses;
        private Dictionary<XRHandJoint, MixedRealityPose> jointPosesDict;
        protected IUnityXRHandJointDataProvider handJointDataProvider;
        protected IUnityXRHandMeshDataProvider handMeshDataProvider;
        protected IMixedRealityCameraSystem cameraSystem;
        private readonly HandRenderingMode handRenderingMode;
        private readonly IHandDataPostProcessor[] postProcessors;

        public bool IsPinching => throw new System.NotImplementedException();

        public float PinchStrength => throw new System.NotImplementedException();

        public bool IsPointing => throw new System.NotImplementedException();

        public bool IsGripping => throw new System.NotImplementedException();

        public float GripStrength => throw new System.NotImplementedException();

        public string TrackedPoseId => throw new System.NotImplementedException();

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
        protected override IReadOnlyDictionary<string, InputFeatureUsage<bool>> DigitalInputFeatureUsageMap { get; set; } = new Dictionary<string, InputFeatureUsage<bool>>
        {
            { pinchPressInputName, CommonUsages.triggerButton },
            { gripPressInputName, CommonUsages.gripButton }
        };

        /// <inheritdoc />
        protected override IReadOnlyDictionary<string, InputFeatureUsage<float>> SingleAxisInputFeatureUsageMap { get; set; } = new Dictionary<string, InputFeatureUsage<float>>
        {
            { gripInputName, CommonUsages.trigger }
        };

        /// <inheritdoc />
        public override void UpdateController()
        {
            // We want to update hand joints data no matter what the rendering mode is. Even if hand
            // rendering is fully disabled, we need to know where joints are for physics and other features to work
            // correctly. It is up to the controller visualizer to not visuaize the data then.
            UpdateHandJoints();

            // Apply post processing to calculate additional hand pose properties.
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

            base.UpdateController();
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

            if (wristPose != ControllerPose)
            {
                ControllerPose = wristPose;
                InputSystem?.RaiseSourcePoseChanged(InputSource, this, ControllerPose);
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
                    Position = ControllerPose.Position + ControllerPose.Rotation * localPose.Position,
                    Rotation = ControllerPose.Rotation * localPose.Rotation
                };

                // Translate to world space.
                pose.Position = cameraSystem.MainCameraRig.RigTransform.TransformPoint(pose.Position);
                pose.Rotation = cameraSystem.MainCameraRig.RigTransform.rotation * pose.Rotation;

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
        public bool TryGetFingerCurlStrength(Definitions.Controllers.Hands.HandFinger handFinger, out float curlStrength)
        {
            throw new System.NotImplementedException();
        }
    }
}
