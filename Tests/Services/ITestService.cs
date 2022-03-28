// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using RealityToolkit.ServiceFramework.Interfaces;
using XRTK.Interfaces;

namespace XRTK.Tests.Services
{
    internal interface ITestService : IService
    {
        bool IsEnabled { get; }
    }
}