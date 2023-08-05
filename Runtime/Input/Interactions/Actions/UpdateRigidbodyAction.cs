// Copyright (c) Reality Collective. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;

namespace RealityToolkit.Input.Interactions.Actions
{
    /// <summary>
    /// This <see cref="IInteractionAction"/> will update the <see cref="Rigidbody"/> on the <see cref="Interactables.IInteractable"/>
    /// object depending on its <see cref="InteractionState"/>.
    /// </summary>
    public class UpdateRigidbodyAction : Action
    {
        private new Rigidbody rigidbody;
        private bool isKinematic;
        private bool useGravity;

        /// <inheritdoc/>
        protected override void Awake()
        {
            base.Awake();
            rigidbody = GetComponentInChildren<Rigidbody>();
            isKinematic = rigidbody.isKinematic;
            useGravity = rigidbody.useGravity;
        }

        /// <inheritdoc/>
        public override void OnStateChanged(InteractionState state)
        {
            if (!Interactable.IsValid)
            {
                return;
            }

            if (state == InteractionState.Selected)
            {
                rigidbody.isKinematic = true;
                rigidbody.useGravity = false;
            }
            else
            {
                rigidbody.isKinematic = isKinematic;
                rigidbody.useGravity = useGravity;
            }
        }
    }
}
