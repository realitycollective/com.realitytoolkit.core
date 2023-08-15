// Copyright (c) Reality Collective. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using RealityCollective.Extensions;
using RealityCollective.ServiceFramework.Services;
using RealityToolkit.EventDatum.Input;
using RealityToolkit.Input.Definitions;
using RealityToolkit.Input.InteractionActions;
using RealityToolkit.Input.Interactors;
using RealityToolkit.Input.Interfaces;
using RealityToolkit.Input.Interfaces.Handlers;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace RealityToolkit.Input.Interactables
{
    /// <summary>
    /// An <see cref="Interactable"/> marks an object that can be interacted with.
    /// </summary>
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(Collider))]
    public class Interactable : MonoBehaviour,
        IInteractable,
        IFocusHandler,
        IPointerHandler,
        ISourceStateHandler
    {
        [SerializeField]
        [Tooltip("Optional label that may be used to identify the interactable or categorize it.")]
        private string label = null;

        [Space]
        [SerializeField]
        [Tooltip("Should near interaction be enabled at startup?")]
        private bool nearInteraction = true;

        [SerializeField]
        [Tooltip("Should far interaction be enabled at startup?")]
        private bool farInteraction = true;

        [Space]
        [SerializeField]
        [Tooltip("The action that will select this interactable, if focused by an interactor.")]
        protected InputAction selectAction = InputAction.None;

        [SerializeField]
        [Tooltip("The action that will grab this interactable, if focused by an interactor.")]
        protected InputAction grabAction = InputAction.None;

        [Space]
        [SerializeField, Tooltip("The focus mode for this interactable.")]
        private InteractableFocusMode focusMode = InteractableFocusMode.Single;

        private readonly HashSet<IInteractor> focusingInteractors = new HashSet<IInteractor>();
        private readonly HashSet<IInteractor> selectingInteractors = new HashSet<IInteractor>();
        private readonly HashSet<IInteractor> grabbingInteractors = new HashSet<IInteractor>();
        private List<IInteractionAction> actions = new List<IInteractionAction>();

        private IInputService inputService = null;
        /// <summary>
        /// The active <see cref="IInputService"/>.
        /// </summary>
        protected IInputService InputService
            => inputService ?? (inputService = ServiceManager.Instance.GetService<IInputService>());

        /// <inheritdoc/>
        public string Label
        {
            get => label;
            set => label = value;
        }

        /// <inheritdoc/>
        public bool IsValid => isActiveAndEnabled && (NearInteractionEnabled || FarInteractionEnabled);

        /// <inheritdoc/>
        public bool IsFocused => focusingInteractors.Count > 0;

        /// <inheritdoc/>
        public bool IsSelected => selectingInteractors.Count > 0;

        /// <inheritdoc/>
        public bool IsGrabbed => grabbingInteractors.Count > 0;

        /// <inheritdoc/>
        public InteractableFocusMode FocusMode => focusMode;

        /// <inheritdoc/>
        public bool NearInteractionEnabled => InputService.NearInteractionEnabled && nearInteraction;

        /// <inheritdoc/>
        public bool FarInteractionEnabled => InputService.FarInteractionEnabled && farInteraction;

        /// <summary>
        /// See <see cref="MonoBehaviour"/>.
        /// </summary>
        protected virtual async void Awake()
        {
            await ServiceManager.WaitUntilInitializedAsync();

            if (this.IsNull())
            {
                // We've been destroyed during the await.
                return;
            }

            InputService.Add(this);
        }

        /// <summary>
        /// See <see cref="MonoBehaviour"/>.
        /// </summary>
        protected virtual void OnEnable()
        {
            OnReset();
        }

        /// <summary>
        /// See <see cref="MonoBehaviour"/>.
        /// </summary>
        protected virtual void OnDestroy()
        {
            if (InputService == null)
            {
                return;
            }

            InputService.Remove(this);
        }

        /// <summary>
        /// The <see cref="Interactable"/> interaction state was reset.
        /// </summary>
        private void OnReset()
        {
            focusingInteractors.Clear();
            selectingInteractors.Clear();
        }

        /// <summary>
        /// The <see cref="IInteractable"/> is focused by <paramref name="interactor"/>.
        /// </summary>
        /// <param name="interactor">The <see cref="IInteractor"/> focusing the object.</param>
        protected virtual void OnFocused(IInteractor interactor)
        {
            if (FocusMode == InteractableFocusMode.None ||
                !IsValidInteractor(interactor))
            {
                return;
            }

            var isFirst = focusingInteractors.Count == 0;

            if (!isFirst && FocusMode == InteractableFocusMode.Single)
            {
                OnUnfocused(focusingInteractors.First());
                isFirst = true;
            }

            if (!focusingInteractors.Add(interactor))
            {
                return;
            }

            for (var i = 0; i < actions.Count; i++)
            {
                var action = actions[i];
                var eventArgs = new Events.InteractionEventArgs
                {
                    Interactable = this,
                    Interactor = interactor
                };

                if (isFirst)
                {
                    action.OnFirstFocusEntered(eventArgs);
                }

                action.OnFocusEntered(eventArgs);
            }
        }

        /// <summary>
        /// The <see cref="IInteractable"/> was unfocused by <paramref name="interactor"/>.
        /// </summary>
        /// <param name="interactor">The <see cref="IInteractor"/> that unfocused the object.</param>
        /// <param name="isCanceled">Was the focus canceled? Defaults to <c>false</c>.</param>
        protected virtual void OnUnfocused(IInteractor interactor, bool isCanceled = false)
        {
            if (FocusMode == InteractableFocusMode.None ||
                !IsValidInteractor(interactor))
            {
                return;
            }

            var isLast = focusingInteractors.Count == 1;

            if (!focusingInteractors.Remove(interactor))
            {
                return;
            }

            for (var i = 0; i < actions.Count; i++)
            {
                var action = actions[i];
                var eventArgs = new Events.InteractionExitEventArgs
                {
                    Interactable = this,
                    Interactor = interactor,
                    IsCanceled = isCanceled
                };

                if (isLast)
                {
                    action.OnLastFocusExited(eventArgs);
                }

                action.OnFocusExited(eventArgs);
            }
        }

        /// <summary>
        /// The <see cref="IInteractable"/> is now selected by <paramref name="interactor"/>.
        /// </summary>
        protected virtual void OnSelected(IInteractor interactor)
        {
            if (!IsValidInteractor(interactor))
            {
                return;
            }

            var isFirst = selectingInteractors.Count == 0;

            if (!selectingInteractors.Add(interactor))
            {
                return;
            }

            for (var i = 0; i < actions.Count; i++)
            {
                var action = actions[i];
                var eventArgs = new Events.InteractionEventArgs
                {
                    Interactable = this,
                    Interactor = interactor
                };

                if (isFirst)
                {
                    action.OnFirstSelectEntered(eventArgs);
                }

                action.OnSelectEntered(eventArgs);
            }
        }

        /// <summary>
        /// The <see cref="IInteractable"/> is no longer selected by <paramref name="interactor"/>.
        /// </summary>
        /// <param name="interactor">The <see cref="IInteractor"/> that stopped selecting the object.</param>
        /// <param name="isCanceled">Was the selection canceled? Defaults to <c>false</c>.</param>
        protected virtual void OnDeselected(IInteractor interactor, bool isCanceled = false)
        {
            if (!IsValidInteractor(interactor))
            {
                return;
            }

            var isLast = selectingInteractors.Count == 1;

            if (!selectingInteractors.Remove(interactor))
            {
                return;
            }

            for (var i = 0; i < actions.Count; i++)
            {
                var action = actions[i];
                var eventArgs = new Events.InteractionExitEventArgs
                {
                    Interactable = this,
                    Interactor = interactor,
                    IsCanceled = isCanceled
                };

                if (isLast)
                {
                    action.OnLastSelectExited(eventArgs);
                }

                action.OnSelectExited(eventArgs);
            }
        }

        /// <summary>
        /// The <see cref="IInteractable"/> is now grabbed by <paramref name="interactor"/>.
        /// </summary>
        protected virtual void OnGrabbed(IInteractor interactor)
        {
            if (!IsValidInteractor(interactor))
            {
                return;
            }

            var isFirst = grabbingInteractors.Count == 0;

            if (!grabbingInteractors.Add(interactor))
            {
                return;
            }

            for (var i = 0; i < actions.Count; i++)
            {
                var action = actions[i];
                var eventArgs = new Events.InteractionEventArgs
                {
                    Interactable = this,
                    Interactor = interactor
                };

                if (isFirst)
                {
                    action.OnFirstGrabEntered(eventArgs);
                }

                action.OnGrabEntered(eventArgs);
            }
        }

        /// <summary>
        /// The <see cref="IInteractable"/> is no longer grabbed by <paramref name="interactor"/>.
        /// </summary>
        /// <param name="interactor">The <see cref="IInteractor"/> that stopped grabbing the object.</param>
        /// <param name="isCanceled">Was the grab canceled? Defaults to <c>false</c>.</param>
        protected virtual void OnDropped(IInteractor interactor, bool isCanceled = false)
        {
            if (!IsValidInteractor(interactor))
            {
                return;
            }

            var isLast = grabbingInteractors.Count == 1;

            if (!grabbingInteractors.Remove(interactor))
            {
                return;
            }

            for (var i = 0; i < actions.Count; i++)
            {
                var action = actions[i];
                var eventArgs = new Events.InteractionExitEventArgs
                {
                    Interactable = this,
                    Interactor = interactor,
                    IsCanceled = isCanceled
                };

                if (isLast)
                {
                    action.OnLastGrabExited(eventArgs);
                }

                action.OnGrabExited(eventArgs);
            }
        }

        /// <inheritdoc/>
        public void Add(IInteractionAction action)
        {
            actions.EnsureListItem(action);
            actions = actions.OrderBy(a => a.SortingOrder).ToList();
        }

        /// <inheritdoc/>
        public void Remove(IInteractionAction action) => actions.SafeRemoveListItem(action);

        private bool IsValidInteractor(IInteractor interactor) =>
            (interactor.IsFarInteractor && FarInteractionEnabled) || (!interactor.IsFarInteractor && NearInteractionEnabled);

        #region IFocusHandler

        /// <inheritdoc/>
        public void OnFocusEnter(FocusEventData eventData)
        {
            OnFocused(eventData.Pointer);
            eventData.Use();
        }

        /// <inheritdoc/>
        public void OnFocusExit(FocusEventData eventData)
        {
            OnUnfocused(eventData.Pointer);
            eventData.Use();
        }

        #endregion

        #region IPointerHandler

        /// <inheritdoc/>
        public void OnPointerDown(PointerEventData eventData)
        {
            if (eventData.InputAction == selectAction)
            {
                OnSelected(eventData.Pointer);
                eventData.Use();
            }
            else if (eventData.InputAction == grabAction)
            {
                OnGrabbed(eventData.Pointer);
                eventData.Use();
            }
        }

        /// <inheritdoc/>
        public void OnPointerUp(PointerEventData eventData)
        {
            if (eventData.InputAction == selectAction)
            {
                OnDeselected(eventData.Pointer);
                eventData.Use();
            }
            else if (eventData.InputAction == grabAction)
            {
                OnDropped(eventData.Pointer);
                eventData.Use();
            }
        }

        /// <inheritdoc/>
        public void OnPointerClicked(PointerEventData eventData) { }

        #endregion IPointerHandler

        #region ISourceStateHandler

        /// <inheritdoc/>
        public void OnSourceDetected(SourceStateEventData eventData) { }

        /// <inheritdoc/>
        public void OnSourceLost(SourceStateEventData eventData)
        {
            // When an input source is lost, we have to check if any
            // of our interactors is gone as well.
            foreach (var interactor in focusingInteractors)
            {
                if (interactor.InputSource.SourceId == eventData.SourceId)
                {
                    OnUnfocused(interactor, true);
                }
            }

            foreach (var interactor in selectingInteractors)
            {
                if (interactor.InputSource.SourceId == eventData.SourceId)
                {
                    OnDeselected(interactor, true);
                }
            }

            foreach (var interactor in grabbingInteractors)
            {
                if (interactor.InputSource.SourceId == eventData.SourceId)
                {
                    OnDropped(interactor, true);
                }
            }
        }

        #endregion
    }
}
