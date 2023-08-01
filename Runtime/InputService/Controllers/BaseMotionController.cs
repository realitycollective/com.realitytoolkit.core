// Copyright (c) Reality Collective. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using RealityCollective.Definitions.Utilities;
using RealityToolkit.Definitions.Controllers;
using RealityToolkit.Definitions.Devices;
using RealityToolkit.Input.Interfaces.Controllers;
using RealityToolkit.Input.Interfaces.Modules;
using UnityEngine;

namespace RealityToolkit.Input.Controllers
{
    /// <summary>
    /// Base <see cref="IMotionController"/> implementation for use as a starting point
    /// to implement own <see cref="IMotionController"/> types.
    /// </summary>
    public abstract class BaseMotionController : BaseController, IMotionController
    {
        /// <summary>
        /// Creates a new instance of a controller.
        /// </summary>
        /// <param name="controllerDataProvider">The <see cref="IControllerServiceModule"/> this controller belongs to.</param>
        /// <param name="trackingState">The initial tracking state of this controller.</param>
        /// <param name="controllerHandedness">The controller's handedness.</param>
        /// <param name="controllerMappingProfile"></param>
        protected BaseMotionController(IControllerServiceModule controllerDataProvider, TrackingState trackingState, Handedness controllerHandedness, ControllerMappingProfile controllerMappingProfile)
            : base(controllerDataProvider, trackingState, controllerHandedness, controllerMappingProfile)
        {
            IsPositionAvailable = false;
            IsPositionApproximate = false;
            IsRotationAvailable = false;
        }

        /// <inheritdoc />
        public Pose Pose { get; protected set; } = Pose.identity;

        /// <inheritdoc />
        public bool IsPositionAvailable { get; protected set; }

        /// <inheritdoc />
        public bool IsPositionApproximate { get; protected set; }

        /// <inheritdoc />
        public bool IsRotationAvailable { get; protected set; }

        /// <inheritdoc />
        public Vector3 AngularVelocity { get; protected set; } = Vector3.zero;

        /// <inheritdoc />
        public Vector3 Velocity { get; protected set; } = Vector3.zero;

        /// <inheritdoc />
        public Vector3 MotionDirection { get; protected set; }
    }
}
