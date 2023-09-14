// Copyright (c) Reality Collective. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using RealityToolkit.Input.Events;
using UnityEngine;

namespace RealityToolkit.Input.InteractionActions
{
    /// <summary>
    /// This <see cref="IInteractionAction"/> will update the <see cref="Rigidbody"/> on the <see cref="Interactables.IInteractable"/>
    /// when interacted with.
    /// </summary>
    public class UpdateRigidbodyAction : BaseInteractionAction
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
        protected override void OnFirstGrabEntered(InteractionEventArgs eventArgs)
        {
            rigidbody.isKinematic = true;
            rigidbody.useGravity = false;
        }

        /// <inheritdoc/>
        protected override void OnLastGrabExited(InteractionExitEventArgs eventArgs)
        {
            rigidbody.isKinematic = isKinematic;
            rigidbody.useGravity = useGravity;
        }
    }
}
