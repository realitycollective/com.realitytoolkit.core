// Copyright (c) Reality Collective. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using RealityCollective.Definitions.Utilities;
using RealityToolkit.Input.Events;
using RealityToolkit.Input.Interactables;
using RealityToolkit.Input.Interactors;
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

        [SerializeField, Tooltip("The handedness of the interactor to perform the action for." +
            "The action will only be performed, if the handedness is a match.")]
        private Handedness targetHandedness = Handedness.Any;

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
        void IInteractionAction.OnActivated(InteractionEventArgs eventArgs)
        {
            if (ShouldPerformAction(eventArgs))
            {
                OnActivated(eventArgs);
            }
        }

        /// <inheritdoc cref="IInteractionAction.OnActivated"/>
        protected virtual void OnActivated(InteractionEventArgs eventArgs) { }

        /// <inheritdoc/>
        void IInteractionAction.OnDeactivated(InteractionExitEventArgs eventArgs)
        {
            if (ShouldPerformAction(eventArgs))
            {
                OnDeactivated(eventArgs);
            }
        }

        /// <inheritdoc cref="IInteractionAction.OnDeactivated"/>
        protected virtual void OnDeactivated(InteractionExitEventArgs eventArgs) { }

        /// <inheritdoc/>
        void IInteractionAction.OnFirstFocusEntered(InteractionEventArgs eventArgs)
        {
            if (ShouldPerformAction(eventArgs))
            {
                OnFirstFocusEntered(eventArgs);
            }
        }

        /// <inheritdoc cref="IInteractionAction.OnFirstFocusEntered"/>
        protected virtual void OnFirstFocusEntered(InteractionEventArgs eventArgs) { }

        /// <inheritdoc/>
        void IInteractionAction.OnFocusEntered(InteractionEventArgs eventArgs)
        {
            if (ShouldPerformAction(eventArgs))
            {
                OnFocusEntered(eventArgs);
            }
        }

        /// <inheritdoc cref="IInteractionAction.OnFocusEntered"/>
        protected virtual void OnFocusEntered(InteractionEventArgs eventArgs) { }

        /// <inheritdoc/>
        void IInteractionAction.OnFocusExited(InteractionExitEventArgs eventArgs)
        {
            if (ShouldPerformAction(eventArgs))
            {
                OnFocusExited(eventArgs);
            }
        }

        /// <inheritdoc cref="IInteractionAction.OnFocusExited"/>
        protected virtual void OnFocusExited(InteractionExitEventArgs eventArgs) { }

        /// <inheritdoc/>
        void IInteractionAction.OnLastFocusExited(InteractionExitEventArgs eventArgs)
        {
            if (ShouldPerformAction(eventArgs))
            {
                OnLastFocusExited(eventArgs);
            }
        }

        /// <inheritdoc cref="IInteractionAction.OnLastFocusExited"/>
        protected virtual void OnLastFocusExited(InteractionExitEventArgs eventArgs) { }

        /// <inheritdoc/>
        void IInteractionAction.OnFirstSelectEntered(InteractionEventArgs eventArgs)
        {
            if (ShouldPerformAction(eventArgs))
            {
                OnFirstSelectEntered(eventArgs);
            }
        }

        /// <inheritdoc cref="IInteractionAction.OnFirstSelectEntered"/>
        protected virtual void OnFirstSelectEntered(InteractionEventArgs eventArgs) { }

        /// <inheritdoc/>
        void IInteractionAction.OnSelectEntered(InteractionEventArgs eventArgs)
        {
            if (ShouldPerformAction(eventArgs))
            {
                OnSelectEntered(eventArgs);
            }
        }

        /// <inheritdoc cref="IInteractionAction.OnSelectEntered"/>
        protected virtual void OnSelectEntered(InteractionEventArgs eventArgs) { }

        /// <inheritdoc/>
        void IInteractionAction.OnSelectExited(InteractionExitEventArgs eventArgs)
        {
            if (ShouldPerformAction(eventArgs))
            {
                OnSelectExited(eventArgs);
            }
        }

        /// <inheritdoc cref="IInteractionAction.OnSelectExited"/>
        protected virtual void OnSelectExited(InteractionExitEventArgs eventArgs) { }

        /// <inheritdoc/>
        void IInteractionAction.OnLastSelectExited(InteractionExitEventArgs eventArgs)
        {
            if (ShouldPerformAction(eventArgs))
            {
                OnLastSelectExited(eventArgs);
            }
        }

        /// <inheritdoc cref="IInteractionAction.OnLastSelectExited"/>
        protected virtual void OnLastSelectExited(InteractionExitEventArgs eventArgs) { }

        /// <inheritdoc/>
        void IInteractionAction.OnFirstGrabEntered(InteractionEventArgs eventArgs)
        {
            if (ShouldPerformAction(eventArgs))
            {
                OnFirstGrabEntered(eventArgs);
            }
        }

        /// <inheritdoc cref="IInteractionAction.OnFirstGrabEntered"/>
        protected virtual void OnFirstGrabEntered(InteractionEventArgs eventArgs) { }

        /// <inheritdoc/>
        void IInteractionAction.OnGrabEntered(InteractionEventArgs eventArgs)
        {
            if (ShouldPerformAction(eventArgs))
            {
                OnGrabEntered(eventArgs);
            }
        }

        /// <inheritdoc cref="IInteractionAction.OnGrabEntered"/>
        protected virtual void OnGrabEntered(InteractionEventArgs eventArgs) { }

        /// <inheritdoc/>
        void IInteractionAction.OnGrabExited(InteractionExitEventArgs eventArgs)
        {
            if (ShouldPerformAction(eventArgs))
            {
                OnGrabExited(eventArgs);
            }
        }

        /// <inheritdoc cref="IInteractionAction.OnGrabExited"/>
        protected virtual void OnGrabExited(InteractionExitEventArgs eventArgs) { }

        /// <inheritdoc/>
        void IInteractionAction.OnLastGrabExited(InteractionExitEventArgs eventArgs)
        {
            if (ShouldPerformAction(eventArgs))
            {
                OnLastGrabExited(eventArgs);
            }
        }

        /// <inheritdoc cref="IInteractionAction.OnLastGrabExited"/>
        protected virtual void OnLastGrabExited(InteractionExitEventArgs eventArgs) { }

        private bool ShouldPerformAction(BaseInteractionEventArgs eventArgs)
        {
            // For non-controller interactors actions are always performed, for now.
            if (eventArgs.Interactor is not IControllerInteractor controllerInteractor)
            {
                return true;
            }

            // For controller interactors we must match the handedness to perform the action.
            var handedness = controllerInteractor.Controller.ControllerHandedness;
            return (handedness & targetHandedness) != 0;
        }
    }
}
