// Copyright (c) Reality Collective. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace RealityToolkit.Input.Events
{
    /// <summary>
    /// Event data associated with the end of an interaction event between an <see cref="Interactors.IInteractor"/> and <see cref="Interactables.IInteractable"/>.
    /// </summary>
    public class BaseInteractionExitEventArgs : BaseInteractionEventArgs
    {
        /// <summary>
        /// Whether the interaction was ended due to being canceled, such as from
        /// either the <see cref="Interactors.IInteractor"/> or <see cref="Interactables.IInteractable"/> being
        /// unregistered due to being disabled or destroyed.
        /// </summary>
        public bool IsCanceled { get; set; }
    }
}
