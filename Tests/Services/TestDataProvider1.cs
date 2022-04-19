// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using RealityToolkit.ServiceFramework.Definitions;
using RealityToolkit.ServiceFramework.Providers;

namespace XRTK.Tests.Services
{
    [System.Runtime.InteropServices.Guid("407D379E-3351-4B2D-9C88-1B54C42B5554")]
    internal class TestDataProvider1 : BaseServiceDataProvider, ITestDataProvider1
    {
        public const string TestName = "Test Data Provider 1";

        public TestDataProvider1(ITestService parentService, string name = TestName, uint priority = 1, BaseProfile profile = null)
            : base(name, priority, profile, parentService)
        { }
    }
}