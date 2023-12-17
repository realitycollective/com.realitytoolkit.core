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
    /// to the <see cref="Interactables.IInteractable"/> pose and keeps them in sync until input is released.
    /// </summary>
    /// <remarks>
    /// Only supports <see cref="IControllerInteractor"/>s.
    /// Does not support <see cref="IPokeInteractor"/>s and will ignore them.
    /// </remarks>
    public class AttachControllerVisualizerBehaviour : BaseInteractionBehaviour
    {
        [SerializeField, Tooltip("Optional local offset from the object's pivot.")]
        private Vector3 poseLocalPositionOffset = Vector3.zero;

        [SerializeField, Tooltip("Optional local offset from the object's pivot.")]
        private Vector3 poseLocalRotationOffset = Vector3.zero;

        private readonly List<IControllerInteractor> attachedInteractors = new List<IControllerInteractor>();

        /// <inheritdoc/>
        protected override void Update()
        {
            for (int i = 0; i < attachedInteractors.Count; i++)
            {
                var interactor = attachedInteractors[i];
                var pose = GetGrabPose(interactor);

                interactor.Controller.Visualizer.PoseDriver.SetPositionAndRotation(pose.position, pose.rotation);
            }
        }

        /// <inheritdoc/>
        protected override void OnSelectEntered(InteractionEventArgs eventArgs)
        {
            if (eventArgs.Interactor is not IControllerInteractor controllerInteractor ||
                eventArgs.Interactor is IPokeInteractor)
            {
                return;
            }

            AttachVisualizer(controllerInteractor);
        }

        /// <inheritdoc/>
        protected override void OnSelectExited(InteractionExitEventArgs eventArgs)
        {
            if (eventArgs.Interactor is not IControllerInteractor controllerInteractor ||
                eventArgs.Interactor is IPokeInteractor)
            {
                return;
            }

            DetachVisualizer(controllerInteractor);
        }

        /// <inheritdoc/>
        protected override void OnGrabEntered(InteractionEventArgs eventArgs)
        {
            if (eventArgs.Interactor is not IControllerInteractor controllerInteractor ||
                eventArgs.Interactor is IPokeInteractor)
            {
                return;
            }

            AttachVisualizer(controllerInteractor);
        }

        /// <inheritdoc/>
        protected override void OnGrabExited(InteractionExitEventArgs eventArgs)
        {
            if (eventArgs.Interactor is not IControllerInteractor controllerInteractor ||
                eventArgs.Interactor is IPokeInteractor)
            {
                return;
            }

            DetachVisualizer(controllerInteractor);
        }

        private void AttachVisualizer(IControllerInteractor currentInteractor)
        {
            attachedInteractors.EnsureListItem(currentInteractor);
            currentInteractor.Controller.Visualizer.VisualizerPoseOverrideSource = transform;
        }

        private void DetachVisualizer(IControllerInteractor currentInteractor)
        {
            attachedInteractors.SafeRemoveListItem(currentInteractor);
            currentInteractor.Controller.Visualizer.VisualizerPoseOverrideSource = null;
        }

        private Pose GetGrabPose(IControllerInteractor controllerInteractor) => new Pose(GetGrabPosition(controllerInteractor), GetGrabRotation(controllerInteractor));

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

        private Quaternion GetGrabRotation(IControllerInteractor interactor)
        {
            var worldAttachmentRotation = Quaternion.Euler(poseLocalRotationOffset);
            var localControllerOffset = interactor.Controller.Visualizer.GripPose.localRotation;
            return transform.rotation * worldAttachmentRotation * localControllerOffset;
        }
    }
}
