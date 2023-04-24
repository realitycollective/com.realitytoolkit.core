// Copyright (c) Reality Collective. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using NUnit.Framework;
using RealityCollective.ServiceFramework.Services;
using RealityToolkit.Input.Interfaces;

namespace RealityToolkit.Tests.Input
{
    public class TestFixture_03_InputServiceTests
    {
        [Test]
        public void Test01_CreateInputService()
        {
            TestUtilities.InitializeRealityToolkit();
            var activeSystemCount = ServiceManager.Instance.ActiveServices.Count;

            // Add Input System
            ServiceManager.Instance.ActiveProfile.AddConfiguration(InputServiceTestUtilities.TestInputServiceConfiguration);
            ServiceManager.Instance.TryCreateAndRegisterService(InputServiceTestUtilities.TestInputServiceConfiguration, out var inputService);

            // Tests
            Assert.IsNotEmpty(ServiceManager.Instance.ActiveServices);
            Assert.IsNotNull(inputService);
            Assert.AreEqual(activeSystemCount + 1, ServiceManager.Instance.ActiveServices.Count);
        }

        [Test]
        public void Test02_TestGetInputService()
        {
            TestUtilities.InitializeRealityToolkit();

            // Add Input System
            ServiceManager.Instance.ActiveProfile.AddConfiguration(InputServiceTestUtilities.TestInputServiceConfiguration);
            ServiceManager.Instance.TryCreateAndRegisterService(InputServiceTestUtilities.TestInputServiceConfiguration, out var service);

            Assert.IsNotEmpty(ServiceManager.Instance.ActiveServices);

            // Retrieve Input System
            var inputService = ServiceManager.Instance.GetService<IInputService>();

            // Tests
            Assert.IsNotNull(service);
            Assert.IsNotNull(inputService);
            Assert.IsTrue(ReferenceEquals(service, inputService));
        }

        [Test]
        public void Test03_TestInputServiceDoesNotExist()
        {
            // Initialize without the default profile configuration
            TestUtilities.InitializeRealityToolkit();

            // Check for Input System
            var inputServiceExists = ServiceManager.Instance.IsServiceRegistered<IInputService>();

            // Tests
            Assert.IsFalse(inputServiceExists);
        }

        [Test]
        public void Test04_TestInputServiceExists()
        {
            // Initialize with the default profile configuration
            TestUtilities.InitializeRealityToolkit();

            // Add Input System
            ServiceManager.Instance.ActiveProfile.AddConfiguration(InputServiceTestUtilities.TestInputServiceConfiguration);
            ServiceManager.Instance.TryCreateAndRegisterService(InputServiceTestUtilities.TestInputServiceConfiguration, out _);

            // Check for Input System
            var inputServiceExists = ServiceManager.Instance.IsServiceRegistered<IInputService>();

            // Tests
            Assert.IsTrue(inputServiceExists);
        }

        [TearDown]
        public void CleanupRealityToolkitTests() => TestUtilities.CleanupScene();
    }
}