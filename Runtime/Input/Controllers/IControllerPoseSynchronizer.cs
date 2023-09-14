// Copyright (c) Reality Collective. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using RealityCollective.Definitions.Utilities;
using RealityToolkit.Input.Definitions;
using RealityToolkit.Input.Interfaces.Handlers;
using UnityEngine;

namespace RealityToolkit.Input.Controllers
{
    /// <summary>
    /// Basic interface for synchronizing to a controller pose.
    /// </summary>
    public interface IControllerPoseSynchronizer : ISourcePoseHandler,
        IInputHandler,
        IInputHandler<float>,
        IInputHandler<Vector2>,
        IInputHandler<Vector3>,
        IInputHandler<Quaternion>,
        IInputHandler<Pose>
    {
        /// <summary>
        /// The <see cref="Transform"/> that will be synchronized with the controller data.
        /// </summary>
        /// <remarks>
        /// Defaults to the <see cref="Transform"/> that this component is attached to.
        /// </remarks>
        Transform PoseDriver { get; }

        /// <summary>
        /// The controller handedness to synchronize with.
        /// </summary>
        Handedness Handedness { get; }

        /// <summary>
        /// The current controller reference.
        /// </summary>
        IController Controller { get; set; }

        /// <summary>
        /// Should the Transform's position be driven from the source pose or from input handler?
        /// </summary>
        bool UseSourcePoseData { get; set; }

        /// <summary>
        /// The input action that will drive the Transform's pose, position, or rotation.
        /// </summary>
        InputAction PoseAction { get; set; }
    }
}
