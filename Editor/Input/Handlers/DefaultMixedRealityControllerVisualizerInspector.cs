// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEditor;
using XRTK.Utilities.UX.Controllers;

namespace XRTK.Editor.Input.Handlers
{
    [CustomEditor(typeof(DefaultMixedRealityControllerVisualizer))]
    public class DefaultMixedRealityControllerVisualizerInspector : ControllerPoseSynchronizerInspector { }
}