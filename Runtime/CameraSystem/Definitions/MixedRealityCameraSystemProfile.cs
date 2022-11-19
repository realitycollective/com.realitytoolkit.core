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
    public class MixedRealityCameraSystemProfile : BaseServiceProfile<IMixedRealityCameraServiceModule>
    {
        [SerializeField]
        [Tooltip("The Global Camera Profile Settings.")]
        private BaseMixedRealityCameraServiceModuleProfile globalCameraProfile = null;

        /// <summary>
        /// The default camera data provider profile <see cref="IMixedRealityCameraServiceModule"/>s will use if no profile is assigned.
        /// </summary>
        public BaseMixedRealityCameraServiceModuleProfile GlobalCameraProfile => globalCameraProfile;
    }
}