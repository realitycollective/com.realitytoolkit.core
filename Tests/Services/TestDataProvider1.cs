// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using RealityToolkit.ServiceFramework.Definitions;
using RealityToolkit.ServiceFramework.Providers;

namespace RealityToolkit.Tests.Services
{
    internal class TestDataProvider1 : BaseServiceDataProvider, ITestDataProvider1
    {
        public TestDataProvider1(ITestService parentService, string name = "Test Data Provider 1", uint priority = 1, BaseProfile profile = null)
            : base(name, priority, profile, parentService)
        { }
    }
}