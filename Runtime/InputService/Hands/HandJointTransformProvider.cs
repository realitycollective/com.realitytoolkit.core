// Copyright (c) Reality Collective. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;
using UnityEngine;

namespace RealityToolkit.Input.Hands
{
    /// <summary>
    /// Default implementation for <see cref="IHandJointTransformProvider"/>. Manages a collection of
    /// <see cref="Transform"/>s stored in a dictionary for fast access using <see cref="GetTransform(HandJoint)"/>.
    /// </summary>
    public class HandJointTransformProvider : MonoBehaviour, IHandJointTransformProvider
    {
        [Serializable]
        public class JointTransformPair
        {
            public HandJoint Joint;
            public Transform Transform;
        }

        [SerializeField, Tooltip("Map transforms to hand joints here.")]
        private List<JointTransformPair> transforms = null;

        private readonly Dictionary<HandJoint, Transform> pairs = new Dictionary<HandJoint, Transform>();

        private void Awake()
        {
            if (transforms == null)
            {
                transforms = new List<JointTransformPair>();
            }

            foreach (var item in transforms)
            {
                pairs.Add(item.Joint, item.Transform);
            }
        }

        /// <inheritdoc/>
        public bool TryGetTransform(HandJoint joint, out Transform transform)
        {
            if (pairs.ContainsKey(joint))
            {
                transform = pairs[joint];
                return true;
            }

            transform = null;
            return false;
        }
    }
}
