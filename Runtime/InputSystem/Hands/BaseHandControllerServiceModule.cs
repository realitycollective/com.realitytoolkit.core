// Copyright (c) Reality Collective. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using RealityCollective.ServiceFramework.Services;
using RealityToolkit.InputSystem.Controllers;
using RealityToolkit.InputSystem.Definitions;
using RealityToolkit.InputSystem.Interfaces;
using System.Collections.Generic;
using UnityEngine;

namespace RealityToolkit.InputSystem.Hands
{
    /// <summary>
    /// Base implementation for <see cref="IHandControllerServiceModule"/>s.
    /// </summary>
    public abstract class BaseHandControllerServiceModule : BaseControllerServiceModule, IHandControllerServiceModule
    {
        /// <inheritdoc />
        protected BaseHandControllerServiceModule(string name, uint priority, BaseHandControllerServiceModuleProfile profile, IMixedRealityInputSystem parentService)
            : base(name, priority, profile, parentService)
        {
            if (!ServiceManager.Instance.TryGetServiceProfile<IMixedRealityInputSystem, MixedRealityInputSystemProfile>(out var inputSystemProfile))
            {
                Debug.LogError($"The {GetType().Name} requires a valid {nameof(MixedRealityInputSystemProfile)} to work.");
                return;
            }

            HandPhysicsEnabled = profile.HandPhysicsEnabled != inputSystemProfile.HandControllerSettings.HandPhysicsEnabled
                ? profile.HandPhysicsEnabled
                : inputSystemProfile.HandControllerSettings.HandPhysicsEnabled;

            UseTriggers = profile.UseTriggers != inputSystemProfile.HandControllerSettings.UseTriggers
                ? profile.UseTriggers
                : inputSystemProfile.HandControllerSettings.UseTriggers;

            BoundsMode = profile.BoundsMode != inputSystemProfile.HandControllerSettings.BoundsMode
                ? profile.BoundsMode
                : inputSystemProfile.HandControllerSettings.BoundsMode;

            if (profile.TrackedPoses != null &&
                profile.TrackedPoses.Count > 0)
            {
                TrackedPoses = profile.TrackedPoses.Count != inputSystemProfile.HandControllerSettings.TrackedPoses.Count
                    ? profile.TrackedPoses
                    : inputSystemProfile.HandControllerSettings.TrackedPoses;
            }
            else
            {
                TrackedPoses = inputSystemProfile.HandControllerSettings.TrackedPoses;
            }
        }

        /// <inheritdoc />
        public bool HandPhysicsEnabled { get; set; }

        /// <inheritdoc />
        public bool UseTriggers { get; set; }

        /// <inheritdoc />
        public HandBoundsLOD BoundsMode { get; set; }

        /// <summary>
        /// Configured <see cref="HandControllerPoseProfile"/>s for pose recognition.
        /// </summary>
        protected IReadOnlyList<HandControllerPoseProfile> TrackedPoses { get; }
    }
}
