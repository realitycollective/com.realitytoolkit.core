// Copyright (c) Reality Collective. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using RealityToolkit.Input.Definitions;
using UnityEngine;
using UnityEngine.Events;

namespace RealityToolkit.Input.InteractionActions
{
    /// <summary>
    /// A utility <see cref="IInteractionAction"/> that provides events for each kind of
    /// interaction on the <see cref="Interactables.IInteractable"/> that you can use to hook up
    /// events in the inspector or via code.
    /// </summary>
    [DisallowMultipleComponent]
    public class InteractionEventsAction : BaseInteractionAction
    {
        [Space]
        [SerializeField]
        [Tooltip("Event raised whenever the interactable's state has changed to focused.")]
        private UnityEvent focused = null;

        [Space]
        [SerializeField]
        [Tooltip("Event raised whenever the interactable's state has changed to unfocused.")]
        private UnityEvent unfocused = null;

        [Space]
        [SerializeField]
        [Tooltip("Event raised whenever the interactable's state has changed to selected.")]
        private UnityEvent selected = null;

        [Space]
        [SerializeField]
        [Tooltip("Event raised whenever the interactable's state has changed to deselected.")]
        private UnityEvent deselected = null;

        /// <inheritdoc/>
        public override void OnStateChanged(InteractionState state)
        {

        }
    }
}
