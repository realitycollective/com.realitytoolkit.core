// Copyright (c) Reality Collective. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using RealityCollective.ServiceFramework.Definitions;
using UnityEngine;

namespace RealityToolkit.Input.Definitions
{
    /// <summary>
    /// Configuration profile settings for setting up and consuming Input Actions.
    /// </summary>
    public class MixedRealityInputActionsProfile : BaseProfile
    {
        [SerializeField]
        [Tooltip("The list of actions users can do in your application.")]
        private InputAction[] inputActions = { };

        /// <summary>
        /// The list of actions users can do in your application.
        /// </summary>
        /// <remarks>Input Actions are device agnostic and can be paired with any number of device inputs across all platforms.</remarks>
        public InputAction[] InputActions => inputActions;
    }
}