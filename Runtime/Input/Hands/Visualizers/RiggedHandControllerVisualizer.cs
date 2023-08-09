// Copyright (c) Reality Collective. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using RealityCollective.Extensions;
using RealityToolkit.EventDatum.Input;
using RealityToolkit.Input.Definitions;
using RealityToolkit.Input.Hands.Poses;
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

        private HandPoseAnimator poseAnimator;
        private HandPose overridePose;

        /// <summary>
        /// Override pose is applied to the visualizer regardless of the input state.
        /// </summary>
        public HandPose OverridePose
        {
            get => overridePose;
            set
            {
                if (value.IsNull())
                {
                    overridePose = null;
                    poseAnimator.Transition(idlePose);
                    return;
                }

                overridePose = value;
                poseAnimator.Transition(overridePose);
            }
        }

        /// <inheritdoc/>
        protected override void Awake()
        {
            base.Awake();
            poseAnimator = new HandPoseAnimator(jointTransformProvider, Handedness);
        }

        /// <inheritdoc/>
        private void Update()
        {
            poseAnimator.Update();
        }

        /// <inheritdoc/>
        public override void OnInputDown(InputEventData eventData)
        {
            base.OnInputDown(eventData);

            if (eventData.InputSource == Controller.InputSource &&
                eventData.InputAction == selectInputAction)
            {
                poseAnimator.Transition(selectPose);
            }
            else if (eventData.InputSource == Controller.InputSource &&
                eventData.InputAction == gripInputAction)
            {
                poseAnimator.Transition(gripPose);
            }
        }

        /// <inheritdoc/>
        public override void OnInputUp(InputEventData eventData)
        {
            if (eventData.InputSource == Controller.InputSource &&
                eventData.InputAction == selectInputAction)
            {
                poseAnimator.Transition(idlePose);
            }
            else if (eventData.InputSource == Controller.InputSource &&
                eventData.InputAction == gripInputAction)
            {
                poseAnimator.Transition(idlePose);
            }

            base.OnInputUp(eventData);
        }

        /// <inheritdoc/>
        public override void OnInputChanged(InputEventData<float> eventData)
        {
            base.OnInputChanged(eventData);

            if (eventData.InputSource == Controller.InputSource &&
                eventData.InputAction == selectInputAction)
            {
                OnSingleAxisInputChanged(eventData.InputData, selectPose);
            }
            else if (eventData.InputSource == Controller.InputSource &&
                eventData.InputAction == gripInputAction)
            {
                OnSingleAxisInputChanged(eventData.InputData, gripPose);
            }
        }

        private void OnSingleAxisInputChanged(float value, HandPose targetPose)
        {
            if (Mathf.Approximately(0f, value))
            {
                poseAnimator.Transition(idlePose);
                return;
            }

            poseAnimator.Transition(targetPose, value);
        }
    }
}
