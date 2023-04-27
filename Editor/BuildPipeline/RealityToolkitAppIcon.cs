// Copyright (c) Reality Collective. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using UnityEngine;

namespace RealityToolkit.Editor.BuildPipeline
{
    /// <summary>
    /// Build profile for saving 3d app icon's path in the build settings.
    /// </summary>
    [Serializable]
    public class RealityToolkitAppIcon
    {
        [SerializeField]
        private string appIconPath = "";

        public string RealityToolkitAppIconPath
        {
            get => appIconPath;
            set => appIconPath = value;
        }
    }
}