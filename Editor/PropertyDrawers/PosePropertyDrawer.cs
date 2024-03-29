﻿// Copyright (c) Reality Collective. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEditor;
using UnityEngine;

namespace RealityToolkit.Editor.PropertyDrawers
{
    [CustomPropertyDrawer(typeof(Pose))]
    public class PosePropertyDrawer : PropertyDrawer
    {
        private readonly GUIContent positionContent = new GUIContent("Position");
        private readonly GUIContent rotationContent = new GUIContent("Rotation");
        private const int NumberOfLines = 3;

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUIUtility.singleLineHeight * NumberOfLines;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            bool lastMode = EditorGUIUtility.wideMode;
            EditorGUIUtility.wideMode = true;
            EditorGUI.BeginProperty(position, label, property);
            EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);
            EditorGUI.indentLevel++;

            var fieldHeight = position.height / NumberOfLines;
            var positionRect = new Rect(position.x, position.y + fieldHeight, position.width, fieldHeight);
            var rotationRect = new Rect(position.x, position.y + fieldHeight * 2, position.width, fieldHeight);

            EditorGUI.PropertyField(positionRect, property.FindPropertyRelative("position"), positionContent);

            EditorGUI.BeginChangeCheck();
            var rotationProperty = property.FindPropertyRelative("rotation");
            var newEulerRotation = EditorGUI.Vector3Field(rotationRect, rotationContent, rotationProperty.quaternionValue.eulerAngles);

            if (EditorGUI.EndChangeCheck())
            {
                rotationProperty.quaternionValue = Quaternion.Euler(newEulerRotation);
            }

            EditorGUI.indentLevel--;
            EditorGUIUtility.wideMode = lastMode;
            EditorGUI.EndProperty();
        }
    }
}