// Copyright (c) Reality Collective. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using RealityToolkit.Definitions.Controllers;
using RealityToolkit.Input.Interfaces.Handlers;
using RealityToolkit.Services.Input.Utilities;
using System;
using UnityEngine;

namespace RealityToolkit.Utilities.UX.Controllers
{
    /// <summary>
    /// The Mixed Reality Visualization component is primarily responsible for synchronizing the user's current input with controller models.
    /// </summary>
    /// <seealso cref="MixedRealityControllerVisualizationProfile"/>
    [System.Runtime.InteropServices.Guid("EA41F336-8B46-4AEA-A8B4-8B93B52E67A9")]
    public class DefaultMixedRealityControllerVisualizer : ControllerPoseSynchronizer, IControllerVisualizer
    {
        /// <inheritdoc />
        public GameObject GameObject
        {
            get
            {
                try
                {
                    return gameObject;
                }
                catch (Exception)
                {
                    return null;
                }
            }
        }
    }
}