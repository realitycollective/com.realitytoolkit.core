// Copyright (c) Reality Collective. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using RealityCollective.Definitions.Utilities;
using RealityToolkit.Definitions.Devices;
using RealityToolkit.Input.Interfaces.Handlers;
using RealityToolkit.Input.Interfaces.Modules;
using UnityEngine;

namespace RealityToolkit.Input.Interfaces.Controllers
{
    /// <summary>
    /// Reality Toolkit controller definition, used to manage a specific controller type.
    /// </summary>
    public interface IController
    {
        /// <summary>
        /// The name of the controller.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Is the controller enabled?
        /// </summary>
        bool Enabled { get; set; }

        /// <summary>
        /// The <see cref="IControllerServiceModule"/> this controller belongs to.
        /// </summary>
        IControllerServiceModule ControllerDataProvider { get; }

        /// <summary>
        /// Outputs the current state of the Input Source, whether it is tracked or not. As defined by the SDK / Unity.
        /// </summary>
        TrackingState TrackingState { get; }

        /// <summary>
        /// The designated hand that the Input Source is managing, as defined by the SDK / Unity.
        /// </summary>
        Handedness ControllerHandedness { get; }

        /// <summary>
        /// The registered Input Source for this controller
        /// </summary>
        IInputSource InputSource { get; }

        /// <summary>
        /// The controller's "Visual" <see cref="Component"/> in the scene.
        /// </summary>
        IControllerVisualizer Visualizer { get; }

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
        /// Mapping definition for this controller, linking the Physical inputs to logical Input System Actions
        /// </summary>
        InteractionMapping[] Interactions { get; }

        /// <summary>
        /// Gets the current position and rotation for the controller, if available.
        /// </summary>
        Pose Pose { get; }

        /// <summary>
        /// Gets how fast the controller rotates or revolves relative to its pivot point.
        /// </summary>
        Vector3 AngularVelocity { get; }

        /// <summary>
        /// Gets the controller's current movement speed.
        /// </summary>
        Vector3 Velocity { get; }

        /// <summary>
        /// The <see cref="IController"/>'s motion direction is a normalized <see cref="Vector3"/>
        /// describing in which direction is moving compared to a previous frame.
        /// </summary>
        Vector3 MotionDirection { get; }

        /// <summary>
        /// Attempts to load the controller model specified in the <see cref="RealityToolkit.Definitions.Controllers.ControllerMappingProfile"/>
        /// to render the controllers in the scene.
        /// </summary>
        /// <returns>True, if controller model is being properly rendered.</returns>
        void TryRenderControllerModel();

        /// <summary>
        /// Updates the controller's state.
        /// </summary>
        void UpdateController();
    }
}