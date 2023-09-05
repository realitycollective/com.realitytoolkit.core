// Copyright (c) Reality Collective. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using RealityToolkit.Definitions.Devices;
using System;
using UnityEngine;

namespace RealityToolkit.Input.Definitions
{
    /// <summary>
    /// Data structure for mapping gestures to <see cref="InputAction"/>s that can be raised by the Input System.
    /// </summary>
    [Serializable]
    public struct GestureMapping
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="description"></param>
        /// <param name="gestureType"></param>
        /// <param name="action"></param>
        public GestureMapping(string description, GestureInputType gestureType, InputAction action)
        {
            this.description = description;
            this.gestureType = gestureType;
            this.action = action;
        }

        [SerializeField]
        private string description;

        /// <summary>
        /// Simple, human readable description of the gesture.
        /// </summary>
        public string Description => description;

        [SerializeField]
        private GestureInputType gestureType;

        /// <summary>
        /// Type of Gesture.
        /// </summary>
        public GestureInputType GestureType => gestureType;

        [SerializeField]
        private InputAction action;

        /// <summary>
        /// Action for the associated gesture.
        /// </summary>
        public InputAction Action => action;
    }
}