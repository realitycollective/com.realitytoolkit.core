// Copyright (c) Reality Collective. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using RealityToolkit.Input.Definitions;
using RealityToolkit.Input.Interactors;

namespace RealityToolkit.Input.InteractionActions
{
    /// <summary>
    /// This <see cref="IInteractionAction"/> will focus lock <see cref="IInteractor"/>s on the <see cref="Interactables.IInteractable"/>
    /// object depending on its <see cref="InteractionState"/>.
    /// </summary>
    public class FocusLockAction : BaseInteractionAction
    {
        private IInteractor currentInteractor;

        /// <inheritdoc/>
        public override void OnStateChanged(InteractionState state)
        {
            if (state == InteractionState.Selected)
            {
                currentInteractor = Interactable.PrimaryInteractor;
                currentInteractor.IsFocusLocked = true;
            }
            else if (currentInteractor != null)
            {
                currentInteractor.IsFocusLocked = false;
                currentInteractor = null;
            }
        }
    }
}
