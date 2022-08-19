// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using RealityCollective.ServiceFramework.Definitions;
using RealityCollective.ServiceFramework.Definitions.Platforms;
using RealityToolkit.InputSystem;
using RealityToolkit.InputSystem.Definitions;
using RealityToolkit.InputSystem.Interfaces;
using RealityToolkit.InputSystem.Providers;
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
            inputSystemProfile.GazeProviderBehaviour = GazeProviderBehaviour.Auto;
            inputSystemProfile.GazeProviderType = typeof(GazeProvider);
            inputSystemProfile.InputActionsProfile = ScriptableObject.CreateInstance<MixedRealityInputActionsProfile>();
            inputSystemProfile.GesturesProfile = ScriptableObject.CreateInstance<MixedRealityGesturesProfile>();
            inputSystemProfile.SpeechCommandsProfile = ScriptableObject.CreateInstance<MixedRealitySpeechCommandsProfile>();

            return inputSystemProfile;
        }
    }
}