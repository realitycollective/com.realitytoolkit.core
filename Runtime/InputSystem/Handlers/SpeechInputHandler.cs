﻿// Copyright (c) Reality Collective. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using RealityToolkit.EventDatum.Input;
using RealityToolkit.Input.Definitions;
using RealityToolkit.Input.Interfaces.Handlers;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace RealityToolkit.Input.Handlers
{
    /// <summary>
    /// This component handles the speech input events raised form the <see cref="IMixedRealityInputSystem"/>.
    /// </summary>
    [DisallowMultipleComponent]
    public class SpeechInputHandler : BaseInputHandler, ISpeechHandler
    {
        /// <summary>
        /// The keywords to be recognized and optional keyboard shortcuts.
        /// </summary>
        public KeywordAndResponse[] Keywords => keywords;

        [SerializeField]
        [Tooltip("The keywords to be recognized and optional keyboard shortcuts.")]
        private KeywordAndResponse[] keywords = new KeywordAndResponse[0];

        [SerializeField]
        [Tooltip("Keywords are persistent across all scenes.  This Speech Input Handler instance will not be destroyed when loading a new scene.")]
        private bool persistentKeywords = false;

        private readonly Dictionary<string, UnityEvent> responses = new Dictionary<string, UnityEvent>();

        #region MonoBehaviour Implementation

        protected override void Start()
        {
            base.Start();

            if (persistentKeywords)
            {
                Debug.Assert(gameObject.transform.parent == null, "Persistent keyword GameObject must be at the root level of the scene hierarchy.");
                DontDestroyOnLoad(gameObject);
            }

            // Convert the struct array into a dictionary, with the keywords and the methods as the values.
            // This helps easily link the keyword recognized to the UnityEvent to be invoked.
            int keywordCount = keywords.Length;
            for (int index = 0; index < keywordCount; index++)
            {
                KeywordAndResponse keywordAndResponse = keywords[index];
                string keyword = keywordAndResponse.Keyword.ToLower();

                if (responses.ContainsKey(keyword))
                {
                    Debug.LogError($"Duplicate keyword \'{keyword}\' specified in \'{gameObject.name}\'.");
                }
                else
                {
                    responses.Add(keyword, keywordAndResponse.Response);
                }
            }
        }

        #endregion MonoBehaviour Implementation

        #region IMixedRealitySpeechHandler Implementation

        void ISpeechHandler.OnSpeechKeywordRecognized(SpeechEventData eventData)
        {
            // Check to make sure the recognized keyword exists in the methods dictionary, then invoke the corresponding method.
            if (enabled && responses.TryGetValue(eventData.RecognizedText.ToLower(), out UnityEvent keywordResponse))
            {
                keywordResponse.Invoke();
                eventData.Use();
            }
        }

        #endregion  IMixedRealitySpeechHandler Implementation
    }
}
