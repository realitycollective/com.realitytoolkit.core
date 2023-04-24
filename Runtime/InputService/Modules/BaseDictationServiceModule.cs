// Copyright (c) Reality Collective. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using RealityCollective.ServiceFramework.Modules;
using RealityToolkit.Definitions.Controllers;
using RealityToolkit.Input.Interfaces;
using RealityToolkit.Input.Interfaces.Speech;
using System.Threading.Tasks;
using UnityEngine;

namespace RealityToolkit.Input.Modules
{
    /// <summary>
    /// Base dictation service module to use when implementing <see cref="IDictationServiceModule"/>s
    /// </summary>
    public abstract class BaseDictationServiceModule : BaseServiceModule, IDictationServiceModule
    {
        /// <inheritdoc />
        protected BaseDictationServiceModule(string name, uint priority, BaseControllerServiceModuleProfile profile, IInputService parentService)
            : base(name, priority, profile, parentService)
        {
            InputService = parentService;
        }

        protected readonly IInputService InputService;

        #region IDictationDataProvider Implementation

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

        #endregion IDictationDataProvider Implementation
    }
}