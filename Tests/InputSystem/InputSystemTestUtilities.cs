// Copyright (c) Reality Collective. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using RealityCollective.ServiceFramework.Definitions;
using RealityCollective.ServiceFramework.Definitions.Platforms;
using RealityToolkit.Input;
using RealityToolkit.Input.Definitions;
using RealityToolkit.Input.Interfaces;
using RealityToolkit.Input.Modules;
using UnityEngine;

namespace RealityToolkit.Tests.InputSystem
{
    public static class InputSystemTestUtilities
    {
        public static ServiceConfiguration<IInputService> TestInputSystemConfiguration
            => new ServiceConfiguration<IInputService>(typeof(InputService), nameof(InputService), 1, new[] { new AllPlatforms() }, SetupInputSystemProfile());

        public static InputServiceProfile SetupInputSystemProfile()
        {
            // Create blank Input System Profiles
            var inputSystemProfile = ScriptableObject.CreateInstance<InputServiceProfile>();
            inputSystemProfile.GazeProviderBehaviour = GazeProviderBehaviour.Auto;
            inputSystemProfile.GazeProviderType = typeof(GazeProvider);
            inputSystemProfile.InputActionsProfile = ScriptableObject.CreateInstance<InputActionsProfile>();
            inputSystemProfile.GesturesProfile = ScriptableObject.CreateInstance<GesturesProfile>();
            inputSystemProfile.SpeechCommandsProfile = ScriptableObject.CreateInstance<SpeechCommandsProfile>();
            inputSystemProfile.AddConfiguration(new ServiceConfiguration<IFocusProvider>(typeof(FocusProvider), nameof(FocusProvider), 0, AllPlatforms.Platforms, null));

            return inputSystemProfile;
        }
    }
}