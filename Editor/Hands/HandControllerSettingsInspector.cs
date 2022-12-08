// Copyright (c) Reality Collective. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using RealityCollective.ServiceFramework.Editor.Profiles;
using RealityToolkit.InputSystem.Hands;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace RealityToolkit.Editor.Hands
{
    [CustomEditor(typeof(HandControllerSettings))]
    public class HandControllerSettingsInspector : BaseProfileInspector
    {
        private SerializedProperty gripThreshold;
        private SerializedProperty renderingMode;
        private SerializedProperty handPhysicsEnabled;
        private SerializedProperty useTriggers;
        private SerializedProperty boundsMode;
        private SerializedProperty trackedPoses;

        private ReorderableList poseProfilesList;
        private int currentlySelectedPoseElement;

        protected override void OnEnable()
        {
            base.OnEnable();

            gripThreshold = serializedObject.FindProperty(nameof(gripThreshold));
            renderingMode = serializedObject.FindProperty(nameof(renderingMode));
            handPhysicsEnabled = serializedObject.FindProperty(nameof(handPhysicsEnabled));
            useTriggers = serializedObject.FindProperty(nameof(useTriggers));
            boundsMode = serializedObject.FindProperty(nameof(boundsMode));
            trackedPoses = serializedObject.FindProperty(nameof(trackedPoses));

            poseProfilesList = new ReorderableList(serializedObject, trackedPoses, true, false, true, true)
            {
                elementHeight = EditorGUIUtility.singleLineHeight * 1.5f
            };
            poseProfilesList.drawHeaderCallback += PoseProfilesList_DrawHeaderCallback;
            poseProfilesList.drawElementCallback += PoseProfilesList_DrawConfigurationOptionElement;
            poseProfilesList.onAddCallback += PoseProfilesList_OnConfigurationOptionAdded;
            poseProfilesList.onRemoveCallback += PoseProfilesList_OnConfigurationOptionRemoved;
        }

        public override void OnInspectorGUI()
        {
            RenderHeader("This profile defines a hand pose that can be recognized at runtime and trigger input events.");

            serializedObject.Update();

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

            serializedObject.ApplyModifiedProperties();
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