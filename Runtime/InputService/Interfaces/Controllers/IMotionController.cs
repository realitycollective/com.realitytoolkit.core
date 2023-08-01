// Copyright (c) Reality Collective. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;

namespace RealityToolkit.Input.Interfaces.Controllers
{
    /// <summary>
    /// A <see cref="IMotionController"/> is a <see cref="IController"/>
    /// that is being tracked in 6DoF.
    /// </summary>
    public interface IMotionController : IController
    {
        /// <summary>
        /// Gets the current position and rotation for the controller, if available.
        /// </summary>
        Pose Pose { get; }

        /// <summary>
        /// Indicates that this controller is currently providing position data.
        /// </summary>
        /// <remarks>
        /// This value may change during usage for some controllers. As a best practice,
        /// be sure to check this value before using position data.
        /// </remarks>
        bool IsPositionAvailable { get; }

        /// <summary>
        /// Indicates the accuracy of the position data being reported.
        /// </summary>
        bool IsPositionApproximate { get; }

        /// <summary>
        /// Indicates that this controller is currently providing rotation data.
        /// </summary>
        /// <remarks>
        /// This value may change during usage for some controllers. As a best practice,
        /// be sure to check this value before using rotation data.
        /// </remarks>
        bool IsRotationAvailable { get; }

        /// <summary>
        /// Gets how fast the controller rotates or revolves relative to its pivot point on each axis.
        /// </summary>
        Vector3 AngularVelocity { get; }

        /// <summary>
        /// Gets the controller's current movement speed as a normalized <see cref="Vector3"/>.
        /// </summary>
        Vector3 Velocity { get; }

        /// <summary>
        /// The <see cref="IController"/>'s motion direction is a normalized <see cref="Vector3"/>
        /// describing in which direction is moving compared to a previous frame.
        /// </summary>
        Vector3 MotionDirection { get; }
    }
}
