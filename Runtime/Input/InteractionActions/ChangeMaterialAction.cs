// Copyright (c) Reality Collective. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using RealityToolkit.Input.Events;
using UnityEngine;

namespace RealityToolkit.Input.InteractionActions
{
    /// <summary>
    /// This <see cref="IInteractionAction"/> will change the main material used on a <see cref="MeshRenderer"/> on the <see cref="Interactables.IInteractable"/>.
    /// </summary>
    public class ChangeMaterialAction : BaseInteractionAction
    {
        [SerializeField]
        private Material normalMaterial = null;

        [SerializeField]
        private Material focusedMaterial = null;

        [SerializeField]
        private Material selectedMaterial = null;

        [SerializeField]
        private MeshRenderer meshRenderer = null;

        /// <inheritdoc/>
        public override void OnFirstFocusEntered(InteractionEventArgs eventArgs)
        {
            meshRenderer.material = focusedMaterial;
        }

        /// <inheritdoc/>
        public override void OnLastFocusExited(InteractionExitEventArgs eventArgs)
        {
            meshRenderer.material = normalMaterial;
        }

        /// <inheritdoc/>
        public override void OnFirstSelectEntered(InteractionEventArgs eventArgs)
        {
            meshRenderer.material = selectedMaterial;
        }

        /// <inheritdoc/>
        public override void OnLastSelectExited(InteractionExitEventArgs eventArgs)
        {
            meshRenderer.material = normalMaterial;
        }
    }
}