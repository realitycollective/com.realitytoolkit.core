﻿// Copyright (c) Reality Collective. All rights reserved.
// Licensed under the MIT License.

using RealityToolkit.Utilities.Rendering;
using UnityEditor;
using UnityEngine;

namespace RealityToolkit.Editor.Rendering
{
    [CustomEditor(typeof(ClippingPlane))]
    public class ClippingPlaneEditor : UnityEditor.Editor
    {
        private bool HasFrameBounds() { return true; }

        private Bounds OnGetFrameBounds()
        {
            var primitive = target as ClippingPlane;
            Debug.Assert(primitive != null);
            return new Bounds(primitive.transform.position, Vector3.one);
        }
    }
}