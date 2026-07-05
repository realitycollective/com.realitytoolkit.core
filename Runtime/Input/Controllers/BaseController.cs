// Copyright (c) Reality Collective. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using RealityCollective.ServiceFramework.Services;
using RealityCollective.Utilities.Extensions;
using RealityToolkit.Definitions.Controllers;
using RealityToolkit.Definitions.Devices;
using RealityToolkit.Input.Definitions;
using RealityToolkit.Input.Extensions;
using RealityToolkit.Input.Interactors;
using RealityToolkit.Input.Interfaces;
using RealityToolkit.Input.Interfaces.Modules;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
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
        /// <param name="inputDevice">The <see cref="InputDevice"/> associated with this controller.</param>
        /// <param name="trackingState">The initial tracking state of this controller.</param>
        /// <param name="controllerHandedness">The controller's handedness.</param>
        /// <param name="controllerProfile">The <see cref="ControllerProfile"/> used to configure the <see cref="IController"/>.</param>
        protected BaseController(IControllerServiceModule controllerDataProvider, InputDevice inputDevice, TrackingState trackingState, Handedness controllerHandedness, ControllerProfile controllerProfile)
        {
            ServiceModule = controllerDataProvider;
            TrackingState = trackingState;
            ControllerHandedness = controllerHandedness;
            InputDevice = inputDevice;

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
            controllerInteractors = (controllerProfile.OverrideControllerInteractors || !ServiceManager.Instance.TryGetServiceProfile<IInputService, InputServiceProfile>(out var inputServiceProfile)) ?
                controllerProfile.ControllerInteractors :
                inputServiceProfile.InteractorsProfile.DefaultControllerInteractors;

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
        /// This dictionary contains <see cref="AxisType.Digital"/> mappings to their respective <see cref="InputFeatureUsage"/> equivalent
        /// used to read the buttons state.
        /// </summary>
        protected virtual IReadOnlyDictionary<string, InputFeatureUsage<bool>> DigitalInputFeatureUsageMap { get; set; } = new Dictionary<string, InputFeatureUsage<bool>>();

        /// <summary>
        /// This dictionary contains <see cref="AxisType.SingleAxis"/> mappings to their respective <see cref="InputFeatureUsage"/> equivalent
        /// used to read the buttons state.
        /// </summary>
        protected virtual IReadOnlyDictionary<string, InputFeatureUsage<float>> SingleAxisInputFeatureUsageMap { get; set; } = new Dictionary<string, InputFeatureUsage<float>>();

        /// <summary>
        /// This dictionary contains <see cref="AxisType.DualAxis"/> mappings to their respective <see cref="InputFeatureUsage"/> equivalent
        /// used to read the buttons state.
        /// </summary>
        protected virtual IReadOnlyDictionary<string, InputFeatureUsage<Vector2>> DualAxisInputFeatureUsageMap { get; set; } = new Dictionary<string, InputFeatureUsage<Vector2>>();

        /// <summary>
        /// The controller's pointer pose in world space.
        /// </summary>
        protected Pose SpatialPointerPose { get; set; }

        /// <summary>
        /// The <see cref="IInputService"/> the <see cref="IController"/>'s <see cref="ServiceModule"/> is registered with.
        /// </summary>
        protected IInputService InputService { get; }

        /// <summary>
        /// The <see cref="InputDevice"/> associated with this controller.
        /// </summary>
        protected InputDevice InputDevice { get; }

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

        /// <inheritdoc />
        public virtual void UpdateController()
        {
            if (!Enabled)
            {
                return;
            }

            UpdateTrackingState();
            UpdateControllerPose();
            UpdateSpatialPointerPose();
            UpdateInteractionMappings();

            if (TrackingState == TrackingState.Tracked)
            {
                MotionDirection = Pose.position - previousPosition;
                MotionDirection.Normalize();
            }

            previousPosition = Pose.position;
        }

        /// <summary>
        /// Updates the controller's <see cref="TrackingState"/>.
        /// </summary>
        protected virtual void UpdateTrackingState()
        {
            var currentTrackingState = TrackingState;
            if (InputDevice.TryGetFeatureValue(CommonUsages.isTracked, out var isTracked))
            {
                TrackingState = isTracked ? TrackingState.Tracked : TrackingState.NotTracked;
            }

            if (TrackingState != currentTrackingState)
            {
                InputService?.RaiseSourceTrackingStateChanged(InputSource, this, TrackingState);
            }
        }

        /// <summary>
        /// Updates the controller's pose.
        /// </summary>
        protected virtual void UpdateControllerPose()
        {
            if (TrackingState != TrackingState.Tracked)
            {
                IsPositionAvailable = false;
                IsPositionApproximate = false;
                IsRotationAvailable = false;
                return;
            }

            IsPositionAvailable = InputDevice.TryGetFeatureValue(CommonUsages.devicePosition, out var position);
            IsRotationAvailable = InputDevice.TryGetFeatureValue(CommonUsages.deviceRotation, out var rotation);
            IsPositionApproximate = false;

            var updatedControllerPose = new Pose(position, rotation);
            if (updatedControllerPose != Pose)
            {
                Pose = updatedControllerPose;
                InputService?.RaiseSourcePoseChanged(InputSource, this, Pose);
            }
        }

        /// <summary>
        /// Updates the controller's spatial pointer pose.
        /// </summary>
        protected virtual void UpdateSpatialPointerPose()
        {
            SpatialPointerPose = Pose;
        }

        /// <summary>
        /// Updates the controller's <see cref="DeviceInputType.ButtonPress"/> mappings.
        /// </summary>
        /// <param name="interactionMapping">The <see cref="InteractionMapping"/> to update.</param>
        /// <param name="inputDevice">The <see cref="InputDevice"/> data is read from.</param>
        protected virtual void UpdateDigitalInteractionMapping(InteractionMapping interactionMapping, InputDevice inputDevice)
        {
            Debug.Assert(interactionMapping.AxisType == AxisType.Digital);

            if (!DigitalInputFeatureUsageMap.ContainsKey(interactionMapping.InputName))
            {
                Debug.LogError($"Interaction mapping {interactionMapping.InputName} is not handled for controller {GetType().Name} - {ControllerHandedness}.");
                return;
            }

            interactionMapping.BoolData = inputDevice.TryGetFeatureValue(DigitalInputFeatureUsageMap[interactionMapping.InputName], out bool value) && value;
        }

        /// <summary>
        /// Updates the controller's <see cref="DeviceInputType.ThumbStick"/> mappings.
        /// </summary>
        /// <param name="interactionMapping">The <see cref="InteractionMapping"/> to update.</param>
        /// <param name="inputDevice">The <see cref="InputDevice"/> data is read from.</param>
        protected virtual void UpdateSingleAxisInteractionMapping(InteractionMapping interactionMapping, InputDevice inputDevice)
        {
            Debug.Assert(interactionMapping.AxisType == AxisType.SingleAxis);

            if (!SingleAxisInputFeatureUsageMap.ContainsKey(interactionMapping.InputName))
            {
                Debug.LogError($"Interaction mapping {interactionMapping.InputName} is not handled for controller {GetType().Name} - {ControllerHandedness}.");
                return;
            }

            if (inputDevice.TryGetFeatureValue(SingleAxisInputFeatureUsageMap[interactionMapping.InputName], out float value))
            {
                interactionMapping.FloatData = value;
            }
        }

        /// <summary>
        /// Updates the controller's <see cref="DeviceInputType.ThumbStick"/> mappings.
        /// </summary>
        /// <param name="interactionMapping">The <see cref="InteractionMapping"/> to update.</param>
        /// <param name="inputDevice">The <see cref="InputDevice"/> data is read from.</param>
        protected virtual void UpdateDualAxisInteractionMapping(InteractionMapping interactionMapping, InputDevice inputDevice)
        {
            Debug.Assert(interactionMapping.AxisType == AxisType.DualAxis);

            if (!DualAxisInputFeatureUsageMap.ContainsKey(interactionMapping.InputName))
            {
                Debug.LogError($"Interaction mapping {interactionMapping.InputName} is not handled for controller {GetType().Name} - {ControllerHandedness}.");
                return;
            }

            if (inputDevice.TryGetFeatureValue(DualAxisInputFeatureUsageMap[interactionMapping.InputName], out Vector2 value))
            {
                interactionMapping.Vector2Data = value;
            }
        }

        /// <summary>
        /// Updates the spatial pointer pose interaction mapping value.
        /// </summary>
        /// <param name="interactionMapping">The spatial pointer pose mapping.</param>
        protected void UpdateSpatialPointer(InteractionMapping interactionMapping)
        {
            Debug.Assert(interactionMapping.AxisType == AxisType.SixDof);
            interactionMapping.PoseData = SpatialPointerPose;
        }

        /// <summary>
        /// Reads controller input and updates mappings.
        /// </summary>
        protected virtual void UpdateInteractionMappings()
        {
            Debug.Assert(Interactions != null && Interactions.Length > 0, $"Interaction mappings must be defined for {GetType().Name} - {ControllerHandedness}.");

            for (var i = 0; i < Interactions.Length; i++)
            {
                var interactionMapping = Interactions[i];
                switch (interactionMapping.InputType)
                {
                    case DeviceInputType.Trigger:
                        UpdateSingleAxisInteractionMapping(interactionMapping, InputDevice);
                        break;
                    case DeviceInputType.ButtonPress:
                        UpdateDigitalInteractionMapping(interactionMapping, InputDevice);
                        break;
                    case DeviceInputType.ThumbStick:
                        UpdateDualAxisInteractionMapping(interactionMapping, InputDevice);
                        break;
                    case DeviceInputType.SpatialPointer:
                        UpdateSpatialPointer(interactionMapping);
                        break;
                    default:
                        Debug.LogError($"Input {interactionMapping.InputType} is not handled for controller {GetType().Name} - {ControllerHandedness}.");
                        break;
                }

                interactionMapping.RaiseInputAction(InputSource, ControllerHandedness);
            }
        }

        private void AssignControllerMappings(InteractionMappingProfile[] interactionMappingProfiles)
        {
            var interactions = new InteractionMapping[interactionMappingProfiles.Length];

            for (int i = 0; i < interactions.Length; i++)
            {
                var interactionProfile = interactionMappingProfiles[i];
                interactions[i] = interactionProfile.InteractionMapping;
            }

            Interactions = interactions;
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

            var rigTransform = InputService.InputRig.RigTransform;
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

        /// <inheritdoc />
        public bool TryGetPose(Space space, out Pose pose)
        {
            if (!IsPositionAvailable || !IsRotationAvailable)
            {
                pose = default;
                return false;
            }

            if (space == Space.Self)
            {
                pose = Pose;
                return true;
            }

            pose = new Pose(
                InputService.InputRig.RigTransform.TransformPoint(Pose.position),
                InputService.InputRig.RigTransform.rotation * Pose.rotation);

            return true;
        }
    }
}
