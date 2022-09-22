// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information. 

using RealityCollective.Editor.Extensions;
using RealityCollective.ServiceFramework.Editor.Profiles;
using RealityCollective.ServiceFramework.Services;
using RealityToolkit.CameraSystem.Definitions;
using UnityEditor;
using UnityEngine;

namespace RealityToolkit.Editor.Profiles.CameraSystem
{
    [CustomEditor(typeof(MixedRealityCameraSystemProfile))]
    public class MixedRealityCameraSystemProfileInspector : ServiceProfileInspector
    {
        private SerializedProperty globalCameraProfile;

        private readonly GUIContent generalSettingsFoldoutHeader = new GUIContent("General Settings");

        protected override void OnEnable()
        {
            base.OnEnable();

            globalCameraProfile = serializedObject.FindProperty(nameof(globalCameraProfile));
            globalCameraProfile.isExpanded = true;
        }

        protected override void RenderConfigurationOptions(bool forceExpanded = false)
        {
            RenderHeader("The Camera Profile helps tweak camera settings no matter what platform you're building for.");

            serializedObject.Update();

            EditorGUI.BeginChangeCheck();

            globalCameraProfile.FoldoutWithBoldLabelPropertyField(generalSettingsFoldoutHeader);

            EditorGUILayout.Space();

            base.DrawDataProviderPropertyDrawer();

            serializedObject.ApplyModifiedProperties();

            if (ServiceManager.Instance != null &&
                ServiceManager.Instance.IsInitialized && EditorGUI.EndChangeCheck())
            {
                EditorApplication.delayCall += () => ServiceManager.Instance.ResetProfile(ServiceManager.Instance.ActiveProfile);
            }
        }
    }
}