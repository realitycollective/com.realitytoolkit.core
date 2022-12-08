﻿// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using RealityCollective.ServiceFramework.Modules;
using RealityToolkit.Definitions.Controllers;
using RealityToolkit.InputSystem.Interfaces;
using RealityToolkit.InputSystem.Interfaces.Speech;
using System.Threading.Tasks;
using UnityEngine;

namespace RealityToolkit.InputSystem.Modules
{
    /// <summary>
    /// Base dictation service module to use when implementing <see cref="IMixedRealityDictationServiceModule"/>s
    /// </summary>
    public abstract class BaseDictationServiceModule : BaseServiceModule, IMixedRealityDictationServiceModule
    {
        /// <inheritdoc />
        protected BaseDictationServiceModule(string name, uint priority, BaseMixedRealityControllerServiceModuleProfile profile, IMixedRealityInputSystem parentService)
            : base(name, priority, profile, parentService)
        {
            InputSystem = parentService;
        }

        protected readonly IMixedRealityInputSystem InputSystem;

        #region IMixedRealityDictationDataProvider Implementation

        /// <inheritdoc />
        public virtual bool IsListening { get; protected set; } = false;

        /// <inheritdoc />
        public virtual void StartRecording(GameObject listener = null, float initialSilenceTimeout = 5, float autoSilenceTimeout = 20, int recordingTime = 10, string micDeviceName = "")
        {
        }

        /// <inheritdoc />
        public virtual Task StartRecordingAsync(GameObject listener = null, float initialSilenceTimeout = 5, float autoSilenceTimeout = 20, int recordingTime = 10, string micDeviceName = "")
        {
            return null;
        }

        /// <inheritdoc />
        public virtual void StopRecording()
        {
        }

        /// <inheritdoc />
        public virtual Task<AudioClip> StopRecordingAsync()
        {
            return null;
        }

        #endregion IMixedRealityDictationDataProvider Implementation
    }
}