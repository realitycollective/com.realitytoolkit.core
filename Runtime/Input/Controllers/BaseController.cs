// Copyright (c) Reality Collective. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using RealityCollective.Definitions.Utilities;
using RealityCollective.Extensions;
using RealityCollective.ServiceFramework.Services;
using RealityToolkit.Definitions.Controllers;
using RealityToolkit.Definitions.Devices;
using RealityToolkit.Input.Interactors;
using RealityToolkit.Input.Interfaces;
using RealityToolkit.Input.Interfaces.Controllers;
using RealityToolkit.Input.Interfaces.Handlers;
using RealityToolkit.Input.Interfaces.Modules;
using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace RealityToolkit.Input.Controllers
{
    /// <summary>
    /// Base Controller class to inherit from for all controllers.
    /// </summary>
    public abstract class BaseController : IController
    {
        /// <summary>
        /// Creates a new instance of a controller.
        /// </summary>
        protected BaseController() { }

        /// <summary>
        /// Creates a new instance of a controller.
        /// </summary>
        /// <param name="controllerDataProvider">The <see cref="IControllerServiceModule"/> this controller belongs to.</param>
        /// <param name="trackingState">The initial tracking state of this controller.</param>
        /// <param name="controllerHandedness">The controller's handedness.</param>
        /// <param name="controllerProfile">The <see cref="ControllerProfile"/> used to configure the <see cref="IController"/>.</param>
        protected BaseController(IControllerServiceModule controllerDataProvider, TrackingState trackingState, Handedness controllerHandedness, ControllerProfile controllerProfile)
        {
            ServiceModule = controllerDataProvider;
            TrackingState = trackingState;
            ControllerHandedness = controllerHandedness;

            var handednessPrefix = string.Empty;

            if (controllerHandedness == Handedness.Left ||
                controllerHandedness == Handedness.Right)
            {
                handednessPrefix = $"{controllerHandedness} ";
            }

            Name = $"{handednessPrefix}{GetType().Name}";

            if (controllerProfile.IsNull())
            {
                throw new Exception($"{nameof(controllerProfile)} cannot be null for {Name}");
            }

            controllerPrefab = controllerProfile.ControllerPrefab;
            controllerInteractors = controllerProfile.ControllerInteractors;

            AssignControllerMappings(controllerProfile.InteractionMappingProfiles);

            // If no controller mappings found, warn the user.  Does not stop the project from running.
            if (Interactions == null || Interactions.Length < 1)
            {
                throw new Exception($"No Controller interaction mappings found for {controllerProfile.name}!");
            }

            if (ServiceManager.Instance.TryGetService<IInputService>(out var inputService))
            {
                Debug.Assert(ReferenceEquals(inputService, controllerDataProvider.ParentService));
                InputService = inputService;
            }

            IsPositionAvailable = false;
            IsPositionApproximate = false;
            IsRotationAvailable = false;

            Enabled = true;
        }

        private Vector3 previousPosition;
        private readonly ControllerPoseSynchronizer controllerPrefab;
        private readonly IReadOnlyList<BaseControllerInteractor> controllerInteractors;

        /// <summary>
        /// The <see cref="IInputService"/> the <see cref="IController"/>'s <see cref="ServiceModule"/> is registered with.
        /// </summary>
        protected IInputService InputService { get; }

        /// <summary>
        /// The default interactions for this controller.
        /// </summary>
        public virtual InteractionMapping[] DefaultInteractions { get; } = new InteractionMapping[0];

        /// <summary>
        /// The Default Left Handed interactions for this controller.
        /// </summary>
        public virtual InteractionMapping[] DefaultLeftHandedInteractions { get; } = new InteractionMapping[0];

        /// <summary>
        /// The Default Right Handed interactions for this controller.
        /// </summary>
        public virtual InteractionMapping[] DefaultRightHandedInteractions { get; } = new InteractionMapping[0];

        /// <summary>
        /// Local offset from the controller position defining where the grip pose is.
        /// The grip pose may be used to attach things to the controller when grabbing objects.
        /// </summary>
        protected virtual Pose GripPoseOffset => Pose.identity;

        /// <inheritdoc />
        public Pose Pose { get; protected set; } = Pose.identity;

        /// <inheritdoc />
        public bool IsPositionAvailable { get; protected set; }

        /// <inheritdoc />
        public bool IsPositionApproximate { get; protected set; }

        /// <inheritdoc />
        public bool IsRotationAvailable { get; protected set; }

        /// <inheritdoc />
        public Vector3 AngularVelocity { get; protected set; } = Vector3.zero;

        /// <inheritdoc />
        public Vector3 Velocity { get; protected set; } = Vector3.zero;

        /// <inheritdoc />
        public Vector3 MotionDirection { get; protected set; }

        /// <inheritdoc />
        public string Name { get; }

        /// <inheritdoc />
        public bool Enabled { get; set; }

        /// <inheritdoc />
        public IControllerServiceModule ServiceModule { get; }

        /// <inheritdoc />
        public TrackingState TrackingState { get; protected set; }

        /// <inheritdoc />
        public Handedness ControllerHandedness { get; }

        /// <inheritdoc />
        public IInputSource InputSource { get; private set; }

        /// <inheritdoc />
        public IControllerVisualizer Visualizer { get; private set; }

        /// <inheritdoc />
        public InteractionMapping[] Interactions { get; private set; } = null;

        /// <summary>
        /// Updates the current readings for the controller.
        /// </summary>
        public virtual void UpdateController()
        {
            if (!Enabled)
            {
                return;
            }

            if (TrackingState == TrackingState.Tracked)
            {
                MotionDirection = Pose.position - previousPosition;
                MotionDirection.Normalize();
            }

            previousPosition = Pose.position;
        }

        /// <summary>
        /// Load the Interaction mappings for this controller from the configured Controller Mapping profile
        /// </summary>
        protected void AssignControllerMappings(InteractionMapping[] mappings) => Interactions = mappings;

        private void AssignControllerMappings(InteractionMappingProfile[] interactionMappingProfiles)
        {
            var interactions = new InteractionMapping[interactionMappingProfiles.Length];

            for (int i = 0; i < interactions.Length; i++)
            {
                var interactionProfile = interactionMappingProfiles[i];
                interactions[i] = interactionProfile.InteractionMapping;
            }

            AssignControllerMappings(interactions);
        }

        /// <inheritdoc />
        public void TryRenderControllerModel()
        {
            if (controllerPrefab.IsNull())
            {
                // If there is no prefab assigned, it is likely intended, since there is many controllers
                // that do not require a controller model, e.g. Xbox, PlayStation etc.
                return;
            }

            var rigTransform = Camera.main.transform.parent;

            var controllerObject = Object.Instantiate(controllerPrefab, rigTransform);
            controllerObject.name = GetType().Name;
            Visualizer = controllerObject.GetComponent<IControllerVisualizer>();

            if (Visualizer != null)
            {
                Visualizer.Controller = this;

                var interactors = new List<IInteractor>();
                for (int j = 0; j < controllerInteractors.Count; j++)
                {
                    var interactorPrefab = controllerInteractors[j];
                    var pointerObject = Object.Instantiate(interactorPrefab, rigTransform);
                    var pointer = pointerObject.GetComponent<IControllerInteractor>();

                    if (pointer != null)
                    {
                        interactors.Add(pointer);
                    }
                    else
                    {
                        Debug.LogError($"{interactorPrefab.name} prefab must have a {nameof(IControllerInteractor)} component attached.");
                    }

                    if (interactors.Count > 0)
                    {
                        InputSource = InputService.RequestNewGenericInputSource(Name, interactors.ToArray());

                        for (int i = 0; i < InputSource?.Pointers?.Length; i++)
                        {
                            var interactor = InputSource.Pointers[i];
                            if (interactor is IControllerInteractor controllerInteractor)
                            {
                                controllerInteractor.Controller = this;
                            }
                        }
                    }
                }
            }
            else
            {
                Debug.LogError($"{GetType().Name} prefab must have a {nameof(IControllerVisualizer)} component attached.");
            }
        }
    }
}
