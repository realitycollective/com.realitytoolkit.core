// Copyright (c) Reality Collective. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using RealityToolkit.EventDatum.Input;
using UnityEngine.EventSystems;

namespace RealityToolkit.Input.Interfaces.Handlers
{
    /// <summary>
    /// Interface to implement to react to speech recognition.
    /// </summary>
    public interface ISpeechHandler : IEventSystemHandler
    {
        void OnSpeechKeywordRecognized(SpeechEventData eventData);
    }
}