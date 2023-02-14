// Copyright (c) Reality Collective. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using RealityToolkit.Definitions.Utilities;
using RealityToolkit.InputSystem.Interfaces.Controllers;
using UnityEngine;

namespace RealityToolkit.InputSystem.Hands
{
    /// <summary>
    /// A <see cref="IHandController"/> is an input device that is driven by hand tracking sensors
    /// providing the user's hand joint poses to the application.
    /// </summary>
    public interface IHandController : IMixedRealityController
    {
        /// <summary>
        /// Gets whether the hand is currently in input down pose
        /// (select / pinch / airtap).
        /// </summary>
        bool IsPinching { get; }

        /// <summary>
        /// Gets the current pinch strength of the hand.
        /// </summary>
        float PinchStrength { get; }

        /// <summary>
        /// Gets whether the hand is currently in a pointing pose.
        /// </summary>
        bool IsPointing { get; }

        /// <summary>
        /// Gets whether the hand is currently in gripping pose.
        /// </summary>
        bool IsGripping { get; }

        /// <summary>
        /// Gets the current grip strength of the hand.
        /// </summary>
        float GripStrength { get; }

        /// <summary>
        /// Get the hands bounds of a given type, if they are available.
        /// </summary>
        /// <param name="handBounds">The requested hand bounds.</param>
        /// <param name="bounds">The bounds if available.</param>
        /// <returns><c>true</c>, if bounds available.</returns>
        bool TryGetBounds(TrackedHandBounds handBounds, out Bounds[] bounds);

        /// <summary>
        /// Get the current pose of a joint of the hand.
        /// </summary>
        /// <remarks>
        /// Hand bones should be oriented along the Z-axis, with the Y-axis indicating the "up" direction,
        /// i.e. joints rotate primarily around the X-axis.
        /// </remarks>
        /// <param name="joint">The joint to get the pose for.</param>
        /// <param name="pose">Pose output parameter containing the pose if found.</param>
        /// <param name="relativeTo">Optional coordinate space to get the pose in. Defaults to <see cref="Space.Self"/>.</param>
        /// <returns><c>true</c>, if the pose is available.</returns>
        bool TryGetJointPose(TrackedHandJoint joint, out MixedRealityPose pose, Space relativeTo = Space.Self);
    }
}