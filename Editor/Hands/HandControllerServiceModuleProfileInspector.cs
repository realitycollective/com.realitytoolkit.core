// Copyright (c) Reality Collective. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using RealityToolkit.Editor.Profiles.InputSystem.Controllers;
using RealityToolkit.InputSystem.Hands;
using UnityEditor;
using UnityEngine;

namespace RealityToolkit.Editor.Hands
{
    [CustomEditor(typeof(HandControllerServiceModuleProfile<>), true, isFallback = true)]
    public class HandControllerServiceModuleProfileInspector : BaseMixedRealityControllerServiceModuleProfileInspector
    {
        private SerializedProperty handControllerSettings;
        private static readonly GUIContent handControllerSettingsLabel = new GUIContent("Hand Tracking Overrides");

        protected override void OnEnable()
        {
            base.OnEnable();
            handControllerSettings = serializedObject.FindProperty(nameof(handControllerSettings));
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            serializedObject.Update();

            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(handControllerSettings, handControllerSettingsLabel);
            EditorGUILayout.HelpBox("Assign an optional hand controler settings profile to override the global settings in the input service profile.", MessageType.Info);

            serializedObject.ApplyModifiedProperties();
        }
    }
}