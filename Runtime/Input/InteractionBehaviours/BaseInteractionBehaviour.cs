// Copyright (c) Reality Collective. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using RealityCollective.Definitions.Utilities;
using RealityToolkit.Input.Events;
using RealityToolkit.Input.Interactables;
using RealityToolkit.Input.Interactors;
using UnityEngine;

namespace RealityToolkit.Input.InteractionBehaviours
{
    /// <summary>
    /// Base implementation for <see cref="IInteractionBehaviour"/>s.
    /// </summary>
    [RequireComponent(typeof(Interactable))]
    public abstract class BaseInteractionBehaviour : MonoBehaviour, IInteractionBehaviour
    {
        [SerializeField, Range(0, 100), Tooltip("Behaviours with a higher sorting order will always be executed after the ones with a lower sorting order.")]
        private short sortingOrder = 0;

        [SerializeField, Tooltip("The handedness of the interactor to perform the Behaviour for." +
            "The Behaviour will only be performed, if the handedness is a match.")]
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
        void IInteractionBehaviour.OnActivated(InteractionEventArgs eventArgs)
        {
            if (ShouldPerformBehaviour(eventArgs))
            {
                OnActivated(eventArgs);
            }
        }

        /// <inheritdoc cref="IInteractionBehaviour.OnActivated"/>
        protected virtual void OnActivated(InteractionEventArgs eventArgs) { }

        /// <inheritdoc/>
        void IInteractionBehaviour.OnDeactivated(InteractionExitEventArgs eventArgs)
        {
            if (ShouldPerformBehaviour(eventArgs))
            {
                OnDeactivated(eventArgs);
            }
        }

        /// <inheritdoc cref="IInteractionBehaviour.OnDeactivated"/>
        protected virtual void OnDeactivated(InteractionExitEventArgs eventArgs) { }

        /// <inheritdoc/>
        void IInteractionBehaviour.OnFirstFocusEntered(InteractionEventArgs eventArgs)
        {
            if (ShouldPerformBehaviour(eventArgs))
            {
                OnFirstFocusEntered(eventArgs);
            }
        }

        /// <inheritdoc cref="IInteractionBehaviour.OnFirstFocusEntered"/>
        protected virtual void OnFirstFocusEntered(InteractionEventArgs eventArgs) { }

        /// <inheritdoc/>
        void IInteractionBehaviour.OnFocusEntered(InteractionEventArgs eventArgs)
        {
            if (ShouldPerformBehaviour(eventArgs))
            {
                OnFocusEntered(eventArgs);
            }
        }

        /// <inheritdoc cref="IInteractionBehaviour.OnFocusEntered"/>
        protected virtual void OnFocusEntered(InteractionEventArgs eventArgs) { }

        /// <inheritdoc/>
        void IInteractionBehaviour.OnFocusExited(InteractionExitEventArgs eventArgs)
        {
            if (ShouldPerformBehaviour(eventArgs))
            {
                OnFocusExited(eventArgs);
            }
        }

        /// <inheritdoc cref="IInteractionBehaviour.OnFocusExited"/>
        protected virtual void OnFocusExited(InteractionExitEventArgs eventArgs) { }

        /// <inheritdoc/>
        void IInteractionBehaviour.OnLastFocusExited(InteractionExitEventArgs eventArgs)
        {
            if (ShouldPerformBehaviour(eventArgs))
            {
                OnLastFocusExited(eventArgs);
            }
        }

        /// <inheritdoc cref="IInteractionBehaviour.OnLastFocusExited"/>
        protected virtual void OnLastFocusExited(InteractionExitEventArgs eventArgs) { }

        /// <inheritdoc/>
        void IInteractionBehaviour.OnFirstSelectEntered(InteractionEventArgs eventArgs)
        {
            if (ShouldPerformBehaviour(eventArgs))
            {
                OnFirstSelectEntered(eventArgs);
            }
        }

        /// <inheritdoc cref="IInteractionBehaviour.OnFirstSelectEntered"/>
        protected virtual void OnFirstSelectEntered(InteractionEventArgs eventArgs) { }

        /// <inheritdoc/>
        void IInteractionBehaviour.OnSelectEntered(InteractionEventArgs eventArgs)
        {
            if (ShouldPerformBehaviour(eventArgs))
            {
                OnSelectEntered(eventArgs);
            }
        }

        /// <inheritdoc cref="IInteractionBehaviour.OnSelectEntered"/>
        protected virtual void OnSelectEntered(InteractionEventArgs eventArgs) { }

        /// <inheritdoc/>
        void IInteractionBehaviour.OnSelectExited(InteractionExitEventArgs eventArgs)
        {
            if (ShouldPerformBehaviour(eventArgs))
            {
                OnSelectExited(eventArgs);
            }
        }

        /// <inheritdoc cref="IInteractionBehaviour.OnSelectExited"/>
        protected virtual void OnSelectExited(InteractionExitEventArgs eventArgs) { }

        /// <inheritdoc/>
        void IInteractionBehaviour.OnLastSelectExited(InteractionExitEventArgs eventArgs)
        {
            if (ShouldPerformBehaviour(eventArgs))
            {
                OnLastSelectExited(eventArgs);
            }
        }

        /// <inheritdoc cref="IInteractionBehaviour.OnLastSelectExited"/>
        protected virtual void OnLastSelectExited(InteractionExitEventArgs eventArgs) { }

        /// <inheritdoc/>
        void IInteractionBehaviour.OnFirstGrabEntered(InteractionEventArgs eventArgs)
        {
            if (ShouldPerformBehaviour(eventArgs))
            {
                OnFirstGrabEntered(eventArgs);
            }
        }

        /// <inheritdoc cref="IInteractionBehaviour.OnFirstGrabEntered"/>
        protected virtual void OnFirstGrabEntered(InteractionEventArgs eventArgs) { }

        /// <inheritdoc/>
        void IInteractionBehaviour.OnGrabEntered(InteractionEventArgs eventArgs)
        {
            if (ShouldPerformBehaviour(eventArgs))
            {
                OnGrabEntered(eventArgs);
            }
        }

        /// <inheritdoc cref="IInteractionBehaviour.OnGrabEntered"/>
        protected virtual void OnGrabEntered(InteractionEventArgs eventArgs) { }

        /// <inheritdoc/>
        void IInteractionBehaviour.OnGrabExited(InteractionExitEventArgs eventArgs)
        {
            if (ShouldPerformBehaviour(eventArgs))
            {
                OnGrabExited(eventArgs);
            }
        }

        /// <inheritdoc cref="IInteractionBehaviour.OnGrabExited"/>
        protected virtual void OnGrabExited(InteractionExitEventArgs eventArgs) { }

        /// <inheritdoc/>
        void IInteractionBehaviour.OnLastGrabExited(InteractionExitEventArgs eventArgs)
        {
            if (ShouldPerformBehaviour(eventArgs))
            {
                OnLastGrabExited(eventArgs);
            }
        }

        /// <inheritdoc cref="IInteractionBehaviour.OnLastGrabExited"/>
        protected virtual void OnLastGrabExited(InteractionExitEventArgs eventArgs) { }

        private bool ShouldPerformBehaviour(BaseInteractionEventArgs eventArgs)
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
