// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.﻿

using RealityToolkit.Definitions.Utilities;
using RealityToolkit.Interfaces;
using UnityEngine;

namespace RealityToolkit.Definitions
{
    [CreateAssetMenu(menuName = "Reality Toolkit/Registered Service Providers Profile", fileName = "MixedRealityRegisteredServiceProvidersProfile", order = (int)CreateProfileMenuItemIndices.RegisteredServiceProviders)]
    public class MixedRealityRegisteredServiceProvidersProfile : BaseMixedRealityServiceProfile<IMixedRealityExtensionService>
    {
    }
}