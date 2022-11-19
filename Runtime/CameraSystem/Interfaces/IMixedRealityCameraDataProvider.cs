// Copyright (c) Reality Collective. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using RealityCollective.ServiceFramework.Interfaces;
using RealityToolkit.CameraSystem.Definitions;

namespace RealityToolkit.CameraSystem.Interfaces
{
    /// <summary>
    /// Base interface for implementing camera data providers to be registered with the <see cref="IMixedRealityCameraSystem"/>
    /// </summary>
    public interface IMixedRealityCameraDataProvider : IServiceModule
    {
        /// <summary>
        /// Is the current camera displaying on an Opaque (AR) device or a VR / immersive device
        /// </summary>
        bool IsOpaque { get; }

        /// <summary>
        /// Is the current camera displaying on a traditional 2d screen or a stereoscopic display?
        /// </summary>
        bool IsStereoscopic { get; }

        /// <summary>
        /// The <see cref="IMixedRealityCameraRig"/> reference for this data provider.
        /// </summary>
        IMixedRealityCameraRig CameraRig { get; }

        /// <summary>
        /// The <see cref="Services.CameraSystem.TrackingType"/> this provider is configured to use.
        /// </summary>
        TrackingType TrackingType { get; }

        /// <summary>
        /// The current head height of the player
        /// </summary>
        float HeadHeight { get; }
    }
}