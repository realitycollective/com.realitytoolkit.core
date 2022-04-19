// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using XRTK.Services;

namespace XRTK.Tests.Services
{
    internal class TestExtensionService2 : BaseExtensionService, ITestExtensionService2
    {
        public TestExtensionService2(string name = "Test Extension Service 2", uint priority = 10) : base(name, priority, null) { }
    }
}