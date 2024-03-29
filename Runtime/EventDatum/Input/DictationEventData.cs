﻿// Copyright (c) Reality Collective. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using RealityToolkit.Input.Interfaces;
using UnityEngine;
using UnityEngine.EventSystems;

namespace RealityToolkit.EventDatum.Input
{
    /// <summary>
    /// Describes an Input Event with voice dictation.
    /// </summary>
    public class DictationEventData : BaseInputEventData
    {
        /// <summary>
        /// String result of the current dictation.
        /// </summary>
        public string DictationResult { get; private set; }

        /// <summary>
        /// Audio Clip of the last Dictation recording Session.
        /// </summary>
        public AudioClip DictationAudioClip { get; private set; }

        /// <inheritdoc />
        public DictationEventData(EventSystem eventSystem) : base(eventSystem) { }

        /// <summary>
        /// Used to initialize/reset the event and populate the data.
        /// </summary>
        /// <param name="inputSource"></param>
        /// <param name="dictationResult"></param>
        /// <param name="dictationAudioClip"></param>
        public void Initialize(IInputSource inputSource, string dictationResult, AudioClip dictationAudioClip = null)
        {
            BaseInitialize(inputSource, RealityToolkit.Input.Definitions.InputAction.None);
            DictationResult = dictationResult;
            DictationAudioClip = dictationAudioClip;
        }
    }
}
