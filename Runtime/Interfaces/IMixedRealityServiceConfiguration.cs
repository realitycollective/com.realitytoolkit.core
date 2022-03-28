// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.﻿

using System.Collections.Generic;
using RealityToolkit.ServiceFramework.Definitions;
using RealityToolkit.ServiceFramework.Interfaces;

namespace XRTK.Interfaces
{
    /// <summary>
    /// The generic interface for <see cref="IMixedRealityServiceConfiguration"/>s.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    // ReSharper disable once UnusedTypeParameter (Used in concrete Implementations)
    public interface IMixedRealityServiceConfiguration<out T> : IMixedRealityServiceConfiguration where T : IService
    {
    }

    /// <summary>
    /// This interface is meant to be used with serialized structs that define valid <see cref="IMixedRealityService"/> configurations.
    /// </summary>
    public interface IMixedRealityServiceConfiguration
    {
        ServiceConfiguration ServiceConfiguration { get; }

        /// <summary>
        /// The runtime platform(s) to run this <see cref="IMixedRealityService"/> to run on.
        /// </summary>
        IReadOnlyList<IMixedRealityPlatform> RuntimePlatforms { get; }
    }
}