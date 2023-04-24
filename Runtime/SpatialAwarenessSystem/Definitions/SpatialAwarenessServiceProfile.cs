// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using RealityCollective.ServiceFramework.Definitions;
using RealityToolkit.Definitions.SpatialObservers;
using RealityToolkit.SpatialAwareness.Interfaces.SpatialObservers;
using System;
using UnityEngine;

namespace RealityToolkit.SpatialAwareness.Definitions
{
    /// <summary>
    /// Configuration profile settings for setting up the spatial awareness system.
    /// </summary>
    public class MixedRealitySpatialAwarenessSystemProfile : BaseServiceProfile<IMixedRealitySpatialAwarenessServiceModule>
    {
        public static readonly Tuple<int, string>[] SpatialAwarenessLayers =
        {
            new Tuple<int, string>(30, SpatialAwarenessMeshesLayerName),
            new Tuple<int, string>(31, SpatialAwarenessSurfacesLayerName)
        };

        /// <summary>
        /// The name of the Spatial Awareness Mesh Physics Layer.
        /// </summary>
        public const string SpatialAwarenessMeshesLayerName = "Spatial Awareness Meshes";

        /// <summary>
        /// The name of the Spatial Awareness Surfaces Physics Layer.
        /// </summary>
        public const string SpatialAwarenessSurfacesLayerName = "Spatial Awareness Surfaces";

        [SerializeField]
        [Tooltip("Indicates how the BaseMixedRealitySpatialMeshObserver is to display surface meshes within the application.")]
        private SpatialMeshDisplayOptions meshDisplayOption = SpatialMeshDisplayOptions.None;

        /// <summary>
        /// Indicates how the <see cref="BaseMixedRealitySpatialMeshObserver"/> is to display surface meshes within the application.
        /// </summary>
        public SpatialMeshDisplayOptions MeshDisplayOption => meshDisplayOption;

        [SerializeField]
        [Tooltip("The global mesh observer profile settings to use for the mesh observer service module if no profile is provided.")]
        private BaseMixedRealitySpatialMeshObserverProfile globalMeshObserverProfile = null;

        /// <summary>
        /// The global mesh observer profile settings to use for the <see cref="IMixedRealitySpatialMeshObserver"/>s if no profile is provided.
        /// </summary>
        public BaseMixedRealitySpatialMeshObserverProfile GlobalMeshObserverProfile => globalMeshObserverProfile;

        [SerializeField]
        [Tooltip("The global mesh observer profile settings to use for the mesh observer service module if no profile is provided.")]
        private BaseMixedRealitySurfaceObserverProfile globalSurfaceObserverProfile = null;

        /// <summary>
        /// The global mesh observer profile settings to use for the <see cref="IMixedRealitySpatialMeshObserver"/>s if no profile is provided.
        /// </summary>
        public BaseMixedRealitySurfaceObserverProfile GlobalSurfaceObserverProfile => globalSurfaceObserverProfile;
    }
}
