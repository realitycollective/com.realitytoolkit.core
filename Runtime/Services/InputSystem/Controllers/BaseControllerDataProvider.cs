// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;
using RealityToolkit.Definitions.Controllers;
using RealityCollective.Definitions.Utilities;
using RealityToolkit.Interfaces.InputSystem;
using RealityToolkit.Interfaces.InputSystem.Controllers;
using RealityToolkit.Interfaces.InputSystem.Providers.Controllers;
using UnityEngine;
using RealityCollective.Extensions;

namespace RealityToolkit.Services.InputSystem.Controllers
{
    /// <summary>
    /// Base controller data provider to inherit from when implementing <see cref="IMixedRealityControllerDataProvider"/>s
    /// </summary>
    public abstract class BaseControllerDataProvider : BaseDataProvider, IMixedRealityControllerDataProvider
    {
        /// <inheritdoc />
        protected BaseControllerDataProvider(string name, uint priority, BaseMixedRealityControllerDataProviderProfile profile, IMixedRealityInputSystem parentService)
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

            InputSystem = parentService;
        }

        protected readonly IMixedRealityInputSystem InputSystem;

        private readonly MixedRealityControllerMappingProfile[] controllerMappingProfiles;

        private readonly List<IMixedRealityController> activeControllers = new List<IMixedRealityController>();

        /// <inheritdoc />
        public IReadOnlyList<IMixedRealityController> ActiveControllers => activeControllers;

        /// <inheritdoc />
        public MixedRealityControllerMappingProfile GetControllerMappingProfile(Type controllerType, Handedness handedness)
        {
            if (TryGetControllerMappingProfile(controllerType, handedness, out var controllerMappingProfile))
            {
                return controllerMappingProfile;
            }

            Debug.LogError($"Failed to find a controller mapping for {controllerType.Name} with with handedness: {handedness}");
            return null;
        }

        /// <inheritdoc />
        public bool TryGetControllerMappingProfile(Type controllerType, Handedness handedness, out MixedRealityControllerMappingProfile controllerMappingProfile)
        {
            if (controllerType == null)
            {
                Debug.LogError($"{nameof(controllerType)} is null!");
                controllerMappingProfile = null;
                return false;
            }

            if (!typeof(IMixedRealityController).IsAssignableFrom(controllerType))
            {
                Debug.LogError($"{controllerType.Name} does not implement {nameof(IMixedRealityController)}");
                controllerMappingProfile = null;
                return false;
            }

            // TODO provide a way to choose profiles with additional args instead of returning the first one found.
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

        protected void AddController(IMixedRealityController controller)
        {
            activeControllers.Add(controller);
        }

        protected void RemoveController(IMixedRealityController controller)
        {
            if (controller != null)
            {
                activeControllers.Remove(controller);
            }
        }
    }
}