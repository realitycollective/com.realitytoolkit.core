// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using RealityCollective.ServiceFramework.Modules;
using RealityToolkit.InputSystem.Definitions;
using RealityToolkit.InputSystem.Interfaces;
using RealityToolkit.InputSystem.Interfaces.Speech;

namespace RealityToolkit.InputSystem.Modules
{
    /// <summary>
    /// Base speech service module to inherit from when implementing <see cref="IMixedRealitySpeechServiceModule"/>s
    /// </summary>
    public abstract class BaseSpeechServiceModule : BaseServiceModule, IMixedRealitySpeechServiceModule
    {
        /// <inheritdoc />
        protected BaseSpeechServiceModule(string name, uint priority, MixedRealitySpeechCommandsProfile profile, IMixedRealityInputSystem parentService)
            : base(name, priority, profile, parentService)
        {
            InputSystem = parentService;
        }

        protected readonly IMixedRealityInputSystem InputSystem;

        /// <inheritdoc />
        public virtual bool IsRecognitionActive { get; protected set; } = false;

        /// <inheritdoc />
        public virtual void StartRecognition() { }

        /// <inheritdoc />
        public virtual void StopRecognition() { }
    }
}