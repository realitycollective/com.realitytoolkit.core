// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace RealityToolkit.InputSystem.Interfaces.Modules
{
    public interface ISimulatedHandControllerServiceModule : ISimulatedControllerServiceModule, IMixedRealityHandControllerServiceModule
    {
        /// <summary>
        /// Gets the simulated hand controller pose animation speed controlling
        /// how fast the hand will translate from one pose to another.
        /// </summary>
        float HandPoseAnimationSpeed { get; }
    }
}