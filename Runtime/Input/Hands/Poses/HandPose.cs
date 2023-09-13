// Copyright (c) Reality Collective. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using RealityCollective.Definitions.Utilities;
using System.Collections.Generic;
using UnityEngine;
using RealityCollective.Extensions;

#if UNITY_EDITOR
using System.Linq;
#endif

namespace RealityToolkit.Input.Hands.Poses
{
    /// <summary>
    /// A <see cref="HandPose"/> stores the bone information for a rigged hand mesh
    /// and makes it reusable for interactions and such, where the hand rig should apply a specific
    /// pose while grabbing something e.g.
    /// </summary>
    public class HandPose : ScriptableObject
    {
        private readonly Dictionary<HandJoint, Pose> posesDict = new Dictionary<HandJoint, Pose>();
        private readonly Dictionary<HandJoint, Pose> mirroredPosesDict = new Dictionary<HandJoint, Pose>();

        [SerializeField, Tooltip("The handedness the pose was recorded with.")]
        private Handedness recordedHandedness = Handedness.Left;

        /// <summary>
        /// The <see cref="Handedness"/> the pose was recorded with.
        /// </summary>
        public Handedness RecordedHandedness
        {
            get => recordedHandedness;
            set => recordedHandedness = value;
        }

        [SerializeField, Tooltip("Recorded joint poses.")]
        private List<JointPose> poses = null;

        /// <summary>
        /// All recorded <see cref="JointPose"/>s.
        /// </summary>
        public List<JointPose> Poses
        {
            get => poses;
            set => poses = value;
        }

        private void OnValidate()
        {
            posesDict.Clear();
            mirroredPosesDict.Clear();

            if (Poses != null)
            {
                foreach (var pose in Poses)
                {
                    posesDict.Add(pose.Joint, pose.Pose);
                    mirroredPosesDict.Add(pose.Joint, MirrorPose(pose.Pose));
                }
            }
        }

        /// <summary>
        /// Gets the recorded <see cref="Pose"/> for <paramref name="joint"/>,
        /// if it exists in the recording.
        /// </summary>
        /// <param name="handedness">The <see cref="Handedness"/> to get the pose for.</param>
        /// <param name="joint">The <see cref="HandJoint"/> to lookup the <see cref="Pose"/> for.</param>
        /// <param name="pose">The found <see cref="Pose"/>, if any.</param>
        /// <returns><c>true</c>, if found.</returns>
        public bool TryGetPose(Handedness handedness, HandJoint joint, out Pose pose)
        {
            if (RecordedHandedness == handedness &&
                posesDict.TryGetValue(joint, out pose))
            {
                return true;
            }
            else if (mirroredPosesDict.TryGetValue(joint, out pose))
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Mirrors the <see cref="Pose"/>. If it was recorded using <see cref="Handedness.Left"/>,
        /// it will be mirrored to <see cref="Handedness.Right"/> and vice versa.
        /// </summary>
        /// <param name="pose">The <see cref="Pose"/> to mirror.</param>
        /// <returns>Mirrored <see cref="Pose"/> for the opposite <see cref="Handedness"/>.</returns>
        private Pose MirrorPose(Pose pose)
        {
            var position = pose.position;
            position.x *= -1f;

            var rotation = pose.rotation.eulerAngles;
            rotation.y = -rotation.y;
            rotation.z = -rotation.z;

            return new Pose(position, Quaternion.Euler(rotation));
        }

#if UNITY_EDITOR

        /// <summary>
        /// Creates a mirrored version of the hand pose asset.
        /// </summary>
        public void Mirror()
        {
            if (Application.isPlaying)
            {
                Debug.LogError($"Cannot create asset while in play mode.", this);
                return;
            }

            var mirroredHandPose = CreateInstance<HandPose>();
            mirroredHandPose.RecordedHandedness = RecordedHandedness == Handedness.Left ?
                Handedness.Right :
                Handedness.Left;

            mirroredHandPose.Poses = Poses.Select(p => new JointPose()
            {
                Joint = p.Joint,
                Pose = new Pose(p.Pose.position.Mul(new Vector3(-1f, 0f, -1f)),
                p.Pose.rotation)
            }).ToList();

            mirroredHandPose.Save();
        }

        /// <summary>
        /// Saves the <see cref="HandPose"/> to an asset file.
        /// </summary>
        public void Save()
        {
            UnityEditor.AssetDatabase.CreateAsset(this, System.IO.Path.Join("Assets", "RealityToolkit.Generated", $"{nameof(HandPose)}_{RecordedHandedness}.asset"));
            UnityEditor.AssetDatabase.Refresh();
        }
#endif
    }
}
