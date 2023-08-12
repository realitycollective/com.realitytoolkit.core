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
using UnityEngine.Events;

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
        IInputHandler
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
        [SerializeField]
        [Tooltip("Event raised whenever the interactable's state has changed.")]
        private UnityEvent<InteractionState> stateChanged = null;

        private InteractionState currentState;
        private readonly Dictionary<uint, IInteractor> focusingInteractors = new Dictionary<uint, IInteractor>();
        private readonly Dictionary<uint, IInteractor> selectingInteractors = new Dictionary<uint, IInteractor>();
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
        public bool NearInteractionEnabled => InputService.NearInteractionEnabled && nearInteraction;

        /// <inheritdoc/>
        public bool FarInteractionEnabled => InputService.FarInteractionEnabled && farInteraction;

        /// <inheritdoc/>
        public InteractionState State
        {
            get => currentState;
            private set
            {
                currentState = value;
                stateChanged?.Invoke(currentState);
                UpdateActions();
            }
        }

        /// <inheritdoc/>
        public IInteractor PrimaryInteractor => selectingInteractors.Values.FirstOrDefault();

        /// <inheritdoc/>
        public IReadOnlyList<IInteractor> Interactors => selectingInteractors.Values.ToList();

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
            State = InteractionState.Normal;
        }

        /// <summary>
        /// Updates all <see cref="IInteractionAction"/>s on the <see cref="IInteractable"/>.
        /// </summary>
        private void UpdateActions()
        {
            for (var i = 0; i < actions.Count; i++)
            {
                actions[i].OnStateChanged(currentState);
            }
        }

        /// <summary>
        /// The <see cref="IInteractable"/> is focused by <paramref name="interactor"/>.
        /// </summary>
        /// <param name="interactor">The <see cref="IInteractor"/> focusing the object.</param>
        protected virtual void OnFocused(IInteractor interactor)
        {
            focusingInteractors.EnsureDictionaryItem(interactor.InputSource.SourceId, interactor, true);
            if (State != InteractionState.Selected)
            {
                State = InteractionState.Focused;
            }
        }

        /// <summary>
        /// The <see cref="IInteractable"/> was unfocused by <paramref name="interactor"/>.
        /// </summary>
        /// <param name="interactor">The <see cref="IInteractor"/> that unfocused the object.</param>
        protected virtual void OnUnfocused(IInteractor interactor)
        {
            if (focusingInteractors.TrySafeRemoveDictionaryItem(interactor.InputSource.SourceId) &&
                focusingInteractors.Count == 0 &&
                State == InteractionState.Focused)
            {
                State = InteractionState.Normal;
            }

            if (selectingInteractors.TryGetValue(interactor.InputSource.SourceId, out _))
            {
                // If an interactor that was interacting with the object has lost focus to it,
                // then that is the same as ending the interaction.
                OnDeselected(interactor);
            }
        }

        /// <summary>
        /// The <see cref="IInteractable"/> is now selected by <paramref name="interactor"/>.
        /// </summary>
        protected virtual void OnSelected(IInteractor interactor)
        {
            selectingInteractors.EnsureDictionaryItem(interactor.InputSource.SourceId, interactor, true);
            State = InteractionState.Selected;
        }

        /// <summary>
        /// The <see cref="IInteractable"/> is no longer selected by <paramref name="interactor"/>.
        /// </summary>
        protected virtual void OnDeselected(IInteractor interactor)
        {
            selectingInteractors.TrySafeRemoveDictionaryItem(interactor.InputSource.SourceId);
            if (selectingInteractors.Count > 0)
            {
                return;
            }

            State = focusingInteractors.Count == 0 ? InteractionState.Normal : InteractionState.Focused;
        }

        /// <inheritdoc/>
        public void Add(IInteractionAction action)
        {
            actions.EnsureListItem(action);
            actions = actions.OrderBy(a => a.SortingOrder).ToList();
        }

        /// <inheritdoc/>
        public void Remove(IInteractionAction action) => actions.SafeRemoveListItem(action);

        /// <inheritdoc/>
        public void OnFocusEnter(FocusEventData eventData) => OnFocused(eventData.Pointer);

        /// <inheritdoc/>
        public void OnFocusExit(FocusEventData eventData) => OnUnfocused(eventData.Pointer);

        /// <inheritdoc/>
        public void OnInputDown(InputEventData eventData)
        {
            if (eventData.InputAction == selectAction)
            {

            }
        }

        /// <inheritdoc/>
        public void OnInputUp(InputEventData eventData)
        {
            if (eventData.InputAction == selectAction)
            {

            }
        }
    }
}
