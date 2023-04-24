// Copyright (c) Reality Collective. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information. 

using RealityCollective.Editor.Utilities;
using RealityCollective.ServiceFramework.Services;
using RealityToolkit.Editor.Utilities;
using RealityToolkit.Input.Interfaces;
using RealityToolkit.SpatialAwareness.Definitions;
using RealityToolkit.SpatialAwareness.Interfaces;
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
                if (ServiceManager.Instance.IsServiceEnabled<IInputService>() &&
                    InputMappingAxisUtility.CheckUnityInputManagerMappings(ControllerMappingUtilities.UnityInputManagerAxes))
                {
                    Debug.Log($"{nameof(IInputService)} was enabled, updated input axis mappings.");
                }
                else if (!ServiceManager.Instance.IsServiceEnabled<IInputService>() &&
                         InputMappingAxisUtility.RemoveMappings(ControllerMappingUtilities.UnityInputManagerAxes))
                {
                    Debug.Log($"{nameof(IInputService)} was disabled, removed input axis mappings.");
                }

                if (ServiceManager.Instance.IsServiceEnabled<ISpatialAwarenessService>() &&
                    LayerUtilities.CheckLayers(SpatialAwarenessSystemProfile.SpatialAwarenessLayers))
                {
                    Debug.Log($"{nameof(ISpatialAwarenessService)} was enabled, spatial mapping layers added to project.");
                }
                else if (!ServiceManager.Instance.IsServiceEnabled<ISpatialAwarenessService>() &&
                         LayerUtilities.RemoveLayers(SpatialAwarenessSystemProfile.SpatialAwarenessLayers))
                {
                    Debug.Log($"{nameof(ISpatialAwarenessService)} was disabled, spatial mapping layers removed to project.");
                }
            }
        }
    }
}
