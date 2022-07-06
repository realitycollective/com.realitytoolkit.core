// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace RealityToolkit.Definitions.Controllers.Hands
{
    /// <summary>
    /// Available hand bounds modes.
    /// </summary>
    public enum HandBoundsLOD
    {
        /// <summary>
        /// Hand bounds are not calculated.
        /// </summary>
        None = 0,
        /// <summary>
        /// Hand bounds mode will only calculate a single bounding
        /// box encapsulating the whole hand.
        /// </summary>
        Low,
        /// <summary>
        /// Fingers bounds mode will create precise bounds for each finger
        /// and the palm to allow for more precise interactions.
        /// </summary>
        High
    }
}