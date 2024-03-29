﻿// Copyright (c) Reality Collective. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using RealityToolkit.Input.Definitions;
using UnityEditor;
using UnityEngine;

namespace RealityToolkit.Editor.PropertyDrawers
{
    [CustomPropertyDrawer(typeof(InputAction))]
    public class InputActionPropertyDrawer : PropertyDrawer
    {
        private readonly InputActionDropdown inputActionDropdown = new InputActionDropdown();

        public override void OnGUI(Rect rect, SerializedProperty property, GUIContent content)
        {
            inputActionDropdown.OnGui(rect, property, content);
        }
    }
}
