// Copyright (c) Reality Collective. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using RealityCollective.Extensions;
using RealityToolkit.Input.Events;
using RealityToolkit.Input.Interactors;
using UnityEngine;

namespace RealityToolkit.Input.InteractionActions
{
    /// <summary>
    /// A <see cref="IInteractionAction"/> that will translate a given <see cref="Transform"/>
    /// in response to being poked by a <see cref="IPokeInteractor"/>.
    /// </summary>
    public class PokeResponseAction : BaseInteractionAction
    {
        [SerializeField, Tooltip("The target transform to respond to the poke. Defaults to local transform if not set.")]
        private Transform target = null;

        [SerializeField, Tooltip("The axes to respond to the poke.")]
        private SnapAxis pokeAxis = SnapAxis.All;

        private IPokeInteractor currentInteractor;
        private Vector3 defaultLocalPosition;
        private Vector3 pokeStartInteractorPosition;

        /// <inheritdoc/>
        protected override void Awake()
        {
            base.Awake();

            if (target.IsNull())
            {
                target = transform;
            }

            defaultLocalPosition = target.localPosition;
        }

        /// <inheritdoc/>
        protected override void Update()
        {
            if (currentInteractor == null)
            {
                return;
            }

            var deltaX = 0f;
            var deltaY = 0f;
            var deltaZ = 0f;

            if ((pokeAxis & SnapAxis.X) != 0)
            {
                var startX = pokeStartInteractorPosition.x;
                var currentX = currentInteractor.GameObject.transform.position.x;
                deltaX = currentX - startX;
            }

            if ((pokeAxis & SnapAxis.Y) != 0)
            {
                var startY = pokeStartInteractorPosition.y;
                var currentY = currentInteractor.GameObject.transform.position.y;
                deltaY = currentY - startY;
            }

            if ((pokeAxis & SnapAxis.Z) != 0)
            {
                var startZ = pokeStartInteractorPosition.z;
                var currentZ = currentInteractor.GameObject.transform.position.z;
                deltaZ = currentZ - startZ;
            }

            var pokeOffset = new Vector3(deltaX, deltaY, deltaZ);
            target.localPosition = defaultLocalPosition + pokeOffset;
        }

        /// <inheritdoc/>
        protected override void OnFirstSelectEntered(InteractionEventArgs eventArgs)
        {
            if (eventArgs.Interactor is not IPokeInteractor pokeInteractor)
            {
                return;
            }

            currentInteractor = pokeInteractor;
            pokeStartInteractorPosition = pokeInteractor.GameObject.transform.position;
        }

        /// <inheritdoc/>
        protected override void OnLastSelectExited(InteractionExitEventArgs eventArgs)
        {
            currentInteractor = null;
            target.localPosition = defaultLocalPosition;
        }
    }
}
