// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using RealityToolkit.Definitions.Utilities;
using RealityToolkit.Interfaces.LocomotionSystem;
using UnityEngine;

namespace RealityToolkit.Definitions.LocomotionSystem
{
    /// <summary>
    /// Configuration profile settings for <see cref="Services.LocomotionSystem.LocomotionSystem"/>.
    /// </summary>
    [CreateAssetMenu(menuName = "Reality Toolkit/Locomotion System Profile", fileName = "MixedRealityLocomotionSystemProfile", order = (int)CreateProfileMenuItemIndices.Input)]
    public class LocomotionSystemProfile : BaseMixedRealityServiceProfile<ILocomotionSystemDataProvider>
    {
        [SerializeField]
        [Tooltip("The teleportation cooldown defines the time that needs to pass after a successful teleportation for another one to be possible.")]
        [Range(0, 10f)]
        private float teleportCooldown = 1f;

        /// <summary>
        /// The teleportation cooldown defines the time that needs to pass after a successful teleportation for another one to be possible.
        /// </summary>
        public float TeleportCooldown
        {
            get => teleportCooldown;
            internal set => teleportCooldown = value;
        }
    }
}
