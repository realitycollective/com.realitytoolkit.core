// Copyright (c) Reality Collective. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using RealityCollective.Extensions;
using RealityToolkit.Input.Controllers;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace RealityToolkit.Input.Hands.Poses
{
    /// <summary>
    /// Records a <see cref="RecordedHandPose"/> using a tracked rigged hand mesh and a <see cref="IHandJointTransformProvider"/>.
    /// </summary>
    [ExecuteInEditMode]
    [RequireComponent(typeof(BaseControllerVisualizer))]
    public class RiggedHandPoseRecorder : MonoBehaviour
    {
        private IHandJointTransformProvider jointTransformProvider;
        private BaseControllerVisualizer visualizer;

        private void Awake()
        {
            if (!Application.isEditor)
            {
                Debug.LogError($"{GetType().Name} is only meant to be used in the editor.");
                this.Destroy();
                return;
            }
        }

#if UNITY_EDITOR
        [ContextMenu("Record pose")]
        public void Record()
        {
            if (!TryGetComponent(out jointTransformProvider))
            {
                Debug.LogError($"{GetType().Name} requires an {nameof(IHandJointTransformProvider)} on the {nameof(GameObject)}.", this);
                return;
            }

            if (!TryGetComponent(out visualizer))
            {
                Debug.LogError($"{GetType().Name} requires an {nameof(BaseControllerVisualizer)} on the {nameof(GameObject)}.", this);
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

            recordedHandPose.Handedness = visualizer.Handedness;
            recordedHandPose.Poses = poses;
            recordedHandPose.Save();
        }
#endif
    }
}
