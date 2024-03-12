// Copyright (c) Reality Collective. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using RealityCollective.Attributes;
using RealityCollective.ServiceFramework;
using RealityCollective.ServiceFramework.Attributes;
using RealityCollective.ServiceFramework.Definitions;
using RealityCollective.ServiceFramework.Definitions.Utilities;
using RealityToolkit.Input.Hands;
using RealityToolkit.Input.Interfaces;
using RealityToolkit.Input.Interfaces.Modules;
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
        [Tooltip("Should near interaction be enabled at startup?")]
        private bool directInteraction = true;

        /// <summary>
        /// Should direct interaction be enabled at startup?
        /// </summary>
        public bool DirectInteraction => directInteraction;

        [SerializeField]
        [Tooltip("Should far interaction be enabled at startup?")]
        private bool farInteraction = true;

        /// <summary>
        /// Should far interaction be enabled at startup?
        /// </summary>
        public bool FarInteraction => farInteraction;

        [SerializeField]
        [Tooltip("How should the gaze provider behave by default?")]
        private GazeProviderBehaviour gazeProviderBehaviour = GazeProviderBehaviour.Auto;

        /// <summary>
        /// How should the gaze provider behave by default?
        /// </summary>
        public GazeProviderBehaviour GazeProviderBehaviour
        {
            get => gazeProviderBehaviour;
            set => gazeProviderBehaviour = value;
        }

        [SerializeField]
        [Tooltip("The concrete type of IGazeProvider to use.")]
        [Implements(typeof(IGazeProvider), TypeGrouping.ByNamespaceFlat)]
        private SystemType gazeProviderType;

        /// <summary>
        /// The concrete type of <see cref="IGazeProvider"/> to use.
        /// </summary>
        public SystemType GazeProviderType
        {
            get => gazeProviderType;
            set => gazeProviderType = value;
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
            set => pointersProfile = value;
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
        private InputActionsProfile inputActionsProfile;

        /// <summary>
        /// Input System Action Mapping profile for setting up avery action a user can make in your application.
        /// </summary>
        public InputActionsProfile InputActionsProfile
        {
            get => inputActionsProfile;
            set => inputActionsProfile = value;
        }

        [SerializeField]
        [Tooltip("Speech Command profile for wiring up Voice Input to Actions.")]
        private SpeechCommandsProfile speechCommandsProfile;

        /// <summary>
        /// Speech commands profile for configured speech commands, for use by the speech recognition system
        /// </summary>
        public SpeechCommandsProfile SpeechCommandsProfile
        {
            get => speechCommandsProfile;
            set => speechCommandsProfile = value;
        }

        [SerializeField]
        [Tooltip("Gesture Mapping Profile for recognizing gestures across all platforms.")]
        private GesturesProfile gesturesProfile;

        /// <summary>
        /// Gesture Mapping Profile for recognizing gestures across all platforms.
        /// </summary>
        public GesturesProfile GesturesProfile
        {
            get => gesturesProfile;
            set => gesturesProfile = value;
        }

        #endregion Profile Options
    }
}
