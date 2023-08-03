// Copyright (c) Reality Collective. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using RealityToolkit.Input.Hands;
using RealityToolkit.Input.Hands.Poses;
using RealityToolkit.Input.Hands.Visualizers;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace RealityToolkit.Editor.Input.Hands.Poses
{
    /// <summary>
    /// A simple recording utility to record <see cref="RecordedHandPose"/>s from any
    /// <see cref="BaseHandControllerVisualizer"/>.
    /// </summary>
    [RequireComponent(typeof(BaseHandControllerVisualizer))]
    public class HandPoseRecorder : MonoBehaviour
    {
        protected IHandJointTransformProvider jointTransformProvider;
        protected BaseHandControllerVisualizer visualizer;

        [ContextMenu("Record pose")]
        public void Record()
        {
            if (!TryGetComponent(out visualizer))
            {
                Debug.LogError($"{GetType().Name} requires an {nameof(BaseHandControllerVisualizer)} on the {nameof(GameObject)}.", this);
                return;
            }

            if (!TryGetComponent(out jointTransformProvider))
            {
                Debug.LogError($"{GetType().Name} requires an {nameof(IHandJointTransformProvider)} on the {nameof(GameObject)}.", this);
                return;
            }

            var recordedHandPose = ScriptableObject.CreateInstance<RecordedHandPose>();
            var poses = new List<RecordedJointPose>();
            var jointCount = Enum.GetNames(typeof(HandJoint)).Length;

            for (int i = 0; i < jointCount; i++)
            {
                var handJoint = (HandJoint)i;

                if (jointTransformProvider.TryGetTransform(handJoint, out var jointTransform))
                {
                    poses.Add(new RecordedJointPose
                    {
                        Joint = handJoint,
                        Pose = new Pose(jointTransform.localPosition, jointTransform.localRotation)
                    });
                }
            }

            recordedHandPose.RecordedHandedness = visualizer.Handedness;
            recordedHandPose.Poses = poses;

            UnityEditor.AssetDatabase.CreateAsset(recordedHandPose, System.IO.Path.Join("Assets", "RealityToolkit.Generated", $"{nameof(RecordedHandPose)}.asset"));
            UnityEditor.AssetDatabase.Refresh();
        }
    }
}
