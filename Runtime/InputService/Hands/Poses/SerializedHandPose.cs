// Copyright (c) Reality Collective. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Newtonsoft.Json;
using RealityCollective.Definitions.Utilities;
using System.Collections.Generic;

namespace RealityToolkit.Input.Hands.Poses
{
    /// <summary>
    /// This is a data transfer model definition used by the <see cref="HandPoseRecorder"/>
    /// during play mode. When in play mode, we cannot save to an asset directly. Instead we
    /// serialize the recorded <see cref="HandPose"/> to JSON and save a text file, that we can
    /// convert to a <see cref="HandPose"/> asset once not in play mode anymore.
    /// </summary>
    public sealed class SerializedHandPose
    {
        /// <summary>
        /// Since the Unity <see cref="UnityEngine.Pose"/> cannot be serialized
        /// to JSON we need a custom transfer model.
        /// </summary>
        public sealed class SerializedJointPose
        {
            [JsonProperty(PropertyName = "joint")]
            public HandJoint Joint { get; set; }

            [JsonProperty(PropertyName = "rosX")]
            public float PosX { get; set; }

            [JsonProperty(PropertyName = "rosY")]
            public float PosY { get; set; }

            [JsonProperty(PropertyName = "rosZ")]
            public float PosZ { get; set; }

            [JsonProperty(PropertyName = "rotX")]
            public float RotX { get; set; }

            [JsonProperty(PropertyName = "rotY")]
            public float RotY { get; set; }

            [JsonProperty(PropertyName = "rotZ")]
            public float RotZ { get; set; }

            [JsonProperty(PropertyName = "rotW")]
            public float RotW { get; set; }
        }

        /// <summary>
        /// The <see cref="Handedness"/> the pose was recorded with.
        /// </summary>
        [JsonProperty(PropertyName = "recordedHandedness")]
        public Handedness RecordedHandedness { get; set; }

        /// <summary>
        /// All recorded <see cref="JointPose"/>s.
        /// </summary>
        [JsonProperty(PropertyName = "poses")]
        public List<SerializedJointPose> Poses { get; set; }
    }
}
