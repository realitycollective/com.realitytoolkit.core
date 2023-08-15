// Copyright (c) Reality Collective. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using RealityToolkit.Input.Events;
using RealityToolkit.Input.Interactors;
using UnityEngine;

namespace RealityToolkit.Input.InteractionActions
{
    /// <summary>
    /// The <see cref="GrabAction"/> is an <see cref="IInteractionAction"/> for use with
    /// <see cref="Interactors.IDirectInteractor"/>s. It allows to "pick up" the <see cref="Interactables.IInteractable"/>
    /// and carry it around.
    /// </summary>
    [DisallowMultipleComponent]
    public class GrabAction : BaseInteractionAction
    {
        private IDirectInteractor grabbingInteractor;

        /// <inheritdoc/>
        protected override void Update()
        {
            if (grabbingInteractor != null)
            {
                transform.position = grabbingInteractor.Controller.Visualizer.GripPose.transform.position;
            }
        }

        /// <inheritdoc/>
        public override void OnFirstGrabEntered(InteractionEventArgs eventArgs)
        {
            if (eventArgs.Interactor is IDirectInteractor directInteractor)
            {
                grabbingInteractor = directInteractor;
                var initialDraggingPosition = directInteractor.Controller.Visualizer.GripPose.position;
                transform.position = initialDraggingPosition;
            }
        }

        /// <inheritdoc/>
        public override void OnGrabExited(InteractionExitEventArgs eventArgs)
        {
            if (eventArgs.Interactor == grabbingInteractor)
            {
                grabbingInteractor = null;
            }
        }
    }
}
