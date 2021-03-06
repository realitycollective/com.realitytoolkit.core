// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using RealityToolkit.Definitions.Utilities;
using UnityEngine;

namespace RealityToolkit.Definitions
{
    [CreateAssetMenu(menuName = "Reality Toolkit/Platform Service Configurations", fileName = "MixedRealityPlatformServiceConfigurationProfile", order = (int)CreateProfileMenuItemIndices.Configuration)]
    public class MixedRealityPlatformServiceConfigurationProfile : BaseMixedRealityProfile
    {
        [SerializeField]
        private RuntimePlatformEntry platformEntries = new RuntimePlatformEntry();

        public RuntimePlatformEntry PlatformEntries => platformEntries;

        [SerializeField]
        private MixedRealityServiceConfiguration[] configurations = new MixedRealityServiceConfiguration[0];

        public MixedRealityServiceConfiguration[] Configurations => configurations;
    }
}