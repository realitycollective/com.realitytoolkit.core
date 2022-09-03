// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information. 

using RealityCollective.Editor.Utilities;
using RealityCollective.ServiceFramework.Services;
using RealityToolkit.Editor.Utilities;
using RealityToolkit.InputSystem.Interfaces;
using RealityToolkit.SpatialAwarenessSystem.Definitions;
using RealityToolkit.SpatialAwarenessSystem.Interfaces;
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
            if (ServiceManager.Instance != null && ServiceManager.Instance.HasActiveProfile)
            {
                if (ServiceManager.Instance.IsServiceEnabled<IMixedRealityInputSystem>() &&
                    InputMappingAxisUtility.CheckUnityInputManagerMappings(ControllerMappingUtilities.UnityInputManagerAxes))
                {
                    Debug.Log($"{nameof(IMixedRealityInputSystem)} was enabled, updated input axis mappings.");
                }
                else if (!ServiceManager.Instance.IsServiceEnabled<IMixedRealityInputSystem>() &&
                         InputMappingAxisUtility.RemoveMappings(ControllerMappingUtilities.UnityInputManagerAxes))
                {
                    Debug.Log($"{nameof(IMixedRealityInputSystem)} was disabled, removed input axis mappings.");
                }

                if (ServiceManager.Instance.IsServiceEnabled<IMixedRealitySpatialAwarenessSystem>() &&
                    LayerUtilities.CheckLayers(MixedRealitySpatialAwarenessSystemProfile.SpatialAwarenessLayers))
                {
                    Debug.Log($"{nameof(IMixedRealitySpatialAwarenessSystem)} was enabled, spatial mapping layers added to project.");
                }
                else if (!ServiceManager.Instance.IsServiceEnabled<IMixedRealitySpatialAwarenessSystem>() &&
                         LayerUtilities.RemoveLayers(MixedRealitySpatialAwarenessSystemProfile.SpatialAwarenessLayers))
                {
                    Debug.Log($"{nameof(IMixedRealitySpatialAwarenessSystem)} was disabled, spatial mapping layers removed to project.");
                }
            }
        }
    }
}
