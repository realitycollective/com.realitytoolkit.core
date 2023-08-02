// Copyright (c) Reality Collective. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using RealityToolkit.EventDatum.Input;
using RealityToolkit.Input.Controllers;
using RealityToolkit.Input.Definitions;
using RealityToolkit.Input.Hands.Poses;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace RealityToolkit.Input.Hands.Visualizers
{
    /// <summary>
    /// Visualizers a controller using a rigged hand mesh.
    /// </summary>
    [System.Runtime.InteropServices.Guid("f7dc4217-86da-4540-a2e4-3e721994e7c8")]
    public class RiggedHandControllerVisualizer : BaseControllerVisualizer
    {
        [Header("Idle")]
        [SerializeField, Tooltip("The hand pose to take when no input is happening.")]
        private RecordedHandPose idlePose = null;

        [Header("Select")]
        [SerializeField, Tooltip("The hand pose to take when the select input is happeing.")]
        private RecordedHandPose selectPose = null;

        [SerializeField, Tooltip("Input action to listen for to transition to the select pose.")]
        private InputAction selectInputAction = InputAction.None;

        private bool animating;
        private float animationStartTime;
        private const float animationDuration = .2f;
        private readonly Dictionary<HandJoint, Pose> animationStartPoses = new Dictionary<HandJoint, Pose>();
        private IHandJointTransformProvider jointTransformProvider;
        private int jointCount;

        /// <summary>
        /// The current <see cref="RecordedHandPose"/> visualized.
        /// </summary>
        public RecordedHandPose CurrentPose { get; private set; }

        private void Awake()
        {
            jointCount = Enum.GetNames(typeof(HandJoint)).Length;

            if (!TryGetComponent(out jointTransformProvider))
            {
                Debug.LogError($"{GetType().Name} requires an {nameof(IHandJointTransformProvider)} on the {nameof(GameObject)}.", this);
                return;
            }
        }

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

            if (eventData.InputSource == Controller.InputSource &&
                eventData.InputAction == selectInputAction)
            {
                Transition(selectPose);
            }
        }

        /// <inheritdoc/>
        public override void OnInputUp(InputEventData eventData)
        {
            if (eventData.InputSource == Controller.InputSource &&
                eventData.InputAction == selectInputAction)
            {
                Transition(idlePose);
            }

            base.OnInputUp(eventData);
        }

        /// <summary>
        /// Transitions the hand into <paramref name="recordedHandPose"/>.
        /// </summary>
        /// <param name="recordedHandPose">Recorded joint pose information.</param>
        private void Transition(RecordedHandPose recordedHandPose, bool animate = true)
        {
            ComputeStartPoses();
            CurrentPose = recordedHandPose;

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

        private void Lerp(float t)
        {
            for (int i = 0; i < jointCount; i++)
            {
                var handJoint = (HandJoint)i;

                if (jointTransformProvider.TryGetTransform(handJoint, out var jointTransform) &&
                    animationStartPoses.TryGetValue(handJoint, out var startPose) &&
                    CurrentPose.TryGetPose(handJoint, out var targetJointPose))
                {
                    jointTransform.SetLocalPositionAndRotation(Vector3.Slerp(startPose.position, targetJointPose.position, t), Quaternion.Slerp(startPose.rotation, targetJointPose.rotation, t));
                }
            }
        }

        private void ComputeStartPoses()
        {
            animationStartPoses.Clear();

            for (int i = 0; i < jointCount; i++)
            {
                var handJoint = (HandJoint)i;

                if (jointTransformProvider.TryGetTransform(handJoint, out var jointTransform))
                {
                    animationStartPoses.Add(handJoint, new Pose(jointTransform.localPosition, jointTransform.localRotation));
                }
            }
        }
    }
}
