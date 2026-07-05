// Copyright (c) Reality Collective. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using RealityCollective.ServiceFramework.Modules;
using RealityCollective.Utilities.Extensions;
using RealityToolkit.Definitions.Controllers;
using RealityToolkit.Definitions.Devices;
using RealityToolkit.Input.Definitions;
using RealityToolkit.Input.Interfaces;
using RealityToolkit.Input.Interfaces.Modules;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

namespace RealityToolkit.Input.Controllers
{
    /// <summary>
    /// Base controller service module to inherit from when implementing <see cref="IControllerServiceModule"/>s
    /// </summary>
    public abstract class BaseControllerServiceModule<T> : BaseServiceModule, IControllerServiceModule where T : IController
    {
        /// <inheritdoc />
        protected BaseControllerServiceModule(string name, uint priority, BaseControllerServiceModuleProfile profile, IInputService parentService)
            : base(name, priority, profile, parentService)
        {
            if (profile.IsNull())
            {
                throw new UnassignedReferenceException($"A {nameof(profile)} is required for {name}");
            }

            controllerMappingProfiles = profile.ControllerMappingProfiles;

            if (controllerMappingProfiles == null ||
                controllerMappingProfiles.Length == 0)
            {
                throw new UnassignedReferenceException($"{nameof(controllerMappingProfiles)} has no defined controller mappings for {name}");
            }

            InputService = parentService;
        }

        protected readonly IInputService InputService;
        private readonly ControllerProfile[] controllerMappingProfiles;
        private readonly List<T> activeControllers = new();
        private readonly List<InputDevice> detectedControllerInputDevices = new();

        /// <summary>
        /// Gets the active controllers of type <typeparamref name="T"/> that are currently being tracked by this service module.
        /// </summary>
        public IReadOnlyList<T> ActiveControllers => activeControllers;

        /// <inheritdoc />
        public override void Update()
        {
            base.Update();
            DetectControllerInputDevices();
            UpdateControllers();
        }

        /// <inheritdoc />
        public ControllerProfile GetControllerMappingProfile(Type controllerType, Handedness handedness)
        {
            if (TryGetControllerMappingProfile(controllerType, handedness, out var controllerMappingProfile))
            {
                return controllerMappingProfile;
            }

            Debug.LogError($"Failed to find a controller mapping for {controllerType.Name} with with handedness: {handedness}");
            return null;
        }

        /// <inheritdoc />
        public bool TryGetControllerMappingProfile(Type controllerType, Handedness handedness, out ControllerProfile controllerMappingProfile)
        {
            if (controllerType == null)
            {
                Debug.LogError($"{nameof(controllerType)} is null!");
                controllerMappingProfile = null;
                return false;
            }

            if (!typeof(IController).IsAssignableFrom(controllerType))
            {
                Debug.LogError($"{controllerType.Name} does not implement {nameof(IController)}");
                controllerMappingProfile = null;
                return false;
            }

            for (int i = 0; i < controllerMappingProfiles.Length; i++)
            {
                if (handedness == controllerMappingProfiles[i].Handedness &&
                    controllerMappingProfiles[i].ControllerType?.Type == controllerType)
                {
                    controllerMappingProfile = controllerMappingProfiles[i];
                    return true;
                }
            }

            controllerMappingProfile = null;
            return false;
        }

        /// <summary>
        /// Tries to get the active controller of type <typeparamref name="T"/> for the specified <see cref="Handedness"/>.
        /// </summary>
        /// <param name="handedness">The handedness of the controller to get.</param>
        /// <param name="controller">The active controller of type <typeparamref name="T"/> if found; otherwise, <c>null</c>.</param>
        /// <returns><c>true</c> if an active controller of type <typeparamref name="T"/> is found for the specified handedness; otherwise, <c>false</c>.</returns>
        public bool TryGetController(Handedness handedness, out T controller)
        {
            for (int i = 0; i < activeControllers.Count; i++)
            {
                if (activeControllers[i].ControllerHandedness == handedness)
                {
                    controller = activeControllers[i];
                    return true;
                }
            }

            controller = default;
            return false;
        }

        private void DetectControllerInputDevices()
        {
            detectedControllerInputDevices.Clear();
            InputDevices.GetDevicesWithCharacteristics(InputDeviceCharacteristics.Controller, detectedControllerInputDevices);

            for (var i = 0; i < detectedControllerInputDevices.Count; i++)
            {
                var device = detectedControllerInputDevices[i];
                if (!device.isValid)
                {
                    continue;
                }

                if (TryGetControllerType(device, out var controllerType))
                {
                    var handedness = GetHandedness(device);
                    CreateControllerIfNotExists(device, handedness, controllerType);
                }
            }

            // Remove controllers that no longer have a valid input device.
            for (int i = activeControllers.Count - 1; i >= 0; i--)
            {
                var controller = activeControllers[i];
                var handedness = controller.ControllerHandedness;
                var deviceStillExists = false;

                for (var j = 0; j < detectedControllerInputDevices.Count; j++)
                {
                    var device = detectedControllerInputDevices[j];
                    if (device.isValid && TryGetControllerType(device, out _) && GetHandedness(device) == handedness)
                    {
                        deviceStillExists = true;
                        break;
                    }
                }

                if (!deviceStillExists)
                {
                    RemoveController(handedness);
                }
            }
        }

        private void UpdateControllers()
        {
            for (int i = 0; i < activeControllers.Count; i++)
            {
                activeControllers[i].UpdateController();
            }
        }

        private void RemoveController(Handedness handedness)
        {
            if (TryGetController(handedness, out var controller))
            {
                InputService?.RaiseSourceLost(controller.InputSource, controller);
                activeControllers.Remove(controller);
            }
        }

        private void CreateControllerIfNotExists(InputDevice inputDevice, Handedness handedness, Type type)
        {
            // Check if a controller already exists.
            if (TryGetController(handedness, out var existingController))
            {
                if (existingController.GetType() == type)
                {
                    // Handedness and type match.
                    return;
                }
                else
                {
                    // If a controller is registered for the right handedness but it is
                    // the wrong type of controller, remove it and continue.
                    RemoveController(handedness);
                }
            }

            try
            {
                var detectedController = (T)Activator.CreateInstance(type, this, inputDevice, TrackingState.Tracked, handedness, GetControllerMappingProfile(type, handedness));
                detectedController.TryRenderControllerModel();
                activeControllers.Add(detectedController);
                InputService?.RaiseSourceDetected(detectedController.InputSource, detectedController);
            }
            catch (Exception ex)
            {
                Debug.LogError($"Failed to create {type.Name}!\n{ex}");
            }
        }

        /// <summary>
        /// Gets the <see cref="Handedness"/> of the given <see cref="InputDevice"/>.
        /// </summary>
        /// <param name="device">The input device to get the handedness for.</param>
        /// <returns>The <see cref="Handedness"/> of the input device.</returns>
        protected Handedness GetHandedness(InputDevice device)
        {
            if ((device.characteristics & InputDeviceCharacteristics.Left) != 0)
            {
                return Handedness.Left;
            }

            if ((device.characteristics & InputDeviceCharacteristics.Right) != 0)
            {
                return Handedness.Right;
            }

            return Handedness.None;
        }

        /// <summary>
        /// Determines whether the specified <see cref="InputDevice"/> is a valid controller for this service module
        /// and what type of controller it corresponds to.
        /// </summary>
        /// <param name="inputDevice">The input device to check.</param>
        /// <param name="controllerType">The type of controller the input device corresponds to.</param>
        /// <returns><c>true</c> if the input device is a valid controller and has a corresponding type; otherwise, <c>false</c>.</returns>
        protected abstract bool TryGetControllerType(InputDevice inputDevice, out Type controllerType);
    }
}