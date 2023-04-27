// Copyright (c) Reality Collective. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using RealityToolkit.Definitions.Devices;

namespace RealityToolkit.Definitions.Utilities
{
    /// <summary>
    /// Influences how the <see cref="InteractionMapping"/> determines state changes that will raise the <see cref="InputAction"/>.
    /// </summary>
    public enum StateChangeType
    {
        /// <summary>
        /// Use this for any <see cref="InteractionMapping"/> which should trigger continuous <see cref="InputAction"/>s from the Control.
        /// </summary>
        Continuous = 0,
        /// <summary>
        ///  Use this for <see cref="InteractionMapping"/> that only trigger an <see cref="InputAction"/> once until the Control is reset to its default state.
        /// </summary>
        Trigger,
    }
}