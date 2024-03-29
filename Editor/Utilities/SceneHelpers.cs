﻿// Copyright (c) Reality Collective. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEditor;
using UnityEngine;

namespace RealityToolkit.Editor.Utilities
{
    public class SceneHelpers
    {
        /// <summary>
        /// Simple scene helper to create the beginnings of a scene, creating the scene root and a floor.
        /// </summary>
        [MenuItem(RealityToolkitPreferences.Editor_Menu_Keyword + "/Tools/Create Floor", false, 1)]
        public static void CreateFloor()
        {
            //check if there is already a Scene Objects GO
            var sceneRoot = GameObject.Find("Scene Objects");
            if(!sceneRoot)
            {
                sceneRoot = new GameObject("Scene Objects");
                var floor = GameObject.CreatePrimitive(PrimitiveType.Plane);
                floor.name = "Ground";
                floor.transform.SetParent(sceneRoot.transform);
            }
        }
    }
}
