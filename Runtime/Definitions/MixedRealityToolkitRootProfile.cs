// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using RealityToolkit.Definitions.Utilities;
using RealityToolkit.Interfaces;
using UnityEngine;

namespace RealityToolkit.Definitions
{
    /// <summary>
    /// The root profile for the Mixed Reality Toolkit's settings.
    /// </summary>
    [CreateAssetMenu(menuName = "Mixed Reality Toolkit/Mixed Reality Toolkit Root Profile", fileName = "MixedRealityToolkitRootProfile", order = (int)CreateProfileMenuItemIndices.Configuration - 1)]
    public sealed class MixedRealityToolkitRootProfile : BaseMixedRealityServiceProfile<IMixedRealitySystem>
    {
        [SerializeField]
        [Tooltip("All the additional non-required services registered with the Mixed Reality Toolkit.")]
        private MixedRealityRegisteredServiceProvidersProfile registeredServiceProvidersProfile = null;

        /// <summary>
        /// All the additional non-required systems, features, and managers registered with the Mixed Reality Toolkit.
        /// </summary>
        public MixedRealityRegisteredServiceProvidersProfile RegisteredServiceProvidersProfile
        {
            get => registeredServiceProvidersProfile;
            internal set => registeredServiceProvidersProfile = value;
        }
    }
}
