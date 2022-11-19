﻿// Copyright (c) Reality Collective. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using RealityCollective.ServiceFramework.Interfaces;
using RealityToolkit.BoundarySystem.Definitions;
using System.Collections.Generic;
using UnityEngine;

namespace RealityToolkit.BoundarySystem.Interfaces
{
    /// <summary>
    /// Boundary data providers provide low level data access for the <see cref="IMixedRealityBoundarySystem"/> to query
    /// platform boundary state.
    /// </summary>
    public interface IMixedRealityBoundaryDataProvider : IServiceModule
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