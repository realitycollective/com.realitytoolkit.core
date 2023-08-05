// Copyright (c) Reality Collective. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using RealityToolkit.Input.Interactions.Interactables;

namespace RealityToolkit.Input.Interactions.Actions
{
    /// <summary>
    /// A <see cref="IInteractionAction"/> is an action performed when the <see cref="IInteractable"/> it is attached to is changing state.
    /// </summary>
    public interface IInteractionAction
    {
        /// <summary>
        /// The <see cref="IInteractable"/> the <see cref="IInteractionAction"/> is attached to.
        /// </summary>
        IInteractable Interactable { get; }

        /// <summary>
        /// Handles a change in the <see cref="IInteractable.State"/>.
        /// </summary>
        /// <param name="state">The new <see cref="InteractionState"/> of the <see cref="IInteractable"/>.</param>
        void OnStateChanged(InteractionState state);
    }
}
