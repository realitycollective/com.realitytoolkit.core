// Copyright (c) Reality Collective. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

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
        private GameObject stayingColliderHit;
        private IInteractable current;

        private void Awake()
        {
            ConfigureTriggerCollider();
        }

        private void ToggleFarInteractors(bool enabled)
        {
            if (!InputService.TryGetInteractors(InputSource, out var interactors))
            {
                return;
            }

            for (var i = 0; i < interactors.Count; i++)
            {
                var interactor = interactors[i];
                if (interactor.IsFarInteractor)
                {
                    interactor.IsOverriden = !enabled;
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

        /// <summary>
        /// <see cref="MonoBehaviour"/>.
        /// </summary>
        /// <param name="other">The other <see cref="Collider"/> involved in this collision.</param>
        protected virtual void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent(out current))
            {
                stayingColliderHit = other.gameObject;
                directResult.UpdateHit(this, other.gameObject);
                ToggleFarInteractors(false);
            }
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
                ToggleFarInteractors(false);
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
                OnRaisePointerClicked(pointerAction);
            }

            if (other.gameObject == stayingColliderHit)
            {
                stayingColliderHit = null;
                directResult.Clear();
                ToggleFarInteractors(true);
            }
        }
    }
}
