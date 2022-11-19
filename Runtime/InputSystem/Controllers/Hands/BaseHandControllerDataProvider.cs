// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using RealityCollective.ServiceFramework.Services;
using RealityToolkit.Definitions.Controllers.Hands;
using RealityToolkit.InputSystem.Definitions;
using RealityToolkit.InputSystem.Interfaces;
using RealityToolkit.InputSystem.Interfaces.Providers;
using System;
using System.Collections.Generic;

namespace RealityToolkit.InputSystem.Controllers.Hands
{
    /// <summary>
    /// Base controller data provider to inherit from when implementing <see cref="IMixedRealityHandController"/>s.
    /// </summary>
    public abstract class BaseHandControllerServiceModule : BaseControllerServiceModule, IMixedRealityHandControllerServiceModule
    {
        /// <inheritdoc />
        protected BaseHandControllerServiceModule(string name, uint priority, BaseHandControllerDataProviderProfile profile, IMixedRealityInputSystem parentService)
            : base(name, priority, profile, parentService)
        {
            if (!ServiceManager.Instance.TryGetServiceProfile<IMixedRealityInputSystem, MixedRealityInputSystemProfile>(out var inputSystemProfile))
            {
                throw new ArgumentException($"Unable to get a valid {nameof(MixedRealityInputSystemProfile)}!");
            }

            RenderingMode = profile.RenderingMode != inputSystemProfile.RenderingMode
                ? profile.RenderingMode
                : inputSystemProfile.RenderingMode;

            HandPhysicsEnabled = profile.HandPhysicsEnabled != inputSystemProfile.HandPhysicsEnabled
                ? profile.HandPhysicsEnabled
                : inputSystemProfile.HandPhysicsEnabled;

            UseTriggers = profile.UseTriggers != inputSystemProfile.UseTriggers
                ? profile.UseTriggers
                : inputSystemProfile.UseTriggers;

            BoundsMode = profile.BoundsMode != inputSystemProfile.BoundsMode
                ? profile.BoundsMode
                : inputSystemProfile.BoundsMode;

            if (profile.TrackedPoses != null &&
                profile.TrackedPoses.Count > 0)
            {
                TrackedPoses = profile.TrackedPoses.Count != inputSystemProfile.TrackedPoses.Count
                    ? profile.TrackedPoses
                    : inputSystemProfile.TrackedPoses;
            }
            else
            {
                TrackedPoses = inputSystemProfile.TrackedPoses;
            }
        }

        /// <inheritdoc />
        public HandRenderingMode RenderingMode { get; set; }

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
