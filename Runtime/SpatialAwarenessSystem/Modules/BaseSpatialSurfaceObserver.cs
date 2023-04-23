// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using RealityCollective.Extensions;
using RealityCollective.ServiceFramework.Services;
using RealityToolkit.Definitions.SpatialObservers;
using RealityToolkit.SpatialAwarenessSystem.Definitions;
using RealityToolkit.SpatialAwarenessSystem.Interfaces;
using RealityToolkit.SpatialAwarenessSystem.Interfaces.SpatialObservers;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace RealityToolkit.SpatialAwarenessSystem.Modules
{
    /// <summary>
    /// Base <see cref="IMixedRealitySpatialSurfaceObserver"/> implementation.
    /// </summary>
    public abstract class BaseMixedRealitySpatialSurfaceObserver : BaseMixedRealitySpatialObserverServiceModule, IMixedRealitySpatialSurfaceObserver
    {
        /// <inheritdoc />
        protected BaseMixedRealitySpatialSurfaceObserver(string name, uint priority, BaseMixedRealitySurfaceObserverProfile profile, IMixedRealitySpatialAwarenessSystem parentService)
            : base(name, priority, profile, parentService)
        {
            if (profile.IsNull())
            {
                profile = ServiceManager.Instance.TryGetServiceProfile<IMixedRealitySpatialAwarenessSystem, MixedRealitySpatialAwarenessSystemProfile>(out var spatialAwarenessSystemProfile)
                    ? spatialAwarenessSystemProfile.GlobalSurfaceObserverProfile
                    : throw new ArgumentException($"Unable to get a valid {nameof(MixedRealitySpatialAwarenessSystemProfile)}!");
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