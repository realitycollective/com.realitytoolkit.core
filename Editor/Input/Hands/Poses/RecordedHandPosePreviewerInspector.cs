// Copyright (c) Reality Collective. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using RealityCollective.Definitions.Utilities;
using RealityToolkit.Editor.Utilities;
using RealityToolkit.Input.Hands.Poses;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace RealityToolkit.Editor
{
    [CustomEditor(typeof(RecordedHandPosePreviewer), true)]
    public class RecordedHandPosePreviewerInspector : UnityEditor.Editor
    {
        public override VisualElement CreateInspectorGUI()
        {
            var inspector = new VisualElement();
            var previewer = (RecordedHandPosePreviewer)target;

            var recordedHandednessField = new EnumField("Previewed Handedness", Handedness.Left)
            {
                bindingPath = "previewedHandedness"
            };

            inspector.Add(recordedHandednessField);

            var serializedPoseField = new ObjectField("Hand Pose")
            {
                objectType = typeof(HandPose),
                bindingPath = "handPose"
            };

            var frameField = new Slider("Frame", 0f, 1f)
            {
                bindingPath = "frame"
            };

            inspector.Add(frameField);
            inspector.Add(serializedPoseField);
            inspector.Add(UIElementsUtilities.Space());
            inspector.Add(new Button(Preview) { text = "Preview" });

            return inspector;
        }

        private void Preview()
        {
            var previewer = (RecordedHandPosePreviewer)target;
            previewer.Preview();
        }
    }
}
