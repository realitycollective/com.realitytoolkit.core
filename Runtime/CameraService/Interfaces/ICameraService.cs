// Copyright (c) Reality Collective. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using RealityCollective.ServiceFramework.Interfaces;
using UnityEngine.XR;

namespace RealityToolkit.CameraService.Interfaces
{
    /// <summary>
    /// The base interface for implementing a mixed reality camera system.
    /// </summary>
    public interface ICameraService : IService
    {
        /// <summary>
        /// The reference to the <see cref="ICameraRig"/> attached to the Main Camera (typically this is the player's camera).
        /// </summary>
        ICameraRig CameraRig { get; }

        /// <summary>
        /// The active <see cref="ICameraServiceModule"/> managing the <see cref="CameraRig"/>.
        /// </summary>
        ICameraServiceModule CameraServiceModule { get; }

        /// <summary>
        /// Gets the active <see cref="XRDisplaySubsystem"/> for the currently loaded
        /// XR plugin / platform.
        /// </summary>
        /// <remarks>The reference is lazy loaded once on first access and then cached for future use.</remarks>
        XRDisplaySubsystem DisplaySubsystem { get; }
    }
}