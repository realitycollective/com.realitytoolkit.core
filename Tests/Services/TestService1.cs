// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using RealityToolkit.ServiceFramework.Services;

namespace RealityToolkit.Tests.Services
{
    internal class TestService1 : BaseServiceWithConstructor, ITestService
    {
        public TestService1(string name = "Test Service 1", uint priority = 0)
            : base(name, priority)
        { }
    }
}