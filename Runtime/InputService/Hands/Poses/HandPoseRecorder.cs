// Copyright (c) Reality Collective. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Newtonsoft.Json;
using RealityCollective.Definitions.Utilities;
using RealityCollective.Extensions;
using RealityToolkit.Input.Hands.Visualizers;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace RealityToolkit.Input.Hands.Poses
{
    /// <summary>
    /// A simple recording utility to record <see cref="HandPose"/>s from any
    /// <see cref="BaseHandControllerVisualizer"/>.
    /// </summary>
    public class HandPoseRecorder : MonoBehaviour
    {
        [SerializeField, Tooltip("The handedness of the hand recorded with.")]
        private Handedness recordedHandedness = Handedness.Left;

        [SerializeField, Tooltip("Assign a serialized pose here and use the Convert context action to convert it to an asset.")]
        private TextAsset serializedPose = null;

        protected IHandJointTransformProvider jointTransformProvider;

        /// <inheritdoc/>
        protected virtual void Awake()
        {
            if (!Application.isEditor)
            {
                Debug.LogError($"{GetType().Name} is only meant to be used in the editor.", this);
                this.Destroy();
                return;
            }
        }

#if UNITY_EDITOR

        /// <summary>
        /// Converts a serialized pose to an asset file.
        /// </summary>
        [ContextMenu("Convert serialized pose")]
        public void Convert()
        {
            if (Application.isPlaying)
            {
                Debug.LogError($"Cannot convert to asset while in play mode.", this);
                return;
            }

            if (serializedPose.IsNull())
            {
                Debug.LogError("No serialized pose assigned. Assign a serialized pose text asset to convert it.", this);
                return;
            }

            var pose = JsonConvert.DeserializeObject<SerializedHandPose>(serializedPose.text);
            var recordedHandPose = ScriptableObject.CreateInstance<HandPose>();
            recordedHandPose.RecordedHandedness = pose.RecordedHandedness;
            recordedHandPose.Poses = pose.Poses.Select(p => new JointPose()
            {
                Joint = p.Joint,
                Pose = new Pose(new Vector3(p.PosX, p.PosY, p.PosZ), new Quaternion(p.RotX, p.RotY, p.RotZ, p.RotW))
            }).ToList();

            Save(recordedHandPose);
            UnityEditor.AssetDatabase.DeleteAsset(UnityEditor.AssetDatabase.GetAssetPath(serializedPose));
            serializedPose = null;
        }

        /// <summary>
        /// Records the current pose of the <see cref="BaseHandControllerVisualizer"/>.
        /// </summary>
        [ContextMenu("Record pose")]
        public void Record()
        {
            if (!TryGetComponent(out jointTransformProvider))
            {
                Debug.LogError($"{GetType().Name} requires an {nameof(IHandJointTransformProvider)} on the {nameof(GameObject)}.", this);
                return;
            }

            var recordedHandPose = ScriptableObject.CreateInstance<HandPose>();
            var poses = new List<JointPose>();
            var jointCount = Enum.GetNames(typeof(HandJoint)).Length;

            for (int i = 0; i < jointCount; i++)
            {
                var handJoint = (HandJoint)i;

                if (jointTransformProvider.TryGetTransform(handJoint, out var jointTransform))
                {
                    poses.Add(new JointPose
                    {
                        Joint = handJoint,
                        Pose = new Pose(jointTransform.localPosition, jointTransform.localRotation)
                    });
                }
            }

            recordedHandPose.RecordedHandedness = recordedHandedness;
            recordedHandPose.Poses = poses;

            if (Application.isPlaying)
            {
                var json = JsonConvert.SerializeObject(new SerializedHandPose
                {
                    RecordedHandedness = recordedHandPose.RecordedHandedness,
                    Poses = recordedHandPose.Poses.Select(p => new SerializedHandPose.SerializedJointPose
                    {
                        Joint = p.Joint,
                        PosX = p.Pose.position.x,
                        PosY = p.Pose.position.y,
                        PosZ = p.Pose.position.z,
                        RotX = p.Pose.rotation.x,
                        RotY = p.Pose.rotation.y,
                        RotZ = p.Pose.rotation.z,
                        RotW = p.Pose.rotation.w
                    }).ToList()
                });

                System.IO.File.WriteAllText(System.IO.Path.Join(Application.dataPath, "RealityToolkit.Generated", $"{nameof(SerializedHandPose)}.txt"), json);
                return;
            }

            // When not playing, we can directly save the asset.
            Save(recordedHandPose);
        }

        /// <summary>
        /// Saves the <paramref name="handPose"/> to an asset file.
        /// </summary>
        /// <param name="handPose">The <see cref="HandPose"/>.</param>
        private void Save(HandPose handPose)
        {
            UnityEditor.AssetDatabase.CreateAsset(handPose, System.IO.Path.Join("Assets", "RealityToolkit.Generated", $"{nameof(HandPose)}.asset"));
            UnityEditor.AssetDatabase.Refresh();
        }
#endif
    }
}
