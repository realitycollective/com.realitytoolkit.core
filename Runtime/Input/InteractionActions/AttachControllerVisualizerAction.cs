// Copyright (c) Reality Collective. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using RealityCollective.Extensions;
using RealityToolkit.Input.Events;
using RealityToolkit.Input.Interactors;
using System.Collections.Generic;
using UnityEngine;

namespace RealityToolkit.Input.InteractionActions
{
    /// <summary>
    /// Attaches the <see cref="IControllerInteractor"/>'s <see cref="Controllers.IControllerVisualizer"/>
    /// to the <see cref="Interactables.IInteractable"/>.
    /// </summary>
    public class AttachControllerVisualizerAction : BaseInteractionAction
    {
        [SerializeField, Tooltip("Optional local offset from the object's pivot.")]
        private Vector3 poseLocalPositionOffset = Vector3.zero;

        [SerializeField, Tooltip("Optional local offset from the object's pivot.")]
        private Vector3 poseLocalRotationOffset = Vector3.zero;

        private readonly List<IControllerInteractor> interactors = new List<IControllerInteractor>();

        /// <inheritdoc/>
        protected override void Update()
        {
            for (int i = 0; i < interactors.Count; i++)
            {
                var interactor = interactors[i];
                var position = GetGrabPosition(interactor);
                var rotation = GetGrabRotation(interactor);

                interactor.Controller.Visualizer.PoseDriver.SetPositionAndRotation(position, rotation);
            }
        }

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

        private void Attach(IControllerInteractor currentInteractor)
        {
            interactors.EnsureListItem(currentInteractor);
            currentInteractor.Controller.Visualizer.VisualizerPoseOverrideSource = transform;
        }

        private void Detach(IControllerInteractor currentInteractor)
        {
            interactors.SafeRemoveListItem(currentInteractor);
            currentInteractor.Controller.Visualizer.VisualizerPoseOverrideSource = null;
        }

        private Vector3 GetGrabPosition(IControllerInteractor currentInteractor)
        {
            var interactablePosition = transform.position;
            var controllerGripPoseOffset = currentInteractor.Controller.Visualizer.GripPose.localPosition;

            return interactablePosition + controllerGripPoseOffset + poseLocalPositionOffset;
        }

        private Quaternion GetGrabRotation(IControllerInteractor currentInteractor)
        {
            var interactableRotation = transform.rotation;
            var controllerGripPoseOffset = currentInteractor.Controller.Visualizer.GripPose.localRotation;

            return interactableRotation * Quaternion.Euler(poseLocalRotationOffset) * controllerGripPoseOffset;
        }
    }
}
