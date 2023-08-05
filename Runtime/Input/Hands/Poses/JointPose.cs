// Copyright (c) Reality Collective. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using UnityEngine;

namespace RealityToolkit.Input.Hands.Poses
{
    /// <summary>
    /// Recorded pose information for a specific <see cref="HandJoint"/>
    /// </summary>
    [Serializable]
    public class JointPose
    {
        /// <summary>
        /// The recorded <see cref="HandJoint"/>.
        /// </summary>
        public HandJoint Joint;

        /// <summary>
        /// The recorded <see cref="UnityEngine.Pose"/> for <see cref="Joint"/>.
        /// </summary>
        public Pose Pose;
    }
}
