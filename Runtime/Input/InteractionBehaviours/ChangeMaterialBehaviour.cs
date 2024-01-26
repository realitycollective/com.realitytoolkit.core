// Copyright (c) Reality Collective. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;

namespace RealityToolkit.Input.InteractionBehaviours
{
    /// <summary>
    /// This <see cref="IInteractionBehaviour"/> will change the main material used on a <see cref="MeshRenderer"/> on the <see cref="Interactables.IInteractable"/>.
    /// </summary>
    public class ChangeMaterialBehaviour : BaseInteractionBehaviour
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
        protected override void Update()
        {
            if (Interactable.IsSelected || Interactable.IsGrabbed)
            {
                meshRenderer.material = selectedMaterial;
            }
            else if (Interactable.IsFocused)
            {
                meshRenderer.material = focusedMaterial;
            }
            else
            {
                meshRenderer.material = normalMaterial;
            }
        }
    }
}