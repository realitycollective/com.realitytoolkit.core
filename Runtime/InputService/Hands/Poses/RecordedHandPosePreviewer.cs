// Copyright (c) Reality Collective. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using RealityCollective.Definitions.Utilities;
using RealityCollective.Extensions;
using UnityEngine;

namespace RealityToolkit.Input.Hands.Poses
{
    /// <summary>
    /// Previews a recorded <see cref="HandPose"/>.
    /// </summary>
    public class RecordedHandPosePreviewer : MonoBehaviour
    {
        [SerializeField, Tooltip("The handedness to preview the pose with.")]
        private Handedness previewedHandedness = Handedness.Left;

        [SerializeField, Tooltip("The hand pose to preview.")]
        private HandPose handPose = null;

        [SerializeField, Range(0f, 1f)]
        private float frame = 1f;

        private IHandJointTransformProvider jointTransformProvider;
        private HandPoseAnimator poseAnimator;

        /// <summary>
        /// Previews the assigned <see cref="handPose"/>.
        /// </summary>
        public void Preview()
        {
            if (handPose.IsNull())
            {
                Debug.LogError("Cannot preview hand pose. No pose assigned.", this);
                return;
            }

            if (!TryGetComponent(out jointTransformProvider))
            {
                Debug.LogError($"{GetType().Name} requires an {nameof(IHandJointTransformProvider)} on the {nameof(UnityEngine.GameObject)}.", this);
                return;
            }

            if (poseAnimator == null)
            {
                poseAnimator = new HandPoseAnimator(jointTransformProvider, previewedHandedness);
            }

            poseAnimator.Transition(handPose, frame);
        }
    }
}
