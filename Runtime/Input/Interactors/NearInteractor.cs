// Copyright (c) Reality Collective. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using RealityToolkit.Input.Interactables;
using UnityEngine;

namespace RealityToolkit.Input.Interactors
{
    /// <summary>
    /// <see cref="IDirectInteractor"/> that interacts with <see cref="IInteractable"/>s that are nearby.
    /// </summary>
    [RequireComponent(typeof(SphereCollider))]
    public class NearInteractor : BaseDirectInteractor
    {
        private SphereCollider sphereCollider;
        private GameObject stayingColliderHit;

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
            }
        }

        private void ConfigureTriggerCollider()
        {
            sphereCollider = GetComponent<SphereCollider>();
            sphereCollider.isTrigger = true;
            sphereCollider.radius = PointerExtent;
        }

        /// <summary>
        /// <see cref="MonoBehaviour"/>.
        /// </summary>
        /// <param name="other">The other <see cref="Collider"/> involved in this collision.</param>
        protected virtual void OnTriggerStay(Collider other)
        {
            if (other.TryGetComponent<IInteractable>(out _))
            {
                stayingColliderHit = other.gameObject;
                directResult.UpdateHit(this, other.gameObject);
                UpdateInteractorPrivilege(false);
            }
        }

        /// <summary>
        /// <see cref="MonoBehaviour"/>.
        /// </summary>
        /// <param name="other">The other <see cref="Collider"/> involved in this collision.</param>
        protected virtual void OnTriggerExit(Collider other)
        {
            if (other.gameObject == stayingColliderHit)
            {
                stayingColliderHit = null;
                directResult.Clear();
                UpdateInteractorPrivilege(true);
            }
        }
    }
}
