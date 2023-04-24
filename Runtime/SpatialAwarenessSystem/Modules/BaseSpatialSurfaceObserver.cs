// Copyright (c) Reality Collective. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using RealityCollective.Extensions;
using RealityCollective.ServiceFramework.Services;
using RealityToolkit.Definitions.SpatialObservers;
using RealityToolkit.SpatialAwareness.Definitions;
using RealityToolkit.SpatialAwareness.Interfaces;
using RealityToolkit.SpatialAwareness.Interfaces.SpatialObservers;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace RealityToolkit.SpatialAwareness.Modules
{
    /// <summary>
    /// Base <see cref="ISpatialSurfaceObserver"/> implementation.
    /// </summary>
    public abstract class BaseSpatialSurfaceObserver : BaseSpatialObserverServiceModule, ISpatialSurfaceObserver
    {
        /// <inheritdoc />
        protected BaseSpatialSurfaceObserver(string name, uint priority, BaseSurfaceObserverProfile profile, ISpatialAwarenessService parentService)
            : base(name, priority, profile, parentService)
        {
            if (profile.IsNull())
            {
                profile = ServiceManager.Instance.TryGetServiceProfile<ISpatialAwarenessService, SpatialAwarenessSystemProfile>(out var spatialAwarenessSystemProfile)
                    ? spatialAwarenessSystemProfile.GlobalSurfaceObserverProfile
                    : throw new ArgumentException($"Unable to get a valid {nameof(SpatialAwarenessSystemProfile)}!");
            }

            if (profile.IsNull())
            {
                throw new ArgumentNullException($"Missing a {profile.GetType().Name} profile for {name}");
            }

            SurfaceFindingMinimumArea = profile.SurfaceFindingMinimumArea;
            DisplayFloorSurfaces = profile.DisplayFloorSurfaces;
            FloorSurfaceMaterial = profile.FloorSurfaceMaterial;
            DisplayCeilingSurfaces = profile.DisplayCeilingSurface;
            CeilingSurfaceMaterial = profile.CeilingSurfaceMaterial;
            DisplayWallSurfaces = profile.DisplayWallSurface;
            WallSurfaceMaterial = profile.WallSurfaceMaterial;
            DisplayPlatformSurfaces = profile.DisplayPlatformSurfaces;
            PlatformSurfaceMaterial = profile.PlatformSurfaceMaterial;
        }

        /// <inheritdoc />
        public float SurfaceFindingMinimumArea { get; }

        /// <inheritdoc />
        public bool DisplayFloorSurfaces { get; set; }

        /// <inheritdoc />
        public Material FloorSurfaceMaterial { get; }

        /// <inheritdoc />
        public bool DisplayCeilingSurfaces { get; set; }

        /// <inheritdoc />
        public Material CeilingSurfaceMaterial { get; }

        /// <inheritdoc />
        public bool DisplayWallSurfaces { get; set; }

        /// <inheritdoc />
        public Material WallSurfaceMaterial { get; }

        /// <inheritdoc />
        public bool DisplayPlatformSurfaces { get; set; }

        /// <inheritdoc />
        public Material PlatformSurfaceMaterial { get; }

        private readonly Dictionary<int, GameObject> planarSurfaces = new Dictionary<int, GameObject>();

        /// <inheritdoc />
        public IReadOnlyDictionary<int, GameObject> PlanarSurfaces => new Dictionary<int, GameObject>(planarSurfaces);
    }
}