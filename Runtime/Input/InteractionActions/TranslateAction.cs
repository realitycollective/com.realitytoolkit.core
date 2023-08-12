// Copyright (c) Reality Collective. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using RealityToolkit.Input.Definitions;
using RealityToolkit.Input.Interactors;
using UnityEngine;

namespace RealityToolkit.Input.InteractionActions
{
    public class TranslateAction : BaseInteractionAction
    {
        private Vector3? previousInteractorPosition;
        private IControllerInteractor primaryInteractor;

        /// <inheritdoc/>
        protected override void Update()
        {
            base.Update();

            if (primaryInteractor == null)
            {
                return;
            }

            if (primaryInteractor is IDirectInteractor directInteractor)
            {
                UpdateDirect(directInteractor);
                return;
            }

            UpdateFar(primaryInteractor);
        }

        private void UpdateDirect(IDirectInteractor directInteractor)
        {
            var interactorPosition = directInteractor.GameObject.transform.position;
            var delta = interactorPosition - previousInteractorPosition.Value;
            transform.Translate(delta);
            previousInteractorPosition = interactorPosition;
        }

        private void UpdateFar(IControllerInteractor controllerInteractor)
        {
            var interactorPosition = controllerInteractor.Result.EndPoint;
            var delta = interactorPosition - previousInteractorPosition.Value;
            transform.Translate(delta);
            previousInteractorPosition = interactorPosition;
        }

        /// <inheritdoc/>
        public override void OnStateChanged(InteractionState state)
        {
            // This action only supports controller interactors.
            if (Interactable.PrimaryInteractor is not IControllerInteractor primaryInteractor)
            {
                this.primaryInteractor = null;
                return;
            }

            if (state == InteractionState.Selected)
            {
                this.primaryInteractor = primaryInteractor;

                if (primaryInteractor is IDirectInteractor directInteractor)
                {
                    previousInteractorPosition = directInteractor.GameObject.transform.position;
                    return;
                }

                previousInteractorPosition = primaryInteractor.Result.EndPoint;
            }
            else
            {
                this.primaryInteractor = null;
            }
        }
    }
}
