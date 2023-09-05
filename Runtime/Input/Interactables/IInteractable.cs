// Copyright (c) Reality Collective. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using RealityToolkit.Input.InteractionActions;
using RealityToolkit.Input.Interactors;

namespace RealityToolkit.Input.Interactables
{
    /// <summary>
    /// An <see cref="IInteractable"/> marks an object that can be interacted with by <see cref="IInteractor"/>s.
    /// </summary>
    public interface IInteractable
    {
        /// <summary>
        /// Gets or sets the <see cref="IInteractable"/>s label that may be used to
        /// identify the interactable or categorize it
        /// </summary>
        string Label { get; set; }

        /// <summary>
        /// Is the <see cref="IInteractable"/> valid for interaciton?
        /// </summary>
        bool IsValid { get; }

        /// <summary>
        /// Is the <see cref="IInteractable"/> currently considered activated?
        /// </summary>
        bool IsActivated { get; }

        /// <summary>
        /// Is the <see cref="IInteractable"/> currently focused by an <see cref="IInteractor"/>?
        /// </summary>
        bool IsFocused { get; }

        /// <summary>
        /// Is the <see cref="IInteractable"/> currently selected by an <see cref="IInteractor"/>?
        /// </summary>
        bool IsSelected { get; }

        /// <summary>
        /// Is the <see cref="IInteractable"/> currently grabbed by an <see cref="IInteractor"/>?
        /// </summary>
        bool IsGrabbed { get; }

        /// <summary>
        /// The <see cref="IInteractable"/>'s focus mode.
        /// </summary>
        InteractableFocusMode FocusMode { get; }

        /// <summary>
        /// The <see cref="IInteractable"/>'s activation mode determines how <see cref="IInteractionAction.OnActivated(Events.InteractionEventArgs)"/>
        /// and <see cref="IInteractionAction.OnDeactivated(Events.InteractionExitEventArgs)"/> are raised.
        /// </summary>
        InteractableActivationMode ActivationMode { get; }

        /// <summary>
        /// Does the <see cref="IInteractable"/> allow direct interaction?
        /// </summary>
        bool DirectInteractionEnabled { get; }

        /// <summary>
        /// Does the <see cref="IInteractable"/> allow interaction from a distance?
        /// </summary>
        bool FarInteractionEnabled { get; }

        /// <summary>
        /// Adds the <paramref name="action"/> to the <see cref="IInteractable"/>.
        /// </summary>
        /// <param name="action">The <see cref="IInteractionAction"/>.</param>
        void Add(IInteractionAction action);

        /// <summary>
        /// Removes the <paramref name="action"/> from the <see cref="IInteractable"/>.
        /// </summary>
        /// <param name="action">The <see cref="IInteractionAction"/>.</param>
        void Remove(IInteractionAction action);
    }
}
