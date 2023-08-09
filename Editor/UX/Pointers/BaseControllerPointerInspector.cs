// Copyright (c) Reality Collective. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.﻿

using RealityCollective.Definitions.Utilities;
using RealityCollective.Editor.Extensions;
using RealityToolkit.Input.Interactions.Interactors;
using UnityEditor;
using UnityEngine;

namespace RealityToolkit.Editor.UX.Pointers
{
    [CustomEditor(typeof(BaseControllerInteractor), true, isFallback = true)]
    public class BaseControllerPointerInspector : UnityEditor.Editor
    {
        private readonly GUIContent basePointerFoldoutHeader = new GUIContent("Base Pointer Settings");
        private static readonly GUIContent SynchronizationSettings = new GUIContent("Synchronization Settings");
        private static readonly string[] HandednessLabels = { "Left", "Right" };

        private SerializedProperty cursorPrefab;
        private SerializedProperty disableCursorOnStart;
        private SerializedProperty uiLayerMask;
        private SerializedProperty setCursorVisibilityOnSourceDetected;
        private SerializedProperty raycastOrigin;
        private SerializedProperty defaultPointerExtent;
        private SerializedProperty activeHoldAction;
        private SerializedProperty pointerAction;
        private SerializedProperty grabAction;
        private SerializedProperty pointerOrientation;
        private SerializedProperty requiresHoldAction;
        private SerializedProperty enablePointerOnStart;
        private SerializedProperty useSourcePoseData;
        private SerializedProperty poseAction;
        private SerializedProperty handedness;
        private SerializedProperty destroyOnSourceLost;

        protected bool DrawBasePointerActions = true;
        protected bool DrawHandednessProperty = true;

        protected virtual void OnEnable()
        {
            useSourcePoseData = serializedObject.FindProperty(nameof(useSourcePoseData));
            poseAction = serializedObject.FindProperty(nameof(poseAction));
            handedness = serializedObject.FindProperty(nameof(handedness));
            destroyOnSourceLost = serializedObject.FindProperty(nameof(destroyOnSourceLost));
            cursorPrefab = serializedObject.FindProperty(nameof(cursorPrefab));
            disableCursorOnStart = serializedObject.FindProperty(nameof(disableCursorOnStart));
            uiLayerMask = serializedObject.FindProperty(nameof(uiLayerMask));
            setCursorVisibilityOnSourceDetected = serializedObject.FindProperty(nameof(setCursorVisibilityOnSourceDetected));
            raycastOrigin = serializedObject.FindProperty(nameof(raycastOrigin));
            defaultPointerExtent = serializedObject.FindProperty(nameof(defaultPointerExtent));
            activeHoldAction = serializedObject.FindProperty(nameof(activeHoldAction));
            pointerAction = serializedObject.FindProperty(nameof(pointerAction));
            grabAction = serializedObject.FindProperty(nameof(grabAction));
            pointerOrientation = serializedObject.FindProperty(nameof(pointerOrientation));
            requiresHoldAction = serializedObject.FindProperty(nameof(requiresHoldAction));
            enablePointerOnStart = serializedObject.FindProperty(nameof(enablePointerOnStart));

            DrawHandednessProperty = false;
        }

        /// <inheritdoc />
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            serializedObject.Update();

            EditorGUILayout.Space();
            EditorGUI.BeginChangeCheck();

            if (useSourcePoseData.FoldoutWithBoldLabelPropertyField(SynchronizationSettings))
            {
                EditorGUI.indentLevel++;

                if (!useSourcePoseData.boolValue)
                {
                    EditorGUILayout.PropertyField(poseAction);
                }

                if (DrawHandednessProperty)
                {
                    var currentHandedness = (Handedness)handedness.enumValueIndex;
                    var handIndex = currentHandedness == Handedness.Right ? 1 : 0;

                    EditorGUI.BeginChangeCheck();
                    var newHandednessIndex = EditorGUILayout.Popup(handedness.displayName, handIndex, HandednessLabels);

                    if (EditorGUI.EndChangeCheck())
                    {
                        currentHandedness = newHandednessIndex == 0 ? Handedness.Left : Handedness.Right;
                        handedness.enumValueIndex = (int)currentHandedness;
                    }
                }

                EditorGUILayout.PropertyField(destroyOnSourceLost);

                EditorGUI.indentLevel--;
            }

            if (cursorPrefab.FoldoutWithBoldLabelPropertyField(basePointerFoldoutHeader))
            {
                EditorGUI.indentLevel++;

                EditorGUILayout.PropertyField(disableCursorOnStart);
                EditorGUILayout.PropertyField(uiLayerMask);
                EditorGUILayout.PropertyField(setCursorVisibilityOnSourceDetected);
                EditorGUILayout.PropertyField(enablePointerOnStart);
                EditorGUILayout.PropertyField(raycastOrigin);
                EditorGUILayout.PropertyField(defaultPointerExtent);
                EditorGUILayout.PropertyField(pointerOrientation);
                EditorGUILayout.PropertyField(pointerAction);
                EditorGUILayout.PropertyField(grabAction);

                if (DrawBasePointerActions)
                {
                    EditorGUILayout.PropertyField(requiresHoldAction);

                    if (requiresHoldAction.boolValue)
                    {
                        EditorGUILayout.PropertyField(activeHoldAction);
                    }
                }

                EditorGUI.indentLevel--;
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}
