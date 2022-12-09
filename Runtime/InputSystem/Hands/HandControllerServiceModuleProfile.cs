// Copyright (c) Reality Collective. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using RealityCollective.Definitions.Utilities;
using RealityToolkit.Definitions.Controllers;
using UnityEngine;

namespace RealityToolkit.InputSystem.Hands
{
    /// <summary>
    /// Configuration options for <see cref="IHandControllerServiceModule"/>
    /// </summary>
    public abstract class HandControllerServiceModuleProfile : BaseMixedRealityControllerServiceModuleProfile
    {
        [SerializeField]
        [Tooltip("Gloabl settings for hand controllers.")]
        private HandControllerSettings handControllerSettings = null;

        /// <summary>
        /// Gloabl settings for hand controllers.
        /// </summary>
        public HandControllerSettings HandControllerSettings
        {
            get => handControllerSettings;
            internal set => handControllerSettings = value;
        }

        public override ControllerDefinition[] GetDefaultControllerOptions()
        {
            return new[]
            {
                new ControllerDefinition(typeof(HandController), Handedness.Left),
                new ControllerDefinition(typeof(HandController), Handedness.Right),
            };
        }
    }
}
