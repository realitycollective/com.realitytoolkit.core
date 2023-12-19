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
        private class JointTransformPair
        {
            public HandJoint Joint;
            public Transform Transform;
        }

        [SerializeField, Tooltip("Map transforms to hand joints here.")]
        private List<JointTransformPair> transforms = null;

        private Dictionary<HandJoint, Transform> Cache { get; } = new Dictionary<HandJoint, Transform>();

        /// <inheritdoc/>
        public event Action JointTransformsChanged;

        private void UpdateCache()
        {
            if (transforms == null)
            {
                transforms = new List<JointTransformPair>();
            }

            if (Cache.Count != transforms.Count)
            {
                Cache.Clear();

                foreach (var item in transforms)
                {
                    Cache.Add(item.Joint, item.Transform);
                }
            }
        }

        private void InvalidateCache() => Cache.Clear();

        /// <inheritdoc/>
        public void SetTransform(HandJoint joint, Transform transform)
        {
            if (transforms == null)
            {
                transforms = new List<JointTransformPair>();
            }

            transforms.Add(new JointTransformPair
            {
                Joint = joint,
                Transform = transform
            });

            // Since we added a new transform, we need to clear the cached
            // dictionary so it gets regenereated next time a transform is looked up.
            InvalidateCache();

            JointTransformsChanged?.Invoke();
        }

        /// <inheritdoc/>
        public bool TryGetTransform(HandJoint joint, out Transform transform)
        {
            UpdateCache();

            if (Cache.ContainsKey(joint))
            {
                transform = Cache[joint];
                return true;
            }

            transform = null;
            return false;
        }
    }
}
