// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine.XR;
using XRTK.Services.CameraSystem;

namespace XRTK.Interfaces.CameraSystem
{
    /// <summary>
    /// The base interface for implementing a mixed reality camera system.
    /// </summary>
    public interface IMixedRealityCameraSystem : IMixedRealitySystem
    {
        /// <summary>
        /// The active <see cref="IMixedRealityCameraDataProvider"/>.
        /// </summary>
        IMixedRealityCameraDataProvider CameraDataProvider { get; }

        /// <summary>
        /// The reference to the <see cref="IMixedRealityCameraRig"/> attached to the Main Camera (typically this is the player's camera).
        /// </summary>
        IMixedRealityCameraRig MainCameraRig { get; }

        /// <summary>
        /// Gets the configured <see cref="TrackingType"/> for the active <see cref="IMixedRealityCameraRig"/>.
        /// </summary>
        TrackingType TrackingType { get; }

        /// <summary>
        /// Gets the active <see cref="XRDisplaySubsystem"/> for the currently loaded
        /// XR plugin / platform.
        /// </summary>
        /// <remarks>The reference is lazy loaded once on first access and then cached for future use.</remarks>
        XRDisplaySubsystem DisplaySubsystem { get; }
    }
}