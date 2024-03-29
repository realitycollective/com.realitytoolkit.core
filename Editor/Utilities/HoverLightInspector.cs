﻿// Copyright (c) Reality Collective. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.﻿

using RealityToolkit.Utilities;
using UnityEditor;
using UnityEngine;

namespace RealityToolkit.Editor.Utilities
{
    [CustomEditor(typeof(HoverLight))]
    public class HoverLightInspector : UnityEditor.Editor
    {
        private bool HasFrameBounds() { return true; }

        private Bounds OnGetFrameBounds()
        {
            var light = target as HoverLight;
            Debug.Assert(light != null);
            return new Bounds(light.transform.position, Vector3.one * light.Radius);
        }
    }
}
