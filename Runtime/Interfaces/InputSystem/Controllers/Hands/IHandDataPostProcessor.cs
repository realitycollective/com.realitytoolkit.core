// Copyright (c) Reality Collective. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using RealityToolkit.Definitions.Controllers.Hands;

namespace RealityToolkit.Interfaces.InputSystem.Controllers.Hands
{
    /// <summary>
    /// Performs additional hand post processing after <see cref="XRHandJoint"/>
    /// pose information has been updated and is available.
    /// </summary>
    public interface IHandDataPostProcessor
    {
        /// <summary>
        /// Performs post processing on the provided <see cref="IHandController"/>.
        /// </summary>
        /// <param name="handData">Available <see cref="HandData"/> from a previous step.</param>
        /// <returns>Returns <see cref="HandData"/> with post processing results.</returns>
        HandData PostProcess(HandData handData);
    }
}