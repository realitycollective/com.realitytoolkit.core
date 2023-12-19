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
    /// Syncs the <see cref="IControllerInteractor"/>'s <see cref="Controllers.IControllerVisualizer"/>
    /// to the <see cref="Interactables.IInteractable"/> using <see cref="Controllers.IControllerVisualizer.OverrideSourcePose"/>.
    /// </summary>
    /// <remarks>
    /// Only supports <see cref="IControllerInteractor"/>s.
    /// Does not support <see cref="IPokeInteractor"/>s and will ignore them.
    /// </remarks>
    public class SyncControllerVisualizerBehaviour : BaseInteractionBehaviour
    {
        [SerializeField, Tooltip("Optional local offset from the object's pivot.")]
        private Vector3 poseLocalPositionOffset = Vector3.zero;

        [SerializeField, Tooltip("Optional local offset from the object's pivot.")]
        private Vector3 poseLocalRotationOffset = Vector3.zero;

        private readonly List<IControllerInteractor> syncedVisualizers = new List<IControllerInteractor>();

        /// <inheritdoc/>
        protected override void Update()
        {
            var syncPose = GetSyncPose();

            for (int i = 0; i < syncedVisualizers.Count; i++)
            {
                var interactor = syncedVisualizers[i];
                interactor.Controller.Visualizer.PoseDriver.SetPositionAndRotation(syncPose.position, syncPose.rotation);
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

            SyncVisualizer(controllerInteractor);
        }

        /// <inheritdoc/>
        protected override void OnSelectExited(InteractionExitEventArgs eventArgs)
        {
            if (eventArgs.Interactor is not IControllerInteractor controllerInteractor ||
                eventArgs.Interactor is IPokeInteractor)
            {
                return;
            }

            UnsyncVisualizer(controllerInteractor);
        }

        /// <inheritdoc/>
        protected override void OnGrabEntered(InteractionEventArgs eventArgs)
        {
            if (eventArgs.Interactor is not IControllerInteractor controllerInteractor ||
                eventArgs.Interactor is IPokeInteractor)
            {
                return;
            }

            SyncVisualizer(controllerInteractor);
        }

        /// <inheritdoc/>
        protected override void OnGrabExited(InteractionExitEventArgs eventArgs)
        {
            if (eventArgs.Interactor is not IControllerInteractor controllerInteractor ||
                eventArgs.Interactor is IPokeInteractor)
            {
                return;
            }

            UnsyncVisualizer(controllerInteractor);
        }

        private void SyncVisualizer(IControllerInteractor currentInteractor)
        {
            syncedVisualizers.EnsureListItem(currentInteractor);
            currentInteractor.Controller.Visualizer.OverrideSourcePose = true;
        }

        private void UnsyncVisualizer(IControllerInteractor currentInteractor)
        {
            syncedVisualizers.SafeRemoveListItem(currentInteractor);
            currentInteractor.Controller.Visualizer.OverrideSourcePose = false;
        }

        private Pose GetSyncPose() => new Pose(transform.TransformPoint(poseLocalPositionOffset), transform.rotation * Quaternion.Euler(poseLocalRotationOffset));
    }
}
