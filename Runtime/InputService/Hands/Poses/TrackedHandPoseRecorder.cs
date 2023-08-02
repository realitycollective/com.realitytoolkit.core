// Copyright (c) Reality Collective. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using RealityCollective.Definitions.Utilities;
using RealityToolkit.Definitions.Controllers.Hands;
using RealityToolkit.EventDatum.Input;
using RealityToolkit.Input.Interfaces.Handlers;
using RealityToolkit.Input.Listeners;
using UnityEngine;

namespace RealityToolkit.Input.Hands.Poses
{
    /// <summary>
    /// Records a <see cref="RecordedHandPose"/> using a tracked <see cref="IHandController"/>.
    /// </summary>
    public class TrackedHandPoseRecorder : InputServiceGlobalListener, IInputHandler<HandData>
    {
        private RecordedHandJoints currentRecording;
        RecordedHandJoints recordedHandJoints = new RecordedHandJoints();
        RecordedHandJoint[] jointPoses = new RecordedHandJoint[HandData.JointCount];

        [SerializeField]
        [Tooltip("The handedness of the hand to record data for.")]
        private Handedness targetHandedness = Handedness.Right;

        [SerializeField]
        [Tooltip("Keycode to trigger saving of the currently recorded data.")]
        private KeyCode saveRecordingKey = KeyCode.Return;

        private void Update()
        {
            // TODO Update this to use an action instead of raw keyboard input
            if (currentRecording != null && UnityEngine.Input.GetKeyUp(saveRecordingKey))
            {
                // TODO dump to XRTK.Generated/Recorded Poses?
                Debug.Log(JsonUtility.ToJson(currentRecording));
            }
        }

        /// <inheritdoc />
        public void OnInputChanged(InputEventData<HandData> eventData)
        {
            if (targetHandedness != eventData.Handedness)
            {
                return;
            }

            for (int i = 0; i < HandData.JointCount; i++)
            {
                jointPoses[i] = new RecordedHandJoint((HandJoint)i, eventData.InputData.Joints[i]);
            }

            recordedHandJoints.Joints = jointPoses;
            currentRecording = recordedHandJoints;
        }
    }
}
