// Copyright (c) Reality Collective. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using RealityCollective.Extensions;
using UnityEngine;

namespace RealityToolkit.Input.Controllers
{
    /// <summary>
    /// Abstract base implementation for <see cref="IControllerVisualizer"/>s.
    /// </summary>
    public abstract class BaseControllerVisualizer : ControllerPoseSynchronizer, IControllerVisualizer
    {
        [SerializeField, Tooltip("Defines the pose to attach poke interactors to. Defaults to the visualizers root transform, if not set.")]
        private Transform pokePose = null;

        [SerializeField, Tooltip("Defines the pose to attach to when held. Defaults to the visualizers root transform, if not set.")]
        private Transform gripPose = null;

        /// <inheritdoc />
        public GameObject GameObject => gameObject;

        /// <inheritdoc />
        public Transform PokePose => pokePose.IsNotNull() ? pokePose : transform;

        /// <inheritdoc />
        public Transform GripPose => gripPose.IsNotNull() ? gripPose : transform;
    }
}
