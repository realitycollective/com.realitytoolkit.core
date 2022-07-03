// Copyright (c) Reality Collective. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;
using RealityToolkit.Definitions.Controllers.Hands;
using RealityToolkit.Definitions.Utilities;
using RealityToolkit.Interfaces.InputSystem.Controllers;

namespace RealityToolkit.Interfaces.InputSystem.Controllers.Hands
{
    /// <summary>
    /// Common interface between legacy hand controller and the new hand controller
    /// implementation.
    /// </summary>
    public interface IHandController : IMixedRealityController
    {
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
        bool TryGetJointPose(XRHandJoint joint, out MixedRealityPose pose, Space relativeTo = Space.Self);

        /// <summary>
        /// Get the hand controllers current <see cref="HandMeshData"/>, if available.
        /// </summary>
        /// <param name="handMeshData">Hand mesh data for rendering the hand as a mesh.</param>
        /// <returns>True, if mesh data available and not <see cref="HandMeshData.Empty"/>.</returns>
        bool TryGetHandMeshData(out HandMeshData handMeshData);
    }
}