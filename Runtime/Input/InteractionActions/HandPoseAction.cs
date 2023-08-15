// Copyright (c) Reality Collective. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using RealityToolkit.Input.Events;
using RealityToolkit.Input.Hands.Poses;
using RealityToolkit.Input.Hands.Visualizers;
using RealityToolkit.Input.Interactors;
using UnityEngine;

namespace RealityToolkit.Input.InteractionActions
{
    /// <summary>
    /// The <see cref="HandPoseAction"/> will animate the <see cref="RiggedHandControllerVisualizer"/>
    /// into the assigned <see cref="handPose"/>, when the <see cref="Interactables.IInteractable"/>
    /// is grabbed.
    [DisallowMultipleComponent]
    public class HandPoseAction : BaseInteractionAction
    {
        [SerializeField, Tooltip("Hand pose applied when grabbing the interactable.")]
        private HandPose handPose = null;

        private RiggedHandControllerVisualizer currentVisualizer;

        /// <inheritdoc/>
        public override void OnGrabEntered(InteractionEventArgs eventArgs)
        {
            if (eventArgs.Interactor is IControllerInteractor controllerInteractor &&
                controllerInteractor.Controller.Visualizer is RiggedHandControllerVisualizer riggedHandControllerVisualizer)
            {
                currentVisualizer = riggedHandControllerVisualizer;
                currentVisualizer.OverridePose = handPose;
            }
        }

        /// <inheritdoc/>
        public override void OnGrabExited(InteractionExitEventArgs eventArgs)
        {
            if (eventArgs.Interactor is IControllerInteractor controllerInteractor &&
                controllerInteractor.Controller.Visualizer is RiggedHandControllerVisualizer riggedHandControllerVisualizer &&
                currentVisualizer == riggedHandControllerVisualizer)
            {
                currentVisualizer.OverridePose = null;
                currentVisualizer = null;
            }
        }
    }
}
