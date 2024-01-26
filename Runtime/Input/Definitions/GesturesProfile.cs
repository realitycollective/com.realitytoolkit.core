// Copyright (c) Reality Collective. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.﻿

using RealityCollective.ServiceFramework.Definitions;
using RealityToolkit.Definitions.Devices;
using UnityEngine;

namespace RealityToolkit.Input.Definitions
{
    /// <summary>
    /// Configuration profile settings for setting up and consuming gesture based input actions.
    /// </summary>
    public class GesturesProfile : BaseProfile
    {
        [SerializeField]
        private GestureMapping[] gestures =
        {
            new GestureMapping("Hold", GestureInputType.Hold, InputAction.None),
            new GestureMapping("Navigation", GestureInputType.Navigation, InputAction.None),
            new GestureMapping("Manipulation", GestureInputType.Manipulation, InputAction.None),
        };

        /// <summary>
        /// The currently configured gestures for the application.
        /// </summary>
        public GestureMapping[] Gestures => gestures;
    }
}