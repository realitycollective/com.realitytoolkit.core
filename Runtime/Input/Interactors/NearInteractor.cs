// Copyright (c) Reality Collective. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using RealityToolkit.Input.Interactables;
using UnityEngine;

namespace RealityToolkit.Input.Interactors
{
    /// <summary>
    /// <see cref="Interfaces.IPointer"/> used for directly interacting with interactables that are touching.
    /// </summary>
    [RequireComponent(typeof(SphereCollider))]
    public class NearInteractor : BaseControllerInteractor
    {
        private SphereCollider sphereCollider;

        /// <inheritdoc />
        public override bool IsFarInteractor => false;

        /// <summary>
        /// Scene <see cref="GameObject"/> with a collider that is currently being hit, if any.
        /// </summary>
        public GameObject PhysicsHit { get; private set; }

        public float PhysicsHitDistance { get; private set; }

        public Vector3 PhysicsHitDirection { get; private set; }

        private void Awake()
        {
            ConfigureTriggerCollider();
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
            if (other.TryGetComponent<IInteractable>(out var interactable) &&
                interactable.IsValid)
            {
                PhysicsHit = other.gameObject;
            }
        }
    }
}
