// Copyright (c) Reality Collective. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.﻿

using RealityCollective.ServiceFramework.Editor.Profiles;
using RealityCollective.ServiceFramework.Editor.PropertyDrawers;
using RealityToolkit.Definitions.Controllers;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace RealityToolkit.Editor.Profiles.Input.Controllers
{
    [CustomEditor(typeof(ControllerProfile))]
    public class BaseControllerMappingProfileInspector : BaseProfileInspector
    {
        private static readonly GUIContent EditButtonContent = new GUIContent("Edit Button Mappings");

        private SerializedProperty controllerType;
        private SerializedProperty handedness;
        private SerializedProperty controllerPrefab;
        private SerializedProperty controllerInteractors;
        private SerializedProperty useCustomInteractions;
        private SerializedProperty interactionMappingProfiles;

        private ControllerProfile controllerMappingProfile;

        private ReorderableList interactionsList;
        private ReorderableList interactorsList;
        private int currentlySelectedElement;
        private int selectedInteractorIndex;

        protected override void OnEnable()
        {
            base.OnEnable();

            controllerType = serializedObject.FindProperty(nameof(controllerType));
            handedness = serializedObject.FindProperty(nameof(handedness));
            controllerPrefab = serializedObject.FindProperty(nameof(controllerPrefab));
            controllerInteractors = serializedObject.FindProperty(nameof(controllerInteractors));
            useCustomInteractions = serializedObject.FindProperty(nameof(useCustomInteractions));
            interactionMappingProfiles = serializedObject.FindProperty(nameof(interactionMappingProfiles));

            controllerMappingProfile = target as ControllerProfile;

            var showButtons = useCustomInteractions.boolValue;

            interactionsList = new ReorderableList(serializedObject, interactionMappingProfiles, false, false, showButtons, showButtons)
            {
                elementHeight = EditorGUIUtility.singleLineHeight * 1.5f
            };
            interactionsList.drawElementCallback += DrawConfigurationOptionElement;
            interactionsList.onAddCallback += OnConfigurationOptionAdded;
            interactionsList.onRemoveCallback += OnConfigurationOptionRemoved;

            interactorsList = new ReorderableList(serializedObject, controllerInteractors, true, false, true, true)
            {
                elementHeight = EditorGUIUtility.singleLineHeight * 1.5f
            };
            interactorsList.drawElementCallback += DrawInteractorConfigurationOptionElement;
            interactorsList.onAddCallback += OnInteractorConfigurationOptionAdded;
            interactorsList.onRemoveCallback += OnInteractorConfigurationOptionRemoved;
        }

        public override void OnInspectorGUI()
        {
            RenderHeader("This profile defines the type of controller that is valid for this service module, which hand it belongs to, and how to visualize this controller in the scene, and binds each interactions on every physical control mechanism or sensor on the device.");

            serializedObject.Update();

            EditorGUILayout.PropertyField(controllerType);
            EditorGUILayout.PropertyField(handedness);
            EditorGUILayout.PropertyField(controllerPrefab);
            EditorGUILayout.Space();

            if (GUILayout.Button(EditButtonContent))
            {
                ControllerPopupWindow.Show(controllerMappingProfile, interactionMappingProfiles);
            }

            EditorGUILayout.Space();
            interactionsList.DoLayoutList();

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Interactors", EditorStyles.boldLabel);
            interactorsList.DoLayoutList();

            serializedObject.ApplyModifiedProperties();
        }

        private void DrawConfigurationOptionElement(Rect position, int index, bool isActive, bool isFocused)
        {
            if (isFocused)
            {
                currentlySelectedElement = index;
            }

            position.height = EditorGUIUtility.singleLineHeight;
            position.y += 3;
            position.xMin += 8;
            var mappingProfileProperty = interactionMappingProfiles.GetArrayElementAtIndex(index);
            ProfilePropertyDrawer.ProfileTypeOverride = typeof(InteractionMappingProfile);
            EditorGUI.PropertyField(position, mappingProfileProperty, GUIContent.none);
        }

        private void OnConfigurationOptionAdded(ReorderableList list)
        {
            interactionMappingProfiles.arraySize += 1;
            var index = interactionMappingProfiles.arraySize - 1;

            var mappingProfileProperty = interactionMappingProfiles.GetArrayElementAtIndex(index);
            mappingProfileProperty.objectReferenceValue = null;
            serializedObject.ApplyModifiedProperties();
        }

        private void OnConfigurationOptionRemoved(ReorderableList list)
        {
            if (currentlySelectedElement >= 0)
            {
                interactionMappingProfiles.DeleteArrayElementAtIndex(currentlySelectedElement);
            }

            serializedObject.ApplyModifiedProperties();
        }

        private void DrawInteractorConfigurationOptionElement(Rect rect, int index, bool isActive, bool isFocused)
        {
            if (isFocused)
            {
                selectedInteractorIndex = index;
            }

            rect.height = EditorGUIUtility.singleLineHeight;
            rect.y += 3;
            var mappingProfileProperty = controllerInteractors.GetArrayElementAtIndex(index);
            EditorGUI.PropertyField(rect, mappingProfileProperty, GUIContent.none);
        }

        private void OnInteractorConfigurationOptionAdded(ReorderableList list)
        {
            controllerInteractors.arraySize += 1;
            var index = controllerInteractors.arraySize - 1;

            var mappingProfileProperty = controllerInteractors.GetArrayElementAtIndex(index);
            mappingProfileProperty.objectReferenceValue = null;
        }

        private void OnInteractorConfigurationOptionRemoved(ReorderableList list)
        {
            if (selectedInteractorIndex >= 0)
            {
                controllerInteractors.DeleteArrayElementAtIndex(selectedInteractorIndex);
            }
        }
    }
}