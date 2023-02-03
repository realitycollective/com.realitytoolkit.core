// Copyright (c) Reality Collective. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using RealityCollective.Attributes;
using RealityCollective.Definitions.Utilities;
using RealityCollective.ServiceFramework.Definitions;
using RealityToolkit.InputSystem.Hands;
using RealityToolkit.InputSystem.Interfaces;
using RealityToolkit.InputSystem.Interfaces.Modules;
using UnityEngine;

namespace RealityToolkit.InputSystem.Definitions
{
    /// <summary>
    /// Configuration profile settings for setting up controller pointers.
    /// </summary>
    public class MixedRealityInputSystemProfile : BaseServiceProfile<IMixedRealityInputServiceModule>
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
        [Implements(typeof(IMixedRealityGazeProvider), TypeGrouping.ByNamespaceFlat)]
        private SystemType gazeProviderType;

        /// <summary>
        /// The concrete type of <see cref="IMixedRealityGazeProvider"/> to use.
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

        #endregion Global Input System Options

        #region Profile Options

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
