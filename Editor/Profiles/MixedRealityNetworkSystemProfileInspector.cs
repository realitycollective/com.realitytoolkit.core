// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using RealityToolkit.Definitions.NetworkingSystem;
using UnityEditor;

namespace RealityToolkit.Editor.Profiles
{
    [CustomEditor(typeof(MixedRealityNetworkSystemProfile))]
    public class MixedRealityNetworkSystemProfileInspector : MixedRealityServiceProfileInspector
    {
        public override void OnInspectorGUI()
        {
            RenderHeader("The Network System Profile helps developers configure networking messages no matter what platform you're building for.");

            base.OnInspectorGUI();
        }
    }
}