// Copyright (c) Reality Collective. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;

namespace RealityToolkit.Input.Interactors
{
    /// <summary>
    /// Defining a <see cref="IDirectInteractor"/> result.
    /// </summary>
    public interface IDirectInteractorResult
    {
        /// <summary>
        /// The <see cref="IDirectInteractor"/>'s previous target, if any.
        /// </summary>
        GameObject PreviousTarget { get; }

        /// <summary>
        /// The <see cref="IDirectInteractor"/>'s current target, if any.
        /// </summary>
        GameObject CurrentTarget { get; }

        /// <summary>
        /// The distance at which the interaction took place from the <see cref="IDirectInteractor"/>'s position.
        /// </summary>
        float Distance { get; }

        /// <summary>
        /// The direction at which the interaction took place from the <see cref="IDirectInteractor"/>'s position.
        /// </summary>
        Vector3 Direction { get; }
    }
}
