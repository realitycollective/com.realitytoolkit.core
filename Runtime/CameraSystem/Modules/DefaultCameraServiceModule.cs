// Copyright (c) Reality Collective. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using RealityToolkit.CameraSystem.Definitions;
using RealityToolkit.CameraSystem.Interfaces;

namespace RealityToolkit.CameraSystem.Modules
{
    /// <summary>
    /// Default and general use <see cref="IMixedRealityCameraServiceModule"/> implementation.
    /// </summary>
    [System.Runtime.InteropServices.Guid("EA4C0C19-E533-4AE8-91A2-6998CB8905BB")]
    public class DefaultCameraServiceModule : BaseCameraServiceModule, IMixedRealityCameraServiceModule
    {
        /// <inheritdoc />
        public DefaultCameraServiceModule(string name, uint priority, BaseMixedRealityCameraServiceModuleProfile profile, IMixedRealityCameraSystem parentService)
            : base(name, priority, profile, parentService) { }
    }
}
