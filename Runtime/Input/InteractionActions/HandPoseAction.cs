// Copyright (c) Reality Collective. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using RealityToolkit.Input.Definitions;
using RealityToolkit.Input.Hands.Poses;
using RealityToolkit.Input.Hands.Visualizers;
using RealityToolkit.Input.Interactors;
using UnityEngine;

namespace RealityToolkit.Input.InteractionActions
{
    public class HandPoseAction : BaseInteractionAction
    {
        [SerializeField, Tooltip("Hand pose applied when grabbing the interactable.")]
        private HandPose handPose = null;

        /// <inheritdoc/>
        public override void OnStateChanged(InteractionState state)
        {
            if (Interactable.PrimaryInteractor is IControllerInteractor controllerInteractor &&
                controllerInteractor.Controller.Visualizer is RiggedHandControllerVisualizer riggedHandControllerVisualizer)
            {
                if (state == InteractionState.Selected)
                {
                    riggedHandControllerVisualizer.OverridePose = handPose;
                    return;
                }

                riggedHandControllerVisualizer.OverridePose = null;
            }
        }
    }
}