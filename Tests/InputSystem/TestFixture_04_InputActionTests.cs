// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using NUnit.Framework;
using RealityCollective.Definitions.Utilities;
using RealityToolkit.Input.Definitions;
using System;

namespace RealityToolkit.Tests.InputSystem
{
    public class TestFixture_04_InputActionTests
    {
        [SetUp]
        public void SetupTests()
        {
            TestUtilities.InitializeRealityToolkit();
        }

        [Test]
        public void Test_01_TestCodeGeneratedActions()
        {
            var pressAction = new InputAction(1, "Pressed", AxisType.Digital);
            var selectAction = new InputAction(2, "Select", AxisType.Digital);

            Assert.IsTrue(selectAction != pressAction);
            Assert.IsTrue(pressAction != InputAction.None);
            Assert.IsTrue(selectAction != InputAction.None);
        }

        [Test]
        public void Test_02_TestBackwardsCompatibility()
        {
            var oldActionWithNoGuid = new InputAction(default, 1, "Select", AxisType.Digital);
            var profileWithGuid = new InputAction(Guid.NewGuid(), 1, "Select", AxisType.Digital);
            Assert.IsTrue(profileWithGuid.ProfileGuid != default);
            Assert.IsTrue(oldActionWithNoGuid == profileWithGuid);
        }

        [TearDown]
        public void CleanupRealityToolkitTests() => TestUtilities.CleanupScene();
    }
}