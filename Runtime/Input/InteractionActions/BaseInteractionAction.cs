// Copyright (c) Reality Collective. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using RealityToolkit.Input.Definitions;
using RealityToolkit.Input.Interactables;
using UnityEngine;

namespace RealityToolkit.Input.InteractionActions
{
    /// <summary>
    /// Base implementation for <see cref="IInteractionAction"/>s.
    /// </summary>
    public abstract class BaseInteractionAction : MonoBehaviour, IInteractionAction
    {
        [SerializeField, Tooltip("Actions with a higher sorting order will always be executed after the ones with a lower sorting order.")]
        private short sortingOrder = 0;

        /// <inheritdoc/>
        public short SortingOrder => sortingOrder;

        /// <inheritdoc/>
        public IInteractable Interactable { get; private set; }

        /// <summary>
        /// <see cref="MonoBehaviour"/>.
        /// </summary>
        protected virtual void Awake() { }

        /// <summary>
        /// <see cref="MonoBehaviour"/>.
        /// </summary>
        protected virtual void OnEnable()
        {
            Interactable = GetComponent<IInteractable>();
            Interactable.Add(this);
        }

        /// <summary>
        /// <see cref="MonoBehaviour"/>.
        /// </summary>
        protected virtual void Update() { }

        /// <summary>
        /// <see cref="MonoBehaviour"/>.
        /// </summary>
        protected virtual void OnDisable()
        {
            if (Interactable != null)
            {
                Interactable.Remove(this);
            }
        }

        /// <summary>
        /// <see cref="MonoBehaviour"/>.
        /// </summary>
        protected virtual void OnDestroy() { }

        /// <inheritdoc/>
        public abstract void OnStateChanged(InteractionState state);
    }
}
