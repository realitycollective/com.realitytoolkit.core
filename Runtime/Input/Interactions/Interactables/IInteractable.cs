// Copyright (c) Reality Collective. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using RealityToolkit.Input.Interactions.Actions;
using RealityToolkit.Input.Interactions.Interactors;
using System.Collections.Generic;

namespace RealityToolkit.Input.Interactions.Interactables
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
        /// Does the <see cref="IInteractable"/> allow near interaction?
        /// </summary>
        bool NearInteractionEnabled { get; }

        /// <summary>
        /// Does the <see cref="IInteractable"/> allow interaction from a distance?
        /// </summary>
        bool FarInteractionEnabled { get; }

        /// <summary>
        /// The current <see cref="InteractionState"/> of the <see cref="IInteractable"/>.
        /// </summary>
        InteractionState State { get; }

        /// <summary>
        /// Gets the primary <see cref="IInteractor"/>. That is the first one
        /// to start interaction with the <see cref="IInteractable"/>.
        /// </summary>
        /// <remarks><c>null</c> if not interacted with.</remarks>
        IInteractor PrimaryInteractor { get; }

        /// <summary>
        /// Gets all <see cref="IInteractor"/>s currently interacting with the <see cref="IInteractable"/>.
        /// </summary>
        /// <remarks>Empty if not interacted with.</remarks>
        IReadOnlyList<IInteractor> Interactors { get; }

        /// <summary>
        /// The <see cref="IInteractable"/> is now selected by <paramref name="interactor"/>.
        /// </summary>
        void OnSelected(IInteractor interactor);

        /// <summary>
        /// The <see cref="IInteractable"/> is no longer selected by <paramref name="interactor"/>.
        /// </summary>
        void OnDeselected(IInteractor interactor);

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
