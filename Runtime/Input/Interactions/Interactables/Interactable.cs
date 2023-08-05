// Copyright (c) Reality Collective. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using RealityCollective.Extensions;
using RealityCollective.ServiceFramework.Services;
using RealityToolkit.Input.Interactions.Actions;
using RealityToolkit.Input.Interactions.Interactors;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace RealityToolkit.Input.Interactions.Interactables
{
    /// <summary>
    /// An <see cref="Interactable"/> marks an object that can be interacted with.
    /// </summary>
    public class Interactable : MonoBehaviour, IInteractable
    {
        [SerializeField]
        [Tooltip("Optional label that may be used to identify the interactable or categorize it.")]
        private string label = null;

        [SerializeField]
        [Tooltip("Should near interaction be enabled at startup?")]
        private bool nearInteraction = true;

        [SerializeField]
        [Tooltip("Should far interaction be enabled at startup?")]
        private bool farInteraction = true;

        [Space]
        [SerializeField]
        [Tooltip("Event raised whenever the interactable's state has changed.")]
        private UnityEvent<InteractionState> stateChanged = null;

        private InteractionState currentState;
        private IInteractionServiceModule interactionServiceModule;
        private readonly Dictionary<uint, IInteractor> focusingInteractors = new Dictionary<uint, IInteractor>();
        private readonly Dictionary<uint, IInteractor> selectingInteractors = new Dictionary<uint, IInteractor>();
        private readonly List<IInteractionAction> actions = new List<IInteractionAction>();

        /// <inheritdoc/>
        public string Label
        {
            get => label;
            set => label = value;
        }

        /// <inheritdoc/>
        public bool IsValid => isActiveAndEnabled && (NearInteractionEnabled || FarInteractionEnabled);

        /// <inheritdoc/>
        public bool NearInteractionEnabled => interactionServiceModule.NearInteractionEnabled && nearInteraction;

        /// <inheritdoc/>
        public bool FarInteractionEnabled => interactionServiceModule.FarInteractionEnabled && farInteraction;

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
        private async void Awake()
        {
            try
            {
                await ServiceManager.WaitUntilInitializedAsync();
                interactionServiceModule = await ServiceManager.Instance.GetServiceAsync<IInteractionServiceModule>();
            }
            catch (System.Exception)
            {
                Debug.LogError($"{nameof(Interactable)} requires the {nameof(IInteractionServiceModule)} to work.");
                this.Destroy();
                return;
            }

            if (this.IsNull())
            {
                // We've been destroyed during the await.
                return;
            }

            interactionServiceModule.Add(this);
        }

        /// <summary>
        /// See <see cref="MonoBehaviour"/>.
        /// </summary>
        private void OnEnable()
        {
            OnReset();
        }

        /// <summary>
        /// See <see cref="MonoBehaviour"/>.
        /// </summary>
        private void OnDestroy()
        {
            if (interactionServiceModule == null)
            {
                return;
            }

            interactionServiceModule.Remove(this);
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

        /// <inheritdoc/>
        public virtual void OnFocused(IInteractor interactor)
        {
            focusingInteractors.EnsureDictionaryItem(interactor.InputSource.SourceId, interactor, true);
            if (State != InteractionState.Selected)
            {
                State = InteractionState.Focused;
            }
        }

        /// <inheritdoc/>
        public virtual void OnUnfocused(IInteractor interactor)
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

        /// <inheritdoc/>
        public virtual void OnSelected(IInteractor interactor)
        {
            selectingInteractors.EnsureDictionaryItem(interactor.InputSource.SourceId, interactor, true);
            State = InteractionState.Selected;
        }

        /// <inheritdoc/>
        public virtual void OnDeselected(IInteractor interactor)
        {
            selectingInteractors.TrySafeRemoveDictionaryItem(interactor.InputSource.SourceId);
            if (selectingInteractors.Count > 0)
            {
                return;
            }

            State = focusingInteractors.Count == 0 ? InteractionState.Normal : InteractionState.Focused;
        }

        /// <inheritdoc/>
        public void Add(IInteractionAction action) => actions.EnsureListItem(action);

        /// <inheritdoc/>
        public void Remove(IInteractionAction action) => actions.SafeRemoveListItem(action);
    }
}
