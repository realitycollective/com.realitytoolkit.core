// Copyright (c) Reality Collective. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using RealityToolkit.Utilities.UX.Controllers.Hands;
using UnityEditor;
using UnityEngine;
using XRTK.Editor.Extensions;
using XRTK.Editor.Input.Handlers;

namespace RealityToolkit.Editor.Input.Handlers
{
    [CustomEditor(typeof(HandControllerVisualizer))]
    public class HandControllerVisualizerInspector : ControllerPoseSynchronizerInspector
    {
        private static readonly GUIContent renderingSettings = new GUIContent("Hand Rendering Settings");

        private SerializedProperty jointsModePrefab;
        private SerializedProperty meshModePrefab;

        protected override void OnEnable()
        {
            base.OnEnable();

            jointsModePrefab = serializedObject.FindProperty(nameof(jointsModePrefab));
            meshModePrefab = serializedObject.FindProperty(nameof(meshModePrefab));
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            serializedObject.Update();

            if (jointsModePrefab.FoldoutWithBoldLabelPropertyField(renderingSettings))
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(meshModePrefab);
                EditorGUI.indentLevel--;
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}