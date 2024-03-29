// Copyright (c) Reality Collective. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace RealityToolkit.Input.Definitions
{
    /// <summary>
    /// Available options for how the configured <see cref="Interfaces.Input.IGazeProvider"/>
    /// should behave.
    /// </summary>
    public enum GazeProviderBehaviour
    {
        /// <summary>
        /// The <see cref="Interfaces.Input.IGazeProvider"/> will be auto-activated
        /// when there is no other <see cref="Interfaces.Providers.Controllers.IController"/> with at least one
        /// <see cref="Interfaces.Input.IPointer"/> attached available. And will become inactive
        /// as soon as one is available.
        /// </summary>
        Auto = 0,
        /// <summary>
        /// The <see cref="Interfaces.Input.IGazeProvider"/> stays inactive until manually enabled.
        /// </summary>
        Inactive,
        /// <summary>
        /// The <see cref="Interfaces.Input.IGazeProvider"/> is always active.
        /// </summary>
        Active
    }
}
