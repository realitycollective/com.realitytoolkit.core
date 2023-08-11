// Copyright (c) Reality Collective. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using RealityToolkit.Input.Definitions;
using RealityToolkit.Input.Interactables;

namespace RealityToolkit.Input.InteractionActions
{
    /// <summary>
    /// A <see cref="IInteractionAction"/> is an action performed when the <see cref="IInteractable"/> it is attached to is changing state.
    /// </summary>
    public interface IInteractionAction
    {
        /// <summary>
        /// When comparing <see cref="IInteractionAction"/>s on the same <see cref="IInteractable"/>,
        /// the one with a higher <see cref="SortingOrder"/> will always be executed after the one with a lower <see cref="SortingOrder"/>.
        /// </summary>
        /// <remarks>Internally the value is stored as a signed 16 bit integer (short) and so is limited to the range -32,768 to 32,767.</remarks>
        short SortingOrder { get; }

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
