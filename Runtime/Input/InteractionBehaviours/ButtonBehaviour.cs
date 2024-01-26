// Copyright (c) Reality Collective. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using RealityToolkit.Input.Events;
using System;
using UnityEngine;
using UnityEngine.Events;

namespace RealityToolkit.Input.InteractionBehaviours
{
    /// <summary>
    /// A <see cref="IInteractionBehaviour"/> for creating <see cref="Interactables.IInteractable"/>s that mimick button behaviour.
    /// </summary>
    public class ButtonBehaviour : BaseInteractionBehaviour
    {
        /// <summary>
        /// <see cref="UnityEvent"/> for when a button is clicked.
        /// </summary>
        [Serializable]
        public class ButtonClickEvent : UnityEvent { }

        [Space]
        [SerializeField, Tooltip("List of click delegates triggered on click.")]
        private ButtonClickEvent click = null;

        /// <summary>
        /// The button was clicked.
        /// </summary>
        public ButtonClickEvent Click => click;

        /// <inheritdoc/>
        protected override void OnActivated(InteractionEventArgs eventArgs)
        {
            Click?.Invoke();
        }
    }
}
