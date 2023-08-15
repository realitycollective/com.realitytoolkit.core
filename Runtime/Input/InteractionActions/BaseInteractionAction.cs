// Copyright (c) Reality Collective. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using RealityToolkit.Input.Events;
using RealityToolkit.Input.Interactables;
using UnityEngine;

namespace RealityToolkit.Input.InteractionActions
{
    /// <summary>
    /// Base implementation for <see cref="IInteractionAction"/>s.
    /// </summary>
    [RequireComponent(typeof(Interactable))]
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
        public virtual void OnFirstFocusEntered(InteractionEventArgs eventArgs) { }

        /// <inheritdoc/>
        public virtual void OnFocusEntered(InteractionEventArgs eventArgs) { }

        /// <inheritdoc/>
        public virtual void OnFocusExited(InteractionExitEventArgs eventArgs) { }

        /// <inheritdoc/>
        public virtual void OnLastFocusExited(InteractionExitEventArgs eventArgs) { }

        /// <inheritdoc/>
        public virtual void OnFirstSelectEntered(InteractionEventArgs eventArgs) { }

        /// <inheritdoc/>
        public virtual void OnSelectEntered(InteractionEventArgs eventArgs) { }

        /// <inheritdoc/>
        public virtual void OnSelectExited(InteractionExitEventArgs eventArgs) { }

        /// <inheritdoc/>
        public virtual void OnLastSelectExited(InteractionExitEventArgs eventArgs) { }

        /// <inheritdoc/>
        public virtual void OnFirstGrabEntered(InteractionEventArgs eventArgs) { }

        /// <inheritdoc/>
        public virtual void OnGrabEntered(InteractionEventArgs eventArgs) { }

        /// <inheritdoc/>
        public virtual void OnGrabExited(InteractionExitEventArgs eventArgs) { }

        /// <inheritdoc/>
        public virtual void OnLastGrabExited(InteractionExitEventArgs eventArgs) { }
    }
}
