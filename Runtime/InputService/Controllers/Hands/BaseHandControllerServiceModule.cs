// Copyright (c) Reality Collective. All rights reserved.
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
    /// Base controller service module to inherit from when implementing <see cref="IHandController"/>s.
    /// </summary>
    public abstract class BaseHandControllerServiceModule : BaseControllerServiceModule, IHandControllerServiceModule
    {
        /// <inheritdoc />
        protected BaseHandControllerServiceModule(string name, uint priority, BaseHandControllerServiceModuleProfile profile, IInputService parentService)
            : base(name, priority, profile, parentService)
        {
            if (!ServiceManager.Instance.TryGetServiceProfile<IInputService, InputServiceProfile>(out var inputServiceProfile))
            {
                throw new ArgumentException($"Unable to get a valid {nameof(InputServiceProfile)}!");
            }

            RenderingMode = profile.RenderingMode != inputServiceProfile.HandControllerSettings.RenderingMode
                ? profile.RenderingMode
                : inputServiceProfile.HandControllerSettings.RenderingMode;

            HandPhysicsEnabled = profile.HandPhysicsEnabled != inputServiceProfile.HandControllerSettings.HandPhysicsEnabled
                ? profile.HandPhysicsEnabled
                : inputServiceProfile.HandControllerSettings.HandPhysicsEnabled;

            UseTriggers = profile.UseTriggers != inputServiceProfile.HandControllerSettings.UseTriggers
                ? profile.UseTriggers
                : inputServiceProfile.HandControllerSettings.UseTriggers;

            BoundsMode = profile.BoundsMode != inputServiceProfile.HandControllerSettings.BoundsMode
                ? profile.BoundsMode
                : inputServiceProfile.HandControllerSettings.BoundsMode;

            if (profile.TrackedPoses != null &&
                profile.TrackedPoses.Count > 0)
            {
                TrackedPoses = profile.TrackedPoses.Count != inputServiceProfile.HandControllerSettings.TrackedPoses.Count
                    ? profile.TrackedPoses
                    : inputServiceProfile.HandControllerSettings.TrackedPoses;
            }
            else
            {
                TrackedPoses = inputServiceProfile.HandControllerSettings.TrackedPoses;
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
