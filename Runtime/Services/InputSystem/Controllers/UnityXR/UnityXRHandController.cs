// Copyright (c) Reality Collective. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using RealityToolkit.Interfaces.InputSystem.Controllers.Hands;
using RealityToolkit.Interfaces.InputSystem.Providers.Controllers.Hands;
using System.Collections.Generic;
using UnityEngine;
using XRTK.Definitions.Controllers;
using XRTK.Definitions.Controllers.Hands;
using XRTK.Definitions.Devices;
using XRTK.Definitions.InputSystem;
using XRTK.Definitions.Utilities;
using XRTK.Extensions;
using XRTK.Interfaces.CameraSystem;
using XRTK.Interfaces.InputSystem;
using XRTK.Interfaces.InputSystem.Providers.Controllers;
using XRTK.Services;
using XRTK.Utilities;

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
            if (MixedRealityToolkit.TryGetSystemProfile<IMixedRealityInputSystem, MixedRealityInputSystemProfile>(out var inputSystemProfile))
            {
                handRenderingMode = inputSystemProfile.RenderingMode;
            }
            else
            {
                handRenderingMode = HandRenderingMode.None;
            }

            FindCameraRig();
        }

        private const string pinchInputName = "Pinch";
        private const string pointInputName = "Point";
        private const string gripInputName = "Grip";
        private const string gripPressInputName = "Grip Press";
        private const string gripPoseInputName = "Grip Pose";
        private const string indexFingerPoseInputName = "Index Finger Pose";
        private const string spatialPointerPoseInputName = "Spatial Pointer Pose";

        private readonly Dictionary<TrackedHandJoint, MixedRealityPose> jointPoses = new Dictionary<TrackedHandJoint, MixedRealityPose>();
        private HandMeshData handMeshData;
        private readonly HandRenderingMode handRenderingMode;
        protected IUnityXRHandJointDataProvider handJointDataProvider;
        protected IUnityXRHandMeshDataProvider handMeshDataProvider;
        private Transform cameraRigTransform;

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
        public override void UpdateController()
        {
            base.UpdateController();

            // We want to update hand joints data no matter what the rendering mode is. Even if hand
            // rendering is fully disabled, we need to know where joints are for physics and other features to work
            // correctly. It is up to the controller visualizer to not visuaize the data then.
            UpdateHandJoints();

            // Some platforms may not have a hand mesh provider so only if one is available in the platform
            // implementation and hand mesh rendering is requested, we update mesh data.
            if (handMeshDataProvider != null && handRenderingMode == HandRenderingMode.Mesh)
            {
                UpdateHandMesh();
            }
        }

        /// <summary>
        /// Updates the controller's hand joint information.
        /// </summary>
        protected virtual void UpdateHandJoints()
        {
            Debug.Assert(handJointDataProvider != null, $"{GetType().Name} has no {nameof(IUnityXRHandJointDataProvider)} to work with.");
            handJointDataProvider.UpdateHandJoints(InputDevice, jointPoses);
        }

        /// <summary>
        /// Updates the controller's hand mesh information.
        /// </summary>
        protected virtual void UpdateHandMesh()
        {
            Debug.Assert(handMeshDataProvider != null, $"{GetType().Name} has no {nameof(IUnityXRHandMeshDataProvider)} to work with.");
            handMeshData = handMeshDataProvider.UpdateHandMesh(InputDevice);
        }

        /// <inheritdoc />
        public bool TryGetJointPose(TrackedHandJoint joint, out MixedRealityPose pose, Space relativeTo = Space.Self)
        {
            if (relativeTo == Space.Self)
            {
                // Return joint pose relative to hand root.
                return jointPoses.TryGetValue(joint, out pose);
            }

            if (jointPoses.TryGetValue(joint, out var localPose))
            {
                pose = new MixedRealityPose
                {
                    // Combine root pose with local joint pose.
                    Position = ControllerPose.Position + ControllerPose.Rotation * localPose.Position,
                    Rotation = ControllerPose.Rotation * localPose.Rotation
                };

                // Translate to world space.
                if (cameraRigTransform.IsNotNull())
                {
                    pose.Position = cameraRigTransform.TransformPoint(pose.Position);
                    pose.Rotation = cameraRigTransform.rotation * pose.Rotation;
                }

                return true;
            }

            pose = MixedRealityPose.ZeroIdentity;
            return false;
        }

        /// <inheritdoc />
        public bool TryGetHandMeshData(out HandMeshData handMeshData)
        {
            if (!this.handMeshData.IsEmpty)
            {
                handMeshData = this.handMeshData;
                return true;
            }

            handMeshData = HandMeshData.Empty;
            return false;
        }

        private void FindCameraRig()
        {
            if (MixedRealityToolkit.TryGetService<IMixedRealityCameraSystem>(out var cameraSystem))
            {
                cameraRigTransform = cameraSystem.MainCameraRig.RigTransform;
            }
            else
            {
                var cameraTransform = CameraCache.Main.transform;
                Debug.Assert(cameraTransform.parent.IsNotNull(), "The camera must be parented.");
                cameraRigTransform = CameraCache.Main.transform.parent;
            }
        }
    }
}
