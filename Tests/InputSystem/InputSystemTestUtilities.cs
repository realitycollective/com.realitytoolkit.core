// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using RealityToolkit.Definitions.InputSystem;
using RealityToolkit.Interfaces.InputSystem;
using RealityToolkit.ServiceFramework.Definitions;
using RealityToolkit.ServiceFramework.Definitions.Platforms;
using RealityToolkit.Services.InputSystem;
using RealityToolkit.Services.InputSystem.Providers;
using UnityEngine;

namespace RealityToolkit.Tests.InputSystem
{
    public static class InputSystemTestUtilities
    {
        public static ServiceConfiguration<IMixedRealityInputSystem> TestInputSystemConfiguration
            => new ServiceConfiguration<IMixedRealityInputSystem>(typeof(MixedRealityInputSystem), nameof(MixedRealityInputSystem), 1, new[] { new AllPlatforms() }, SetupInputSystemProfile());

        public static MixedRealityInputSystemProfile SetupInputSystemProfile()
        {
            // Create blank Input System Profiles
            var inputSystemProfile = ScriptableObject.CreateInstance<MixedRealityInputSystemProfile>();
            inputSystemProfile.FocusProviderType = typeof(FocusProvider);
            inputSystemProfile.GazeProviderBehaviour = GazeProviderBehaviour.Auto;
            inputSystemProfile.GazeProviderType = typeof(GazeProvider);
            inputSystemProfile.InputActionsProfile = ScriptableObject.CreateInstance<MixedRealityInputActionsProfile>();
            inputSystemProfile.GesturesProfile = ScriptableObject.CreateInstance<MixedRealityGesturesProfile>();
            inputSystemProfile.SpeechCommandsProfile = ScriptableObject.CreateInstance<MixedRealitySpeechCommandsProfile>();

            return inputSystemProfile;
        }
    }
}