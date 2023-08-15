// Copyright (c) Reality Collective. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace RealityToolkit.Input.Interactables
{
    /// <summary>
    /// Options for how an <see cref="IInteractable"/> may be activated by <see cref="Interactors.IInteractor"/>s.
    /// </summary>
    public enum InteractableActivationMode
    {
        /// <summary>
        /// <see cref="IInteractable"/> cannot be activated at all.
        /// </summary>
        None = 0,
        /// <summary>
        /// The <see cref="IInteractable"/> has only one activation state.
        /// Only <see cref="InteractionActions.IInteractionAction.OnActivated(Events.InteractionEventArgs)"/>
        /// is raised once when the <see cref="IInteractable"/> is activated. The <see cref="IInteractable"/>'s
        /// <see cref="IInteractable.IsActivated"/> state returns back to <c>false</c> right after that.
        /// </summary>
        Button,
        /// <summary>
        /// The <see cref="IInteractable"/> has two activation states, activated and deactivated.
        /// <see cref="InteractionActions.IInteractionAction.OnActivated(Events.InteractionEventArgs)"/>
        /// and <see cref="InteractionActions.IInteractionAction.OnDeactivated(Events.InteractionExitEventArgs)"/>
        /// are raised accordingly.
        /// </summary>
        Toggle,
    }
}
