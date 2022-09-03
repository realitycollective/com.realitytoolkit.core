// Copyright (c) Reality Collective. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using RealityCollective.ServiceFramework.Definitions;
using RealityToolkit.CameraSystem.Interfaces;
using UnityEngine;

namespace RealityToolkit.CameraSystem.Definitions
{
    /// <summary>
    /// Configuration profile for the <see cref="MixedRealityCameraSystem"/>.
    /// </summary>
    public class MixedRealityCameraSystemProfile : BaseServiceProfile<IMixedRealityCameraDataProvider>
    {
        [SerializeField]
        [Tooltip("The Global Camera Profile Settings.")]
        private BaseMixedRealityCameraDataProviderProfile globalCameraProfile = null;

        /// <summary>
        /// The default camera data provider profile <see cref="IMixedRealityCameraDataProvider"/>s will use if no profile is assigned.
        /// </summary>
        public BaseMixedRealityCameraDataProviderProfile GlobalCameraProfile => globalCameraProfile;
    }
}