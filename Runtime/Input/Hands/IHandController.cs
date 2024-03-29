﻿// Copyright (c) Reality Collective. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using RealityToolkit.Input.Controllers;
using UnityEngine;

namespace RealityToolkit.Input.Hands
{
    /// <summary>
    /// Controller definition, used to manage a hand controller.
    /// </summary>
    public interface IHandController : IController
    {
        /// <summary>
        /// Gets whether the hand is currently in input down pose
        /// (select / pinch / airtap).
        /// </summary>
        bool IsPinching { get; }

        /// <summary>
        /// Gets the current pinch strength (index and thumb) of the hand.
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
        /// Gets the current grip strength (fist) of the hand.
        /// </summary>
        float GripStrength { get; }

        /// <summary>
        /// Gets the curl strength per finger.
        /// </summary>
        float[] FingerCurlStrengths { get; }

        /// <summary>
        /// Gets the hands current pose.
        /// </summary>
        string TrackedPoseId { get; }

        /// <summary>
        /// Get the hands bounds of a given type, if they are available.
        /// </summary>
        /// <param name="handBounds">The requested hand bounds.</param>
        /// <param name="bounds">The bounds if available.</param>
        /// <returns>True, if bounds available.</returns>
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
        /// <returns>True, if the pose is available.</returns>
        bool TryGetJointPose(HandJoint joint, out Pose pose, Space relativeTo = Space.Self);

        /// <summary>
        /// Gets the curl strength for a finger, if available.
        /// </summary>
        /// <param name="handFinger">The <see cref="HandFinger"/> to lookup strength for.</param>
        /// <param name="curlStrength">The finger's curl strength is a value from 0 to 1.</param>
        /// <returns>True, if success.</returns>
        bool TryGetFingerCurlStrength(HandFinger handFinger, out float curlStrength);
    }
}