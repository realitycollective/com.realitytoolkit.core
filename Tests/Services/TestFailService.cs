// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using RealityToolkit.Services;

namespace RealityToolkit.Tests.Services
{
    internal class TestFailService : BaseService, ITestFailService
    {
        public override string Name => "Fail Service";
    }
}