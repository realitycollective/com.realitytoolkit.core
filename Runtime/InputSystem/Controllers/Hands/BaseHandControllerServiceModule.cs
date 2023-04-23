// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using RealityCollective.ServiceFramework.Services;
using RealityToolkit.Definitions.Controllers.Hands;
using RealityToolkit.Input.Definitions;
using RealityToolkit.Input.Interfaces;
using RealityToolkit.Input.Interfaces.Modules;
using System;
using System.Collections.Generic;

namespace RealityToolkit.Input.Controllers.Hands
{
    /// <summary>
    /// Base controller service module to inherit from when implementing <see cref="IMixedRealityHandController"/>s.
    /// </summary>
    public abstract class BaseHandControllerServiceModule : BaseControllerServiceModule, IMixedRealityHandControllerServiceModule
    {
        /// <inheritdoc />
        protected BaseHandControllerServiceModule(string name, uint priority, BaseHandControllerServiceModuleProfile profile, IInputService parentService)
            : base(name, priority, profile, parentService)
        {
            if (!ServiceManager.Instance.TryGetServiceProfile<IInputService, InputServiceProfile>(out var inputSystemProfile))
            {
                throw new ArgumentException($"Unable to get a valid {nameof(InputServiceProfile)}!");
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
