// Copyright (c) Reality Collective. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

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

#if UNITY_EDITOR
        /// <summary>
        /// Saves the <see cref="RecordedHandPose"/> into an asset file.
        /// </summary>
        public void Save()
        {

        }
#endif
    }
}
