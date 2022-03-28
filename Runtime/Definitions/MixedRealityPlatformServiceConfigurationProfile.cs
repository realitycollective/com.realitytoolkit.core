// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using RealityToolkit.ServiceFramework.Definitions;
using UnityEngine;
using CreateProfileMenuItemIndices = XRTK.Definitions.Utilities.CreateProfileMenuItemIndices;

namespace XRTK.Definitions
{
    [CreateAssetMenu(menuName = "Mixed Reality Toolkit/Platform Service Configurations", fileName = "MixedRealityPlatformServiceConfigurationProfile", order = (int)CreateProfileMenuItemIndices.Configuration)]
    public class MixedRealityPlatformServiceConfigurationProfile : BaseProfile
    {
        [SerializeField]
        private RuntimePlatformEntry platformEntries = new RuntimePlatformEntry();

        public RuntimePlatformEntry PlatformEntries => platformEntries;

        [SerializeField]
        private MixedRealityServiceConfiguration[] configurations = new MixedRealityServiceConfiguration[0];

        public MixedRealityServiceConfiguration[] Configurations => configurations;
    }
}