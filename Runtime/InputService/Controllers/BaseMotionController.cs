// Copyright (c) Reality Collective. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using RealityToolkit.Input.Interfaces.Controllers;
using UnityEngine;

namespace RealityToolkit.Input.Controllers
{
    /// <summary>
    /// Base <see cref="IMotionController"/> implementation for use as a starting point
    /// to implement own <see cref="IMotionController"/> types.
    /// </summary>
    public abstract class BaseMotionController : BaseController, IMotionController
    {
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
