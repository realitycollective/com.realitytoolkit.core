// Copyright (c) Reality Collective. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using RealityToolkit.Input.Hands.Poses;
using RealityToolkit.Input.Hands.Visualizers;
using RealityToolkit.Input.Interactions.Interactors;
using UnityEngine;

namespace RealityToolkit.Input.Interactions.Actions
{
    public class HandPoseAction : BaseInteractionAction
    {
        [SerializeField, Tooltip("Hand pose applied when grabbing the interactable.")]
        private HandPose handPose = null;

        /// <inheritdoc/>
        public override void OnStateChanged(InteractionState state)
        {
            if (Interactable.PrimaryInteractor == null ||
                Interactable.PrimaryInteractor is not IControllerInteractor controllerInteractor ||
                controllerInteractor.Controller.Visualizer is not RiggedHandControllerVisualizer riggedHandControllerVisualizer)
            {
                return;
            }

            riggedHandControllerVisualizer.OverridePose = handPose;
        }
    }
}
