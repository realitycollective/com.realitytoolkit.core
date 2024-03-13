// Copyright (c) Reality Collective. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using NUnit.Framework;
using RealityCollective.ServiceFramework.Services;
using RealityToolkit.Input.Definitions;
using RealityToolkit.Input.Interfaces;

namespace RealityToolkit.Tests.Input
{
    public class TextFixture_05_GazeProviderTests
    {
        [SetUp]
        public void SetUpGazeProviderTests()
        {
            TestUtilities.InitializeRealityToolkit();

            ServiceManager.Instance.ActiveProfile.AddConfiguration(InputServiceTestUtilities.TestInputServiceConfiguration);
            ServiceManager.Instance.TryCreateAndRegisterService(InputServiceTestUtilities.TestInputServiceConfiguration, out var service);
        }

        [Test]
        public void Test01_GazeProviderSetAuto()
        {
            var inputService = ServiceManager.Instance.GetService<IInputService>();
            inputService.SetGazeProviderBehaviour(GazeProviderBehaviour.Auto);

            if (AnyControllerWithPointersAttached(inputService))
            {
                Assert.IsNull(inputService.GazeProvider);
            }
            //else
            //{
            //    Assert.IsNotNull(inputService.GazeProvider);
            //}
        }

        [Test]
        public void Test02_GazeProviderSetInactive()
        {
            var inputService = ServiceManager.Instance.GetService<IInputService>();
            inputService.SetGazeProviderBehaviour(GazeProviderBehaviour.Inactive);

            Assert.IsNull(inputService.GazeProvider);
        }

        [Test]
        public void Test03_GazeProviderSetActive()
        {
            var inputService = ServiceManager.Instance.GetService<IInputService>();
            inputService.SetGazeProviderBehaviour(GazeProviderBehaviour.Active);

            Assert.IsNotNull(inputService.GazeProvider);
        }

        [TearDown]
        public void CleanupRealityToolkitTests() => TestUtilities.CleanupScene();

        private bool AnyControllerWithPointersAttached(IInputService inputService)
        {
            if (inputService.DetectedControllers != null && inputService.DetectedControllers.Count > 0)
            {
                foreach (var detectedController in inputService.DetectedControllers)
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
