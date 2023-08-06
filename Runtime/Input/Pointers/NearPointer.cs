// Copyright (c) Reality Collective. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;

namespace RealityToolkit.Input.Pointers
{
    /// <summary>
    /// <see cref="Interfaces.IPointer"/> used for directly interacting with interactables that are touching.
    /// </summary>
    [RequireComponent(typeof(SphereCollider))]
    public class NearPointer : BaseControllerPointer
    {
        private SphereCollider sphereCollider;

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
        protected virtual void OnTriggerEnter(Collider other)
        {

        }

        /// <summary>
        /// <see cref="MonoBehaviour"/>.
        /// </summary>
        /// <param name="other">The other <see cref="Collider"/> involved in this collision.</param>
        protected virtual void OnTriggerStay(Collider other)
        {

        }

        /// <summary>
        /// <see cref="MonoBehaviour"/>.
        /// </summary>
        /// <param name="other">The other <see cref="Collider"/> involved in this collision.</param>
        protected virtual void OnTriggerExit(Collider other)
        {

        }
    }
}
