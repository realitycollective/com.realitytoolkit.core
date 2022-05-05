// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using RealityToolkit.Definitions.Utilities;
using RealityToolkit.Interfaces.NetworkingSystem;
using UnityEngine;

namespace RealityToolkit.Definitions.NetworkingSystem
{
    /// <summary>
    /// Configuration profile settings for setting up networking.
    /// </summary>
    [CreateAssetMenu(menuName = "Mixed Reality Toolkit/Network System Profile", fileName = "MixedRealityNetworkSystemProfile", order = (int)CreateProfileMenuItemIndices.Networking)]
    public class MixedRealityNetworkSystemProfile : BaseMixedRealityServiceProfile<IMixedRealityNetworkDataProvider>
    {
    }
}