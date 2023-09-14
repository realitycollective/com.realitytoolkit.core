// Copyright (c) Reality Collective. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using RealityCollective.Extensions;
using UnityEngine;

namespace RealityToolkit.Input.Hands.Poses
{
    /// <summary>
    /// Previews a recorded <see cref="HandPose"/>.
    /// </summary>
    public class RecordedHandPosePreviewer : MonoBehaviour
    {
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
                Debug.LogError($"{GetType().Name} requires an {nameof(IHandJointTransformProvider)} on the {nameof(GameObject)}.", this);
                return;
            }

            poseAnimator ??= new HandPoseAnimator(jointTransformProvider);
            poseAnimator.Transition(handPose, frame);
        }
    }
}
