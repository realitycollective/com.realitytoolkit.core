// Copyright (c) Reality Collective. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace RealityToolkit.Input.Interactables
{
    /// <summary>
    /// Options for how an <see cref="IInteractable"/> may be focused by <see cref="Interactors.IInteractor"/>s.
    /// </summary>
    public enum InteractableFocusMode
    {
        /// <summary>
        /// <see cref="IInteractable"/> cannot be focused at all.
        /// </summary>
        None = 0,
        /// <summary>
        /// <see cref="IInteractable"/> can only be focused by a single <see cref="Interactors.IInteractor"/>.
        /// Another <see cref="Interactors.IInteractor"/> gaining focus on the <see cref="IInteractable"/> will auto
        /// unfocus the previous <see cref="Interactors.IInteractor"/>.
        /// </summary>
        Single,
        /// <summary>
        /// Multiple <see cref="Interactors.IInteractor"/>s can focus the <see cref="IInteractable"/>.
        /// </summary>
        Multiple,
    }
}
