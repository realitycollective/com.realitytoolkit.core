// Copyright (c) Reality Collective. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using RealityCollective.Extensions;
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
        protected BaseHandControllerServiceModule(string name, uint priority, HandControllerServiceModuleProfile profile, IMixedRealityInputSystem parentService)
            : base(name, priority, profile, parentService)
        {
            if (!ServiceManager.Instance.TryGetServiceProfile<IMixedRealityInputSystem, MixedRealityInputSystemProfile>(out var inputSystemProfile))
            {
                Debug.LogError($"The {GetType().Name} requires a valid {nameof(MixedRealityInputSystemProfile)} to work.");
                return;
            }

            HandPhysicsEnabled = profile.HandControllerSettings.IsNotNull()
                ? profile.HandControllerSettings.HandPhysicsEnabled
                : inputSystemProfile.HandControllerSettings.HandPhysicsEnabled;

            UseTriggers = profile.HandControllerSettings.IsNotNull()
                ? profile.HandControllerSettings.UseTriggers
                : inputSystemProfile.HandControllerSettings.UseTriggers;

            BoundsMode = profile.HandControllerSettings.IsNotNull()
                ? profile.HandControllerSettings.BoundsMode
                : inputSystemProfile.HandControllerSettings.BoundsMode;

            TrackedPoses = profile.HandControllerSettings.IsNotNull()
                    ? profile.HandControllerSettings.TrackedPoses
                    : inputSystemProfile.HandControllerSettings.TrackedPoses;
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
