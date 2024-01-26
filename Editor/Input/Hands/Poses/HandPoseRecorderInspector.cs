// Copyright (c) Reality Collective. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using RealityCollective.Definitions.Utilities;
using RealityToolkit.Editor.Utilities;
using RealityToolkit.Input.Hands.Poses;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace RealityToolkit.Editor.Input.Hands.Poses
{
    [CustomEditor(typeof(HandPoseRecorder), true)]
    public class HandPoseRecorderInspector : UnityEditor.Editor
    {
        public override VisualElement CreateInspectorGUI()
        {
            var inspector = new VisualElement();
            var recorder = (HandPoseRecorder)target;

            var recordedHandednessField = new EnumField("Recorded Handedness", Handedness.Left)
            {
                bindingPath = "recordedHandedness"
            };

            inspector.Add(recordedHandednessField);

            var serializedPoseField = new ObjectField("Serialized Pose")
            {
                objectType = typeof(TextAsset),
                bindingPath = "serializedPose"
            };

            inspector.Add(serializedPoseField);
            inspector.Add(UIElementsUtilities.Space());
            inspector.Add(new Button(Convert) { text = "Convert" });
            inspector.Add(new Button(Record) { text = "Record" });

            return inspector;
        }

        private void Convert()
        {
            var recorder = (HandPoseRecorder)target;
            recorder.Convert();
        }

        private void Record()
        {
            var recorder = (HandPoseRecorder)target;
            recorder.Record();
        }
    }
}
