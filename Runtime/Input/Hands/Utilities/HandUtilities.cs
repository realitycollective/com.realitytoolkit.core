// Copyright (c) Reality Collective. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;

namespace RealityToolkit.Input.Hands.Utilities
{
    /// <summary>
    /// Hand controller utilities.
    /// </summary>
    public static class HandUtilities
    {
        /// <summary>
        /// Gets an estimated <see cref="HandJoint.IndexMetacarpal"/> pose.
        /// Requires known <see cref="HandJoint.ThumbMetacarpal"/> and
        /// <see cref="HandJoint.LittleMetacarpal"/> poses.
        /// </summary>
        /// <param name="jointPoses">Known joint poses.</param>
        /// <returns>Estimated <see cref="HandJoint.IndexMetacarpal"/> pose.</returns>
        public static Pose GetEstimatedIndexMetacarpalPose(Pose[] jointPoses)
        {
            Pose thumbMetacarpalPose = jointPoses[(int)HandJoint.ThumbMetacarpal];
            Pose littleMetacarpalPose = jointPoses[(int)HandJoint.LittleMetacarpal];

            Vector3 indexMetacarpalPosition = Vector3.Lerp(thumbMetacarpalPose.position, littleMetacarpalPose.position, .2f);
            Quaternion indexMetacarpalRotation = jointPoses[(int)HandJoint.Wrist].rotation;

            return new Pose(indexMetacarpalPosition, indexMetacarpalRotation);
        }

        /// <summary>
        /// Gets an estimated <see cref="HandJoint.RingMetacarpal"/> pose.
        /// Requires known <see cref="HandJoint.ThumbMetacarpal"/> and
        /// <see cref="HandJoint.LittleMetacarpal"/> poses.
        /// </summary>
        /// <param name="jointPoses">Known joint poses.</param>
        /// <returns>Estimated <see cref="HandJoint.RingMetacarpal"/> pose.</returns>
        public static Pose GetEstimatedRingMetacarpalPose(Pose[] jointPoses)
        {
            Pose thumbMetacarpalPose = jointPoses[(int)HandJoint.ThumbMetacarpal];
            Pose littleMetacarpalPose = jointPoses[(int)HandJoint.LittleMetacarpal];

            Vector3 ringMetacarpalPosition = Vector3.Lerp(thumbMetacarpalPose.position, littleMetacarpalPose.position, .8f);
            Quaternion ringMetacarpalRotation = jointPoses[(int)HandJoint.LittleMetacarpal].rotation;

            return new Pose(ringMetacarpalPosition, ringMetacarpalRotation);
        }

        /// <summary>
        /// Gets an estimated <see cref="HandJoint.MiddleMetacarpal"/> pose.
        /// Requires known <see cref="HandJoint.ThumbMetacarpal"/> and
        /// <see cref="HandJoint.LittleMetacarpal"/> poses.
        /// </summary>
        /// <param name="jointPoses">Known joint poses.</param>
        /// <returns>Estimated <see cref="HandJoint.MiddleMetacarpal"/> pose.</returns>
        public static Pose GetEstimatedMiddleMetacarpalPose(Pose[] jointPoses)
        {
            Pose thumbMetacarpalPose = jointPoses[(int)HandJoint.ThumbMetacarpal];
            Pose littleMetacarpalPose = jointPoses[(int)HandJoint.LittleMetacarpal];

            Vector3 middleMetacarpalPosition = Vector3.Lerp(thumbMetacarpalPose.position, littleMetacarpalPose.position, .5f);
            Quaternion middleMetacarpalRotation = jointPoses[(int)HandJoint.Wrist].rotation;

            return new Pose(middleMetacarpalPosition, middleMetacarpalRotation);
        }

        /// <summary>
        /// Gets an estimated <see cref="HandJoint.Palm"/> pose.
        /// Requires known <see cref="HandJoint.MiddleMetacarpal"/> and
        /// <see cref="HandJoint.MiddleProximal"/> poses.
        /// </summary>
        /// <param name="jointPoses">Known joint poses.</param>
        /// <returns>Estimated <see cref="HandJoint.Palm"/> pose.</returns>
        public static Pose GetEstimatedPalmPose(Pose[] jointPoses)
        {
            Pose middleMetacarpalPose = GetEstimatedMiddleMetacarpalPose(jointPoses);
            Pose middleProximalPose = jointPoses[(int)HandJoint.MiddleProximal];

            Vector3 palmPosition = Vector3.Lerp(middleMetacarpalPose.position, middleProximalPose.position, .5f);
            Quaternion palmRotation = middleMetacarpalPose.rotation;

            return new Pose(palmPosition, palmRotation);
        }
    }
}