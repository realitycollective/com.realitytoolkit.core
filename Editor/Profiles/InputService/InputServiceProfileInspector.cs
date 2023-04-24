// Copyright (c) Reality Collective. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using RealityCollective.Editor.Extensions;
using RealityCollective.Extensions;
using RealityCollective.ServiceFramework.Editor.Profiles;
using RealityCollective.ServiceFramework.Services;
using RealityToolkit.Definitions.Controllers;
using RealityToolkit.Definitions.Controllers.Hands;
using RealityToolkit.Editor.Profiles.Input.Controllers;
using RealityToolkit.Input.Definitions;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace RealityToolkit.Editor.Profiles.Input
{
    [CustomEditor(typeof(InputServiceProfile))]
    public class InputServiceProfileInspector : ServiceProfileInspector
    {
        private static readonly GUIContent GazeProviderBehaviourContent = new GUIContent("Gaze Provider Mode");
        private static readonly GUIContent GazeProviderContent = new GUIContent("Gaze Provider");
        private static readonly GUIContent GazeCursorPrefabContent = new GUIContent("Gaze Cursor Prefab");
        private static readonly GUIContent GlobalHandSettingsContent = new GUIContent("Global Hand Settings");
        private static readonly GUIContent ShowControllerMappingsContent = new GUIContent("Controller Action Mappings");

        private SerializedProperty gazeProviderBehaviour;
        private SerializedProperty gazeProviderType;
        private SerializedProperty gazeCursorPrefab;

        private SerializedProperty pointersProfile;

        private SerializedProperty gripThreshold;
        private SerializedProperty renderingMode;
        private SerializedProperty handPhysicsEnabled;
        private SerializedProperty useTriggers;
        private SerializedProperty boundsMode;
        private SerializedProperty trackedPoses;

        private SerializedProperty inputActionsProfile;
        private SerializedProperty speechCommandsProfile;
        private SerializedProperty gesturesProfile;

        private bool showGlobalHandOptions;
        private bool showAggregatedSimpleControllerMappingProfiles;
        private ReorderableList poseProfilesList;
        private int currentlySelectedPoseElement;

        private Dictionary<string, Tuple<BaseControllerServiceModuleProfile, ControllerMappingProfile>> controllerMappingProfiles;

        protected override void OnEnable()
        {
            base.OnEnable();

            gazeProviderBehaviour = serializedObject.FindProperty(nameof(gazeProviderBehaviour));
            gazeProviderType = serializedObject.FindProperty(nameof(gazeProviderType));
            gazeCursorPrefab = serializedObject.FindProperty(nameof(gazeCursorPrefab));

            pointersProfile = serializedObject.FindProperty(nameof(pointersProfile));

            gripThreshold = serializedObject.FindProperty(nameof(gripThreshold));
            renderingMode = serializedObject.FindProperty(nameof(renderingMode));
            handPhysicsEnabled = serializedObject.FindProperty(nameof(handPhysicsEnabled));
            useTriggers = serializedObject.FindProperty(nameof(useTriggers));
            boundsMode = serializedObject.FindProperty(nameof(boundsMode));
            trackedPoses = serializedObject.FindProperty(nameof(trackedPoses));

            inputActionsProfile = serializedObject.FindProperty(nameof(inputActionsProfile));
            gesturesProfile = serializedObject.FindProperty(nameof(gesturesProfile));
            speechCommandsProfile = serializedObject.FindProperty(nameof(speechCommandsProfile));

            controllerMappingProfiles = new Dictionary<string, Tuple<BaseControllerServiceModuleProfile, ControllerMappingProfile>>();

            for (int i = 0; i < Configurations?.arraySize; i++)
            {
                var configurationProperty = Configurations.GetArrayElementAtIndex(i);
                var configurationProfileProperty = configurationProperty.FindPropertyRelative("profile");

                if (configurationProfileProperty != null &&
                    configurationProfileProperty.objectReferenceValue is BaseControllerServiceModuleProfile controllerDataProviderProfile)
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
                            controllerMappingProfiles.Add(guid, new Tuple<BaseControllerServiceModuleProfile, ControllerMappingProfile>(controllerDataProviderProfile, mappingProfile));
                        }
                    }
                }
            }

            poseProfilesList = new ReorderableList(serializedObject, trackedPoses, true, false, true, true)
            {
                elementHeight = EditorGUIUtility.singleLineHeight * 1.5f
            };
            poseProfilesList.drawHeaderCallback += PoseProfilesList_DrawHeaderCallback;
            poseProfilesList.drawElementCallback += PoseProfilesList_DrawConfigurationOptionElement;
            poseProfilesList.onAddCallback += PoseProfilesList_OnConfigurationOptionAdded;
            poseProfilesList.onRemoveCallback += PoseProfilesList_OnConfigurationOptionRemoved;
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

            showGlobalHandOptions = EditorGUILayoutExtensions.FoldoutWithBoldLabel(showGlobalHandOptions, GlobalHandSettingsContent, true);

            if (showGlobalHandOptions)
            {
                EditorGUILayout.HelpBox("Global hand tracking options applied to all platforms that support hand tracking. You may override these globals per platform in the platform's hand controller service module profile.", MessageType.Info);
                EditorGUI.indentLevel++;
                EditorGUILayout.Space();
                EditorGUILayout.LabelField("General Hand Settings", EditorStyles.boldLabel);
                EditorGUILayout.PropertyField(gripThreshold);
                EditorGUILayout.Space();
                EditorGUILayout.LabelField("Hand Rendering Settings", EditorStyles.boldLabel);
                EditorGUILayout.PropertyField(renderingMode);
                EditorGUILayout.Space();
                EditorGUILayout.LabelField("Hand Physics Settings", EditorStyles.boldLabel);
                EditorGUILayout.PropertyField(handPhysicsEnabled);
                EditorGUILayout.PropertyField(useTriggers);
                EditorGUILayout.PropertyField(boundsMode);
                poseProfilesList.DoLayoutList();
                EditorGUI.indentLevel--;
            }

            EditorGUILayout.Space();

            EditorGUILayout.PropertyField(pointersProfile);
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

                    if (profileEditor is BaseControllerServiceModuleProfileInspector inspector)
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

        private void PoseProfilesList_DrawHeaderCallback(Rect rect)
        {
            EditorGUI.LabelField(rect, "Tracked Hand Poses");
        }

        private void PoseProfilesList_DrawConfigurationOptionElement(Rect rect, int index, bool isActive, bool isFocused)
        {
            if (isFocused)
            {
                currentlySelectedPoseElement = index;
            }

            rect.height = EditorGUIUtility.singleLineHeight;
            rect.y += 3;
            var poseDataProperty = trackedPoses.GetArrayElementAtIndex(index);
            var selectedPoseData = EditorGUI.ObjectField(rect, poseDataProperty.objectReferenceValue, typeof(HandControllerPoseProfile), false) as HandControllerPoseProfile;

            if (selectedPoseData != null)
            {
                selectedPoseData.ParentProfile = ThisProfile;
            }

            poseDataProperty.objectReferenceValue = selectedPoseData;
        }

        private void PoseProfilesList_OnConfigurationOptionAdded(ReorderableList list)
        {
            trackedPoses.arraySize += 1;
            var index = trackedPoses.arraySize - 1;

            var mappingProfileProperty = trackedPoses.GetArrayElementAtIndex(index);
            mappingProfileProperty.objectReferenceValue = null;
            serializedObject.ApplyModifiedProperties();
        }

        private void PoseProfilesList_OnConfigurationOptionRemoved(ReorderableList list)
        {
            if (currentlySelectedPoseElement >= 0)
            {
                trackedPoses.DeleteArrayElementAtIndex(currentlySelectedPoseElement);
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}
