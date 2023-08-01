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
        /// Mapping definition for this controller, linking the Physical inputs to logical Input System Actions
        /// </summary>
        InteractionMapping[] Interactions { get; }

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