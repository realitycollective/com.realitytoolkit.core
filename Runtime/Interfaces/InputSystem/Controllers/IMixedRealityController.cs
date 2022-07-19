// Copyright (c) Reality Collective. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using RealityCollective.Definitions.Utilities;
using RealityToolkit.Definitions.Devices;
using RealityToolkit.Definitions.Utilities;
using RealityToolkit.Interfaces.InputSystem.Handlers;
using RealityToolkit.Interfaces.InputSystem.Providers.Controllers;
using UnityEngine;

namespace RealityToolkit.Interfaces.InputSystem.Controllers
{
    /// <summary>
    /// Main controller interface. A <see cref="IMixedRealityController"/> is a e.g. a hand-held device,
    /// or anything really that is used to provide input to the application.
    /// </summary>
    public interface IMixedRealityController
    {
        /// <summary>
        /// The name of the controller.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Is the controller enabled?
        /// </summary>
        bool Enabled { get; }

        /// <summary>
        /// The data provider service this controller belongs to.
        /// </summary>
        IMixedRealityControllerDataProvider DataProvider { get; }

        /// <summary>
        /// Indicates that this controller is currently providing position data.
        /// </summary>
        /// <remarks>
        /// This value may change during usage for some controllers. As a best practice,
        /// be sure to check this value before using <see cref="Pose"/>.
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
        /// be sure to check this value before using <see cref="Pose"/>.
        /// </remarks>
        bool IsRotationAvailable { get; }

        /// <summary>
        /// The controller's pose in world space.
        /// </summary>
        /// <remarks>
        /// Check <see cref="IsPositionAvailable"/>, <see cref="IsRotationAvailable"/> and <see cref="IsPositionApproximate"/>
        /// to determine quality of pose data.
        /// </remarks>
        MixedRealityPose Pose { get; }

        /// <summary>
        /// Outputs the current state of the Input Source, whether it is tracked or not.
        /// </summary>
        TrackingState TrackingState { get; }

        /// <summary>
        /// The designated hand that the controller is mapped to.
        /// </summary>
        Handedness Handedness { get; }

        /// <summary>
        /// The registered Input Source for this controller.
        /// </summary>
        IMixedRealityInputSource InputSource { get; }

        /// <summary>
        /// The controller's "Visual" <see cref="Component"/> in the scene.
        /// </summary>
        IMixedRealityControllerVisualizer Visualizer { get; }

        /// <summary>
        /// Mapping definition for this controller, linking the Physical inputs to logical Input System Actions
        /// </summary>
        MixedRealityInteractionMapping[] Interactions { get; }

        /// <summary>
        /// Gets how fast the controller rotates or revolves relative to its pivot point.
        /// </summary>
        Vector3 AngularVelocity { get; }

        /// <summary>
        /// Gets the controller's current movement speed.
        /// </summary>
        Vector3 Velocity { get; }

        /// <summary>
        /// Attempts to load the controller model render settings from the <see cref="Definitions.Controllers.MixedRealityControllerVisualizationProfile"/>
        /// to render the controllers in the scene.
        /// </summary>
        /// <param name="useAlternatePoseAction">Should the rendered controller use the alternate pose action?</param>
        /// <returns>True, if controller model is being properly rendered.</returns>
        void TryRenderControllerModel(bool useAlternatePoseAction = false);

        /// <summary>
        /// Updates the controller's state.
        /// </summary>
        /// <remarks>
        /// This API is for use by the controller's <see cref="DataProvider"/> only!
        /// </remarks>
        void UpdateController();
    }
}