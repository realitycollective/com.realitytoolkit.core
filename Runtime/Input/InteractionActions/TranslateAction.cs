// Copyright (c) Reality Collective. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using RealityToolkit.Input.Definitions;
using RealityToolkit.Input.Interactors;
using UnityEngine;

namespace RealityToolkit.Input.Interactions.Actions
{
    public class TranslateAction : BaseInteractionAction
    {
        private Vector3? previousInteractorPosition;
        private IControllerInteractor primaryInteractor;

        /// <inheritdoc/>
        protected override void Update()
        {
            base.Update();
            //var interactorPosition = primaryInteractor.InputSource.InteractingPointer.Result.EndPoint;
            //var delta = interactorPosition - previousInteractorPosition.Value;
            //transform.Translate(delta);
            //previousInteractorPosition = interactorPosition;
        }

        /// <inheritdoc/>
        public override void OnStateChanged(InteractionState state)
        {
            // This action only supports controller interactors.
            if (Interactable.PrimaryInteractor == null ||
                Interactable.PrimaryInteractor is not IControllerInteractor primaryInteractor)
            {
                return;
            }

            if (state == InteractionState.Selected)
            {
                this.primaryInteractor = primaryInteractor;
                //previousInteractorPosition = primaryInteractor.InputSource.InteractingPointer.Result.EndPoint;
            }
        }
    }
}
