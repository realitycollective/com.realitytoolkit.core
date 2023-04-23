// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using UnityEngine;
using UnityEngine.Events;

namespace RealityToolkit.Input.Definitions
{
    /// <summary>
    /// Data class that maps <see cref="Definitions.InputAction"/>s to <see cref="UnityEngine.Events.UnityEvent"/>s wired up in the inspector.
    /// </summary>
    [Serializable]
    public class InputActionEventPair
    {
        [SerializeField]
        [Tooltip("The MixedRealityInputAction to listen for to invoke the UnityEvent.")]
        private InputAction inputAction = InputAction.None;

        /// <summary>
        /// The <see cref="Definitions.InputAction"/> to listen for to invoke the <see cref="UnityEvent"/>.
        /// </summary>
        public InputAction InputAction => inputAction;

        [SerializeField]
        [Tooltip("The UnityEvent to invoke when MixedRealityInputAction is raised.")]
        private UnityEvent unityEvent = null;

        /// <summary>
        /// The <see cref="UnityEvent"/> to invoke when <see cref="Definitions.InputAction"/> is raised.
        /// </summary>
        public UnityEvent UnityEvent => unityEvent;
    }
}