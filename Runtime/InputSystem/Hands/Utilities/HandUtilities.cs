// Copyright (c) Reality Collective. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using RealityToolkit.Definitions.Utilities;
using UnityEngine;

namespace RealityToolkit.InputSystem.Hands.Utilities
{
    /// <summary>
    /// Hand controller utilities.
    /// </summary>
    public static class HandUtilities
    {
        public static TrackedHandJoint GetParentJoint(TrackedHandJoint trackedHandJoint)
        {
            switch (trackedHandJoint)
            {
                case TrackedHandJoint.Palm:
                    return TrackedHandJoint.Wrist;
                case TrackedHandJoint.ThumbMetacarpal:
                    return TrackedHandJoint.Wrist;
                case TrackedHandJoint.ThumbProximal:
                    return TrackedHandJoint.ThumbMetacarpal;
                case TrackedHandJoint.ThumbDistal:
                    return TrackedHandJoint.ThumbProximal;
                case TrackedHandJoint.ThumbTip:
                    return TrackedHandJoint.ThumbDistal;
                case TrackedHandJoint.IndexMetacarpal:
                    return TrackedHandJoint.Wrist;
                case TrackedHandJoint.IndexProximal:
                    return TrackedHandJoint.IndexMetacarpal;
                case TrackedHandJoint.IndexIntermediate:
                    return TrackedHandJoint.IndexProximal;
                case TrackedHandJoint.IndexDistal:
                    return TrackedHandJoint.IndexIntermediate;
                case TrackedHandJoint.IndexTip:
                    return TrackedHandJoint.IndexDistal;
                case TrackedHandJoint.MiddleMetacarpal:
                    return TrackedHandJoint.Wrist;
                case TrackedHandJoint.MiddleProximal:
                    return TrackedHandJoint.MiddleMetacarpal;
                case TrackedHandJoint.MiddleIntermediate:
                    return TrackedHandJoint.MiddleProximal;
                case TrackedHandJoint.MiddleDistal:
                    return TrackedHandJoint.MiddleIntermediate;
                case TrackedHandJoint.MiddleTip:
                    return TrackedHandJoint.MiddleDistal;
                case TrackedHandJoint.RingMetacarpal:
                    return TrackedHandJoint.Wrist;
                case TrackedHandJoint.RingProximal:
                    return TrackedHandJoint.RingMetacarpal;
                case TrackedHandJoint.RingIntermediate:
                    return TrackedHandJoint.RingProximal;
                case TrackedHandJoint.RingDistal:
                    return TrackedHandJoint.RingIntermediate;
                case TrackedHandJoint.RingTip:
                    return TrackedHandJoint.RingDistal;
                case TrackedHandJoint.LittleMetacarpal:
                    return TrackedHandJoint.Wrist;
                case TrackedHandJoint.LittleProximal:
                    return TrackedHandJoint.LittleMetacarpal;
                case TrackedHandJoint.LittleIntermediate:
                    return TrackedHandJoint.LittleProximal;
                case TrackedHandJoint.LittleDistal:
                    return TrackedHandJoint.LittleIntermediate;
                case TrackedHandJoint.LittleTip:
                    return TrackedHandJoint.LittleDistal;
                case TrackedHandJoint.Wrist:
                default:
                    return TrackedHandJoint.Wrist;
            }
        }

        /// <summary>
        /// Gets an estimated <see cref="TrackedHandJoint.IndexMetacarpal"/> pose.
        /// Requires known <see cref="TrackedHandJoint.ThumbMetacarpal"/> and
        /// <see cref="TrackedHandJoint.LittleMetacarpal"/> poses.
        /// </summary>
        /// <param name="jointPoses">Known joint poses.</param>
        /// <returns>Estimated <see cref="TrackedHandJoint.IndexMetacarpal"/> pose.</returns>
        public static MixedRealityPose GetEstimatedIndexMetacarpalPose(MixedRealityPose[] jointPoses)
        {
            MixedRealityPose thumbMetacarpalPose = jointPoses[(int)TrackedHandJoint.ThumbMetacarpal];
            MixedRealityPose littleMetacarpalPose = jointPoses[(int)TrackedHandJoint.LittleMetacarpal];

            Vector3 indexMetacarpalPosition = Vector3.Lerp(thumbMetacarpalPose.Position, littleMetacarpalPose.Position, .2f);
            Quaternion indexMetacarpalRotation = jointPoses[(int)TrackedHandJoint.Wrist].Rotation;

            return new MixedRealityPose(indexMetacarpalPosition, indexMetacarpalRotation);
        }

        /// <summary>
        /// Gets an estimated <see cref="TrackedHandJoint.RingMetacarpal"/> pose.
        /// Requires known <see cref="TrackedHandJoint.ThumbMetacarpal"/> and
        /// <see cref="TrackedHandJoint.LittleMetacarpal"/> poses.
        /// </summary>
        /// <param name="jointPoses">Known joint poses.</param>
        /// <returns>Estimated <see cref="TrackedHandJoint.RingMetacarpal"/> pose.</returns>
        public static MixedRealityPose GetEstimatedRingMetacarpalPose(MixedRealityPose[] jointPoses)
        {
            MixedRealityPose thumbMetacarpalPose = jointPoses[(int)TrackedHandJoint.ThumbMetacarpal];
            MixedRealityPose littleMetacarpalPose = jointPoses[(int)TrackedHandJoint.LittleMetacarpal];

            Vector3 ringMetacarpalPosition = Vector3.Lerp(thumbMetacarpalPose.Position, littleMetacarpalPose.Position, .8f);
            Quaternion ringMetacarpalRotation = jointPoses[(int)TrackedHandJoint.LittleMetacarpal].Rotation;

            return new MixedRealityPose(ringMetacarpalPosition, ringMetacarpalRotation);
        }

        /// <summary>
        /// Gets an estimated <see cref="TrackedHandJoint.MiddleMetacarpal"/> pose.
        /// Requires known <see cref="TrackedHandJoint.ThumbMetacarpal"/> and
        /// <see cref="TrackedHandJoint.LittleMetacarpal"/> poses.
        /// </summary>
        /// <param name="jointPoses">Known joint poses.</param>
        /// <returns>Estimated <see cref="TrackedHandJoint.MiddleMetacarpal"/> pose.</returns>
        public static MixedRealityPose GetEstimatedMiddleMetacarpalPose(MixedRealityPose[] jointPoses)
        {
            MixedRealityPose thumbMetacarpalPose = jointPoses[(int)TrackedHandJoint.ThumbMetacarpal];
            MixedRealityPose littleMetacarpalPose = jointPoses[(int)TrackedHandJoint.LittleMetacarpal];

            Vector3 middleMetacarpalPosition = Vector3.Lerp(thumbMetacarpalPose.Position, littleMetacarpalPose.Position, .5f);
            Quaternion middleMetacarpalRotation = jointPoses[(int)TrackedHandJoint.Wrist].Rotation;

            return new MixedRealityPose(middleMetacarpalPosition, middleMetacarpalRotation);
        }

        /// <summary>
        /// Gets an estimated <see cref="TrackedHandJoint.Palm"/> pose.
        /// Requires known <see cref="TrackedHandJoint.MiddleMetacarpal"/> and
        /// <see cref="TrackedHandJoint.MiddleProximal"/> poses.
        /// </summary>
        /// <param name="jointPoses">Known joint poses.</param>
        /// <returns>Estimated <see cref="TrackedHandJoint.Palm"/> pose.</returns>
        public static MixedRealityPose GetEstimatedPalmPose(MixedRealityPose[] jointPoses)
        {
            MixedRealityPose middleMetacarpalPose = GetEstimatedMiddleMetacarpalPose(jointPoses);
            MixedRealityPose middleProximalPose = jointPoses[(int)TrackedHandJoint.MiddleProximal];

            Vector3 palmPosition = Vector3.Lerp(middleMetacarpalPose.Position, middleProximalPose.Position, .5f);
            Quaternion palmRotation = middleMetacarpalPose.Rotation;

            return new MixedRealityPose(palmPosition, palmRotation);
        }
    }
}