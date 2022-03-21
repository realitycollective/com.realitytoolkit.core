// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using RealityToolkit.Interfaces.InputSystem.Controllers.Hands;
using UnityEngine;
using XRTK.Definitions.Controllers.Hands;

namespace XRTK.Interfaces.InputSystem.Controllers.Hands
{
    /// <summary>
    /// Controller definition, used to manage a hand controller.
    /// </summary>
    public interface IMixedRealityHandController : IHandController
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
        /// Gets the curl strength for a finger, if available.
        /// </summary>
        /// <param name="handFinger">The <see cref="HandFinger"/> to lookup strength for.</param>
        /// <param name="curlStrength">The finger's curl strength is a value from 0 to 1.</param>
        /// <returns>True, if success.</returns>
        bool TryGetFingerCurlStrength(HandFinger handFinger, out float curlStrength);
    }
}