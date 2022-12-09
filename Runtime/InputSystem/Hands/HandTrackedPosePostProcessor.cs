﻿// Copyright (c) Reality Collective. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using RealityCollective.Definitions.Utilities;
using RealityCollective.Extensions;
using RealityToolkit.InputSystem.Extensions;
using System;
using System.Collections.Generic;
using UnityEngine.XR;

namespace RealityToolkit.InputSystem.Hands
{
    /// <summary>
    /// The hand pose processor uses the recorded hand pose definitions
    /// configured in <see cref="Definitions.InputSystem.MixedRealityInputSystemProfile.TrackedPoses"/>
    /// or the platform's <see cref="HandControllerServiceModuleProfile.TrackedPoses"/>
    /// and attempts to recognize a hand's current pose during runtime to provide for
    /// <see cref="HandData.TrackedPoseId"/>.
    /// </summary>
    public sealed class HandTrackedPosePostProcessor : BaseHandPostProcessor
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="handController">The <see cref="IHandController"/> to post process <see cref="HandData"/> for.</param>
        /// <param name="handControllerSettings">Configuration to use when post processing information for the <see cref="IHandController"/>.</param>
        public HandTrackedPosePostProcessor(IHandController handController, HandControllerSettings handControllerSettings)
            : base(handController, handControllerSettings)
        {
            bakedHandDatas = new HandData[handControllerSettings.TrackedPoses.Count];
            definitions = new Dictionary<int, HandControllerPoseProfile>();

            for (int i = 0; i < handControllerSettings.TrackedPoses.Count; i++)
            {
                var item = handControllerSettings.TrackedPoses[i];
                if (item.DidBake)
                {
                    bakedHandDatas[i] = item.ToHandData();
                    definitions.Add(i, item);
                }
                else
                {
                    throw new ArgumentException($"Pose definition {item.Id} was not baked. Only baked poses are supported for recognition.");
                }
            }
        }

        private const int RECOGNITION_FRAME_DELIMITER = 10;
        private const float CURL_STRENGTH_DELTA_THRESHOLD = .25f;
        private const float GRIP_STRENGTH_DELTA_THRESHOLD = .2f;

        private readonly HandData[] bakedHandDatas;
        private readonly Dictionary<int, HandControllerPoseProfile> definitions;
        private int passedFramesSinceRecognitionLeftHand = 0;
        private int passedFramesSinceRecognitionRightHand = 0;

        /// <summary>
        /// The last recognized pose of the left hand.
        /// </summary>
        private string LastTrackedPoseIdLeftHand { get; set; }

        /// <summary>
        /// The last recognized pose of the right hand.
        /// </summary>
        private string LastTrackedPoseIdRightHand { get; set; }

        /// <inheritdoc />
        public override HandData PostProcess(HandData handData)
        {
            // Recognition is pretty expensive so we don't want to
            // do it every frame.
            if (Hand.ControllerHandedness == Handedness.Right && passedFramesSinceRecognitionRightHand < RECOGNITION_FRAME_DELIMITER)
            {
                passedFramesSinceRecognitionRightHand++;
                handData.TrackedPoseId = LastTrackedPoseIdRightHand;
                return handData;
            }
            else if (Hand.ControllerHandedness == Handedness.Left && passedFramesSinceRecognitionLeftHand < RECOGNITION_FRAME_DELIMITER)
            {
                passedFramesSinceRecognitionLeftHand++;
                handData.TrackedPoseId = LastTrackedPoseIdLeftHand;
                return handData;
            }

            var currentHighestProbability = 0f;
            HandControllerPoseProfile recognizedPose = null;

            for (int i = 0; i < bakedHandDatas.Length; i++)
            {
                var bakedHandData = bakedHandDatas[i];
                var probability = Compare(handData, bakedHandData);

                if (probability > currentHighestProbability)
                {
                    currentHighestProbability = probability;
                    recognizedPose = definitions[i];
                }
            }

            handData.TrackedPoseId = recognizedPose.IsNull() ? null : recognizedPose.Id;
            if (Hand.ControllerHandedness == Handedness.Right)
            {
                LastTrackedPoseIdRightHand = handData.TrackedPoseId;
                passedFramesSinceRecognitionRightHand = 0;
            }
            else
            {
                LastTrackedPoseIdLeftHand = handData.TrackedPoseId;
                passedFramesSinceRecognitionLeftHand = 0;
            }

            return handData;
        }

        private static float Compare(HandData runtimeHandData, HandData bakedHandData)
        {
            const int totalTests = 6;
            var passedTests = 0;

            // If the gripping states are not the same it is very unlikely
            // poses are the same so we can quit right away.
            if (runtimeHandData.IsGripping == bakedHandData.IsGripping)
            {
                var runtimeGripStrength = runtimeHandData.GripStrength;
                var bakedGripStrength = bakedHandData.GripStrength;
                if (Math.Abs(runtimeGripStrength - bakedGripStrength) <= GRIP_STRENGTH_DELTA_THRESHOLD)
                {
                    passedTests++;
                }

                var runtimeThumbCurl = runtimeHandData.FingerCurlStrengths[(int)HandFinger.Thumb];
                var bakedThumbCurl = bakedHandData.FingerCurlStrengths[(int)HandFinger.Thumb];
                if (Math.Abs(runtimeThumbCurl - bakedThumbCurl) <= CURL_STRENGTH_DELTA_THRESHOLD)
                {
                    passedTests++;
                }

                var runtimeIndexCurl = runtimeHandData.FingerCurlStrengths[(int)HandFinger.Index];
                var bakedIndexCurl = bakedHandData.FingerCurlStrengths[(int)HandFinger.Index];
                if (Math.Abs(runtimeIndexCurl - bakedIndexCurl) <= CURL_STRENGTH_DELTA_THRESHOLD)
                {
                    passedTests++;
                }

                var runtimeMiddleCurl = runtimeHandData.FingerCurlStrengths[(int)HandFinger.Middle];
                var bakedMiddleCurl = bakedHandData.FingerCurlStrengths[(int)HandFinger.Middle];
                if (Math.Abs(runtimeMiddleCurl - bakedMiddleCurl) <= CURL_STRENGTH_DELTA_THRESHOLD)
                {
                    passedTests++;
                }

                var runtimeRingCurl = runtimeHandData.FingerCurlStrengths[(int)HandFinger.Ring];
                var bakedRingCurl = bakedHandData.FingerCurlStrengths[(int)HandFinger.Ring];
                if (Math.Abs(runtimeRingCurl - bakedRingCurl) <= CURL_STRENGTH_DELTA_THRESHOLD)
                {
                    passedTests++;
                }

                var runtimeLittleCurl = runtimeHandData.FingerCurlStrengths[(int)HandFinger.Pinky];
                var bakedLittleCurl = bakedHandData.FingerCurlStrengths[(int)HandFinger.Pinky];
                if (Math.Abs(runtimeLittleCurl - bakedLittleCurl) <= CURL_STRENGTH_DELTA_THRESHOLD)
                {
                    passedTests++;
                }
            }

            // The more tests have passed, the more likely it is
            // the poses are the same.
            return passedTests / (float)totalTests;
        }
    }
}
