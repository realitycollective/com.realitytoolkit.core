// Copyright (c) Reality Collective. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using RealityToolkit.Input.Events;
using RealityToolkit.Input.Hands.Poses;
using RealityToolkit.Input.Hands.Visualizers;
using RealityToolkit.Input.Interactors;
using UnityEngine;

namespace RealityToolkit.Input.InteractionBehaviours
{
    /// <summary>
    /// The <see cref="GrabHandPoseAction"/> will animate the <see cref="RiggedHandControllerVisualizer"/>
    /// into the assigned <see cref="grabPose"/>, when the <see cref="Interactables.IInteractable"/> is grabbed.
    public class GrabHandPoseAction : BaseInteractionBehaviour
    {
        [SerializeField, Tooltip("Hand pose applied when grabbing the interactable.")]
        private HandPose grabPose = null;

        /// <inheritdoc/>
        protected override void OnGrabEntered(InteractionEventArgs eventArgs)
        {
            if (eventArgs.Interactor is IDirectInteractor directInteractor &&
                directInteractor.Controller.Visualizer is RiggedHandControllerVisualizer riggedHandControllerVisualizer)
            {
                riggedHandControllerVisualizer.OverridePose = grabPose;
            }
        }

        /// <inheritdoc/>
        protected override void OnGrabExited(InteractionExitEventArgs eventArgs)
        {
            if (eventArgs.Interactor is IDirectInteractor directInteractor &&
                directInteractor.Controller.Visualizer is RiggedHandControllerVisualizer riggedHandControllerVisualizer)
            {
                riggedHandControllerVisualizer.OverridePose = null;
            }
        }
    }
}
