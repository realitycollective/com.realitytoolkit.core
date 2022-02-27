// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace XRTK.Services.InputSystem
{
    /// <summary>
    /// Available options for how the configured <see cref="Interfaces.InputSystem.IMixedRealityGazeProvider"/>
    /// should behave.
    /// </summary>
    public enum GazeProviderBehaviour
    {
        /// <summary>
        /// The <see cref="Interfaces.InputSystem.IMixedRealityGazeProvider"/> will be auto-activated
        /// when there is no other <see cref="Interfaces.Providers.Controllers.IMixedRealityController"/> with at least one
        /// <see cref="Interfaces.InputSystem.IMixedRealityPointer"/> attached available. And will become inactive
        /// as soon as one is available.
        /// </summary>
        Auto = 0,
        /// <summary>
        /// The <see cref="Interfaces.InputSystem.IMixedRealityGazeProvider"/> stays inactive until manually enabled.
        /// </summary>
        Inactive,
        /// <summary>
        /// The <see cref="Interfaces.InputSystem.IMixedRealityGazeProvider"/> is always active.
        /// </summary>
        Active
    }
}
