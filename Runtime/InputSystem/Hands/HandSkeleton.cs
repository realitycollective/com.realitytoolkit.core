// Copyright (c) Reality Collective. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using RealityCollective.Extensions;
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
                palm.IsNotNull() &&
                wrist.IsNotNull() &&
                thumbMetacarpal.IsNotNull() &&
                thumbProximal.IsNotNull() &&
                thumbDistal.IsNotNull() &&
                thumbTip.IsNotNull() &&
                indexMetacarpal.IsNotNull() &&
                indexProximal.IsNotNull() &&
                indexIntermediate.IsNotNull() &&
                indexDistal.IsNotNull() &&
                indexTip.IsNotNull() &&
                middleMetacarpal.IsNotNull() &&
                middleProximal.IsNotNull() &&
                middleIntermediate.IsNotNull() &&
                middleDistal.IsNotNull() &&
                middleTip.IsNotNull() &&
                ringMetacarpal.IsNotNull() &&
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
            palm.SetParent(transform);

            thumbMetacarpal = new GameObject(nameof(TrackedHandJoint.ThumbMetacarpal)).transform;
            thumbMetacarpal.SetParent(transform);

            thumbProximal = new GameObject(nameof(TrackedHandJoint.ThumbProximal)).transform;
            thumbProximal.SetParent(thumbMetacarpal);

            thumbDistal = new GameObject(nameof(TrackedHandJoint.ThumbDistal)).transform;
            thumbDistal.SetParent(thumbProximal);

            thumbTip = new GameObject(nameof(TrackedHandJoint.ThumbTip)).transform;
            thumbTip.SetParent(thumbDistal);

            indexMetacarpal = new GameObject(nameof(TrackedHandJoint.IndexMetacarpal)).transform;
            indexMetacarpal.SetParent(transform);

            indexProximal = new GameObject(nameof(TrackedHandJoint.IndexProximal)).transform;
            indexProximal.SetParent(indexMetacarpal);

            indexIntermediate = new GameObject(nameof(TrackedHandJoint.IndexIntermediate)).transform;
            indexIntermediate.SetParent(indexProximal);

            indexDistal = new GameObject(nameof(TrackedHandJoint.IndexDistal)).transform;
            indexDistal.SetParent(indexIntermediate);

            indexTip = new GameObject(nameof(TrackedHandJoint.IndexTip)).transform;
            indexTip.SetParent(indexDistal);

            middleMetacarpal = new GameObject(nameof(TrackedHandJoint.MiddleMetacarpal)).transform;
            middleMetacarpal.SetParent(transform);

            middleProximal = new GameObject(nameof(TrackedHandJoint.MiddleProximal)).transform;
            middleProximal.SetParent(middleMetacarpal);

            middleIntermediate = new GameObject(nameof(TrackedHandJoint.MiddleIntermediate)).transform;
            middleIntermediate.SetParent(middleProximal);

            middleDistal = new GameObject(nameof(TrackedHandJoint.MiddleDistal)).transform;
            middleDistal.SetParent(middleIntermediate);

            middleTip = new GameObject(nameof(TrackedHandJoint.MiddleTip)).transform;
            middleTip.SetParent(middleDistal);

            ringMetacarpal = new GameObject(nameof(TrackedHandJoint.RingMetacarpal)).transform;
            ringMetacarpal.SetParent(transform);

            ringProximal = new GameObject(nameof(TrackedHandJoint.RingProximal)).transform;
            ringProximal.SetParent(ringMetacarpal);

            ringIntermediate = new GameObject(nameof(TrackedHandJoint.RingIntermediate)).transform;
            ringIntermediate.SetParent(ringProximal);

            ringDistal = new GameObject(nameof(TrackedHandJoint.RingDistal)).transform;
            ringDistal.SetParent(ringIntermediate);

            ringTip = new GameObject(nameof(TrackedHandJoint.RingTip)).transform;
            ringTip.SetParent(ringDistal);

            littleMetacarpal = new GameObject(nameof(TrackedHandJoint.LittleMetacarpal)).transform;
            littleMetacarpal.SetParent(transform);

            littleProximal = new GameObject(nameof(TrackedHandJoint.LittleProximal)).transform;
            littleProximal.SetParent(littleMetacarpal);

            littleIntermediate = new GameObject(nameof(TrackedHandJoint.LittleIntermediate)).transform;
            littleIntermediate.SetParent(littleProximal);

            littleDistal = new GameObject(nameof(TrackedHandJoint.LittleDistal)).transform;
            littleDistal.SetParent(littleIntermediate);

            littleTip = new GameObject(nameof(TrackedHandJoint.LittleTip)).transform;
            littleTip.SetParent(littleDistal);
        }
    }
}