// Copyright (c) Reality Collective. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine.XR;
using RealityToolkit.Definitions.Controllers.Hands;

namespace RealityToolkit.Interfaces.InputSystem.Providers.Controllers.Hands
{
    public interface IUnityXRHandMeshDataProvider
    {
        /// <summary>
        /// Gets updated <see cref="HandMeshData"/>.
        /// </summary>
        /// <param name="inputDevice">The <see cref="InputDevice"/> to read hand mesh data for.</param>
        /// <returns>Updated <see cref="HandMeshData"/>.</returns>
        HandMeshData UpdateHandMesh(InputDevice inputDevice);
    }
}
