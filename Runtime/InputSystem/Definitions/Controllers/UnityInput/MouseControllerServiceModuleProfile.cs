// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using RealityCollective.Definitions.Utilities;
using RealityToolkit.InputSystem.Controllers.UnityInput;

namespace RealityToolkit.Definitions.Controllers.UnityInput.Profiles
{
    public class MouseControllerServiceModuleProfile : BaseMixedRealityControllerServiceModuleProfile
    {
        public override ControllerDefinition[] GetDefaultControllerOptions()
        {
            return new[]
            {
                new ControllerDefinition(typeof(MouseController), Handedness.Any, true),
            };
        }
    }
}