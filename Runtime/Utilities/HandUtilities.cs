// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using RealityToolkit.Definitions.Controllers.Hands;
using UnityEngine;

namespace RealityToolkit.Utilities
{
    /// <summary>
    /// Hand controller utilities.
    /// </summary>
    public static class HandUtilities
    {
        /// <summary>
        /// Gets an estimated <see cref="TrackedHandJoint.IndexMetacarpal"/> pose.
        /// Requires known <see cref="TrackedHandJoint.ThumbMetacarpal"/> and
        /// <see cref="TrackedHandJoint.LittleMetacarpal"/> poses.
        /// </summary>
        /// <param name="jointPoses">Known joint poses.</param>
        /// <returns>Estimated <see cref="TrackedHandJoint.IndexMetacarpal"/> pose.</returns>
        public static Pose GetEstimatedIndexMetacarpalPose(Pose[] jointPoses)
        {
            Pose thumbMetacarpalPose = jointPoses[(int)TrackedHandJoint.ThumbMetacarpal];
            Pose littleMetacarpalPose = jointPoses[(int)TrackedHandJoint.LittleMetacarpal];

            Vector3 indexMetacarpalPosition = Vector3.Lerp(thumbMetacarpalPose.position, littleMetacarpalPose.position, .2f);
            Quaternion indexMetacarpalRotation = jointPoses[(int)TrackedHandJoint.Wrist].rotation;

            return new Pose(indexMetacarpalPosition, indexMetacarpalRotation);
        }

        /// <summary>
        /// Gets an estimated <see cref="TrackedHandJoint.RingMetacarpal"/> pose.
        /// Requires known <see cref="TrackedHandJoint.ThumbMetacarpal"/> and
        /// <see cref="TrackedHandJoint.LittleMetacarpal"/> poses.
        /// </summary>
        /// <param name="jointPoses">Known joint poses.</param>
        /// <returns>Estimated <see cref="TrackedHandJoint.RingMetacarpal"/> pose.</returns>
        public static Pose GetEstimatedRingMetacarpalPose(Pose[] jointPoses)
        {
            Pose thumbMetacarpalPose = jointPoses[(int)TrackedHandJoint.ThumbMetacarpal];
            Pose littleMetacarpalPose = jointPoses[(int)TrackedHandJoint.LittleMetacarpal];

            Vector3 ringMetacarpalPosition = Vector3.Lerp(thumbMetacarpalPose.position, littleMetacarpalPose.position, .8f);
            Quaternion ringMetacarpalRotation = jointPoses[(int)TrackedHandJoint.LittleMetacarpal].rotation;

            return new Pose(ringMetacarpalPosition, ringMetacarpalRotation);
        }

        /// <summary>
        /// Gets an estimated <see cref="TrackedHandJoint.MiddleMetacarpal"/> pose.
        /// Requires known <see cref="TrackedHandJoint.ThumbMetacarpal"/> and
        /// <see cref="TrackedHandJoint.LittleMetacarpal"/> poses.
        /// </summary>
        /// <param name="jointPoses">Known joint poses.</param>
        /// <returns>Estimated <see cref="TrackedHandJoint.MiddleMetacarpal"/> pose.</returns>
        public static Pose GetEstimatedMiddleMetacarpalPose(Pose[] jointPoses)
        {
            Pose thumbMetacarpalPose = jointPoses[(int)TrackedHandJoint.ThumbMetacarpal];
            Pose littleMetacarpalPose = jointPoses[(int)TrackedHandJoint.LittleMetacarpal];

            Vector3 middleMetacarpalPosition = Vector3.Lerp(thumbMetacarpalPose.position, littleMetacarpalPose.position, .5f);
            Quaternion middleMetacarpalRotation = jointPoses[(int)TrackedHandJoint.Wrist].rotation;

            return new Pose(middleMetacarpalPosition, middleMetacarpalRotation);
        }

        /// <summary>
        /// Gets an estimated <see cref="TrackedHandJoint.Palm"/> pose.
        /// Requires known <see cref="TrackedHandJoint.MiddleMetacarpal"/> and
        /// <see cref="TrackedHandJoint.MiddleProximal"/> poses.
        /// </summary>
        /// <param name="jointPoses">Known joint poses.</param>
        /// <returns>Estimated <see cref="TrackedHandJoint.Palm"/> pose.</returns>
        public static Pose GetEstimatedPalmPose(Pose[] jointPoses)
        {
            Pose middleMetacarpalPose = GetEstimatedMiddleMetacarpalPose(jointPoses);
            Pose middleProximalPose = jointPoses[(int)TrackedHandJoint.MiddleProximal];

            Vector3 palmPosition = Vector3.Lerp(middleMetacarpalPose.position, middleProximalPose.position, .5f);
            Quaternion palmRotation = middleMetacarpalPose.rotation;

            return new Pose(palmPosition, palmRotation);
        }
    }
}