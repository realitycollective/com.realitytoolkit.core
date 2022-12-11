﻿// Copyright (c) Reality Collective. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using RealityToolkit.InputSystem.Interfaces.Modules;

namespace RealityToolkit.InputSystem.Hands
{
    public interface IHandControllerServiceModule : IMixedRealityControllerServiceModule
    {
        /// <summary>
        /// Are hand physics enabled?
        /// </summary>
        bool HandPhysicsEnabled { get; set; }

        /// <summary>
        /// Shall hand colliders be triggers?
        /// </summary>
        bool UseTriggers { get; set; }

        /// <summary>
        /// Gets the configured hand bounds mode to be used with hand physics.
        /// </summary>
        HandBoundsLOD BoundsMode { get; set; }
    }
}