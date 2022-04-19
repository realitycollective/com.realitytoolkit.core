// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using RealityToolkit.ServiceFramework.Services;

namespace XRTK.Tests.Services
{
    internal class TestService1 : BaseServiceWithConstructor, ITestService
    {
        public const string TestName = "Test Service 1";

        public TestService1(string name = TestName, uint priority = 0)
            : base(name, priority)
        { }
    }
}