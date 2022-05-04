// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;
using RealityToolkit.Definitions.Controllers;
using RealityToolkit.Definitions.InputSystem;
using RealityToolkit.Editor.Extensions;
using RealityToolkit.Editor.Profiles.InputSystem.Controllers;
using RealityToolkit.Services;
using UnityEditor;
using UnityEngine;
using RealityToolkit.Extensions;

namespace RealityToolkit.Editor.Profiles.InputSystem
{
    [CustomEditor(typeof(MixedRealityInputSystemProfile))]
    public class MixedRealityInputSystemProfileInspector : MixedRealityServiceProfileInspector
    {
        private static readonly GUIContent FocusProviderContent = new GUIContent("Focus Provider");
        private static readonly GUIContent GazeProviderContent = new GUIContent("Gaze Provider");
        private static readonly GUIContent GazeCursorPrefabContent = new GUIContent("Gaze Cursor Prefab");
        private static readonly GUIContent GlobalPointerSettingsContent = new GUIContent("Global Pointer Settings");
        private static readonly GUIContent ShowControllerMappingsContent = new GUIContent("Controller Action Mappings");

        private SerializedProperty focusProviderType;
        private SerializedProperty gazeProviderBehaviour;
        private SerializedProperty gazeProviderType;
        private SerializedProperty gazeCursorPrefab;

        private SerializedProperty handControllerSettings;

        private SerializedProperty pointingExtent;
        private SerializedProperty pointerRaycastLayerMasks;
        private SerializedProperty drawDebugPointingRays;
        private SerializedProperty debugPointingRayColors;

        private SerializedProperty inputActionsProfile;
        private SerializedProperty speechCommandsProfile;
        private SerializedProperty gesturesProfile;

        private bool showGlobalPointerOptions;
        private bool showGlobalHandOptions;
        private bool showAggregatedSimpleControllerMappingProfiles;

        private Dictionary<string, Tuple<BaseMixedRealityControllerDataProviderProfile, MixedRealityControllerMappingProfile>> controllerMappingProfiles;

        protected override void OnEnable()
        {
            base.OnEnable();

            focusProviderType = serializedObject.FindProperty(nameof(focusProviderType));
            gazeProviderBehaviour = serializedObject.FindProperty(nameof(gazeProviderBehaviour));
            gazeProviderType = serializedObject.FindProperty(nameof(gazeProviderType));
            gazeCursorPrefab = serializedObject.FindProperty(nameof(gazeCursorPrefab));

            pointingExtent = serializedObject.FindProperty(nameof(pointingExtent));
            pointerRaycastLayerMasks = serializedObject.FindProperty(nameof(pointerRaycastLayerMasks));
            drawDebugPointingRays = serializedObject.FindProperty(nameof(drawDebugPointingRays));
            debugPointingRayColors = serializedObject.FindProperty(nameof(debugPointingRayColors));

            handControllerSettings = serializedObject.FindProperty(nameof(handControllerSettings));

            inputActionsProfile = serializedObject.FindProperty(nameof(inputActionsProfile));
            gesturesProfile = serializedObject.FindProperty(nameof(gesturesProfile));
            speechCommandsProfile = serializedObject.FindProperty(nameof(speechCommandsProfile));

            controllerMappingProfiles = new Dictionary<string, Tuple<BaseMixedRealityControllerDataProviderProfile, MixedRealityControllerMappingProfile>>();

            for (int i = 0; i < Configurations?.arraySize; i++)
            {
                var configurationProperty = Configurations.GetArrayElementAtIndex(i);
                var configurationProfileProperty = configurationProperty.FindPropertyRelative("profile");

                if (configurationProfileProperty != null &&
                    configurationProfileProperty.objectReferenceValue is BaseMixedRealityControllerDataProviderProfile controllerDataProviderProfile)
                {
                    if (controllerDataProviderProfile.IsNull() ||
                        controllerDataProviderProfile.ControllerMappingProfiles == null)
                    {
                        continue;
                    }

                    foreach (var mappingProfile in controllerDataProviderProfile.ControllerMappingProfiles)
                    {
                        if (mappingProfile.IsNull()) { continue; }

                        AssetDatabase.TryGetGUIDAndLocalFileIdentifier(mappingProfile, out var guid, out long _);

                        if (!controllerMappingProfiles.ContainsKey(guid))
                        {
                            controllerMappingProfiles.Add(guid, new Tuple<BaseMixedRealityControllerDataProviderProfile, MixedRealityControllerMappingProfile>(controllerDataProviderProfile, mappingProfile));
                        }
                    }
                }
            }
        }

        public override void OnInspectorGUI()
        {
            RenderHeader("The Input System Profile helps developers configure input no matter what platform you're building for.");

            serializedObject.Update();
            EditorGUI.BeginChangeCheck();

            EditorGUILayout.PropertyField(focusProviderType, FocusProviderContent);
            EditorGUILayout.PropertyField(gazeProviderBehaviour);
            EditorGUILayout.PropertyField(gazeProviderType, GazeProviderContent);
            EditorGUILayout.PropertyField(gazeCursorPrefab, GazeCursorPrefabContent);

            EditorGUILayout.Space();

            showGlobalPointerOptions = EditorGUILayoutExtensions.FoldoutWithBoldLabel(showGlobalPointerOptions, GlobalPointerSettingsContent, true);

            if (showGlobalPointerOptions)
            {
                EditorGUILayout.HelpBox("Global pointer options applied to all controllers that support pointers. You may override these globals per controller mapping profile.", MessageType.Info);
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(pointingExtent);
                EditorGUILayout.Space();
                EditorGUILayout.PropertyField(pointerRaycastLayerMasks, true);
                EditorGUILayout.Space();

                EditorGUI.BeginChangeCheck();
                EditorGUI.indentLevel--;
                var newValue = EditorGUILayout.ToggleLeft(new GUIContent(drawDebugPointingRays.displayName, drawDebugPointingRays.tooltip), drawDebugPointingRays.boolValue);
                EditorGUI.indentLevel++;

                if (EditorGUI.EndChangeCheck())
                {
                    drawDebugPointingRays.boolValue = newValue;
                }

                EditorGUILayout.Space();
                EditorGUILayout.PropertyField(debugPointingRayColors, true);
                EditorGUI.indentLevel--;
            }

            EditorGUILayout.Space();

            EditorGUILayout.PropertyField(handControllerSettings);
            EditorGUILayout.PropertyField(inputActionsProfile);
            EditorGUILayout.PropertyField(speechCommandsProfile);
            EditorGUILayout.PropertyField(gesturesProfile);

            EditorGUILayout.Space();

            showAggregatedSimpleControllerMappingProfiles = EditorGUILayoutExtensions.FoldoutWithBoldLabel(showAggregatedSimpleControllerMappingProfiles, ShowControllerMappingsContent);

            if (showAggregatedSimpleControllerMappingProfiles)
            {
                foreach (var controllerMappingProfile in controllerMappingProfiles)
                {
                    var (dataProviderProfile, mappingProfile) = controllerMappingProfile.Value;
                    var profileEditor = CreateEditor(dataProviderProfile);

                    if (profileEditor is BaseMixedRealityControllerDataProviderProfileInspector inspector)
                    {
                        inspector.RenderControllerMappingButton(mappingProfile);
                    }

                    profileEditor.Destroy();
                }
            }

            serializedObject.ApplyModifiedProperties();

            if (EditorGUI.EndChangeCheck() &&
                MixedRealityToolkit.IsInitialized)
            {
                EditorApplication.delayCall += () => MixedRealityToolkit.Instance.ResetProfile(MixedRealityToolkit.Instance.ActiveProfile);
            }

            base.OnInspectorGUI();
        }
    }
}
