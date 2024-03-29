﻿// Copyright (c) Reality Collective. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using RealityToolkit.Input.Interfaces.Modules;

namespace RealityToolkit.Input.Interfaces.Speech
{
    /// <summary>
    /// Reality Toolkit controller definition, used to manage a specific controller type
    /// </summary>
    public interface ISpeechServiceModule : IInputServiceModule
    {
        /// <summary>
        /// Query whether or not the speech system is active
        /// </summary>
        bool IsRecognitionActive { get; }

        /// <summary>
        /// Make sure the keyword recognizer is on, then stop it.
        /// Otherwise, leave it alone because it's already in the desired state.
        /// </summary>
        void StartRecognition();

        /// <summary>
        /// Make sure the keyword recognizer is on, then stop it.
        /// Otherwise, leave it alone because it's already in the desired state.
        /// </summary>
        void StopRecognition();
    }
}
