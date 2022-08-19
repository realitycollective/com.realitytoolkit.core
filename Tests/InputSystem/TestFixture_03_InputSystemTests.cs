// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using NUnit.Framework;
using RealityCollective.ServiceFramework.Services;
using RealityToolkit.InputSystem.Interfaces;
using UnityEngine;
using UnityEngine.TestTools;

namespace RealityToolkit.Tests.InputSystem
{
    public class TestFixture_03_InputSystemTests
    {
        [Test]
        public void Test01_CreateMixedRealityInputSystem()
        {
            TestUtilities.InitializeMixedRealityToolkitScene(false);
            var activeSystemCount = ServiceManager.Instance.ActiveServices.Count;

            // Add Input System
            ServiceManager.Instance.ActiveProfile.AddConfiguration(InputSystemTestUtilities.TestInputSystemConfiguration);
            ServiceManager.Instance.TryCreateAndRegisterService(InputSystemTestUtilities.TestInputSystemConfiguration, out var inputSystem);

            // Tests
            Assert.IsNotEmpty(ServiceManager.Instance.ActiveServices);
            Assert.IsNotNull(inputSystem);
            Assert.AreEqual(activeSystemCount + 1, ServiceManager.Instance.ActiveServices.Count);
        }

        [Test]
        public void Test02_TestGetMixedRealityInputSystem()
        {
            TestUtilities.InitializeMixedRealityToolkitScene(false);

            // Add Input System
            ServiceManager.Instance.ActiveProfile.AddConfiguration(InputSystemTestUtilities.TestInputSystemConfiguration);
            ServiceManager.Instance.TryCreateAndRegisterService(InputSystemTestUtilities.TestInputSystemConfiguration, out var service);

            Assert.IsNotEmpty(ServiceManager.Instance.ActiveServices);

            // Retrieve Input System
            var inputSystem = ServiceManager.Instance.GetService<IMixedRealityInputSystem>();

            // Tests
            Assert.IsNotNull(service);
            Assert.IsNotNull(inputSystem);
            Assert.IsTrue(ReferenceEquals(service, inputSystem));
        }

        [Test]
        public void Test03_TestMixedRealityInputSystemDoesNotExist()
        {
            // Initialize without the default profile configuration
            TestUtilities.InitializeMixedRealityToolkitScene(false);

            // Check for Input System
            var inputSystemExists = ServiceManager.Instance.IsServiceRegistered<IMixedRealityInputSystem>();

            // Tests
            Assert.IsFalse(inputSystemExists);
            LogAssert.Expect(LogType.Error, $"Unable to find {nameof(IMixedRealityInputSystem)} service.");
        }

        [Test]
        public void Test04_TestMixedRealityInputSystemExists()
        {
            // Initialize with the default profile configuration
            TestUtilities.InitializeMixedRealityToolkitScene(false);

            // Add Input System
            ServiceManager.Instance.ActiveProfile.AddConfiguration(InputSystemTestUtilities.TestInputSystemConfiguration);
            ServiceManager.Instance.TryCreateAndRegisterService(InputSystemTestUtilities.TestInputSystemConfiguration, out _);

            // Check for Input System
            var inputSystemExists = ServiceManager.Instance.IsServiceRegistered<IMixedRealityInputSystem>();

            // Tests
            Assert.IsTrue(inputSystemExists);
        }

        [TearDown]
        public void CleanupMixedRealityToolkitTests()
        {
            TestUtilities.CleanupScene();
        }
    }
}