// Copyright (c) Reality Collective. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using RealityCollective.Definitions.Utilities;
using RealityToolkit.Definitions.Controllers;
using RealityToolkit.Input.Controllers;
using System;
using System.Collections.Generic;

namespace RealityToolkit.Input.Interfaces.Modules
{
    /// <summary>
    /// Reality Toolkit service module definition, used to instantiate and manage controllers and joysticks
    /// </summary>
    public interface IControllerServiceModule : IInputServiceModule
    {
        /// <summary>
        /// Retrieve all controllers currently registered with this device at runtime (if direct access is required).
        /// </summary>
        IReadOnlyList<IController> ActiveControllers { get; }

        /// <summary>
        /// Gets the registered controller mapping profile for the provided <see cref="IController"/>
        /// </summary>
        /// <param name="controllerType">The type of the <see cref="IController"/> to lookup the profile for.</param>
        /// <param name="handedness">The <see cref="Handedness"/> the profile should be configured for.</param>
        /// <returns><see cref="ControllerProfile"/> or <c>null</c>.</returns>
        /// <remarks>
        /// Currently you can register more than one controller type and handedness into the
        /// <see cref="BaseControllerServiceModuleProfile"/>, but this method will only return the first one found.
        /// </remarks>
        ControllerProfile GetControllerMappingProfile(Type controllerType, Handedness handedness);

        /// <summary>
        /// Gets the <see cref="ControllerProfile"/> for <paramref name="controllerType"/>, if it exists.
        /// </summary>
        /// <param name="controllerType">The type of the <see cref="IController"/> to lookup the profile for.</param>
        /// <param name="handedness">The <see cref="Handedness"/> the profile should be configured for.</param>
        /// <param name="controllerMappingProfile">The found <see cref="ControllerProfile"/>.</param>
        /// <returns><c>true</c>, if found, otherwise <c>false</c>.</returns>
        bool TryGetControllerMappingProfile(Type controllerType, Handedness handedness, out ControllerProfile controllerMappingProfile);
    }
}