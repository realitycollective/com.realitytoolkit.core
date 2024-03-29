﻿// Copyright (c) Reality Collective. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using RealityToolkit.Input.Attributes;
using RealityToolkit.Input.Definitions;
using UnityEditor;
using UnityEngine;

namespace RealityToolkit.Editor.PropertyDrawers
{
    [CustomPropertyDrawer(typeof(AxisConstraintAttribute))]
    public class AxisConstraintAttributeDrawer : PropertyDrawer
    {
        private readonly InputActionDropdown inputActionDropdown = new InputActionDropdown();

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var constraintAttribute = attribute as AxisConstraintAttribute;

            if (property.type == nameof(InputAction))
            {
                inputActionDropdown.OnGui(position, property, label, constraintAttribute.AxisConstraint);
            }
            else
            {
                var color = GUI.color;
                GUI.color = Color.red;
                EditorGUI.LabelField(position, $"{nameof(AxisConstraintAttribute)} is not supported for {property.type}");
                GUI.color = color;
            }
        }
    }
}