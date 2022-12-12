// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.﻿

using RealityCollective.Definitions.Utilities;
using RealityCollective.ServiceFramework.Editor.Profiles;
using RealityToolkit.Definitions.Controllers;
using RealityToolkit.Editor.PropertyDrawers;
using RealityToolkit.InputSystem.Interfaces.Handlers;
using UnityEditor;
using UnityEngine;

namespace RealityToolkit.Editor.Profiles.InputSystem.Controllers
{
    [CustomEditor(typeof(MixedRealityControllerVisualizationProfile))]
    public class MixedRealityControllerVisualizationProfileInspector : BaseProfileInspector
    {
        private SerializedProperty controllerVisualizationType;
        private SerializedProperty model;
        private SerializedProperty pointerPose;
        private SerializedProperty alternatePointerPose;
        private SerializedProperty controllerVisualizationSettings;

        private MixedRealityControllerVisualizationProfile controllerVisualizationProfile;

        private readonly MixedRealityInputActionDropdown inputActionDropdown = new MixedRealityInputActionDropdown();

        private float defaultLabelWidth;

        protected override void OnEnable()
        {
            base.OnEnable();

            defaultLabelWidth = EditorGUIUtility.labelWidth;

            controllerVisualizationProfile = target as MixedRealityControllerVisualizationProfile;

            controllerVisualizationType = serializedObject.FindProperty(nameof(controllerVisualizationType));
            model = serializedObject.FindProperty(nameof(model));
            pointerPose = serializedObject.FindProperty(nameof(pointerPose));
            alternatePointerPose = serializedObject.FindProperty(nameof(alternatePointerPose));
            controllerVisualizationSettings = serializedObject.FindProperty(nameof(controllerVisualizationSettings));
        }

        public override void OnInspectorGUI()
        {
            RenderHeader("This profile controls how the controller should be rendered in the scene.");

            serializedObject.Update();

            EditorGUIUtility.labelWidth = 168f;

            var modelPrefab = model.objectReferenceValue as GameObject;

            EditorGUILayout.PropertyField(controllerVisualizationType);

            if (controllerVisualizationProfile.ControllerVisualizationType == null ||
                controllerVisualizationProfile.ControllerVisualizationType.Type == null)
            {
                EditorGUILayout.HelpBox("A controller visualization type must be defined!", MessageType.Error);
            }

            EditorGUI.BeginChangeCheck();
            modelPrefab = EditorGUILayout.ObjectField(new GUIContent(model.displayName, "Note: If the default model is not found, the fallback is the global left hand model."), modelPrefab, typeof(GameObject), false) as GameObject;

            if (EditorGUI.EndChangeCheck() && CheckVisualizer(modelPrefab))
            {
                model.objectReferenceValue = modelPrefab;
            }

            inputActionDropdown.OnGui(new GUIContent(pointerPose.displayName, pointerPose.tooltip), pointerPose, AxisType.SixDof);
            inputActionDropdown.OnGui(new GUIContent(alternatePointerPose.displayName, alternatePointerPose.tooltip), alternatePointerPose, AxisType.SixDof);

            EditorGUIUtility.labelWidth = defaultLabelWidth;

            serializedObject.ApplyModifiedProperties();
        }

        private bool CheckVisualizer(GameObject modelPrefab)
        {
            if (modelPrefab == null) { return true; }

            if (PrefabUtility.GetPrefabAssetType(modelPrefab) == PrefabAssetType.NotAPrefab)
            {
                Debug.LogWarning("Assigned GameObject must be a prefab");
                return false;
            }

            var componentList = modelPrefab.GetComponentsInChildren<IMixedRealityControllerVisualizer>();

            if (componentList == null || componentList.Length == 0)
            {
                if (controllerVisualizationProfile.ControllerVisualizationType != null &&
                    controllerVisualizationProfile.ControllerVisualizationType.Type != null)
                {
                    modelPrefab.AddComponent(controllerVisualizationProfile.ControllerVisualizationType.Type);
                    return true;
                }

                Debug.LogError("No controller visualization type specified!");
            }
            else if (componentList.Length == 1)
            {
                return true;
            }
            else if (componentList.Length > 1)
            {
                Debug.LogWarning("Found too many IMixedRealityControllerVisualizer components on your prefab. There can only be one.");
            }

            return false;
        }
    }
}
