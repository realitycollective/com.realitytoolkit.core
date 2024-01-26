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
        /// If set, the <see cref="IControllerPoseSynchronizer.PoseDriver"/>'s pose in the scene
        /// is override and the actual <see cref="IController.InputSource"/> pose is ignored.
        /// </summary>
        /// <remarks>
        /// When active, set the <see cref="IControllerPoseSynchronizer.PoseDriver"/> position and rotation
        /// directly to move the <see cref="IControllerVisualizer"/>.
        /// </remarks>
        bool OverrideSourcePose { get; set; }

        /// <summary>
        /// The <see cref="Transform"/> defining where <see cref="Interactors.IPokeInteractor"/>s
        /// attached to the <see cref="IController"/> should be located at.
        /// </summary>
        Transform PokePose { get; }

        /// <summary>
        /// The <see cref="Transform"/> defining the <see cref="IControllerVisualizer"/>s grip pose.
        /// That is the pose where objects are attached to when gripped.
        /// </summary>
        Transform GripPose { get; }
    }
}