// Copyright (c) Reality Collective. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using RealityCollective.Definitions.Utilities;
using System.Collections.Generic;
using UnityEngine;

namespace RealityToolkit.Input.Hands.Poses
{
    /// <summary>
    /// A <see cref="RecordedHandPose"/> stores the bone information for a rigged hand mesh
    /// and makes it reusable for interactions and such, where the hand rig should apply a specific
    /// pose while grabbing something e.g.
    /// </summary>
    public class RecordedHandPose : ScriptableObject
    {
        private readonly Dictionary<HandJoint, Pose> poseDict = new Dictionary<HandJoint, Pose>();

        [SerializeField, Tooltip("The handedness the pose was recorded with.")]
        private Handedness handedness = Handedness.Left;

        /// <summary>
        /// The <see cref="RealityCollective.Definitions.Utilities.Handedness"/> the pose was recorded with.
        /// </summary>
        public Handedness Handedness
        {
            get => handedness;
            set => handedness = value;
        }

        [SerializeField, Tooltip("Recorded joint poses.")]
        private List<RecordedJointPose> poses = null;

        /// <summary>
        /// All recorded <see cref="RecordedJointPose"/>s.
        /// </summary>
        public List<RecordedJointPose> Poses
        {
            get => poses;
            set => poses = value;
        }

        private void OnValidate()
        {
            poseDict.Clear();

            if (Poses != null)
            {
                foreach (var pose in Poses)
                {
                    poseDict.Add(pose.Joint, pose.Pose);
                }
            }
        }

        /// <summary>
        /// Gets the recorded <see cref="Pose"/> for <paramref name="joint"/>,
        /// if it exists in the recording.
        /// </summary>
        /// <param name="joint">The <see cref="HandJoint"/> to lookup the <see cref="Pose"/> for.</param>
        /// <param name="pose">The found <see cref="Pose"/>, if any.</param>
        /// <returns><c>true</c>, if found.</returns>
        public bool TryGetPose(HandJoint joint, out Pose pose)
        {
            if (poseDict.TryGetValue(joint, out pose))
            {
                return true;
            }

            return false;
        }

#if UNITY_EDITOR
        /// <summary>
        /// Saves the <see cref="RecordedHandPose"/> into an asset file.
        /// </summary>
        public void Save()
        {
            UnityEditor.AssetDatabase.CreateAsset(this, System.IO.Path.Join("Assets", "RealityToolkit.Generated", $"{nameof(RecordedHandPose)}.asset"));
            UnityEditor.AssetDatabase.Refresh();
        }
#endif
    }
}
