// Copyright (c) Reality Collective. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using RealityToolkit.Input.Events;
using RealityToolkit.Input.Interactors;

namespace RealityToolkit.Input.InteractionActions
{
    /// <summary>
    /// Attaches the <see cref="IControllerInteractor"/>'s <see cref="Controllers.IControllerVisualizer"/>
    /// to the <see cref="Interactables.IInteractable"/>.
    /// </summary>
    public class AttachControllerVisualizerAction : BaseInteractionAction
    {
        /// <inheritdoc/>
        public override void OnSelectEntered(InteractionEventArgs eventArgs)
        {
            if (eventArgs.Interactor is not IControllerInteractor controllerInteractor)
            {
                return;
            }

            Attach(controllerInteractor);
        }

        /// <inheritdoc/>
        public override void OnSelectExited(InteractionExitEventArgs eventArgs)
        {
            if (eventArgs.Interactor is not IControllerInteractor controllerInteractor)
            {
                return;
            }

            Detach(controllerInteractor);
        }

        /// <inheritdoc/>
        public override void OnGrabEntered(InteractionEventArgs eventArgs)
        {
            if (eventArgs.Interactor is not IControllerInteractor controllerInteractor)
            {
                return;
            }

            Attach(controllerInteractor);
        }

        /// <inheritdoc/>
        public override void OnGrabExited(InteractionExitEventArgs eventArgs)
        {
            if (eventArgs.Interactor is not IControllerInteractor controllerInteractor)
            {
                return;
            }

            Detach(controllerInteractor);
        }

        private void Attach(IControllerInteractor currentInteractor) => currentInteractor.Controller.Visualizer.VisualizerPoseOverrideSource = transform;

        private void Detach(IControllerInteractor currentInteractor) => currentInteractor.Controller.Visualizer.VisualizerPoseOverrideSource = null;
    }
}
