﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.﻿

using RealityCollective.Editor.Extensions;
using RealityCollective.Extensions;
using RealityToolkit.Editor.Input.Handlers;
using RealityToolkit.Utilities.UX.Pointers;
using UnityEditor;
using UnityEngine;

namespace RealityToolkit.Editor.UX.Pointers
{
    [CustomEditor(typeof(BaseControllerPointer), true, isFallback = true)]
    public class BaseControllerPointerInspector : ControllerPoseSynchronizerInspector
    {
        private readonly GUIContent basePointerFoldoutHeader = new GUIContent("Base Pointer Settings");

        private SerializedProperty cursorPrefab;
        private SerializedProperty disableCursorOnStart;
        private SerializedProperty setCursorVisibilityOnSourceDetected;
        private SerializedProperty raycastOrigin;
        private SerializedProperty defaultPointerExtent;
        private SerializedProperty activeHoldAction;
        private SerializedProperty pointerAction;
        private SerializedProperty pointerOrientation;
        private SerializedProperty requiresHoldAction;
        private SerializedProperty enablePointerOnStart;
        private SerializedProperty interactionMode;
        private SerializedProperty nearInteractionCollider;

        protected bool DrawBasePointerActions = true;

        protected override void OnEnable()
        {
            base.OnEnable();

            cursorPrefab = serializedObject.FindProperty(nameof(cursorPrefab));
            disableCursorOnStart = serializedObject.FindProperty(nameof(disableCursorOnStart));
            setCursorVisibilityOnSourceDetected = serializedObject.FindProperty(nameof(setCursorVisibilityOnSourceDetected));
            raycastOrigin = serializedObject.FindProperty(nameof(raycastOrigin));
            defaultPointerExtent = serializedObject.FindProperty(nameof(defaultPointerExtent));
            activeHoldAction = serializedObject.FindProperty(nameof(activeHoldAction));
            pointerAction = serializedObject.FindProperty(nameof(pointerAction));
            pointerOrientation = serializedObject.FindProperty(nameof(pointerOrientation));
            requiresHoldAction = serializedObject.FindProperty(nameof(requiresHoldAction));
            enablePointerOnStart = serializedObject.FindProperty(nameof(enablePointerOnStart));
            interactionMode = serializedObject.FindProperty(nameof(interactionMode));
            nearInteractionCollider = serializedObject.FindProperty(nameof(nearInteractionCollider));

            DrawHandednessProperty = false;
        }

        /// <inheritdoc />
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            serializedObject.Update();

            if (cursorPrefab.FoldoutWithBoldLabelPropertyField(basePointerFoldoutHeader))
            {
                EditorGUI.indentLevel++;

                EditorGUILayout.PropertyField(disableCursorOnStart);
                EditorGUILayout.PropertyField(setCursorVisibilityOnSourceDetected);
                EditorGUILayout.PropertyField(enablePointerOnStart);
                EditorGUILayout.PropertyField(interactionMode);

                var interactionModeValue = (Definitions.InteractionMode)interactionMode.intValue;

                if (interactionModeValue.HasFlags(Definitions.InteractionMode.Near))
                {
                    EditorGUILayout.PropertyField(nearInteractionCollider);
                }

                if (interactionModeValue.HasFlags(Definitions.InteractionMode.Far))
                {
                    EditorGUILayout.PropertyField(raycastOrigin);
                    EditorGUILayout.PropertyField(defaultPointerExtent);
                }

                EditorGUILayout.PropertyField(pointerOrientation);
                EditorGUILayout.PropertyField(pointerAction);

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
