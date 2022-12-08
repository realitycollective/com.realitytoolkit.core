using RealityCollective.Extensions;
using RealityCollective.ServiceFramework.Services;
using RealityToolkit.CameraSystem.Interfaces;
using RealityToolkit.Definitions.Utilities;
using RealityToolkit.Utilities;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

namespace RealityToolkit.InputSystem.Hands
{
    public class TrackedHandJointPoseProvider : ITrackedHandJointPoseProvider
    {
        private static readonly HandFinger[] HandFingers = Enum.GetValues(typeof(HandFinger)) as HandFinger[];
        private readonly List<Bone> fingerBones = new List<Bone>();
        private Transform cameraRigTransform;

        public void UpdateHandJoints(InputDevice inputDevice, ref MixedRealityPose[] jointPoses, ref Dictionary<TrackedHandJoint, MixedRealityPose> jointPosesDictionary)
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
                        jointPosesDictionary[TrackedHandJoint.Palm] = rootPose;
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
        private TrackedHandJoint ConvertToTrackedHandJoint(HandFinger finger, int index)
        {
            switch (finger)
            {
                case HandFinger.Thumb: return (index == 0) ? TrackedHandJoint.Wrist : TrackedHandJoint.ThumbMetacarpal + index - 1;
                case HandFinger.Index: return TrackedHandJoint.IndexMetacarpal + index;
                case HandFinger.Middle: return TrackedHandJoint.MiddleMetacarpal + index;
                case HandFinger.Ring: return TrackedHandJoint.RingMetacarpal + index;
                case HandFinger.Pinky: return TrackedHandJoint.LittleMetacarpal + index;
                default: throw new ArgumentOutOfRangeException($"Unknown {nameof(TrackedHandJoint)} detected.");
            }
        }

        private void FindCameraRig()
        {
            if (ServiceManager.Instance.TryGetService<IMixedRealityCameraSystem>(out var cameraService))
            {
                cameraRigTransform = cameraService.MainCameraRig.RigTransform;
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
