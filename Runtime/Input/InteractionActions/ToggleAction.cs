// Copyright (c) Reality Collective. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using RealityToolkit.Input.Events;
using System;
using UnityEngine;
using UnityEngine.Events;

namespace RealityToolkit.Input.InteractionActions
{
    /// <summary>
    /// A <see cref="IInteractionAction"/> for creating <see cref="Interactables.IInteractable"/>s that mimick toggle button behaviour.
    /// </summary>
    [DisallowMultipleComponent]
    public class ToggleAction : BaseInteractionAction
    {
        /// <summary>
        /// <see cref="UnityEvent"/> for when a toggle is toggled.
        /// </summary>
        [Serializable]
        public class ToggleEvent : UnityEvent<bool> { }

        [Space]
        [SerializeField, Tooltip("List of delegates triggered on value change.")]
        private ToggleEvent valueChanged = null;

        /// <summary>
        /// The toggle <see cref="IsOn"/> value has changed.
        /// </summary>
        public ToggleEvent ValueChanged => valueChanged;

        /// <summary>
        /// Is the toggle currently on or off?
        /// </summary>
        public bool IsOn => Interactable.IsActivated;

        /// <inheritdoc/>
        protected override void OnActivated(InteractionEventArgs eventArgs)
        {
            ValueChanged?.Invoke(IsOn);
        }

        /// <inheritdoc/>
        protected override void OnDeactivated(InteractionExitEventArgs eventArgs)
        {
            ValueChanged?.Invoke(IsOn);
        }
    }
}
