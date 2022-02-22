// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using RealityToolkit.ServiceFramework.Interfaces;
using XRTK.Definitions;
using XRTK.Extensions;

namespace XRTK.Services
{
    /// <summary>
    /// The base class for Mixed Reality Systems to inherit from.
    /// </summary>
    public abstract class BaseSystem : RealityToolkit.ServiceFramework.Services.BaseServiceWithConstructor, IService
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="profile"></param>
        protected BaseSystem(BaseMixedRealityProfile profile)
        {
            if (profile.IsNull())
            {
                throw new ArgumentException($"Missing the profile for {base.Name} system!");
            }
        }
    }
}