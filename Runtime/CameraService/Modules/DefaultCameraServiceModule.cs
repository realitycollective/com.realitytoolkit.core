// Copyright (c) Reality Collective. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using RealityToolkit.CameraService.Definitions;
using RealityToolkit.CameraService.Interfaces;

namespace RealityToolkit.CameraService.Modules
{
    /// <summary>
    /// Default and general use <see cref="ICameraServiceModule"/> implementation.
    /// </summary>
    [System.Runtime.InteropServices.Guid("EA4C0C19-E533-4AE8-91A2-6998CB8905BB")]
    public class DefaultCameraServiceModule : BaseCameraServiceModule, ICameraServiceModule
    {
        /// <inheritdoc />
        public DefaultCameraServiceModule(string name, uint priority, BaseCameraServiceModuleProfile profile, ICameraService parentService)
            : base(name, priority, profile, parentService) { }
    }
}
