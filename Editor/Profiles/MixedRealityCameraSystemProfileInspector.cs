// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information. 

using RealityCollective.Editor.Extensions;
using RealityToolkit.Definitions.CameraSystem;
using RealityToolkit.Services;
using UnityEditor;
using UnityEngine;

namespace RealityToolkit.Editor.Profiles.CameraSystem
{
    [CustomEditor(typeof(MixedRealityCameraSystemProfile))]
    public class MixedRealityCameraSystemProfileInspector : MixedRealityServiceProfileInspector
    {
        private SerializedProperty globalCameraProfile;

        private readonly GUIContent generalSettingsFoldoutHeader = new GUIContent("General Settings");

        protected override void OnEnable()
        {
            base.OnEnable();

            globalCameraProfile = serializedObject.FindProperty(nameof(globalCameraProfile));
        }

        public override void OnInspectorGUI()
        {
            RenderHeader("The Camera Profile helps tweak camera settings no matter what platform you're building for.");

            serializedObject.Update();

            EditorGUI.BeginChangeCheck();

            globalCameraProfile.FoldoutWithBoldLabelPropertyField(generalSettingsFoldoutHeader);

            EditorGUILayout.Space();

            base.OnInspectorGUI();

            serializedObject.ApplyModifiedProperties();

            if (MixedRealityToolkit.IsInitialized && EditorGUI.EndChangeCheck())
            {
                EditorApplication.delayCall += () => MixedRealityToolkit.Instance.ResetProfile(MixedRealityToolkit.Instance.ActiveProfile);
            }
        }
    }
}