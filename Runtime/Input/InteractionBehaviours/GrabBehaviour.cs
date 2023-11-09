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
        private Vector3 grabPoseLocalOffset = Vector3.zero;

        [SerializeField, Tooltip("Optional local offset from the object's pivot.")]
        private Vector3 grabPoseLocalRotationOffset = Vector3.zero;

        private IDirectInteractor grabbingInteractor;

        /// <inheritdoc/>
        protected override void Update()
        {
            if (grabbingInteractor != null)
            {
                transform.SetPositionAndRotation(GetGrabPosition(), GetGrabRotation());
            }
        }

        /// <inheritdoc/>
        protected override void OnFirstGrabEntered(InteractionEventArgs eventArgs)
        {
            if (eventArgs.Interactor is IDirectInteractor directInteractor)
            {
                grabbingInteractor = directInteractor;
                transform.SetPositionAndRotation(GetGrabPosition(), GetGrabRotation());
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

        private Vector3 GetGrabPosition() => grabbingInteractor.Controller.Visualizer.GripPose.transform.position + transform.TransformDirection(grabPoseLocalOffset);

        private Quaternion GetGrabRotation() => grabbingInteractor.Controller.Visualizer.GripPose.transform.rotation * Quaternion.Euler(grabPoseLocalRotationOffset);
    }
}
