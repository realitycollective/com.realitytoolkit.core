﻿// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using RealityToolkit.Definitions.Utilities;
using RealityToolkit.Services.InputSystem.Controllers.UnityInput;

namespace RealityToolkit.Definitions.Controllers.UnityInput.Profiles
{
    [System.Runtime.InteropServices.Guid("344B09FD-88CA-4C4D-BD90-0F406771CF3D")]
    public class TouchScreenControllerDataProviderProfile : BaseMixedRealityControllerDataProviderProfile
    {
        public override ControllerDefinition[] GetDefaultControllerOptions()
        {
            return new[]
            {
                new ControllerDefinition(typeof(UnityTouchController), Handedness.Any, true)
            };
        }
    }
}