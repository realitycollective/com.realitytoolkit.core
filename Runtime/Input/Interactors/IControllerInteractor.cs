// Copyright (c) Reality Collective. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using RealityToolkit.Input.Interfaces.Controllers;
using UnityEngine;

namespace RealityToolkit.Input.Interactors
{
    /// <summary>
    /// An <see cref="IInteractor"/> that is driven by a <see cref="IController"/>.
    /// </summary>
    public interface IControllerInteractor : IInteractor
    {
        /// <summary>
        /// The <see cref="UnityEngine.GameObject"/> reference for this <see cref="IControllerInteractor"/>.
        /// </summary>
        GameObject GameObject { get; }

        /// <summary>
        /// The <see cref="IController"/> driving the <see cref="IInteractor"/>.
        /// </summary>
        IController Controller { get; set; }
    }
}
