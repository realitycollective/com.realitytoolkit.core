// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using RealityCollective.Editor.Extensions;
using RealityCollective.Extensions;
using RealityCollective.ServiceFramework.Editor.Profiles;
using RealityCollective.ServiceFramework.Services;
using RealityToolkit.Definitions.Controllers;
using RealityToolkit.Editor.Profiles.InputSystem.Controllers;
using RealityToolkit.InputSystem.Definitions;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace RealityToolkit.Editor.Profiles.InputSystem
{
    [CustomEditor(typeof(MixedRealityInputSystemProfile))]
    public class MixedRealityInputSystemProfileInspector : ServiceProfileInspector
    {
        private static readonly GUIContent GazeProviderBehaviourContent = new GUIContent("Gaze Provider Mode");
        private static readonly GUIContent GazeProviderContent = new GUIContent("Gaze Provider");
        private static readonly GUIContent GazeCursorPrefabContent = new GUIContent("Gaze Cursor Prefab");
        private static readonly GUIContent GlobalPointerSettingsContent = new GUIContent("Global Pointer Settings");
        private static readonly GUIContent ShowControllerMappingsContent = new GUIContent("Controller Action Mappings");

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
        private bool showAggregatedSimpleControllerMappingProfiles;
        private ReorderableList poseProfilesList;
        private int currentlySelectedPoseElement;

        private Dictionary<string, Tuple<BaseMixedRealityControllerServiceModuleProfile, MixedRealityControllerMappingProfile>> controllerMappingProfiles;

        protected override void OnEnable()
        {
            base.OnEnable();

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

            controllerMappingProfiles = new Dictionary<string, Tuple<BaseMixedRealityControllerServiceModuleProfile, MixedRealityControllerMappingProfile>>();

            for (int i = 0; i < Configurations?.arraySize; i++)
            {
                var configurationProperty = Configurations.GetArrayElementAtIndex(i);
                var configurationProfileProperty = configurationProperty.FindPropertyRelative("profile");

                if (configurationProfileProperty != null &&
                    configurationProfileProperty.objectReferenceValue is BaseMixedRealityControllerServiceModuleProfile controllerDataProviderProfile)
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
                            controllerMappingProfiles.Add(guid, new Tuple<BaseMixedRealityControllerServiceModuleProfile, MixedRealityControllerMappingProfile>(controllerDataProviderProfile, mappingProfile));
                        }
                    }
                }
            }
        }

        protected override void RenderConfigurationOptions(bool forceExpanded = false)
        {
            RenderHeader("The Input System Profile helps developers configure input no matter what platform you're building for.");

            serializedObject.Update();
            EditorGUI.BeginChangeCheck();

            EditorGUILayout.PropertyField(gazeProviderBehaviour, GazeProviderBehaviourContent);
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

                    if (profileEditor is BaseMixedRealityControllerServiceModuleProfileInspector inspector)
                    {
                        inspector.RenderControllerMappingButton(mappingProfile);
                    }

                    profileEditor.Destroy();
                }
            }

            DrawServiceModulePropertyDrawer();

            serializedObject.ApplyModifiedProperties();

            if (EditorGUI.EndChangeCheck() &&
                ServiceManager.IsActiveAndInitialized)
            {
                EditorApplication.delayCall += () => ServiceManager.Instance.ResetProfile(ServiceManager.Instance.ActiveProfile);
            }
        }
    }
}
