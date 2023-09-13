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
    /// The <see cref="FocusHandPoseAction"/> will animate the <see cref="RiggedHandControllerVisualizer"/>
    /// into the assigned <see cref="focusPose"/>, when the <see cref="Interactables.IInteractable"/> is focused.
    public class FocusHandPoseAction : BaseInteractionAction
    {
        [SerializeField, Tooltip("Hand pose applied when focusing the interactable.")]
        private HandPose focusPose = null;

        /// <inheritdoc/>
        protected override void OnFirstFocusEntered(InteractionEventArgs eventArgs)
        {
            if (Interactable.IsSelected || Interactable.IsGrabbed)
            {
                return;
            }

            if (eventArgs.Interactor is IDirectInteractor directInteractor &&
               directInteractor.Controller.Visualizer is RiggedHandControllerVisualizer riggedHandControllerVisualizer)
            {
                riggedHandControllerVisualizer.OverridePose = focusPose;
            }
        }

        /// <inheritdoc/>
        protected override void OnLastFocusExited(InteractionExitEventArgs eventArgs)
        {
            if (Interactable.IsSelected || Interactable.IsGrabbed)
            {
                return;
            }

            if (eventArgs.Interactor is IDirectInteractor directInteractor &&
                directInteractor.Controller.Visualizer is RiggedHandControllerVisualizer riggedHandControllerVisualizer)
            {
                riggedHandControllerVisualizer.OverridePose = null;
            }
        }
    }
}
