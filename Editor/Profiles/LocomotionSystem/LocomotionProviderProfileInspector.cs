﻿// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.﻿

using RealityToolkit.Definitions.LocomotionSystem;
using UnityEditor;
using UnityEngine;
using RealityToolkit.Editor.Extensions;

namespace RealityToolkit.Editor.Profiles.LocomotionSystem
{
    [CustomEditor(typeof(BaseLocomotionProviderProfile))]
    public class LocomotionProviderProfileInspector : BaseMixedRealityProfileInspector
    {
        private SerializedProperty startupBehaviour;
        private SerializedProperty inputAction;

        private static readonly GUIContent generalSettingsHeader = new GUIContent("General Settings");

        protected override void OnEnable()
        {
            base.OnEnable();

            startupBehaviour = serializedObject.FindProperty(nameof(startupBehaviour));
            inputAction = serializedObject.FindProperty(nameof(inputAction));
        }

        public override void OnInspectorGUI()
        {
            RenderHeader("This profile defines behaviour for the locomotion provider.");

            serializedObject.Update();

            if (startupBehaviour.FoldoutWithBoldLabelPropertyField(generalSettingsHeader))
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(inputAction);
                EditorGUI.indentLevel--;
            }

            EditorGUILayout.Space();

            serializedObject.ApplyModifiedProperties();
        }
    }
}
