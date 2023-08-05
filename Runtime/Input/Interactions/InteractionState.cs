// Copyright (c) Reality Collective. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;

namespace RealityToolkit.Input.Interactions
{
    /// <summary>
    /// Possible interaction states for <see cref="Interactables.IInteractable"/>s.
    /// </summary>
    [Flags]
    public enum InteractionState
    {
        /// <summary>
        /// The <see cref="Interactables.IInteractable"/> is in its default state, that is
        /// it's not being interacted with.
        /// </summary>
        Normal = 0,
        /// <summary>
        /// The <see cref="Interactables.IInteractable"/> is in its focused state,
        /// that is it's being targeted by at least one <see cref="Interactors.IInteractor"/>.
        /// </summary>
        Focused,
        /// <summary>
        /// The <see cref="Interactables.IInteractable"/> is in its selected state,
        /// that is it's being interactd with by at least one <see cref="Interactors.IInteractor"/>.
        /// </summary>
        Selected
    }
}
