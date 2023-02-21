// Copyright (c) Reality Collective. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using RealityCollective.ServiceFramework.Interfaces;
using RealityToolkit.CameraService.Definitions;

namespace RealityToolkit.CameraService.Interfaces
{
    /// <summary>
    /// Base interface for implementing camera service modules to be registered with the <see cref="ICameraService"/>
    /// </summary>
    public interface ICameraServiceModule : IServiceModule
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
        /// The <see cref="ICameraRig"/> reference for this service module.
        /// </summary>
        ICameraRig CameraRig { get; }

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