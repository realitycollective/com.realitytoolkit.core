// Copyright (c) Reality Collective. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using RealityToolkit.Definitions.Utilities;
using System.Collections.Generic;
using UnityEngine.XR;

namespace RealityToolkit.InputSystem.Hands
{
    /// <summary>
    /// Defines the programming interface for new joint provider implementations. <see cref="ITrackedHandJointPoseProvider"/>s
    /// provide tracked joint pose information for hand controllers.
    /// </summary>
    public interface ITrackedHandJointPoseProvider
    {
        /// <summary>
        /// Updates hand joint data and writes it into <paramref name="jointPoses"/>.
        /// </summary>
        /// <param name="inputDevice">The <see cref="InputDevice"/> to read hand joint data for.</param>
        /// <param name="jointPoses"><see cref="MixedRealityPose"/> array reference that will be updated with latest joint pose information.
        /// The array poses are ordered in <see cref="TrackedHandJoint"/> ascending order.</param>
        /// <param name="jointPosesDictionary"><see cref="Dictionary{TKey, TValue}"/> populated with joint information for fast access.
        /// The <see cref="TrackedHandJoint"/> is used as a key to retrieve the <see cref="MixedRealityPose"/>.</param>
        void UpdateHandJoints(InputDevice inputDevice, ref MixedRealityPose[] jointPoses, ref Dictionary<TrackedHandJoint, MixedRealityPose> jointPosesDictionary);
    }
}
