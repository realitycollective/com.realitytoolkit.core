// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using RealityToolkit.ServiceFramework.Definitions;
using RealityToolkit.ServiceFramework.Providers;

namespace RealityToolkit.Tests.Services
{
    internal class TestDataProvider2 : BaseServiceDataProvider, ITestDataProvider2
    {
        public TestDataProvider2(ITestService parentService, string name = "Test Data Provider 2", uint priority = 2, BaseProfile profile = null)
            : base(name, priority, profile, parentService)
        { }
    }
}