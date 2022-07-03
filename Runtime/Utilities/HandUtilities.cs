// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using RealityToolkit.Definitions.Controllers.Hands;
using RealityToolkit.Definitions.Utilities;
using UnityEngine;

namespace RealityToolkit.Utilities
{
    /// <summary>
    /// Hand controller utilities.
    /// </summary>
    public static class HandUtilities
    {
        /// <summary>
        /// Gets an estimated <see cref="XRHandJoint.IndexMetacarpal"/> pose.
        /// Requires known <see cref="XRHandJoint.ThumbMetacarpal"/> and
        /// <see cref="XRHandJoint.LittleMetacarpal"/> poses.
        /// </summary>
        /// <param name="jointPoses">Known joint poses.</param>
        /// <returns>Estimated <see cref="XRHandJoint.IndexMetacarpal"/> pose.</returns>
        public static MixedRealityPose GetEstimatedIndexMetacarpalPose(MixedRealityPose[] jointPoses)
        {
            MixedRealityPose thumbMetacarpalPose = jointPoses[(int)XRHandJoint.ThumbMetacarpal];
            MixedRealityPose littleMetacarpalPose = jointPoses[(int)XRHandJoint.LittleMetacarpal];

            Vector3 indexMetacarpalPosition = Vector3.Lerp(thumbMetacarpalPose.Position, littleMetacarpalPose.Position, .2f);
            Quaternion indexMetacarpalRotation = jointPoses[(int)XRHandJoint.Wrist].Rotation;

            return new MixedRealityPose(indexMetacarpalPosition, indexMetacarpalRotation);
        }

        /// <summary>
        /// Gets an estimated <see cref="XRHandJoint.RingMetacarpal"/> pose.
        /// Requires known <see cref="XRHandJoint.ThumbMetacarpal"/> and
        /// <see cref="XRHandJoint.LittleMetacarpal"/> poses.
        /// </summary>
        /// <param name="jointPoses">Known joint poses.</param>
        /// <returns>Estimated <see cref="XRHandJoint.RingMetacarpal"/> pose.</returns>
        public static MixedRealityPose GetEstimatedRingMetacarpalPose(MixedRealityPose[] jointPoses)
        {
            MixedRealityPose thumbMetacarpalPose = jointPoses[(int)XRHandJoint.ThumbMetacarpal];
            MixedRealityPose littleMetacarpalPose = jointPoses[(int)XRHandJoint.LittleMetacarpal];

            Vector3 ringMetacarpalPosition = Vector3.Lerp(thumbMetacarpalPose.Position, littleMetacarpalPose.Position, .8f);
            Quaternion ringMetacarpalRotation = jointPoses[(int)XRHandJoint.LittleMetacarpal].Rotation;

            return new MixedRealityPose(ringMetacarpalPosition, ringMetacarpalRotation);
        }

        /// <summary>
        /// Gets an estimated <see cref="XRHandJoint.MiddleMetacarpal"/> pose.
        /// Requires known <see cref="XRHandJoint.ThumbMetacarpal"/> and
        /// <see cref="XRHandJoint.LittleMetacarpal"/> poses.
        /// </summary>
        /// <param name="jointPoses">Known joint poses.</param>
        /// <returns>Estimated <see cref="XRHandJoint.MiddleMetacarpal"/> pose.</returns>
        public static MixedRealityPose GetEstimatedMiddleMetacarpalPose(MixedRealityPose[] jointPoses)
        {
            MixedRealityPose thumbMetacarpalPose = jointPoses[(int)XRHandJoint.ThumbMetacarpal];
            MixedRealityPose littleMetacarpalPose = jointPoses[(int)XRHandJoint.LittleMetacarpal];

            Vector3 middleMetacarpalPosition = Vector3.Lerp(thumbMetacarpalPose.Position, littleMetacarpalPose.Position, .5f);
            Quaternion middleMetacarpalRotation = jointPoses[(int)XRHandJoint.Wrist].Rotation;

            return new MixedRealityPose(middleMetacarpalPosition, middleMetacarpalRotation);
        }

        /// <summary>
        /// Gets an estimated <see cref="XRHandJoint.Palm"/> pose.
        /// Requires known <see cref="XRHandJoint.MiddleMetacarpal"/> and
        /// <see cref="XRHandJoint.MiddleProximal"/> poses.
        /// </summary>
        /// <param name="jointPoses">Known joint poses.</param>
        /// <returns>Estimated <see cref="XRHandJoint.Palm"/> pose.</returns>
        public static MixedRealityPose GetEstimatedPalmPose(MixedRealityPose[] jointPoses)
        {
            MixedRealityPose middleMetacarpalPose = GetEstimatedMiddleMetacarpalPose(jointPoses);
            MixedRealityPose middleProximalPose = jointPoses[(int)XRHandJoint.MiddleProximal];

            Vector3 palmPosition = Vector3.Lerp(middleMetacarpalPose.Position, middleProximalPose.Position, .5f);
            Quaternion palmRotation = middleMetacarpalPose.Rotation;

            return new MixedRealityPose(palmPosition, palmRotation);
        }
    }
}