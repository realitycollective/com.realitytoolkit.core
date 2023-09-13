// Copyright (c) Reality Collective. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using RealityToolkit.Input.Events;
using RealityToolkit.Input.Interactors;
using UnityEngine;

namespace RealityToolkit.Input.InteractionActions
{
    /// <summary>
    /// This <see cref="IInteractionAction"/> will focus lock <see cref="IInteractor"/>s on the <see cref="Interactables.IInteractable"/>,
    /// when the <see cref="Interactables.IInteractable.IsSelected"/> or <see cref="Interactables.IInteractable.IsGrabbed"/>.
    /// </summary>
    [DisallowMultipleComponent]
    public class FocusLockAction : BaseInteractionAction
    {
        /// <inheritdoc/>
        protected override void OnSelectEntered(InteractionEventArgs eventArgs)
        {
            if (eventArgs.Interactor is IDirectInteractor)
            {
                return;
            }

            eventArgs.Interactor.IsFocusLocked = true;
        }

        /// <inheritdoc/>
        protected override void OnSelectExited(InteractionExitEventArgs eventArgs)
        {
            if (eventArgs.Interactor is IDirectInteractor)
            {
                return;
            }

            eventArgs.Interactor.IsFocusLocked = Interactable.IsGrabbed;
        }

        /// <inheritdoc/>
        protected override void OnGrabEntered(InteractionEventArgs eventArgs)
        {
            if (eventArgs.Interactor is IDirectInteractor)
            {
                return;
            }

            eventArgs.Interactor.IsFocusLocked = true;
        }

        /// <inheritdoc/>
        protected override void OnGrabExited(InteractionExitEventArgs eventArgs)
        {
            if (eventArgs.Interactor is IDirectInteractor)
            {
                return;
            }

            eventArgs.Interactor.IsFocusLocked = Interactable.IsSelected;
        }
    }
}
