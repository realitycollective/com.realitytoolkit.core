// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using NUnit.Framework;
using RealityToolkit.Definitions.Controllers;
using RealityToolkit.Definitions.Controllers.UnityInput.Profiles;
using RealityToolkit.Editor.Utilities;
using RealityToolkit.Extensions;
using RealityToolkit.ServiceFramework.Definitions.Platforms;
using RealityToolkit.ServiceFramework.Interfaces;
using RealityToolkit.Services.InputSystem.Controllers.OpenVR;
using RealityToolkit.Utilities;
using System.Collections.Generic;
using UnityEngine;

namespace RealityToolkit.Tests.Core
{
    public class TestFixture_02_MixedRealityToolkitUtilityTests
    {
        private void SetupServiceLocator()
        {
            TestUtilities.InitializeMixedRealityToolkitScene(false);
        }

        private readonly List<IPlatform> testPlatforms = new List<IPlatform> { new EditorPlatform(), new WindowsStandalonePlatform() };

        [Test]
        public void Test_01_ConfirmExtensionServiceProviderConfigurationNotPresent()
        {
            SetupServiceLocator();
        }

        [Test]
        public void Test_03_ConfirmControllerMappingConfigurationNotPresent()
        {
            SetupServiceLocator();
            var controllerTypes = new[] { typeof(GenericOpenVRController) };

            var controllerDataMappingProfile = ScriptableObject.CreateInstance<UnityInputControllerDataProfile>();

            Assert.IsFalse(controllerDataMappingProfile.ValidateControllerProfiles(controllerTypes, false));
        }

        [Test]
        public void Test_04_ConfirmGenereicControllerTextureExists()
        {
            var controllerMappingProfile = ScriptableObject.CreateInstance<MixedRealityControllerMappingProfile>();
            controllerMappingProfile.ControllerType = typeof(GenericOpenVRController);

            // Right / Any hand textures
            controllerMappingProfile.Handedness = Definitions.Utilities.Handedness.Right;
            var controllerTexture = ControllerMappingUtilities.GetControllerTextureScaled(controllerMappingProfile);
            Assert.IsNotNull(controllerTexture);

            // Left hand textures
            controllerMappingProfile.Handedness = Definitions.Utilities.Handedness.Left;
            controllerTexture = ControllerMappingUtilities.GetControllerTextureScaled(controllerMappingProfile);
            Assert.IsNotNull(controllerTexture);
        }

        [Test]
        public void Test_05_ConfirmGenereicControllerTextureDoesNotExist()
        {
            var controllerMappingProfile = ScriptableObject.CreateInstance<MixedRealityControllerMappingProfile>();
            controllerMappingProfile.ControllerType = typeof(GenericOpenVRController);

            // Right / Any hand textures
            controllerMappingProfile.Handedness = Definitions.Utilities.Handedness.Right;
            var controllerTexture = ControllerMappingUtilities.GetControllerTexture(controllerMappingProfile);
            Assert.IsNull(controllerTexture); // For generic controllers we expect non-scaled texture to not exist.

            // Left hand textures
            controllerMappingProfile.Handedness = Definitions.Utilities.Handedness.Left;
            controllerTexture = ControllerMappingUtilities.GetControllerTexture(controllerMappingProfile);
            Assert.IsNull(controllerTexture); // For generic controllers we expect non-scaled texture to not exist.
        }


        [Test]
        public void Test_06_ConfirmProfileControllerTextureUsed()
        {
            var controllerMappingProfile = ScriptableObject.CreateInstance<MixedRealityControllerMappingProfile>();
            controllerMappingProfile.ControllerType = typeof(GenericOpenVRController);

            var dummyTexture = new Texture2D(1, 1, TextureFormat.ARGB32, false);
            controllerMappingProfile.LightThemeLeftControllerTexture = dummyTexture;
            controllerMappingProfile.LightThemeLeftControllerTextureScaled = dummyTexture;
            controllerMappingProfile.LightThemeRightControllerTexture = dummyTexture;
            controllerMappingProfile.LightThemeRightControllerTextureScaled = dummyTexture;
            controllerMappingProfile.DarkThemeLeftControllerTexture = dummyTexture;
            controllerMappingProfile.DarkThemeLeftControllerTextureScaled = dummyTexture;
            controllerMappingProfile.DarkThemeRightControllerTexture = dummyTexture;
            controllerMappingProfile.DarkThemeRightControllerTextureScaled = dummyTexture;

            // Right / Any hand textures
            controllerMappingProfile.Handedness = Definitions.Utilities.Handedness.Right;
            var controllerTexture = ControllerMappingUtilities.GetControllerTexture(controllerMappingProfile);
            Assert.AreSame(controllerTexture, dummyTexture);
            controllerTexture = ControllerMappingUtilities.GetControllerTextureScaled(controllerMappingProfile);
            Assert.AreSame(controllerTexture, dummyTexture);

            // Left hand textures
            controllerMappingProfile.Handedness = Definitions.Utilities.Handedness.Left;
            controllerTexture = ControllerMappingUtilities.GetControllerTexture(controllerMappingProfile);
            Assert.AreSame(controllerTexture, dummyTexture);
            controllerTexture = ControllerMappingUtilities.GetControllerTextureScaled(controllerMappingProfile);
            Assert.AreSame(controllerTexture, dummyTexture);

            dummyTexture.Destroy();
        }
    }
}
