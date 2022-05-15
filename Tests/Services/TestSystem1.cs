// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using RealityToolkit.ServiceFramework.Definitions;
using RealityToolkit.Services;

namespace RealityToolkit.Tests.Services
{
    internal class TestSystemProfile : BaseProfile
    {
    }

    internal class TestSystem1 : BaseSystem, ITestSystem
    {
        /// <inheritdoc />
        public TestSystem1(string name = "Test System 1", uint priority = 0, TestSystemProfile profile = null) : base(name, priority, profile)
        {
        }
    }

    internal class TestSystem2 : BaseSystem, ITestSystem
    {
        /// <inheritdoc />
        public TestSystem2(string name = "Test System 2", uint priority = 0, TestSystemProfile profile = null) : base(name, priority, profile)
        {
        }
    }
}