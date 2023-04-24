// Copyright (c) Reality Collective. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using RealityCollective.Definitions.Utilities;
using RealityToolkit.Definitions.Controllers.Hands;

namespace RealityToolkit.Input.Interfaces.Controllers.Hands
{
    /// <summary>
    /// <see cref="HandData"/> post processor definition.
    /// A post processor may recieve <see cref="HandData"/>
    /// from a <see cref="IMixedRealityHandControllerDataProvider"/> just before the actual
    /// <see cref="IHandController"/> is updated with it to perform last minute
    /// processing on it.
    /// </summary>
    public interface IHandDataPostProcessor
    {
        /// <summary>
        /// Performs post processing on the provided <see cref="HandData"/>.
        /// </summary>
        /// <param name="handedness">The <see cref="Handedness"/> of the <see cref="IHandController"/> the
        /// data is being prepared for.</param>
        /// <param name="handData">The <see cref="HandData"/> provided by the <see cref="IMixedRealityHandControllerDataProvider"/>.</param>
        /// <returns>Returns modified <see cref="HandData"/> after post processing was applied.</returns>
        HandData PostProcess(Handedness handedness, HandData handData);
    }
}