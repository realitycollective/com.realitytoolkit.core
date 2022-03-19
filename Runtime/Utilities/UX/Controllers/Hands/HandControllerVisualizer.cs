// Copyright (c) Reality Collective. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;
using XRTK.Interfaces.InputSystem.Handlers;
using XRTK.Services.InputSystem.Utilities;

namespace RealityToolkit.Utilities.UX.Controllers.Hands
{
    /// <summary>
    /// Default visualizer for <see cref="Interfaces.InputSystem.Controllers.Hands.IHandController"/>s.
    /// </summary>
    [System.Runtime.InteropServices.Guid("f6654aca-e4c2-4653-8033-1465fe9f2fd1")]
    public class HandControllerVisualizer : ControllerPoseSynchronizer, IMixedRealityControllerVisualizer
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
                catch
                {
                    return null;
                }
            }
        }
    }
}
