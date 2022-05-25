// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information. 

using RealityCollective.Editor.Utilities;
using RealityToolkit.Definitions.SpatialAwarenessSystem;
using RealityToolkit.Editor.Utilities;
using RealityToolkit.Interfaces.InputSystem;
using RealityToolkit.Interfaces.SpatialAwarenessSystem;
using RealityToolkit.Services;
using UnityEditor;
using UnityEngine;

namespace RealityToolkit.Editor
{
    [InitializeOnLoad]
    public static class EditorActiveProfileChangeHandler
    {
        static EditorActiveProfileChangeHandler()
        {
            EditorApplication.hierarchyChanged += EditorApplication_hierarchyChanged;
        }

        private static void EditorApplication_hierarchyChanged()
        {
            if (MixedRealityToolkit.HasActiveProfile)
            {
                if (MixedRealityToolkit.IsSystemEnabled<IMixedRealityInputSystem>() &&
                    InputMappingAxisUtility.CheckUnityInputManagerMappings(ControllerMappingUtilities.UnityInputManagerAxes))
                {
                    Debug.Log($"{nameof(IMixedRealityInputSystem)} was enabled, updated input axis mappings.");
                }
                else if (!MixedRealityToolkit.IsSystemEnabled<IMixedRealityInputSystem>() &&
                         InputMappingAxisUtility.RemoveMappings(ControllerMappingUtilities.UnityInputManagerAxes))
                {
                    Debug.Log($"{nameof(IMixedRealityInputSystem)} was disabled, removed input axis mappings.");
                }

                if (MixedRealityToolkit.IsSystemEnabled<IMixedRealitySpatialAwarenessSystem>() &&
                    LayerUtilities.CheckLayers(MixedRealitySpatialAwarenessSystemProfile.SpatialAwarenessLayers))
                {
                    Debug.Log($"{nameof(IMixedRealitySpatialAwarenessSystem)} was enabled, spatial mapping layers added to project.");
                }
                else if (!MixedRealityToolkit.IsSystemEnabled<IMixedRealitySpatialAwarenessSystem>() &&
                         LayerUtilities.RemoveLayers(MixedRealitySpatialAwarenessSystemProfile.SpatialAwarenessLayers))
                {
                    Debug.Log($"{nameof(IMixedRealitySpatialAwarenessSystem)} was disabled, spatial mapping layers removed to project.");
                }
            }
        }
    }
}
