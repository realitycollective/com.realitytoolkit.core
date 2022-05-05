// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using RealityToolkit.Interfaces;
using UnityEngine;

namespace RealityToolkit.Attributes
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
    public class RuntimePlatformAttribute : PropertyAttribute
    {
        public Type Platform { get; }

        public RuntimePlatformAttribute(Type platformType)
        {
            if (typeof(IMixedRealityPlatform).IsAssignableFrom(platformType))
            {
                Platform = platformType;
            }
            else
            {
                throw new ArgumentException($"{nameof(platformType)} must implement {nameof(IMixedRealityPlatform)}");
            }
        }
    }
}
