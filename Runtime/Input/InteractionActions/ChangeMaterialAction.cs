// Copyright (c) Reality Collective. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using RealityToolkit.Input.Definitions;
using UnityEngine;

namespace RealityToolkit.Input.InteractionActions
{
    /// <summary>
    /// This <see cref="IInteractionAction"/> will change the main material used on a <see cref="MeshRenderer"/> on the <see cref="Interactables.IInteractable"/>
    /// depending on its <see cref="InteractionState"/>.
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
        public override void OnStateChanged(InteractionState state)
        {
            switch (state)
            {
                case InteractionState.Normal:
                    meshRenderer.material = normalMaterial;
                    break;
                case InteractionState.Focused:
                    meshRenderer.material = focusedMaterial;
                    break;
                case InteractionState.Selected:
                    meshRenderer.material = selectedMaterial;
                    break;
            }
        }
    }
}