﻿// Copyright (c) Reality Collective. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using RealityCollective.ServiceFramework.Definitions;
using UnityEngine;

namespace RealityToolkit.InputSystem.Hands
{
    /// <summary>
    /// A hand controller pose definition with recorded hand joint data.
    /// Defined hand poses can be recognized and trigger input actions.
    /// </summary>
    public class HandControllerPoseProfile : BaseProfile
    {
        [SerializeField]
        [Tooltip("A unique ID to the pose. Can be a descriptive name, but must be unique!")]
        private string id = string.Empty;

        /// <summary>
        /// A unique ID to the pose.
        /// </summary>
        public string Id => id;

        [SerializeField]
        [Tooltip("Describes the hand pose.")]
        private string description = string.Empty;

        /// <summary>
        /// Describes the hand pose.
        /// </summary>
        public string Description => description;

        [SerializeField]
        [Tooltip("Is this the default (idle) hand pose?")]
        private bool isDefault = false;

        /// <summary>
        /// Is this the default (idle) hand pose?
        /// </summary>
        public bool IsDefault => isDefault;

        [SerializeField]
        [Tooltip("Key used to trigger the gesture simulation.")]
        private KeyCode keyCode = KeyCode.None;

        /// <summary>
        /// Gets the key code used to trigger this gesture simulation.
        /// </summary>
        public KeyCode KeyCode => keyCode;

        [SerializeField]
        [Tooltip("Assign JSON definition file containing simulated gesture information.")]
        private TextAsset data = null;

        /// <summary>
        /// Gets the gesture definition's joint information used to simulate the gesture.
        /// </summary>
        public TextAsset Data => data;
    }
}