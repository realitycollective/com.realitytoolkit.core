﻿// Copyright (c) Reality Collective. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using RealityCollective.Definitions.Utilities;
using UnityEngine;
using RealityToolkit.Input.Definitions;
using RealityCollective.ServiceFramework.Services;
using RealityToolkit.Input.Interfaces;

#if UNITY_STANDALONE_WIN || UNITY_WSA || UNITY_EDITOR_WIN
using UnityEngine.Windows.Speech;
#endif // UNITY_STANDALONE_WIN || UNITY_WSA || UNITY_EDITOR_WIN

namespace RealityToolkit.Input.Modules
{
    /// <summary>
    /// Speech service module for windows 10 based platforms.
    /// </summary>
    [System.Runtime.InteropServices.Guid("12E24F3D-7689-4863-A403-DD7A80DA3C25")]
    public class WindowsSpeechServiceModule : BaseSpeechServiceModule
    {
        /// <inheritdoc />
        public WindowsSpeechServiceModule(string name, uint priority, SpeechCommandsProfile profile, IInputService parentService)
            : base(name, priority, profile, parentService)
        {
#if UNITY_WSA && UNITY_EDITOR
            if (!UnityEditor.PlayerSettings.WSA.GetCapability(UnityEditor.PlayerSettings.WSACapability.Microphone))
            {
                UnityEditor.PlayerSettings.WSA.SetCapability(UnityEditor.PlayerSettings.WSACapability.Microphone, true);
            }
#endif

#if UNITY_STANDALONE_WIN || UNITY_WSA || UNITY_EDITOR_WIN

            if (!ServiceManager.Instance.TryGetServiceProfile<IInputService, InputServiceProfile>(out var inputServiceProfile))
            {
                throw new ArgumentException($"Unable to get a valid {nameof(InputServiceProfile)}!");
            }

            autoStartBehavior = inputServiceProfile.SpeechCommandsProfile.SpeechRecognizerStartBehavior;
            RecognitionConfidenceLevel = inputServiceProfile.SpeechCommandsProfile.SpeechRecognitionConfidenceLevel;
            commands = inputServiceProfile.SpeechCommandsProfile.SpeechCommands;

            var newKeywords = new string[commands.Length];

            for (int i = 0; i < commands.Length; i++)
            {
                newKeywords[i] = commands[i].Keyword;
            }

            if (keywordRecognizer == null)
            {
                try
                {
                    keywordRecognizer = new KeywordRecognizer(newKeywords, (ConfidenceLevel)RecognitionConfidenceLevel);
                }
                catch (UnityException e)
                {
                    switch (e.Message)
                    {
                        case string message when message.Contains("Speech recognition is not supported on this machine."):
                            Debug.LogWarning($"Skipping {nameof(WindowsSpeechServiceModule)} registration.\n{e.Message}");
                            break;
                        default:
                            throw;
                    }
                }
            }
#endif // UNITY_STANDALONE_WIN || UNITY_WSA || UNITY_EDITOR_WIN
        }

#if UNITY_STANDALONE_WIN || UNITY_WSA || UNITY_EDITOR_WIN

        private static AutoStartBehavior autoStartBehavior;

        private static SpeechCommands[] commands;

        private static IInputSource inputSource;

        private static KeywordRecognizer keywordRecognizer;

        #region IService Implementation

        /// <inheritdoc />
        public override void Enable()
        {
            base.Enable();

            if (!Application.isPlaying || commands.Length == 0 || keywordRecognizer == null) { return; }

            inputSource = InputService?.RequestNewGenericInputSource("Windows Speech Input Source");

            keywordRecognizer.OnPhraseRecognized += KeywordRecognizer_OnPhraseRecognized;

            if (autoStartBehavior == AutoStartBehavior.AutoStart)
            {
                StartRecognition();
            }
        }

        /// <inheritdoc />
        public override void Update()
        {
            base.Update();

            if (keywordRecognizer == null || !keywordRecognizer.IsRunning) { return; }

            for (int i = 0; i < commands.Length; i++)
            {
                if (UnityEngine.Input.GetKeyDown(commands[i].KeyCode))
                {
                    OnPhraseRecognized((ConfidenceLevel)RecognitionConfidenceLevel, TimeSpan.Zero, DateTime.UtcNow, commands[i].Keyword);
                }
            }
        }

        /// <inheritdoc />
        public override void Disable()
        {
            base.Disable();

            if (!Application.isPlaying || commands.Length == 0 || keywordRecognizer == null) { return; }

            StopRecognition();

            keywordRecognizer.OnPhraseRecognized -= KeywordRecognizer_OnPhraseRecognized;
        }

        protected override void OnDispose(bool finalizing)
        {
            if (finalizing)
            {
                keywordRecognizer?.Dispose();
                keywordRecognizer = null;
            }

            base.OnDispose(finalizing);
        }

        #endregion IService Implementation

        #region ISpeechDataProvider Implementation

        /// <inheritdoc />
        public override bool IsRecognitionActive => keywordRecognizer != null && keywordRecognizer.IsRunning;

        /// <summary>
        /// The <see cref="RecognitionConfidenceLevel"/> that the <see cref="KeywordRecognizer"/> is using.
        /// </summary>
        public RecognitionConfidenceLevel RecognitionConfidenceLevel { get; }

        /// <inheritdoc />
        public override void StartRecognition()
        {
            if (keywordRecognizer != null && !keywordRecognizer.IsRunning)
            {
                keywordRecognizer.Start();
            }
        }

        /// <inheritdoc />
        public override void StopRecognition()
        {
            if (keywordRecognizer != null && keywordRecognizer.IsRunning)
            {
                keywordRecognizer.Stop();
            }
        }

        private void KeywordRecognizer_OnPhraseRecognized(PhraseRecognizedEventArgs args)
        {
            OnPhraseRecognized(args.confidence, args.phraseDuration, args.phraseStartTime, args.text);
        }

        private void OnPhraseRecognized(ConfidenceLevel confidence, TimeSpan phraseDuration, DateTime phraseStartTime, string text)
        {
            for (int i = 0; i < commands.Length; i++)
            {
                if (commands[i].Keyword == text)
                {
                    InputService.RaiseSpeechCommandRecognized(inputSource, commands[i].Action, (RecognitionConfidenceLevel)confidence, phraseDuration, phraseStartTime, text);
                    break;
                }
            }
        }

        #endregion ISpeechDataProvider Implementation

#endif // UNITY_STANDALONE_WIN || UNITY_WSA || UNITY_EDITOR_WIN

    }
}
