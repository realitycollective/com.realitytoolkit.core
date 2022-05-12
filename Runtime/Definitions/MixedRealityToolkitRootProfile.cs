// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using RealityToolkit.ServiceFramework.Definitions;
using UnityEngine;

namespace RealityToolkit.Definitions
{
    /// <summary>
    /// The root profile for the Mixed Reality Toolkit's settings.
    /// </summary>
    [CreateAssetMenu(menuName = "Mixed Reality Toolkit/Mixed Reality Toolkit Root Profile", fileName = "MixedRealityToolkitRootProfile", order = (int)Utilities.CreateProfileMenuItemIndices.Configuration - 1)]
    public sealed class MixedRealityToolkitRootProfile : ServiceProvidersProfile
    { }
}
