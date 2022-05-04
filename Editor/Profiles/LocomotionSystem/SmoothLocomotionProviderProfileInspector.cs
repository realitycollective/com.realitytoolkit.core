// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.﻿

using RealityToolkit.Definitions.LocomotionSystem;
using UnityEditor;
using UnityEngine;

namespace RealityToolkit.Editor.Profiles.LocomotionSystem
{
    [CustomEditor(typeof(SmoothLocomotionProviderProfile))]
    public class SmoothLocomotionProviderProfileInspector : LocomotionProviderProfileInspector
    {
        private SerializedProperty speed;

        private static readonly GUIContent speedLabel = new GUIContent("Speed (m/s)");

        protected override void OnEnable()
        {
            base.OnEnable();

            speed = serializedObject.FindProperty(nameof(speed));
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            serializedObject.Update();

            EditorGUILayout.PropertyField(speed, speedLabel);

            serializedObject.ApplyModifiedProperties();
        }
    }
}
