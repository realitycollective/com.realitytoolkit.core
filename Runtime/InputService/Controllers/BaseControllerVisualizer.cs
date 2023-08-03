// Copyright (c) Reality Collective. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using RealityToolkit.Input.Interfaces.Handlers;
using UnityEngine;

namespace RealityToolkit.Input.Controllers
{
    /// <summary>
    /// Abstract base implementation for <see cref="IControllerVisualizer"/>s.
    /// </summary>
    public abstract class BaseControllerVisualizer : ControllerPoseSynchronizer, IControllerVisualizer
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
