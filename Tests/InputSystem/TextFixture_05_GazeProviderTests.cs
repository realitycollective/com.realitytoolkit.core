// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using NUnit.Framework;
using XRTK.Interfaces.InputSystem;
using XRTK.Services;
using XRTK.Tests.InputSystem;

namespace XRTK.Tests
{
    public class TextFixture_05_GazeProviderTests
    {
        [SetUp]
        public void SetUpGazeProviderTests()
        {
            TestUtilities.InitializeMixedRealityToolkitScene(false);

            MixedRealityToolkit.Instance.ActiveProfile.AddConfiguration(InputSystemTestUtilities.TestInputSystemConfiguration);
            MixedRealityToolkit.TryCreateAndRegisterService(InputSystemTestUtilities.TestInputSystemConfiguration, out var service);
        }

        [Test]
        public void Test01_GazeProviderSetAuto()
        {
            var inputSystem = MixedRealityToolkit.GetService<IMixedRealityInputSystem>();
            inputSystem.SetGazeProviderBehaviour(XRTK.Services.InputSystem.Providers.GazeProviderBehaviour.Auto);

            if (AnyControllerWithPointersAttached(inputSystem))
            {
                Assert.IsNull(inputSystem.GazeProvider);
            }
            else
            {
                Assert.IsNotNull(inputSystem.GazeProvider);
            }
        }

        [Test]
        public void Test02_GazeProviderSetInactive()
        {
            var inputSystem = MixedRealityToolkit.GetService<IMixedRealityInputSystem>();
            inputSystem.SetGazeProviderBehaviour(XRTK.Services.InputSystem.Providers.GazeProviderBehaviour.Inactive);

            Assert.IsNull(inputSystem.GazeProvider);
        }

        [Test]
        public void Test03_GazeProviderSetActive()
        {
            var inputSystem = MixedRealityToolkit.GetService<IMixedRealityInputSystem>();
            inputSystem.SetGazeProviderBehaviour(XRTK.Services.InputSystem.Providers.GazeProviderBehaviour.Active);

            Assert.IsNotNull(inputSystem.GazeProvider);
        }

        [TearDown]
        public void CleanupMixedRealityToolkitTests()
        {
            TestUtilities.CleanupScene();
        }

        private bool AnyControllerWithPointersAttached(IMixedRealityInputSystem inputSystem)
        {
            if (inputSystem.DetectedControllers != null && inputSystem.DetectedControllers.Count > 0)
            {
                foreach (var detectedController in inputSystem.DetectedControllers)
                {
                    if (detectedController.InputSource.Pointers != null && detectedController.InputSource.Pointers.Length > 0)
                    {
                        return true;
                    }
                }
            }

            return false;
        }
    }
}
