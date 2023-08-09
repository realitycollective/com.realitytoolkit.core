// Copyright (c) Reality Collective. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using RealityCollective.Definitions.Utilities;
using RealityCollective.Extensions;
using RealityCollective.ServiceFramework.Services;
using RealityToolkit.Definitions.Controllers;
using RealityToolkit.Definitions.Devices;
using RealityToolkit.Input.Interactions.Interactors;
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
        /// <param name="controllerMappingProfile"></param>
        protected BaseController(IControllerServiceModule controllerDataProvider, TrackingState trackingState, Handedness controllerHandedness, ControllerMappingProfile controllerMappingProfile)
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

            if (controllerMappingProfile.IsNull())
            {
                throw new Exception($"{nameof(controllerMappingProfile)} cannot be null for {Name}");
            }

            controllerPrefab = controllerMappingProfile.ControllerPrefab;
            var pointers = AssignControllerMappings(controllerMappingProfile.InteractionMappingProfiles);

            // If no controller mappings found, warn the user.  Does not stop the project from running.
            if (Interactions == null || Interactions.Length < 1)
            {
                throw new Exception($"No Controller interaction mappings found for {controllerMappingProfile.name}!");
            }

            if (ServiceManager.Instance.TryGetService<IInputService>(out var inputService))
            {
                Debug.Assert(ReferenceEquals(inputService, controllerDataProvider.ParentService));
                InputService = inputService;
                InputSource = InputService.RequestNewGenericInputSource(Name, pointers);

                for (int i = 0; i < InputSource?.Pointers?.Length; i++)
                {
                    InputSource.Pointers[i].Controller = this;
                }
            }

            IsPositionAvailable = false;
            IsPositionApproximate = false;
            IsRotationAvailable = false;

            Enabled = true;
        }

        private Vector3 previousPosition;
        private readonly ControllerPoseSynchronizer controllerPrefab;

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

        private IPointer[] AssignControllerMappings(InteractionMappingProfile[] interactionMappingProfiles)
        {
            var pointers = new List<IPointer>();
            var interactions = new InteractionMapping[interactionMappingProfiles.Length];

            for (int i = 0; i < interactions.Length; i++)
            {
                var interactionProfile = interactionMappingProfiles[i];

                for (int j = 0; j < interactionProfile.PointerProfiles.Length; j++)
                {
                    var pointerProfile = interactionProfile.PointerProfiles[j];
                    var rigTransform = Camera.main.transform.parent;
                    var pointerObject = Object.Instantiate(pointerProfile.PointerPrefab, rigTransform);
                    var pointer = pointerObject.GetComponent<IPointer>();

                    if (pointer != null)
                    {
                        pointers.Add(pointer);
                    }
                    else
                    {
                        Debug.LogWarning($"Failed to attach {pointerProfile.PointerPrefab.name} to {GetType().Name} {ControllerHandedness}.");
                    }
                }

                interactions[i] = interactionProfile.InteractionMapping;
            }

            AssignControllerMappings(interactions);

            return pointers.Count > 0 ? pointers.ToArray() : null;
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
            }
            else
            {
                Debug.LogError($"{GetType().Name} prefab must have a {nameof(IControllerVisualizer)} component attached.");
            }
        }
    }
}
