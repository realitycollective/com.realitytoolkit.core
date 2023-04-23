// Copyright (c) Reality Collective. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using RealityCollective.ServiceFramework.Interfaces;
using RealityToolkit.Boundary.Definitions;
using System.Collections.Generic;
using UnityEngine;

namespace RealityToolkit.Boundary.Interfaces
{
    /// <summary>
    /// Boundary service module provide low level data access for the <see cref="IBoundaryService"/> to query
    /// platform boundary state.
    /// </summary>
    public interface IMixedRealityBoundaryServiceModule : IServiceModule
    {
        /// <summary>
        /// Gets the current boundary visibility.
        /// </summary>
        BoundaryVisibility Visibility { get; }

        /// <summary>
        /// Gets whether boundaries have been configured and are active.
        /// </summary>
        bool IsPlatformConfigured { get; }

        /// <summary>
        /// Tries to retrieve up to date boundary points in world space.
        /// </summary>
        /// <param name="geometry">The list of points associated with the boundary geometry.</param>
        /// <returns>True, if valid geometry was successfully returned, otherwise false.</returns>
        bool TryGetBoundaryGeometry(ref List<Vector3> geometry);
    }
}