// Copyright (c) Reality Collective. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using RealityCollective.Extensions;
using RealityToolkit.Definitions.Utilities;
using System;
using UnityEngine;

namespace RealityToolkit.InputSystem.Hands
{
    /// <summary>
    /// A <see cref="TrackedHandJoint"/> based hand skeleton.
    /// </summary>
    public class HandSkeleton : MonoBehaviour
    {
        [SerializeField]
        private Transform wrist = null;

        [SerializeField]
        private Transform palm = null;

        [Header("Thumb")]
        [SerializeField]
        private Transform thumbMetacarpal = null;

        [SerializeField]
        private Transform thumbProximal = null;

        [SerializeField]
        private Transform thumbDistal = null;

        [SerializeField]
        private Transform thumbTip = null;

        [Header("Index Finger")]
        [SerializeField]
        private Transform indexMetacarpal = null;

        [SerializeField]
        private Transform indexProximal = null;

        [SerializeField]
        private Transform indexIntermediate = null;

        [SerializeField]
        private Transform indexDistal = null;

        [SerializeField]
        private Transform indexTip = null;

        [Header("Middle Finger")]
        [SerializeField]
        private Transform middleMetacarpal = null;

        [SerializeField]
        private Transform middleProximal = null;

        [SerializeField]
        private Transform middleIntermediate = null;

        [SerializeField]
        private Transform middleDistal = null;

        [SerializeField]
        private Transform middleTip = null;

        [Header("Ring Finger")]
        [SerializeField]
        private Transform ringMetacarpal = null;

        [SerializeField]
        private Transform ringProximal = null;

        [SerializeField]
        private Transform ringIntermediate = null;

        [SerializeField]
        private Transform ringDistal = null;

        [SerializeField]
        private Transform ringTip = null;

        [Header("Little Finger")]
        [SerializeField]
        private Transform littleMetacarpal = null;

        [SerializeField]
        private Transform littleProximal = null;

        [SerializeField]
        private Transform littleIntermediate = null;

        [SerializeField]
        private Transform littleDistal = null;

        [SerializeField]
        private Transform littleTip = null;

        public Transform Wrist => wrist;

        public Transform Palm => palm;

        public Transform ThumbMetacarpal => thumbMetacarpal;

        public Transform ThumbProximal => thumbProximal;

        public Transform ThumbDistal => thumbDistal;

        public Transform ThumbTip => thumbTip;

        public Transform IndexMetacarpal => indexMetacarpal;

        public Transform IndexProximal => indexProximal;

        public Transform IndexIntermediate => indexIntermediate;

        public Transform IndexDistal => indexDistal;

        public Transform IndexTip => indexTip;

        public Transform MiddleMetacarpal => middleMetacarpal;

        public Transform MiddleProximal => middleProximal;

        public Transform MiddleIntermediate => middleIntermediate;

        public Transform MiddleDistal => middleDistal;

        public Transform MiddleTip => middleTip;

        public Transform RingMetacarpal => ringMetacarpal;

        public Transform RingProximal => ringProximal;

        public Transform RingIntermediate => ringIntermediate;

        public Transform RingDistal => ringDistal;

        public Transform RingTip => ringTip;

        public Transform LittleMetacarpal => littleMetacarpal;

        public Transform LittleProximal => littleProximal;

        public Transform LittleIntermediate => littleIntermediate;

        public Transform LittleDistal => littleDistal;

        public Transform LittleTip => littleTip;

        /// <summary>
        /// If <c>true</c>, the skeleton has initialized correctly and all hand bones
        /// have been found in the object's hierarchy. Use <see cref="TryAutoSetup"/> to
        /// setup the skeleton and validate.
        /// </summary>
        public bool IsSetUp =>
                wrist.IsNotNull() &&
                thumbMetacarpal.IsNotNull() &&
                thumbProximal.IsNotNull() &&
                thumbDistal.IsNotNull() &&
                thumbTip.IsNotNull() &&
                indexProximal.IsNotNull() &&
                indexIntermediate.IsNotNull() &&
                indexDistal.IsNotNull() &&
                indexTip.IsNotNull() &&
                middleProximal.IsNotNull() &&
                middleIntermediate.IsNotNull() &&
                middleDistal.IsNotNull() &&
                middleTip.IsNotNull() &&
                ringProximal.IsNotNull() &&
                ringIntermediate.IsNotNull() &&
                ringDistal.IsNotNull() &&
                ringTip.IsNotNull() &&
                littleMetacarpal.IsNotNull() &&
                littleProximal.IsNotNull() &&
                littleIntermediate.IsNotNull() &&
                littleDistal.IsNotNull() &&
                littleTip.IsNotNull();

        /// <summary>
        /// Attempts to find all bones / <see cref="Transform"/>s in the hierarchy using <see cref="jointNames"/>,
        /// that are necessary for the hand skeleton to be considered <see cref="IsSetUp"/>.
        /// </summary>
        [ContextMenu("Try Auto Setup")]
        public void TryAutoSetup()
        {
            if (IsSetUp)
            {
                // The skeleton is already set up.
                return;
            }

            //var jointNames = Enum.GetNames(typeof(TrackedHandJoint));
        }

        /// <summary>
        /// Creates the skeleton transforms in the object's hierarchy.
        /// </summary>
        [ContextMenu("Create")]
        public void Create()
        {
            if (IsSetUp)
            {
                // The skeleton is already set up.
                return;
            }

            wrist = new GameObject(nameof(TrackedHandJoint.Wrist)).transform;
            wrist.SetParent(transform);

            palm = new GameObject(nameof(TrackedHandJoint.Palm)).transform;
            palm.SetParent(wrist);

            thumbMetacarpal = new GameObject(nameof(TrackedHandJoint.ThumbMetacarpal)).transform;
            thumbMetacarpal.SetParent(wrist);

            thumbProximal = new GameObject(nameof(TrackedHandJoint.ThumbProximal)).transform;
            thumbProximal.SetParent(thumbMetacarpal);

            thumbDistal = new GameObject(nameof(TrackedHandJoint.ThumbDistal)).transform;
            thumbDistal.SetParent(thumbProximal);

            thumbTip = new GameObject(nameof(TrackedHandJoint.ThumbTip)).transform;
            thumbTip.SetParent(thumbDistal);

            indexMetacarpal = new GameObject(nameof(TrackedHandJoint.IndexMetacarpal)).transform;
            indexMetacarpal.SetParent(wrist);

            indexProximal = new GameObject(nameof(TrackedHandJoint.IndexProximal)).transform;
            indexProximal.SetParent(indexMetacarpal);

            indexIntermediate = new GameObject(nameof(TrackedHandJoint.IndexIntermediate)).transform;
            indexIntermediate.SetParent(indexProximal);

            indexDistal = new GameObject(nameof(TrackedHandJoint.IndexDistal)).transform;
            indexDistal.SetParent(indexIntermediate);

            indexTip = new GameObject(nameof(TrackedHandJoint.IndexTip)).transform;
            indexTip.SetParent(indexDistal);

            middleMetacarpal = new GameObject(nameof(TrackedHandJoint.MiddleMetacarpal)).transform;
            middleMetacarpal.SetParent(wrist);

            middleProximal = new GameObject(nameof(TrackedHandJoint.MiddleProximal)).transform;
            middleProximal.SetParent(middleMetacarpal);

            middleIntermediate = new GameObject(nameof(TrackedHandJoint.MiddleIntermediate)).transform;
            middleIntermediate.SetParent(middleProximal);

            middleDistal = new GameObject(nameof(TrackedHandJoint.MiddleDistal)).transform;
            middleDistal.SetParent(middleIntermediate);

            middleTip = new GameObject(nameof(TrackedHandJoint.MiddleTip)).transform;
            middleTip.SetParent(middleDistal);

            ringMetacarpal = new GameObject(nameof(TrackedHandJoint.RingMetacarpal)).transform;
            ringMetacarpal.SetParent(wrist);

            ringProximal = new GameObject(nameof(TrackedHandJoint.RingProximal)).transform;
            ringProximal.SetParent(ringMetacarpal);

            ringIntermediate = new GameObject(nameof(TrackedHandJoint.RingIntermediate)).transform;
            ringIntermediate.SetParent(ringProximal);

            ringDistal = new GameObject(nameof(TrackedHandJoint.RingDistal)).transform;
            ringDistal.SetParent(ringIntermediate);

            ringTip = new GameObject(nameof(TrackedHandJoint.RingTip)).transform;
            ringTip.SetParent(ringDistal);

            littleMetacarpal = new GameObject(nameof(TrackedHandJoint.LittleMetacarpal)).transform;
            littleMetacarpal.SetParent(wrist);

            littleProximal = new GameObject(nameof(TrackedHandJoint.LittleProximal)).transform;
            littleProximal.SetParent(littleMetacarpal);

            littleIntermediate = new GameObject(nameof(TrackedHandJoint.LittleIntermediate)).transform;
            littleIntermediate.SetParent(littleProximal);

            littleDistal = new GameObject(nameof(TrackedHandJoint.LittleDistal)).transform;
            littleDistal.SetParent(littleIntermediate);

            littleTip = new GameObject(nameof(TrackedHandJoint.LittleTip)).transform;
            littleTip.SetParent(littleDistal);
        }

        /// <summary>
        /// Gets the <see cref="Transform"/> for <paramref name="trackedHandJoint"/>.
        /// </summary>
        /// <param name="trackedHandJoint">The <see cref="TrackedHandJoint"/> to find the <see cref="Transform"/> for.</param>
        /// <returns><see cref="Transform"/>.</returns>
        public Transform Get(TrackedHandJoint trackedHandJoint)
        {
            switch (trackedHandJoint)
            {
                case TrackedHandJoint.Wrist:
                    return Wrist;
                case TrackedHandJoint.Palm:
                    return Palm;
                case TrackedHandJoint.ThumbMetacarpal:
                    return ThumbMetacarpal;
                case TrackedHandJoint.ThumbProximal:
                    return ThumbProximal;
                case TrackedHandJoint.ThumbDistal:
                    return ThumbDistal;
                case TrackedHandJoint.ThumbTip:
                    return ThumbTip;
                case TrackedHandJoint.IndexMetacarpal:
                    return IndexMetacarpal;
                case TrackedHandJoint.IndexProximal:
                    return IndexProximal;
                case TrackedHandJoint.IndexIntermediate:
                    return IndexIntermediate;
                case TrackedHandJoint.IndexDistal:
                    return IndexDistal;
                case TrackedHandJoint.IndexTip:
                    return IndexTip;
                case TrackedHandJoint.MiddleMetacarpal:
                    return MiddleMetacarpal;
                case TrackedHandJoint.MiddleProximal:
                    return MiddleProximal;
                case TrackedHandJoint.MiddleIntermediate:
                    return MiddleIntermediate;
                case TrackedHandJoint.MiddleDistal:
                    return MiddleDistal;
                case TrackedHandJoint.MiddleTip:
                    return MiddleTip;
                case TrackedHandJoint.RingMetacarpal:
                    return RingMetacarpal;
                case TrackedHandJoint.RingProximal:
                    return RingProximal;
                case TrackedHandJoint.RingIntermediate:
                    return RingIntermediate;
                case TrackedHandJoint.RingDistal:
                    return RingDistal;
                case TrackedHandJoint.RingTip:
                    return RingTip;
                case TrackedHandJoint.LittleMetacarpal:
                    return LittleMetacarpal; ;
                case TrackedHandJoint.LittleProximal:
                    return LittleProximal;
                case TrackedHandJoint.LittleIntermediate:
                    return LittleIntermediate;
                case TrackedHandJoint.LittleDistal:
                    return LittleDistal;
                case TrackedHandJoint.LittleTip:
                    return LittleTip;
            }

            return null;
        }

        /// <summary>
        /// Sets a new <paramref name="pose"/> for the <paramref name="trackedHandJoint"/>.
        /// </summary>
        /// <param name="trackedHandJoint">The <see cref="TrackedHandJoint"/> to update the pose for.</param>
        /// <param name="pose">The new <see cref="MixedRealityPose"/> for the <paramref name="trackedHandJoint"/>.</param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public void Set(TrackedHandJoint trackedHandJoint, MixedRealityPose pose)
        {
            var target = Get(trackedHandJoint);
            if (target.IsNotNull())
            {
                target.localPosition = pose.Position;
                target.localRotation = pose.Rotation;
            }
        }
    }
}