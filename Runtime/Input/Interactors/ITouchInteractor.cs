// Copyright (c) Reality Collective. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;

namespace RealityToolkit.Input.Interactors
{
    /// <summary>
    /// Interface for touch <see cref="IInteractor"/>s.
    /// </summary>
    public interface ITouchInteractor : IInteractor
    {
        /// <summary>
        /// Current finger id of the touch.
        /// </summary>
        int FingerId { get; set; }

        /// <summary>
        /// Current touch ray.
        /// </summary>
        Ray TouchRay { get; set; }
    }
}