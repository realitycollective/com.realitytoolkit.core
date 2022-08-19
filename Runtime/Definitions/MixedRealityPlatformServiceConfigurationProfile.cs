// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using RealityCollective.ServiceFramework.Definitions;
using UnityEngine;

namespace RealityToolkit.Definitions
{
    [CreateAssetMenu(menuName = "Reality Toolkit/Platform Service Configurations", fileName = "MixedRealityPlatformServiceConfigurationProfile", order = (int)CreateProfileMenuItemIndices.Configuration)]
    public class MixedRealityPlatformServiceConfigurationProfile : BaseProfile
    {
        [SerializeField]
        private RuntimePlatformEntry platformEntries = new RuntimePlatformEntry();

        public RuntimePlatformEntry PlatformEntries => platformEntries;

        [SerializeField]
        private ServiceConfiguration[] configurations = new ServiceConfiguration[0];

        public ServiceConfiguration[] Configurations => configurations;
    }
}