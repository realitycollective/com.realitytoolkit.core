// Copyright (c) Reality Collective. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using RealityToolkit.Input.Interfaces;

namespace RealityToolkit.Input.Interactions.Interactors
{
    /// <summary>
    /// An <see cref="IInteractor"/> marks an object that can interact with <see cref="Interactables.IInteractable"/>s.
    /// </summary>
    public interface IInteractor
    {
        /// <summary>
        /// The registered <see cref="IInputSource"/> for this <see cref="IInteractor"/>.
        /// </summary>
        IInputSource InputSource { get; }
    }
}
