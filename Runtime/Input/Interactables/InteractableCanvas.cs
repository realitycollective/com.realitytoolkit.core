// Copyright (c) Reality Collective. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using RealityCollective.Extensions;
using RealityCollective.ServiceFramework.Services;
using RealityToolkit.Input.Interactors;
using RealityToolkit.Input.Interfaces;
using UnityEngine;

namespace RealityToolkit.Input.Interactables
{
    /// <summary>
    /// A <see cref="Canvas"/> that can be interacted with by <see cref="Interactors.IInteractor"/>s.
    /// </summary>
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Canvas))]
    public class InteractableCanvas : Interactable
    {
        [SerializeField, Tooltip("The canvas component.")]
        private Canvas canvas = null;

        /// <summary>
        /// <see cref="MonoBehaviour"/>.
        /// </summary>
        private void OnValidate()
        {
            if (canvas.IsNull())
            {
                canvas = GetComponent<Canvas>();
            }
        }

        /// <inheritdoc/>
        protected override void OnEnable()
        {
            if (canvas.IsNull())
            {
                canvas = GetComponent<Canvas>();
            }

            Debug.Assert(canvas.IsNotNull(), $"The {nameof(InteractableCanvas)} requires a {nameof(Canvas)} component on the game object.");

            if (ServiceManager.IsActiveAndInitialized &&
                ServiceManager.Instance.TryGetService<IInputService>(out var inputService) &&
                canvas.isRootCanvas && canvas.renderMode == RenderMode.WorldSpace)
            {
                canvas.worldCamera = inputService.FocusProvider.UIRaycastCamera;
            }
        }

        /// <inheritdoc/>
        protected override void OnFocused(IInteractor interactor)
        {
            // Override base behaviour and do nothing.
            // At this time we do not want this type of interactable handle these state changes.
        }

        /// <inheritdoc/>
        protected override void OnUnfocused(IInteractor interactor)
        {
            // Override base behaviour and do nothing.
            // At this time we do not want this type of interactable handle these state changes.
        }

        /// <inheritdoc/>
        public override void OnSelected(IInteractor interactor)
        {
            // Override base behaviour and do nothing.
            // At this time we do not want this type of interactable handle these state changes.
        }

        /// <inheritdoc/>
        public override void OnDeselected(IInteractor interactor)
        {
            // Override base behaviour and do nothing.
            // At this time we do not want this type of interactable handle these state changes.
        }
    }
}
