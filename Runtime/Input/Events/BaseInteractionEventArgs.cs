// Copyright (c) Reality Collective. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using RealityToolkit.Input.Interactables;
using RealityToolkit.Input.Interactors;

namespace RealityToolkit.Input.Events
{
    /// <summary>
    /// Event data associated with an interaction event between an <see cref="IInteractor"/> and <see cref="IInteractable"/>.
    /// </summary>
    public abstract class BaseInteractionEventArgs
    {
        /// <summary>
        /// The Interactor associated with the interaction event.
        /// </summary>
        public IInteractor Interactor { get; set; }

        /// <summary>
        /// The Interactable associated with the interaction event.
        /// </summary>
        public IInteractable Interactable { get; set; }
    }
}
