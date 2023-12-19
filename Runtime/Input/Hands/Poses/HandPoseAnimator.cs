using System;
using System.Collections.Generic;
using UnityEngine;

namespace RealityToolkit.Input.Hands.Poses
{
    /// <summary>
    /// Animation utility for blending between <see cref="HandPose"/>s.
    /// </summary>
    public class HandPoseAnimator
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="jointTransformProvider">The <see cref="IHandJointTransformProvider"/> used to access joints.</param>
        public HandPoseAnimator(IHandJointTransformProvider jointTransformProvider)
        {
            jointCount = Enum.GetNames(typeof(HandJoint)).Length;
            this.jointTransformProvider = jointTransformProvider;
            this.jointTransformProvider.JointTransformsChanged += JointTransformProvider_JointTransformsChanged;
        }

        /// <summary>
        /// Destructor.
        /// </summary>
        ~HandPoseAnimator()
        {
            if (jointTransformProvider != null)
            {
                jointTransformProvider.JointTransformsChanged -= JointTransformProvider_JointTransformsChanged;
            }
        }

        private readonly int jointCount;
        private readonly IHandJointTransformProvider jointTransformProvider;
        private readonly Dictionary<HandJoint, Pose> startFramePoses = new Dictionary<HandJoint, Pose>();
        private bool animating;
        private float animationStartTime;
        private const float animationDuration = .2f;

        /// <summary>
        /// The current <see cref="HandPose"/> visualized.
        /// </summary>
        public HandPose CurrentPose { get; private set; }

        /// <summary>
        /// Updates the animation.
        /// This must be called every frame.
        /// </summary>
        public void Update()
        {
            if (!animating)
            {
                return;
            }

            var t = (Time.time - animationStartTime) / animationDuration;
            Slerp(t);

            if (t >= 1f)
            {
                animating = false;
            }
        }

        /// <summary>
        /// Transitions the hand into <paramref name="targetPose"/>.
        /// </summary>
        /// <param name="targetPose">Recorded joint pose information.</param>
        /// <param name="animate">If set, the transition will be animated. Defaults to <c>true</c>.</param>
        public void Transition(HandPose targetPose, bool animate = true)
        {
            if (CurrentPose == targetPose)
            {
                return;
            }

            ComputeStartFrame(targetPose);
            CurrentPose = targetPose;

            if (animate)
            {
                animationStartTime = Time.time;
                animating = true;
            }
            else
            {
                animating = false;
                Slerp(1f);
            }
        }

        /// <summary>
        /// Transitions the hand into <paramref name="targetPose"/> at frame <paramref name="t"/>.
        /// </summary>
        /// <param name="targetPose">Recorded joint pose information.</param>
        /// <param name="t">The interpolation frame time between the current pose and the target pose.</param>
        public void Transition(HandPose targetPose, float t)
        {
            ComputeStartFrame(targetPose);
            CurrentPose = targetPose;
            animating = false;
            Slerp(t);
        }

        /// <summary>
        /// Linearly interpolates all <see cref="HandJoint"/>s on the rig between the <see cref="animationStartPoses"/>
        /// and the <see cref="CurrentPose"/>.
        /// </summary>
        /// <param name="t">The pose frame to apply. Value in range <c>[0,1]</c> inclusive, where <c>0f</c> is the start frame and <c>1f</c> is the final pose.</param>
        private void Slerp(float t)
        {
            for (int i = 0; i < jointCount; i++)
            {
                var handJoint = (HandJoint)i;

                if (jointTransformProvider.TryGetTransform(handJoint, out var jointTransform) &&
                    startFramePoses.TryGetValue(handJoint, out var startPose) &&
                    CurrentPose.TryGetPose(handJoint, out var targetJointPose))
                {
                    jointTransform.SetLocalPositionAndRotation(Vector3.Slerp(startPose.position, targetJointPose.position, t), Quaternion.Slerp(startPose.rotation, targetJointPose.rotation, t));
                }
            }
        }

        /// <summary>
        /// Computes a start frame to start animating into a new pose from.
        /// That way pose transitions can dynamically transition from any given
        /// state.
        /// </summary>
        private void ComputeStartFrame(HandPose targetPose)
        {
            if (targetPose == CurrentPose &&
                !Application.isEditor)
            {
                // No need to compute a new start frame if the target pose has not changed,
                // unless we are in the editor. For pose accurate pose recording we always want to make sure,
                // we are working with up-to-date poses.
                return;
            }

            startFramePoses.Clear();

            for (int i = 0; i < jointCount; i++)
            {
                var handJoint = (HandJoint)i;

                // For any given joint on the rig record the current pose as the starting point.
                if (jointTransformProvider.TryGetTransform(handJoint, out var jointTransform))
                {
                    startFramePoses.Add(handJoint, new Pose(jointTransform.localPosition, jointTransform.localRotation));
                }
            }
        }

        private void JointTransformProvider_JointTransformsChanged()
        {
            // Whenever the joint providers joints have changed, we must
            // invalidate the current pose and re-transition to make sure we are working
            // with latest data.
            var currentPose = CurrentPose;
            CurrentPose = null;
            Transition(currentPose, false);
        }
    }
}
