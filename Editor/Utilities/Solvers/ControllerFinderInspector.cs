﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEditor;
using UnityEngine;
using XRTK.Utilities.Solvers;

namespace XRTK.Editor.Utilities.Solvers
{
    [CustomEditor(typeof(ControllerFinder))]
    public abstract class ControllerFinderInspector : UnityEditor.Editor
    {
        private SerializedProperty handednessProperty;

        protected virtual void OnEnable()
        {
            handednessProperty = serializedObject.FindProperty("handedness");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Controller Options", new GUIStyle("Label") { fontStyle = FontStyle.Bold });
            EditorGUILayout.Space();
            EditorGUI.indentLevel++;

            EditorGUILayout.PropertyField(handednessProperty);

            EditorGUI.indentLevel--;
            serializedObject.ApplyModifiedProperties();
        }
    }
}
