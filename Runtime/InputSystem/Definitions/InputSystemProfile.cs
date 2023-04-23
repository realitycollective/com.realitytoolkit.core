// Copyright (c) Reality Collective. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using RealityCollective.Attributes;
using RealityCollective.Definitions.Utilities;
using RealityCollective.ServiceFramework.Definitions;
using RealityToolkit.Definitions.Controllers.Hands;
using RealityToolkit.Input.Interfaces;
using RealityToolkit.Input.Interfaces.Modules;
using System.Collections.Generic;
using UnityEngine;

namespace RealityToolkit.Input.Definitions
{
    /// <summary>
    /// Configuration profile settings for setting up controller pointers.
    /// </summary>
    public class InputServiceProfile : BaseServiceProfile<IInputServiceModule>
    {
        #region Global Input System Options

        [SerializeField]
        [Tooltip("How should the gaze provider behave by default?")]
        private GazeProviderBehaviour gazeProviderBehaviour = GazeProviderBehaviour.Auto;

        /// <summary>
        /// How should the gaze provider behave by default?
        /// </summary>
        public GazeProviderBehaviour GazeProviderBehaviour
        {
            get => gazeProviderBehaviour;
            internal set => gazeProviderBehaviour = value;
        }

        [SerializeField]
        [Tooltip("The concrete type of IMixedRealityGazeProvider to use.")]
        [Implements(typeof(IGazeProvider), TypeGrouping.ByNamespaceFlat)]
        private SystemType gazeProviderType;

        /// <summary>
        /// The concrete type of <see cref="IGazeProvider"/> to use.
        /// </summary>
        public SystemType GazeProviderType
        {
            get => gazeProviderType;
            internal set => gazeProviderType = value;
        }

        [Prefab]
        [SerializeField]
        [Tooltip("The gaze cursor prefab to use on the Gaze pointer.")]
        private GameObject gazeCursorPrefab = null;

        /// <summary>
        /// The gaze cursor prefab to use on the Gaze pointer.
        /// </summary>
        public GameObject GazeCursorPrefab => gazeCursorPrefab;

        [SerializeField]
        [Tooltip("Global configuration settings for pointers in the input service.")]
        private PointersProfile pointersProfile;

        /// <summary>
        /// Global configuration settings for pointers in the input service.
        /// </summary>
        public PointersProfile PointersProfile
        {
            get => pointersProfile;
            internal set => pointersProfile = value;
        }

        #region Global Hand Options

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

        #endregion Global Hand Options

        #endregion Global Input System Options

        #region Profile Options

        [SerializeField]
        [Tooltip("Input System Action Mapping profile for setting up avery action a user can make in your application.")]
        private MixedRealityInputActionsProfile inputActionsProfile;

        /// <summary>
        /// Input System Action Mapping profile for setting up avery action a user can make in your application.
        /// </summary>
        public MixedRealityInputActionsProfile InputActionsProfile
        {
            get => inputActionsProfile;
            internal set => inputActionsProfile = value;
        }

        [SerializeField]
        [Tooltip("Speech Command profile for wiring up Voice Input to Actions.")]
        private MixedRealitySpeechCommandsProfile speechCommandsProfile;

        /// <summary>
        /// Speech commands profile for configured speech commands, for use by the speech recognition system
        /// </summary>
        public MixedRealitySpeechCommandsProfile SpeechCommandsProfile
        {
            get => speechCommandsProfile;
            internal set => speechCommandsProfile = value;
        }

        [SerializeField]
        [Tooltip("Gesture Mapping Profile for recognizing gestures across all platforms.")]
        private MixedRealityGesturesProfile gesturesProfile;

        /// <summary>
        /// Gesture Mapping Profile for recognizing gestures across all platforms.
        /// </summary>
        public MixedRealityGesturesProfile GesturesProfile
        {
            get => gesturesProfile;
            internal set => gesturesProfile = value;
        }

        #endregion Profile Options
    }
}
