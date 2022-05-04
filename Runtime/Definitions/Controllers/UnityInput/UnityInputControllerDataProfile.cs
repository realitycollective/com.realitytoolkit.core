// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using RealityToolkit.Definitions.Utilities;
using RealityToolkit.Services.InputSystem.Controllers.UnityInput;

namespace RealityToolkit.Definitions.Controllers.UnityInput.Profiles
{
    public class UnityInputControllerDataProfile : BaseMixedRealityControllerDataProviderProfile
    {
        public override ControllerDefinition[] GetDefaultControllerOptions()
        {
            return new[]
            {
                new ControllerDefinition(typeof(GenericJoystickController), Handedness.None, true),
                new ControllerDefinition(typeof(XboxController))
            };
        }
    }
}