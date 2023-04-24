// Copyright (c) Reality Collective. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using RealityCollective.ServiceFramework.Definitions;
using RealityCollective.ServiceFramework.Definitions.Platforms;
using RealityToolkit.Input;
using RealityToolkit.Input.Definitions;
using RealityToolkit.Input.Interfaces;
using RealityToolkit.Input.Modules;
using UnityEngine;

namespace RealityToolkit.Tests.Input
{
    public static class InputServiceTestUtilities
    {
        public static ServiceConfiguration<IInputService> TestInputServiceConfiguration
            => new ServiceConfiguration<IInputService>(typeof(InputService), nameof(InputService), 1, new[] { new AllPlatforms() }, SetupInputServiceProfile());

        public static InputServiceProfile SetupInputServiceProfile()
        {
            // Create blank Input System Profiles
            var inputServiceProfile = ScriptableObject.CreateInstance<InputServiceProfile>();
            inputServiceProfile.GazeProviderBehaviour = GazeProviderBehaviour.Auto;
            inputServiceProfile.GazeProviderType = typeof(GazeProvider);
            inputServiceProfile.InputActionsProfile = ScriptableObject.CreateInstance<InputActionsProfile>();
            inputServiceProfile.GesturesProfile = ScriptableObject.CreateInstance<GesturesProfile>();
            inputServiceProfile.SpeechCommandsProfile = ScriptableObject.CreateInstance<SpeechCommandsProfile>();
            inputServiceProfile.AddConfiguration(new ServiceConfiguration<IFocusProvider>(typeof(FocusProvider), nameof(FocusProvider), 0, AllPlatforms.Platforms, null));

            return inputServiceProfile;
        }
    }
}