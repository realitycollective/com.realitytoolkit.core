﻿// Copyright (c) Reality Collective. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using RealityToolkit.Input.Controllers;
using RealityToolkit.Input.Interfaces.Handlers;

namespace RealityToolkit.Utilities.UX.Controllers
{
    /// <summary>
    /// The Mixed Reality Visualization component is primarily responsible for synchronizing the user's current input with controller models.
    /// </summary>
    /// <seealso cref="ControllerVisualizationProfile"/>
    [System.Runtime.InteropServices.Guid("EA41F336-8B46-4AEA-A8B4-8B93B52E67A9")]
    public class DefaultControllerVisualizer : BaseControllerVisualizer, IControllerVisualizer { }
}