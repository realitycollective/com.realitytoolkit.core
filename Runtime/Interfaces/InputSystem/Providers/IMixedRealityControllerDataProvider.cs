// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;
using XRTK.Definitions.Controllers;
using XRTK.Definitions.Utilities;
using XRTK.Interfaces.InputSystem.Controllers;

namespace XRTK.Interfaces.InputSystem.Providers.Controllers
{
    /// <summary>
    /// Mixed Reality Toolkit data provider definition, used to instantiate and manage controllers and joysticks
    /// </summary>
    public interface IMixedRealityControllerDataProvider : IMixedRealityInputDataProvider
    {
        /// <summary>
        /// Retrieve all controllers currently registered with this device at runtime (if direct access is required).
        /// </summary>
        IReadOnlyList<IMixedRealityController> ActiveControllers { get; }

        /// <summary>
        /// Gets the registered controller mapping profile for the provided <see cref="IMixedRealityController"/>
        /// </summary>
        /// <param name="controllerType">The type of the <see cref="IMixedRealityController"/> to lookup the profile for.</param>
        /// <param name="handedness">The <see cref="Handedness"/> the profile should be configured for.</param>
        /// <returns><see cref="MixedRealityControllerMappingProfile"/> or <c>null</c>.</returns>
        /// <remarks>
        /// Currently you can register more than one controller type and handedness into the
        /// <see cref="BaseMixedRealityControllerDataProviderProfile"/>, but this method will only return the first one found.
        /// </remarks>
        MixedRealityControllerMappingProfile GetControllerMappingProfile(Type controllerType, Handedness handedness);

        /// <summary>
        /// Gets the <see cref="MixedRealityControllerMappingProfile"/> for <paramref name="controllerType"/>, if it exists.
        /// </summary>
        /// <param name="controllerType">The type of the <see cref="IMixedRealityController"/> to lookup the profile for.</param>
        /// <param name="handedness">The <see cref="Handedness"/> the profile should be configured for.</param>
        /// <param name="controllerMappingProfile">The found <see cref="MixedRealityControllerMappingProfile"/>.</param>
        /// <returns><c>true</c>, if found, otherwise <c>false</c>.</returns>
        bool TryGetControllerMappingProfile(Type controllerType, Handedness handedness, out MixedRealityControllerMappingProfile controllerMappingProfile);
    }
}