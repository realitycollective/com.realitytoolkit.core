// Copyright (c) Reality Collective. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using RealityToolkit.Input.Definitions;

namespace RealityToolkit.Input.Interactions.Actions
{
    /// <summary>
    /// This <see cref="IInteractionAction"/> will focus lock <see cref="Input.Interfaces.IPointer"/>s on the <see cref="Interactables.IInteractable"/>
    /// object depending on its <see cref="InteractionState"/>.
    /// </summary>
    public class FocusLockAction : BaseInteractionAction
    {
        /// <inheritdoc/>
        public override void OnStateChanged(InteractionState state)
        {
            if (Interactable.PrimaryInteractor == null)
            {
                return;
            }

            for (var i = 0; i < Interactable.PrimaryInteractor.InputSource.Pointers.Length; i++)
            {
                Interactable.PrimaryInteractor.InputSource.Pointers[i].IsFocusLocked = state == InteractionState.Selected;
            }
        }
    }
}
