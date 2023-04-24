// Copyright (c) Reality Collective. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using RealityCollective.Definitions.Utilities;
using RealityCollective.ServiceFramework.Definitions;
using UnityEngine;

namespace RealityToolkit.Input.Definitions
{
    /// <summary>
    /// Configuration profile settings for setting up and consuming Speech Commands.
    /// </summary>
    public class MixedRealitySpeechCommandsProfile : BaseProfile
    {
        [SerializeField]
        [Tooltip("Whether the recognizer should be activated on start.")]
        private AutoStartBehavior startBehavior = AutoStartBehavior.AutoStart;

        /// <summary>
        /// The list of Speech Commands users use in your application.
        /// </summary>
        public AutoStartBehavior SpeechRecognizerStartBehavior => startBehavior;

        [SerializeField]
        [Tooltip("Select the minimum confidence level for recognized words")]
        private RecognitionConfidenceLevel recognitionConfidenceLevel = RecognitionConfidenceLevel.Medium;

        /// <summary>
        /// The speech recognizer's minimum confidence level setting that will raise the action.
        /// </summary>
        public RecognitionConfidenceLevel SpeechRecognitionConfidenceLevel => recognitionConfidenceLevel;

        [SerializeField]
        [Tooltip("The list of Speech Commands users use in your application.")]
        private SpeechCommands[] speechCommands = new SpeechCommands[0];

        /// <summary>
        /// The list of Speech Commands users use in your application.
        /// </summary>
        public SpeechCommands[] SpeechCommands => speechCommands;
    }
}