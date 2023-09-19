// Copyright (c) Reality Collective. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using RealityCollective.Extensions;
using RealityToolkit.Input.Events;
using RealityToolkit.Input.Interactors;
using System.Collections.Generic;
using UnityEngine;

namespace RealityToolkit.Input.InteractionBehaviours
{
    /// <summary>
    /// Attaches the <see cref="IControllerInteractor"/>'s <see cref="Controllers.IControllerVisualizer"/>
    /// to the <see cref="Interactables.IInteractable"/> pose.
    /// </summary>
    public class AttachControllerVisualizerAction : BaseInteractionBehaviour
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
                var pose = GetGrabPose(interactor);

                interactor.Controller.Visualizer.PoseDriver.SetPositionAndRotation(pose.position, pose.rotation);
            }
        }

        /// <inheritdoc/>
        protected override void OnSelectEntered(InteractionEventArgs eventArgs)
        {
            if (eventArgs.Interactor is not IControllerInteractor controllerInteractor)
            {
                return;
            }

            Attach(controllerInteractor);
        }

        /// <inheritdoc/>
        protected override void OnSelectExited(InteractionExitEventArgs eventArgs)
        {
            if (eventArgs.Interactor is not IControllerInteractor controllerInteractor)
            {
                return;
            }

            Detach(controllerInteractor);
        }

        /// <inheritdoc/>
        protected override void OnGrabEntered(InteractionEventArgs eventArgs)
        {
            if (eventArgs.Interactor is not IControllerInteractor controllerInteractor)
            {
                return;
            }

            Attach(controllerInteractor);
        }

        /// <inheritdoc/>
        protected override void OnGrabExited(InteractionExitEventArgs eventArgs)
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

        private Pose GetGrabPose(IControllerInteractor controllerInteractor)
        {
            return new Pose(GetGrabPosition(controllerInteractor), GetGrabRotation(controllerInteractor));
        }

        private Vector3 GetGrabPosition(IControllerInteractor currentInteractor)
        {
            // Move the controller to the interactable's attachment point.
            var worldAttachmentPosition = transform.TransformPoint(poseLocalPositionOffset);

            // Adjust controller's position based on its own grab pose offset.
            var localControllerOffset = currentInteractor.Controller.Visualizer.PoseDriver.TransformPoint(currentInteractor.Controller.Visualizer.GripPose.localPosition) -
                currentInteractor.Controller.Visualizer.PoseDriver.position;

            // Combine.
            worldAttachmentPosition += localControllerOffset;

            return worldAttachmentPosition;
        }

        private Quaternion GetGrabRotation(IControllerInteractor currentInteractor)
        {
            var worldAttachmentRotation = Quaternion.Euler(poseLocalRotationOffset);
            var localControllerOffset = currentInteractor.Controller.Visualizer.GripPose.localRotation;
            return transform.rotation * worldAttachmentRotation * localControllerOffset;
        }
    }
}
