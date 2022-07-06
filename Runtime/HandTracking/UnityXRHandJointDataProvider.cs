// Copyright (c) Reality Collective. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using RealityCollective.Extensions;
using RealityToolkit.Definitions.Controllers.Hands;
using RealityToolkit.Definitions.Utilities;
using RealityToolkit.Interfaces.CameraSystem;
using RealityToolkit.Interfaces.InputSystem.Providers.Controllers.Hands;
using RealityToolkit.Utilities;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using HandFinger = UnityEngine.XR.HandFinger;

namespace RealityToolkit.Services.InputSystem.Controllers.UnityXR
{
    /// <summary>
    /// Provides hand joints data for use with <see cref="UnityXRHandController"/>.
    /// </summary>
    public class UnityXRHandJointDataProvider : IUnityXRHandJointDataProvider
    {
        private static readonly HandFinger[] HandFingers = Enum.GetValues(typeof(HandFinger)) as HandFinger[];
        private readonly List<Bone> fingerBones = new List<Bone>();
        private Transform cameraRigTransform;

        public void UpdateHandJoints(InputDevice inputDevice, ref MixedRealityPose[] jointPoses, ref Dictionary<XRHandJoint, MixedRealityPose> jointPosesDictionary)
        {
            if (cameraRigTransform.IsNull())
            {
                FindCameraRig();
            }

            if (inputDevice.TryGetFeatureValue(CommonUsages.handData, out Hand hand))
            {
                foreach (HandFinger finger in HandFingers)
                {
                    if (hand.TryGetRootBone(out Bone rootBone) && TryReadHandJoint(rootBone, out MixedRealityPose rootPose))
                    {
                        jointPosesDictionary[XRHandJoint.Palm] = rootPose;
                    }

                    if (hand.TryGetFingerBones(finger, fingerBones))
                    {
                        for (int i = 0; i < fingerBones.Count; i++)
                        {
                            if (TryReadHandJoint(fingerBones[i], out MixedRealityPose pose))
                            {
                                jointPosesDictionary[ConvertToTrackedHandJoint(finger, i)] = pose;
                            }
                        }
                    }
                }
            }
        }

        private bool TryReadHandJoint(Bone bone, out MixedRealityPose pose)
        {
            bool positionAvailable = bone.TryGetPosition(out Vector3 position);
            bool rotationAvailable = bone.TryGetRotation(out Quaternion rotation);

            if (positionAvailable && rotationAvailable)
            {
                // We want input sources to follow the playspace, so fold in the playspace transform here to
                // put the pose into world space.
                position = cameraRigTransform.TransformPoint(position);
                rotation = cameraRigTransform.rotation * rotation;

                pose = new MixedRealityPose(position, rotation);
                return true;
            }

            pose = MixedRealityPose.ZeroIdentity;
            return false;
        }

        /// <summary>
        /// Converts a Unity finger bone into a <see cref="XRHandJoint"/>.
        /// </summary>
        /// <param name="finger">The Unity classification of the current finger.</param>
        /// <param name="index">The Unity index of the current finger bone.</param>
        /// <returns>The current Unity finger bone converted into a <see cref="XRHandJoint"/>.</returns>
        private XRHandJoint ConvertToTrackedHandJoint(HandFinger finger, int index)
        {
            switch (finger)
            {
                case HandFinger.Thumb: return (index == 0) ? XRHandJoint.Wrist : XRHandJoint.ThumbMetacarpal + index - 1;
                case HandFinger.Index: return XRHandJoint.IndexMetacarpal + index;
                case HandFinger.Middle: return XRHandJoint.MiddleMetacarpal + index;
                case HandFinger.Ring: return XRHandJoint.RingMetacarpal + index;
                case HandFinger.Pinky: return XRHandJoint.LittleMetacarpal + index;
                default: return XRHandJoint.Unknown;
            }
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