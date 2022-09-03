// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.﻿

using RealityCollective.ServiceFramework.Editor.Profiles;
using RealityToolkit.LocomotionSystem.Definitions;
using UnityEditor;

namespace RealityToolkit.Editor.Profiles.LocomotionSystem
{
    [CustomEditor(typeof(TeleportValidationProviderProfile))]
    public class TeleportValidationProviderProfileInspector : BaseProfileInspector
    {
        private SerializedProperty anchorsOnly;
        private SerializedProperty validLayers;
        private SerializedProperty invalidLayers;
        private SerializedProperty upDirectionThreshold;
        private SerializedProperty maxDistance;
        private SerializedProperty maxHeightDistance;

        protected override void OnEnable()
        {
            base.OnEnable();

            anchorsOnly = serializedObject.FindProperty(nameof(anchorsOnly));
            validLayers = serializedObject.FindProperty(nameof(validLayers));
            invalidLayers = serializedObject.FindProperty(nameof(invalidLayers));
            upDirectionThreshold = serializedObject.FindProperty(nameof(upDirectionThreshold));
            maxDistance = serializedObject.FindProperty(nameof(maxDistance));
            maxHeightDistance = serializedObject.FindProperty(nameof(maxHeightDistance));
        }

        public override void OnInspectorGUI()
        {
            RenderHeader("This profile defines the set of rules to validate against when deciding whether a teleport target location is valid or not.");

            serializedObject.Update();

            EditorGUILayout.PropertyField(anchorsOnly);
            EditorGUILayout.PropertyField(validLayers);
            EditorGUILayout.PropertyField(invalidLayers);
            EditorGUILayout.PropertyField(upDirectionThreshold);
            EditorGUILayout.PropertyField(maxDistance);
            EditorGUILayout.PropertyField(maxHeightDistance);

            serializedObject.ApplyModifiedProperties();
        }
    }
}
