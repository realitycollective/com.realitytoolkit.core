// Copyright (c) Reality Collective. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;

namespace RealityToolkit.Input.Controllers
{
    /// <summary>
    /// Interface for configuring controller visualization.
    /// </summary>
    public interface IControllerVisualizer : IControllerPoseSynchronizer
    {
        /// <summary>
        /// The <see cref="UnityEngine.GameObject"/> reference for this controller.
        /// </summary>
        /// <remarks>
        /// This reference may not always be available when called.
        /// </remarks>
        GameObject GameObject { get; }

        /// <summary>
        /// The <see cref="Transform"/> defining the <see cref="IControllerVisualizer"/>s grip pose.
        /// That is the pose where objects are attached to when gripped.
        /// </summary>
        Transform GripPose { get; }
    }
}