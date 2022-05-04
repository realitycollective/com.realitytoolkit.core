// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using RealityToolkit.Interfaces;

namespace RealityToolkit.Tests.Services
{
    internal interface ITestService : IMixedRealityService
    {
        bool IsEnabled { get; }
    }
}