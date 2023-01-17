// Copyright (c) Reality Collective. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using RealityCollective.Definitions.Utilities;
using RealityCollective.Extensions;
using RealityCollective.ServiceFramework.Services;
using RealityToolkit.CameraSystem.Interfaces;
using RealityToolkit.Definitions.Controllers;
using RealityToolkit.Definitions.Devices;
using RealityToolkit.Definitions.Utilities;
using RealityToolkit.InputSystem.Interfaces;
using RealityToolkit.InputSystem.Interfaces.Controllers;
using RealityToolkit.InputSystem.Interfaces.Handlers;
using RealityToolkit.InputSystem.Interfaces.Modules;
using RealityToolkit.Services.InputSystem.Utilities;
using RealityToolkit.Utilities;
using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace RealityToolkit.InputSystem.Controllers
{
    /// <summary>
    /// Base Controller class to inherit from for all controllers.
    /// </summary>
    public abstract class BaseController : IMixedRealityController
    {
        /// <summary>
        /// Creates a new instance of a controller.
        /// </summary>
        protected BaseController() { }

        /// <summary>
        /// Creates a new instance of a controller.
        /// </summary>
        /// <param name="controllerDataProvider">The <see cref="IMixedRealityControllerServiceModule"/> this controller belongs to.</param>
        /// <param name="trackingState">The initial tracking state of this controller.</param>
        /// <param name="controllerHandedness">The controller's handedness.</param>
        /// <param name="controllerMappingProfile"></param>
        protected BaseController(IMixedRealityControllerServiceModule controllerDataProvider, TrackingState trackingState, Handedness controllerHandedness, MixedRealityControllerMappingProfile controllerMappingProfile)
        {
            ControllerDataProvider = controllerDataProvider;
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

            if (ServiceManager.Instance.TryGetService<IMixedRealityInputSystem>(out var inputSystem))
            {
                Debug.Assert(ReferenceEquals(inputSystem, controllerDataProvider.ParentService));
                InputSystem = inputSystem;
                InputSource = InputSystem.RequestNewGenericInputSource(Name, pointers);

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

        protected readonly IMixedRealityInputSystem InputSystem;

        private readonly ControllerPoseSynchronizer controllerPrefab;

        /// <summary>
        /// The default interactions for this controller.
        /// </summary>
        public virtual MixedRealityInteractionMapping[] DefaultInteractions { get; } = new MixedRealityInteractionMapping[0];

        /// <summary>
        /// The Default Left Handed interactions for this controller.
        /// </summary>
        public virtual MixedRealityInteractionMapping[] DefaultLeftHandedInteractions { get; } = new MixedRealityInteractionMapping[0];

        /// <summary>
        /// The Default Right Handed interactions for this controller.
        /// </summary>
        public virtual MixedRealityInteractionMapping[] DefaultRightHandedInteractions { get; } = new MixedRealityInteractionMapping[0];

        /// <summary>
        /// Local offset from the controller position defining where the grip pose is.
        /// The grip pose may be used to attach things to the controller when grabbing objects.
        /// </summary>
        protected virtual MixedRealityPose GripPoseOffset => MixedRealityPose.ZeroIdentity;

        #region IMixedRealityController Implementation

        /// <inheritdoc />
        public string Name { get; }

        /// <inheritdoc />
        public bool Enabled { get; set; }

        /// <inheritdoc />
        public IMixedRealityControllerServiceModule ControllerDataProvider { get; }

        /// <inheritdoc />
        public TrackingState TrackingState { get; protected set; }

        /// <inheritdoc />
        public Handedness ControllerHandedness { get; }

        /// <inheritdoc />
        public IMixedRealityInputSource InputSource { get; private set; }

        /// <inheritdoc />
        public IMixedRealityControllerVisualizer Visualizer { get; private set; }

        /// <inheritdoc />
        public bool IsPositionAvailable { get; protected set; }

        /// <inheritdoc />
        public bool IsPositionApproximate { get; protected set; }

        /// <inheritdoc />
        public bool IsRotationAvailable { get; protected set; }

        /// <inheritdoc />
        public MixedRealityInteractionMapping[] Interactions { get; private set; } = null;

        /// <inheritdoc />
        public MixedRealityPose Pose { get; protected set; } = MixedRealityPose.ZeroIdentity;

        /// <inheritdoc />
        public Vector3 AngularVelocity { get; protected set; } = Vector3.zero;

        /// <inheritdoc />
        public Vector3 Velocity { get; protected set; } = Vector3.zero;

        #endregion IMixedRealityController Implementation

        /// <summary>
        /// Updates the current readings for the controller.
        /// </summary>
        public virtual void UpdateController() { }

        /// <summary>
        /// Load the Interaction mappings for this controller from the configured Controller Mapping profile
        /// </summary>
        protected void AssignControllerMappings(MixedRealityInteractionMapping[] mappings) => Interactions = mappings;

        private IMixedRealityPointer[] AssignControllerMappings(MixedRealityInteractionMappingProfile[] interactionMappingProfiles)
        {
            var pointers = new List<IMixedRealityPointer>();
            var interactions = new MixedRealityInteractionMapping[interactionMappingProfiles.Length];

            for (int i = 0; i < interactions.Length; i++)
            {
                var interactionProfile = interactionMappingProfiles[i];

                for (int j = 0; j < interactionProfile.PointerProfiles.Length; j++)
                {
                    var pointerProfile = interactionProfile.PointerProfiles[j];
                    var rigTransform = ServiceManager.Instance.TryGetService<IMixedRealityCameraSystem>(out var cameraSystem)
                        ? cameraSystem.MainCameraRig.RigTransform
                        : CameraCache.Main.transform.parent;
                    var pointerObject = Object.Instantiate(pointerProfile.PointerPrefab, rigTransform);
                    var pointer = pointerObject.GetComponent<IMixedRealityPointer>();

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
                Debug.LogError($"No prefab is assigned for controller {GetType().Name}. Please assign a prefab to spawn when the controller is detected in the controller's profile configuration.");
                return;
            }

            var rigTransform = ServiceManager.Instance.TryGetService<IMixedRealityCameraSystem>(out var cameraSystem)
                    ? cameraSystem.MainCameraRig.RigTransform
                    : CameraCache.Main.transform.parent;

            var controllerObject = Object.Instantiate(controllerPrefab, rigTransform);
            controllerObject.name = $"{GetType().Name}_Visualization";
            Visualizer = controllerObject.GetComponent<IMixedRealityControllerVisualizer>();

            if (Visualizer != null)
            {
                Visualizer.Controller = this;
            }
            else
            {
                Debug.LogError($"{GetType().Name} prefab must have a {nameof(IMixedRealityControllerVisualizer)} component attached.");
            }
        }
    }
}
