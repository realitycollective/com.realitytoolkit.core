// Copyright (c) Reality Collective. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using RealityToolkit.Input.Definitions;
using RealityToolkit.Input.Interactables;
using UnityEngine;

namespace RealityToolkit.Input.Interactors
{
    /// <summary>
    /// <see cref="IDirectInteractor"/> that interacts with <see cref="IInteractable"/>s that by touching them.
    /// </summary>
    [RequireComponent(typeof(SphereCollider))]
    public class PokeInteractor : BaseDirectInteractor, IPokeInteractor
    {
        private SphereCollider sphereCollider;
        private IInteractable current;

        private void Awake()
        {
            ConfigureTriggerCollider();
        }

        private void UpdateInteractorPrivilege(bool enabled)
        {
            if (!InputService.TryGetInteractors(InputSource, out var interactors))
            {
                return;
            }

            for (var i = 0; i < interactors.Count; i++)
            {
                var interactor = interactors[i];
                if ((IInteractor)this == interactor)
                {
                    continue;
                }

                if (interactor is IControllerInteractor controllerInteractor)
                {
                    controllerInteractor.DirectPrivilege = !enabled;
                }

                if (interactor is IDirectInteractor directInteractor)
                {
                    directInteractor.PokePrivilege = !enabled;
                }
            }
        }

        private void ConfigureTriggerCollider()
        {
            sphereCollider = GetComponent<SphereCollider>();
            sphereCollider.isTrigger = true;
            sphereCollider.radius = PointerExtent;
        }

        private void Update()
        {
            if (Controller == null || Controller.Visualizer == null)
            {
                return;
            }

            transform.SetPositionAndRotation(Controller.Visualizer.PokePose.position, Controller.Visualizer.PokePose.rotation);
        }

        /// <inheritdoc/>
        protected override void OnRaisePointerDown(InputAction inputAction)
        {
            UpdateInteractorPrivilege(false);
            base.OnRaisePointerDown(inputAction);
            UpdateInteractorPrivilege(true);
        }

        /// <inheritdoc/>
        protected override void OnRaisePointerUp(InputAction inputAction)
        {
            UpdateInteractorPrivilege(false);
            base.OnRaisePointerUp(inputAction);
            UpdateInteractorPrivilege(true);
        }

        /// <inheritdoc/>
        /// <remarks>
        /// Whenever the <see cref="IPokeInteractor"/> has completed a poke it has to perform clean up.
        /// It also has to assign temporarily poke privilege to all <see cref="IInteractor"/>s.
        /// </remarks>
        protected override void OnRaisePointerClicked(InputAction inputAction)
        {
            UpdateInteractorPrivilege(false);
            base.OnRaisePointerClicked(inputAction);
            current = null;
            directResult.Clear();
            UpdateInteractorPrivilege(true);
        }

        /// <summary>
        /// <see cref="MonoBehaviour"/>.
        /// </summary>
        /// <param name="other">The other <see cref="Collider"/> involved in this collision.</param>
        protected virtual void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent(out current))
            {
                directResult.UpdateHit(this, other.gameObject);
                OnRaisePointerDown(pointerAction);
            }
        }

        /// <summary>
        /// <see cref="MonoBehaviour"/>.
        /// </summary>
        /// <param name="other">The other <see cref="Collider"/> involved in this collision.</param>
        protected virtual void OnTriggerExit(Collider other)
        {
            if (other.TryGetComponent<IInteractable>(out var exitInteractable) &&
                current == exitInteractable)
            {
                OnRaisePointerUp(pointerAction);
                OnRaisePointerClicked(pointerAction);
            }
        }
    }
}
