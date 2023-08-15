// Copyright (c) Reality Collective. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using RealityToolkit.Input.Events;
using RealityToolkit.Input.Interactors;
using UnityEngine;

namespace RealityToolkit.Input.InteractionActions
{
    /// <summary>
    /// This <see cref="IInteractionAction"/> will focus lock <see cref="IInteractor"/>s on the <see cref="Interactables.IInteractable"/>
    /// object depending on its <see cref="InteractionState"/>.
    /// </summary>
    [DisallowMultipleComponent]
    public class FocusLockAction : BaseInteractionAction
    {
        /// <inheritdoc/>
        public override void OnFocusEntered(InteractionEventArgs eventArgs)
        {
            if (eventArgs.Interactor is IDirectInteractor)
            {
                return;
            }

            eventArgs.Interactor.IsFocusLocked = true;
        }

        /// <inheritdoc/>
        public override void OnFocusExited(InteractionExitEventArgs eventArgs)
        {
            if (eventArgs.Interactor is IDirectInteractor)
            {
                return;
            }

            eventArgs.Interactor.IsFocusLocked = false;
        }
    }
}
