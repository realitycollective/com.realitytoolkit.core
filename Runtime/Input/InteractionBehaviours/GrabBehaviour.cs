// Copyright (c) Reality Collective. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using RealityToolkit.Input.Events;
using RealityToolkit.Input.Interactors;
using UnityEngine;

namespace RealityToolkit.Input.InteractionBehaviours
{
    /// <summary>
    /// The <see cref="GrabBehaviour"/> is an <see cref="IInteractionBehaviour"/> for use with
    /// <see cref="IDirectInteractor"/>s. It allows to "pick up" the <see cref="Interactables.IInteractable"/>
    /// and carry it around.
    /// </summary>
    public class GrabBehaviour : BaseInteractionBehaviour
    {
        [SerializeField, Tooltip("Optional local offset from the object's pivot.")]
        private Vector3 poseLocalPositionOffset = Vector3.zero;

        [SerializeField, Tooltip("Optional local offset from the object's pivot.")]
        private Vector3 poseLocalRotationOffset = Vector3.zero;

        private IDirectInteractor grabbingInteractor;

        /// <inheritdoc/>
        protected override void Update()
        {
            if (grabbingInteractor != null)
            {
                var pose = GetGrabPose();
                transform.SetPositionAndRotation(pose.position, pose.rotation);
            }
        }

        /// <inheritdoc/>
        protected override void OnFirstGrabEntered(InteractionEventArgs eventArgs)
        {
            if (eventArgs.Interactor is IDirectInteractor directInteractor)
            {
                grabbingInteractor = directInteractor;
                var pose = GetGrabPose();
                transform.SetPositionAndRotation(pose.position, pose.rotation);
            }
        }

        /// <inheritdoc/>
        protected override void OnGrabExited(InteractionExitEventArgs eventArgs)
        {
            if (eventArgs.Interactor == grabbingInteractor)
            {
                grabbingInteractor = null;
            }
        }

        private Pose GetGrabPose()
        {
            return new Pose(
                GetGrabPosition(),
                GetGrabRotation());
        }

        private Vector3 GetGrabPosition()
        {
            // Move the controller to the interactable's attachment point.
            var worldAttachmentPosition = transform.TransformPoint(poseLocalPositionOffset);

            // Adjust controller's position based on its own grab pose offset.
            var localControllerOffset = grabbingInteractor.Controller.Visualizer.PoseDriver.TransformPoint(grabbingInteractor.Controller.Visualizer.GripPose.localPosition) -
                grabbingInteractor.Controller.Visualizer.PoseDriver.position;

            // Combine.
            worldAttachmentPosition += localControllerOffset;

            return worldAttachmentPosition;
        }

        private Quaternion GetGrabRotation()
        {
            var worldAttachmentRotation = Quaternion.Euler(poseLocalRotationOffset);
            var localControllerOffset = grabbingInteractor.Controller.Visualizer.GripPose.localRotation;
            return transform.rotation * worldAttachmentRotation * localControllerOffset;
        }
    }
}
