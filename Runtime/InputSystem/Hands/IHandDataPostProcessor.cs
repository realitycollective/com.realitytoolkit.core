// Copyright (c) Reality Collective. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace RealityToolkit.InputSystem.Hands
{
    /// <summary>
    /// Performs additional hand post processing after <see cref="TrackedHandJoint"/>
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