// Copyright (c) Reality Collective. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using RealityCollective.Extensions;
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

        private RiggedHandControllerVisualizer currentRiggedHandControllerVisualizer;

        /// <inheritdoc/>
        public override void OnStateChanged(InteractionState state)
        {
            if (state == InteractionState.Selected &&
                Interactable.PrimaryInteractor is IControllerInteractor controllerInteractor &&
                controllerInteractor.Controller.Visualizer is RiggedHandControllerVisualizer riggedHandControllerVisualizer)
            {
                currentRiggedHandControllerVisualizer = riggedHandControllerVisualizer;
                currentRiggedHandControllerVisualizer.OverridePose = handPose;
            }
            else if (currentRiggedHandControllerVisualizer.IsNotNull())
            {
                currentRiggedHandControllerVisualizer.OverridePose = null;
                currentRiggedHandControllerVisualizer = null;
            }
        }
    }
}
