// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using XRTK.Definitions;
using XRTK.Definitions.Controllers;
using XRTK.Definitions.Controllers.UnityInput.Profiles;
using XRTK.Definitions.Platforms;
using XRTK.Editor.Utilities;
using XRTK.Extensions;
using XRTK.Interfaces;
using XRTK.Services;
using XRTK.Services.InputSystem.Controllers.OpenVR;
using XRTK.Tests.Services;
using XRTK.Utilities;

namespace XRTK.Tests.Core
{
    public class TestFixture_02_MixedRealityToolkitUtilityTests
    {
        /*
        private void SetupServiceLocator()
        {
            TestUtilities.InitializeMixedRealityToolkitScene(false);
            MixedRealityToolkit.Instance.ActiveProfile.RegisteredServiceProvidersProfile = ScriptableObject.CreateInstance<MixedRealityRegisteredServiceProvidersProfile>();
        }

        private readonly List<IMixedRealityPlatform> testPlatforms = new List<IMixedRealityPlatform> { new EditorPlatform(), new WindowsStandalonePlatform() };

        [Test]
        public void Test_01_ConfirmExtensionServiceProviderConfigurationNotPresent()
        {
            SetupServiceLocator();
            var profile = MixedRealityToolkit.Instance.ActiveProfile.RegisteredServiceProvidersProfile;
            var dataProviderTypes = new[] { typeof(TestExtensionService1) };
            IMixedRealityServiceConfiguration<IMixedRealityExtensionService>[] newConfigs =
            {
                new MixedRealityServiceConfiguration<IMixedRealityExtensionService>(typeof(TestExtensionService1), "Test Extension Service 1", 2, testPlatforms, null)
            };

            Assert.IsFalse(profile.ValidateService(dataProviderTypes, newConfigs, false));
        }

        [Test]
        public void Test_02_ConfirmExtensionServiceProviderConfigurationPresent()
        {
            SetupServiceLocator();
            var profile = MixedRealityToolkit.Instance.ActiveProfile.RegisteredServiceProvidersProfile;
            var dataProviderTypes = new[] { typeof(TestExtensionService1) };
            var newConfig = new MixedRealityServiceConfiguration<IMixedRealityExtensionService>(typeof(TestExtensionService1), "Test Extension Service 1", 2, testPlatforms, null);
            Debug.Assert(newConfig != null);
            var newConfigs = profile.RegisteredServiceConfigurations.AddItem(newConfig);
            Debug.Assert(newConfigs != null);
            profile.RegisteredServiceConfigurations = newConfigs;
            Assert.IsTrue(profile.ValidateService(dataProviderTypes, newConfigs, false));
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
        */
    }
}
