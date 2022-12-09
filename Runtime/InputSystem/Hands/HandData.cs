// Copyright (c) Reality Collective. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using RealityToolkit.Definitions.Utilities;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace RealityToolkit.InputSystem.Hands
{
    /// <summary>
    /// An <see cref="Interfaces.Providers.Controllers.Hands.IMixedRealityHandController"/>'s data
    /// in a single frame.
    /// </summary>
    [Serializable]
    public struct HandData
    {
        /// <summary>
        /// Creates a new hand data from joint poses.
        /// </summary>
        /// <param name="jointPoses">Joint pose values.</param>
        public HandData(MixedRealityPose[] jointPoses)
        {
            if (jointPoses.Length != JointCount)
            {
                throw new ArgumentException($"{nameof(HandData)} expects {JointCount} joint poses.");
            }

            RootPose = jointPoses[0];
            Joints = new MixedRealityPose[JointCount];
            Array.Copy(jointPoses, Joints, JointCount);

            JointsDict = new Dictionary<TrackedHandJoint, MixedRealityPose>();
            for (var i = 0; i < Joints.Length; i++)
            {
                JointsDict.Add((TrackedHandJoint)i, Joints[i]);
            }

            PinchStrength = 0;
            GripStrength = 0;
            PointerPose = MixedRealityPose.ZeroIdentity;
            IsPinching = false;
            IsPointing = false;
            IsGripping = false;
            TrackedPoseId = null;
            FingerCurlStrengths = new float[] { 0, 0, 0, 0, 0 };
            Bounds = new Dictionary<TrackedHandBounds, Bounds[]>();
        }

        /// <summary>
        /// Gets the total count of joints the XRTK hand controller supports.
        /// </summary>
        public static readonly int JointCount = Enum.GetNames(typeof(TrackedHandJoint)).Length;

        /// <summary>
        /// Is the hand currently in a pinch pose?
        /// </summary>
        public bool IsPinching { get; set; }

        /// <summary>
        /// What's the pinch strength for index and thumb?
        /// </summary>
        public float PinchStrength { get; set; }

        /// <summary>
        /// Is the hand currently in a pointing pose?
        /// </summary>
        public bool IsPointing { get; set; }

        /// <summary>
        /// Is the hand currently in a gripping pose?
        /// </summary>
        public bool IsGripping { get; set; }

        /// <summary>
        /// Finger curling values per hand finger.
        /// </summary>
        public float[] FingerCurlStrengths { get; set; }

        /// <summary>
        /// What's the grip strength of the hand?
        /// </summary>
        public float GripStrength { get; set; }

        /// <summary>
        /// The hand's pointer pose, relative to <see cref="CameraSystem.Interfaces.IMixedRealityCameraRig.RigTransform"/>.
        /// </summary>
        public MixedRealityPose PointerPose { get; set; }

        /// <summary>
        /// Recognized hand pose, if any.
        /// Recognizable hand poses are defined in <see cref="HandControllerServiceModuleProfile.TrackedPoses"/>
        /// or <see cref="InputSystem.MixedRealityInputSystemProfile.TrackedPoses"/>.
        /// </summary>
        public string TrackedPoseId { get; set; }

        /// <summary>
        /// The hands root pose. <see cref="Joints"/> poses are relative to the root pose.
        /// The root pose itself is relative to <see cref="Interfaces.CameraSystem.IMixedRealityCameraRig.RigTransform"/>.
        /// </summary>
        public MixedRealityPose RootPose { get; set; }

        /// <summary>
        /// Pose information for each hand joint, relative to <see cref="RootPose"/>.
        /// </summary>
        public MixedRealityPose[] Joints { get; set; }

        /// <summary>
        /// Pose information for each hand joint in a dictionary.
        /// </summary>
        public Dictionary<TrackedHandJoint, MixedRealityPose> JointsDict { get; set; }

        /// <summary>
        /// Available hand bounds.
        /// </summary>
        public Dictionary<TrackedHandBounds, Bounds[]> Bounds { get; set; }
    }
}