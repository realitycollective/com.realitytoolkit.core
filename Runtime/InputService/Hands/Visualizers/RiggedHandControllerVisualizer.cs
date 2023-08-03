// Copyright (c) Reality Collective. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using RealityToolkit.EventDatum.Input;
using RealityToolkit.Input.Definitions;
using RealityToolkit.Input.Hands.Poses;
using System.Collections.Generic;
using UnityEngine;

namespace RealityToolkit.Input.Hands.Visualizers
{
    /// <summary>
    /// Visualizers a controller using a rigged hand mesh.
    /// </summary>
    [System.Runtime.InteropServices.Guid("f7dc4217-86da-4540-a2e4-3e721994e7c8")]
    public class RiggedHandControllerVisualizer : BaseHandControllerVisualizer
    {
        [Header("Idle")]
        [SerializeField, Tooltip("The hand pose to take when no input is happening.")]
        private HandPose idlePose = null;

        [Header("Select")]
        [SerializeField, Tooltip("The hand pose to take when the select input is happeing.")]
        private HandPose selectPose = null;

        [SerializeField, Tooltip("Input action to listen for to transition to the select pose.")]
        private InputAction selectInputAction = InputAction.None;

        [Header("Grip")]
        [SerializeField, Tooltip("The hand pose to take when the grip input is happeing.")]
        private HandPose gripPose = null;

        [SerializeField, Tooltip("Input action to listen for to transition to the grip pose.")]
        private InputAction gripInputAction = InputAction.None;

        private bool animating;
        private float animationStartTime;
        private const float animationDuration = .2f;
        private float previousSingleAxisInputValue;
        private readonly Dictionary<HandJoint, Pose> animationStartPoses = new Dictionary<HandJoint, Pose>();

        /// <summary>
        /// The current <see cref="HandPose"/> visualized.
        /// </summary>
        public HandPose CurrentPose { get; private set; }

        private void Update()
        {
            if (!animating)
            {
                return;
            }

            var t = (Time.time - animationStartTime) / animationDuration;
            Lerp(t);

            if (t >= 1f)
            {
                animating = false;
            }
        }

        /// <inheritdoc/>
        public override void OnInputDown(InputEventData eventData)
        {
            base.OnInputDown(eventData);

            //if (eventData.InputSource == Controller.InputSource &&
            //    eventData.InputAction == selectInputAction)
            //{
            //    Transition(selectPose);
            //}
            //else if (eventData.InputSource == Controller.InputSource &&
            //    eventData.InputAction == gripInputAction)
            //{
            //    Transition(gripPose);
            //}
        }

        /// <inheritdoc/>
        public override void OnInputUp(InputEventData eventData)
        {
            //if (eventData.InputSource == Controller.InputSource &&
            //    eventData.InputAction == selectInputAction)
            //{
            //    Transition(idlePose);
            //}
            //else if (eventData.InputSource == Controller.InputSource &&
            //    eventData.InputAction == gripInputAction)
            //{
            //    Transition(idlePose);
            //}

            base.OnInputUp(eventData);
        }

        /// <inheritdoc/>
        public override void OnInputChanged(InputEventData<float> eventData)
        {
            base.OnInputChanged(eventData);

            if (eventData.InputSource == Controller.InputSource &&
                eventData.InputAction == selectInputAction)
            {
                if (Mathf.Approximately(0f, eventData.InputData))
                {
                    Transition(idlePose);
                    return;
                }

                Transition(selectPose, eventData.InputData);
            }
            else if (eventData.InputSource == Controller.InputSource &&
                eventData.InputAction == gripInputAction)
            {
                if (Mathf.Approximately(0f, eventData.InputData))
                {
                    Transition(idlePose);
                }
                else if (eventData.InputData < previousSingleAxisInputValue)
                {
                    Transition(idlePose, 1f - eventData.InputData);
                }
                else if (eventData.InputData > previousSingleAxisInputValue)
                {
                    Transition(gripPose, eventData.InputData);
                }

                previousSingleAxisInputValue = eventData.InputData;
            }
        }

        /// <summary>
        /// Transitions the hand into <paramref name="targetPose"/>.
        /// </summary>
        /// <param name="targetPose">Recorded joint pose information.</param>
        /// <param name="animate">If set, the transition will be animated. Defaults to <c>true</c>.</param>
        private void Transition(HandPose targetPose, bool animate = true)
        {
            ComputeStartFrame();
            CurrentPose = targetPose;

            if (animate)
            {
                animationStartTime = Time.time;
                animating = true;
            }
            else
            {
                animating = false;
                Lerp(1f);
            }
        }

        /// <summary>
        /// Transitions the hand into <paramref name="targetPose"/> at frame <paramref name="t"/>.
        /// </summary>
        /// <param name="targetPose">Recorded joint pose information.</param>
        /// <param name="t">The interpolation frame time between the current pose and the target pose.</param>
        private void Transition(HandPose targetPose, float t)
        {
            ComputeStartFrame();
            CurrentPose = targetPose;
            animating = false;
            Lerp(t);
        }

        /// <summary>
        /// Linearly interpolates all <see cref="HandJoint"/>s on the rig between the <see cref="animationStartPoses"/>
        /// and the <see cref="CurrentPose"/>.
        /// </summary>
        /// <param name="t">The pose frame to apply. Value in range <c>[0,1]</c> inclusive, where <c>0f</c> is the start frame and <c>1f</c> is the final pose.</param>
        private void Lerp(float t)
        {
            for (int i = 0; i < jointCount; i++)
            {
                var handJoint = (HandJoint)i;

                if (jointTransformProvider.TryGetTransform(handJoint, out var jointTransform) &&
                    animationStartPoses.TryGetValue(handJoint, out var startPose) &&
                    CurrentPose.TryGetPose(Controller.ControllerHandedness, handJoint, out var targetJointPose))
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
        private void ComputeStartFrame()
        {
            animationStartPoses.Clear();

            for (int i = 0; i < jointCount; i++)
            {
                var handJoint = (HandJoint)i;

                // For any given joint on the rig record the current pose as the starting point.
                if (jointTransformProvider.TryGetTransform(handJoint, out var jointTransform))
                {
                    animationStartPoses.Add(handJoint, new Pose(jointTransform.localPosition, jointTransform.localRotation));
                }
            }
        }
    }
}
