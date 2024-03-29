﻿// Copyright (c) Reality Collective. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Collections.Generic;
using UnityEngine;

namespace RealityToolkit.Utilities.Audio.Influencers
{
    /// <summary>
    /// An audio effect that limits the frequency range of a sound to simulate being played 
    /// over various telephony or radio sources.
    /// </summary>
    /// <remarks>
    /// For the best results, also attach an <see cref="AudioInfluencerController"/> to the sound
    /// source. This will ensure that the proper frequencies will be restored
    /// when audio influencers are used in the scene.
    /// </remarks>
    [DisallowMultipleComponent]
    [RequireComponent(typeof(AudioSource))]
    [RequireComponent(typeof(AudioLowPassFilter))]
    [RequireComponent(typeof(AudioHighPassFilter))]
    public class AudioLoFiEffect : MonoBehaviour
    {
        [SerializeField]
        [Tooltip("The quality level of the simulated audio source.")]
        private AudioLoFiSourceQualityType sourceQuality;

        /// <summary>
        /// The quality level of the simulated audio source (ex: AM radio).
        /// </summary>
        public AudioLoFiSourceQualityType SourceQuality
        {
            get => sourceQuality;
            set => sourceQuality = value;
        }

        /// <summary>
        /// The audio influencer controller that will be updated when filter settings are changed.
        /// </summary>
        [SerializeField]
        [HideInInspector]   // The inspector will already have a reference to the object, this avoids duplication.
        private AudioInfluencerController influencerController = null;

        /// <summary>
        /// The audio filter settings that match the selected source quality.
        /// </summary>
        private AudioLoFiFilterSettings filterSettings;

        /// <summary>
        /// The filters used to simulate the source quality.
        /// </summary>
        private AudioLowPassFilter lowPassFilter;
        private AudioHighPassFilter highPassFilter;

        /// <summary>
        /// Collection used to look up the filter settings that match the selected
        /// source quality.
        /// </summary>
        private readonly Dictionary<AudioLoFiSourceQualityType, AudioLoFiFilterSettings> sourceQualityFilterSettings = new Dictionary<AudioLoFiSourceQualityType, AudioLoFiFilterSettings>();

        private void OnValidate()
        {
            influencerController = gameObject.GetComponent<AudioInfluencerController>();
        }

        private void Awake()
        {
            LoadQualityFilterSettings();
            filterSettings = sourceQualityFilterSettings[SourceQuality];

            lowPassFilter = gameObject.GetComponent<AudioLowPassFilter>();
            lowPassFilter.cutoffFrequency = filterSettings.LowPassCutoff;

            highPassFilter = gameObject.GetComponent<AudioHighPassFilter>();
            highPassFilter.cutoffFrequency = filterSettings.HighPassCutoff;
        }

        private void Update()
        {
            AudioLoFiFilterSettings newSettings = sourceQualityFilterSettings[SourceQuality];

            if (newSettings == filterSettings) { return; }

            // If we have an attached AudioInfluencerController, we must let it know
            // about our filter settings change, otherwise other effects may not behave
            // as expected.
            if (influencerController != null)
            {
                influencerController.NativeLowPassCutoffFrequency = newSettings.LowPassCutoff;
                influencerController.NativeHighPassCutoffFrequency = newSettings.HighPassCutoff;
            }

            filterSettings = newSettings;
            lowPassFilter.cutoffFrequency = filterSettings.LowPassCutoff;
            highPassFilter.cutoffFrequency = filterSettings.HighPassCutoff;
        }

        /// <summary>
        /// Populates the source quality filter settings collection.
        /// </summary>
        private void LoadQualityFilterSettings()
        {
            if (sourceQualityFilterSettings.Keys.Count > 0) { return; }

            sourceQualityFilterSettings.Add(
                AudioLoFiSourceQualityType.FullRange,
                new AudioLoFiFilterSettings(10, 22000));    // Frequency range: 10 Hz - 22 kHz
            sourceQualityFilterSettings.Add(
                AudioLoFiSourceQualityType.NarrowBandTelephony,
                new AudioLoFiFilterSettings(300, 3400));    // Frequency range: 300 Hz - 3.4 kHz
            sourceQualityFilterSettings.Add(
                AudioLoFiSourceQualityType.WideBandTelephony,
                new AudioLoFiFilterSettings(50, 7000));     // Frequency range: 50 Hz - 7 kHz
            sourceQualityFilterSettings.Add(
                AudioLoFiSourceQualityType.AmRadio,
                new AudioLoFiFilterSettings(40, 5000));     // Frequency range: 40 Hz - 5 kHz
            sourceQualityFilterSettings.Add(
                AudioLoFiSourceQualityType.FmRadio,
                new AudioLoFiFilterSettings(30, 15000));    // Frequency range: 30 Hz - 15 kHz
        }

        /// <summary>
        /// Settings for the filters used to simulate a low fidelity sound source.
        /// </summary>
        /// <remarks>
        /// This struct is solely for the private use of the AudioLoFiEffect class.
        /// </remarks>
        private struct AudioLoFiFilterSettings
        {
            /// <summary>
            /// The frequency below which sound will be heard.
            /// </summary>
            public float LowPassCutoff { get; }

            /// <summary>
            /// The frequency above which sound will be heard.
            /// </summary>
            public float HighPassCutoff { get; }

            /// <summary>
            /// FilterSettings constructor.
            /// </summary>
            /// <param name="highPassCutoff">High pass filter cutoff frequency.</param>
            /// <param name="lowPassCutoff">Low pass filter cutoff frequency.</param>
            public AudioLoFiFilterSettings(float highPassCutoff, float lowPassCutoff) : this()
            {
                HighPassCutoff = highPassCutoff;
                LowPassCutoff = lowPassCutoff;
            }

            /// <summary>
            /// Checks to see if two FilterSettings objects are equivalent.
            /// </summary>
            /// <returns>True if equivalent, false otherwise.</returns>
            public static bool operator ==(AudioLoFiFilterSettings a, AudioLoFiFilterSettings b)
            {
                return a.Equals(b);
            }

            /// <summary>
            /// Checks to see if two FilterSettings objects are not equivalent.
            /// </summary>
            /// <returns>False if equivalent, true otherwise.</returns>
            public static bool operator !=(AudioLoFiFilterSettings a, AudioLoFiFilterSettings b)
            {
                return !(a.Equals(b));
            }

            /// <summary>
            /// Checks to see if a object is equivalent to this AudioLoFiFilterSettings.
            /// </summary>
            /// <returns>True if equivalent, false otherwise.</returns>
            public override bool Equals(object obj)
            {
                return obj != null && (obj is AudioLoFiFilterSettings other && Equals(other));
            }

            /// <summary>
            /// Checks to see if a object is equivalent to this AudioLoFiFilterSettings.
            /// </summary>
            /// <returns>True if equivalent, false otherwise.</returns>
            public bool Equals(AudioLoFiFilterSettings other)
            {
                return (other.LowPassCutoff.Equals(LowPassCutoff)) && (other.HighPassCutoff.Equals(HighPassCutoff));
            }

            /// <summary>
            /// Generates a hash code representing this FilterSettings.
            /// </summary>
            /// <returns></returns>
            public override int GetHashCode()
            {
                return $"[{GetType().Name}] Low: {LowPassCutoff}, High: {HighPassCutoff}".GetHashCode();
            }
        }
    }
}
