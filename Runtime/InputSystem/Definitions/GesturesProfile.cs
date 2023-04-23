// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.﻿

using RealityCollective.ServiceFramework.Definitions;
using RealityToolkit.Definitions.Devices;
using UnityEngine;

namespace RealityToolkit.InputSystem.Definitions
{
    /// <summary>
    /// Configuration profile settings for setting up and consuming gesture based input actions.
    /// </summary>
    public class MixedRealityGesturesProfile : BaseProfile
    {
        [SerializeField]
        private MixedRealityGestureMapping[] gestures =
        {
            new MixedRealityGestureMapping("Hold", GestureInputType.Hold, MixedRealityInputAction.None),
            new MixedRealityGestureMapping("Navigation", GestureInputType.Navigation, MixedRealityInputAction.None),
            new MixedRealityGestureMapping("Manipulation", GestureInputType.Manipulation, MixedRealityInputAction.None),
        };

        /// <summary>
        /// The currently configured gestures for the application.
        /// </summary>
        public MixedRealityGestureMapping[] Gestures => gestures;
    }
}