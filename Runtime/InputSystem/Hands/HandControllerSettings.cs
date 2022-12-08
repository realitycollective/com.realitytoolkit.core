// Copyright (c) Reality Collective. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Collections.Generic;
using UnityEngine;

namespace RealityToolkit.InputSystem.Hands
{
    public class HandControllerSettings : MonoBehaviour
    {
        [SerializeField]
        [Range(.5f, 1f)]
        [Tooltip("Threshold in range [0.5, 1] that defines when a hand is considered to be grabing.")]
        private float gripThreshold = .8f;

        /// <summary>
        /// Threshold in range [0, 1] that defines when a hand is considered to be grabing.
        /// </summary>
        public float GripThreshold => gripThreshold;

        [SerializeField]
        [Tooltip("Defines what kind of data should be aggregated for the hands rendering.")]
        private HandRenderingMode renderingMode = HandRenderingMode.Joints;

        /// <summary>
        /// Defines what kind of data should be aggregated for the hands rendering.
        /// </summary>
        public HandRenderingMode RenderingMode => renderingMode;

        [SerializeField]
        [Tooltip("If set, hands will be setup with colliders and a rigidbody to work with Unity's physics system.")]
        private bool handPhysicsEnabled = false;

        /// <summary>
        /// If set, hands will be setup with colliders and a rigidbody to work with Unity's physics system.
        /// </summary>
        public bool HandPhysicsEnabled => handPhysicsEnabled;

        [SerializeField]
        [Tooltip("If set, hand colliders will be setup as triggers.")]
        private bool useTriggers = false;

        /// <summary>
        /// If set, hand colliders will be setup as triggers.
        /// </summary>
        public bool UseTriggers => useTriggers;

        [SerializeField]
        [Tooltip("Set the bounds mode to use for calculating hand bounds.")]
        private HandBoundsLOD boundsMode = HandBoundsLOD.Low;

        /// <summary>
        /// Set the bounds mode to use for calculating hand bounds.
        /// </summary>
        public HandBoundsLOD BoundsMode => boundsMode;

        [SerializeField]
        [Tooltip("Hand controller poses tracked.")]
        private HandControllerPoseProfile[] trackedPoses = null;

        /// <summary>
        /// Hand controller poses tracked.
        /// </summary>
        public IReadOnlyList<HandControllerPoseProfile> TrackedPoses => trackedPoses;
    }
}
