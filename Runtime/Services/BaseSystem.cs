// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using RealityToolkit.Extensions;
using RealityToolkit.Interfaces;
using RealityToolkit.ServiceFramework.Definitions;
using RealityToolkit.ServiceFramework.Services;
using System;

namespace RealityToolkit.Services
{
    /// <summary>
    /// The base class for Mixed Reality Systems to inherit from.
    /// </summary>
    public abstract class BaseSystem : BaseServiceWithConstructor, IMixedRealitySystem
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="profile"></param>
        protected BaseSystem(BaseProfile profile)
        {
            if (profile.IsNull())
            {
                throw new ArgumentException($"Missing the profile for {base.Name} system!");
            }
        }
    }
}