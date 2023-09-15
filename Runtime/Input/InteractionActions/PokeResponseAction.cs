// Copyright (c) Reality Collective. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using RealityCollective.Extensions;
using RealityToolkit.Input.Events;
using RealityToolkit.Input.Interactors;
using System;
using UnityEngine;

namespace RealityToolkit.Input.InteractionActions
{
    /// <summary>
    /// A <see cref="IInteractionAction"/> that will translate a given <see cref="Transform"/>
    /// in response to being poked by a <see cref="IPokeInteractor"/>.
    /// </summary>
    public class PokeResponseAction : BaseInteractionAction
    {
        [Serializable]
        private struct PokeResponseLimits
        {
            public Vector3 Min;
            public Vector3 Max;
        }

        [SerializeField, Tooltip("The target transform to respond to the poke. Defaults to local transform if not set.")]
        private Transform target = null;

        [SerializeField, Tooltip("The axes to respond to the poke.")]
        private SnapAxis pokeAxis = SnapAxis.All;

        [SerializeField, Tooltip("Offset limitations per axis.")]
        private PokeResponseLimits responseLimits = default;

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

            var targetLocalPosition = defaultLocalPosition;

            if ((pokeAxis & SnapAxis.X) != 0)
            {
                var startX = pokeStartInteractorPosition.x;
                var currentX = currentInteractor.GameObject.transform.position.x;
                var deltaX = currentX - startX;

                var targetX = defaultLocalPosition.x + deltaX;
                targetLocalPosition.x = Mathf.Clamp(targetX, responseLimits.Min.x, responseLimits.Max.x);
            }

            if ((pokeAxis & SnapAxis.Y) != 0)
            {
                var startY = pokeStartInteractorPosition.y;
                var currentY = currentInteractor.GameObject.transform.position.y;
                var deltaY = currentY - startY;

                var targetY = defaultLocalPosition.y + deltaY;
                targetLocalPosition.y = Mathf.Clamp(targetY, responseLimits.Min.y, responseLimits.Max.y);
            }

            if ((pokeAxis & SnapAxis.Z) != 0)
            {
                var startZ = pokeStartInteractorPosition.z;
                var currentZ = currentInteractor.GameObject.transform.position.z;
                var deltaZ = currentZ - startZ;

                var targetZ = defaultLocalPosition.z + deltaZ;
                targetLocalPosition.z = Mathf.Clamp(targetZ, responseLimits.Min.z, responseLimits.Max.z);
            }

            target.localPosition = targetLocalPosition;
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
