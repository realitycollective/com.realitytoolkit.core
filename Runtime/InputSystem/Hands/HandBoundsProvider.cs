// Copyright (c) Reality Collective. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using RealityToolkit.Definitions.Utilities;
using System.Collections.Generic;
using UnityEngine;

namespace RealityToolkit.InputSystem.Hands
{
    /// <summary>
    /// Computes bounds for each <see cref="HandBoundsLOD"/> of a <see cref="IHandController"/>.
    /// </summary>
    internal sealed class HandBoundsProvider
    {
        private readonly Bounds[] cachedPalmBounds = new Bounds[4];
        private readonly Bounds[] cachedThumbBounds = new Bounds[2];
        private readonly Bounds[] cachedIndexFingerBounds = new Bounds[2];
        private readonly Bounds[] cachedMiddleFingerBounds = new Bounds[2];
        private readonly Bounds[] cachedRingFingerBounds = new Bounds[2];
        private readonly Bounds[] cachedLittleFingerBounds = new Bounds[2];
        private readonly Dictionary<TrackedHandBounds, Bounds[]> bounds = new Dictionary<TrackedHandBounds, Bounds[]>();

        public IReadOnlyDictionary<TrackedHandBounds, Bounds[]> Bounds => bounds;

        public void Update(HandBoundsLOD handBoundsLOD, ref Dictionary<TrackedHandJoint, MixedRealityPose> jointPosesDictionary)
        {
            if (handBoundsLOD == HandBoundsLOD.Low)
            {
                UpdateHandBounds(ref jointPosesDictionary);
            }
            else if (handBoundsLOD == HandBoundsLOD.High)
            {
                UpdatePalmBounds(ref jointPosesDictionary);
                UpdateThumbBounds(ref jointPosesDictionary);
                UpdateIndexFingerBounds(ref jointPosesDictionary);
                UpdateMiddleFingerBounds(ref jointPosesDictionary);
                UpdateRingFingerBounds(ref jointPosesDictionary);
                UpdateLittleFingerBounds(ref jointPosesDictionary);
            }
        }

        private void UpdatePalmBounds(ref Dictionary<TrackedHandJoint, MixedRealityPose> jointPosesDictionary)
        {
            if (jointPosesDictionary.TryGetValue(TrackedHandJoint.LittleMetacarpal, out var pinkyMetacarpalPose) &&
                jointPosesDictionary.TryGetValue(TrackedHandJoint.LittleProximal, out var pinkyKnucklePose) &&
                jointPosesDictionary.TryGetValue(TrackedHandJoint.RingMetacarpal, out var ringMetacarpalPose) &&
                jointPosesDictionary.TryGetValue(TrackedHandJoint.RingProximal, out var ringKnucklePose) &&
                jointPosesDictionary.TryGetValue(TrackedHandJoint.MiddleMetacarpal, out var middleMetacarpalPose) &&
                jointPosesDictionary.TryGetValue(TrackedHandJoint.MiddleProximal, out var middleKnucklePose) &&
                jointPosesDictionary.TryGetValue(TrackedHandJoint.IndexMetacarpal, out var indexMetacarpalPose) &&
                jointPosesDictionary.TryGetValue(TrackedHandJoint.IndexProximal, out var indexKnucklePose))
            {
                // Palm bounds are a composite of each finger's metacarpal -> knuckle joint bounds.
                // Excluding the thumb here.

                // Index
                var indexPalmBounds = new Bounds(indexMetacarpalPose.Position, Vector3.zero);
                indexPalmBounds.Encapsulate(indexKnucklePose.Position);
                cachedPalmBounds[0] = indexPalmBounds;

                // Middle
                var middlePalmBounds = new Bounds(middleMetacarpalPose.Position, Vector3.zero);
                middlePalmBounds.Encapsulate(middleKnucklePose.Position);
                cachedPalmBounds[1] = middlePalmBounds;

                // Ring
                var ringPalmBounds = new Bounds(ringMetacarpalPose.Position, Vector3.zero);
                ringPalmBounds.Encapsulate(ringKnucklePose.Position);
                cachedPalmBounds[2] = ringPalmBounds;

                // Pinky
                var pinkyPalmBounds = new Bounds(pinkyMetacarpalPose.Position, Vector3.zero);
                pinkyPalmBounds.Encapsulate(pinkyKnucklePose.Position);
                cachedPalmBounds[3] = pinkyPalmBounds;

                // Update cached bounds entry.
                if (bounds.ContainsKey(TrackedHandBounds.Palm))
                {
                    bounds[TrackedHandBounds.Palm] = cachedPalmBounds;
                }
                else
                {
                    bounds.Add(TrackedHandBounds.Palm, cachedPalmBounds);
                }
            }
        }

        private void UpdateHandBounds(ref Dictionary<TrackedHandJoint, MixedRealityPose> jointPosesDictionary)
        {
            if (jointPosesDictionary.TryGetValue(TrackedHandJoint.Palm, out var palmPose))
            {
                var newHandBounds = new Bounds(palmPose.Position, Vector3.zero);

                foreach (var kvp in jointPosesDictionary)
                {
                    if (kvp.Key == TrackedHandJoint.Palm)
                    {
                        continue;
                    }

                    newHandBounds.Encapsulate(kvp.Value.Position);
                }

                if (bounds.ContainsKey(TrackedHandBounds.Hand))
                {
                    bounds[TrackedHandBounds.Hand] = new[] { newHandBounds };
                }
                else
                {
                    bounds.Add(TrackedHandBounds.Hand, new[] { newHandBounds });
                }
            }
        }

        private void UpdateThumbBounds(ref Dictionary<TrackedHandJoint, MixedRealityPose> jointPosesDictionary)
        {
            if (jointPosesDictionary.TryGetValue(TrackedHandJoint.ThumbMetacarpal, out var knucklePose) &&
                jointPosesDictionary.TryGetValue(TrackedHandJoint.ThumbProximal, out var middlePose) &&
                jointPosesDictionary.TryGetValue(TrackedHandJoint.ThumbTip, out var tipPose))
            {
                // Thumb bounds include metacarpal -> proximal and proximal -> tip bounds.

                // Knuckle to middle joint bounds.
                var knuckleToMiddleBounds = new Bounds(knucklePose.Position, Vector3.zero);
                knuckleToMiddleBounds.Encapsulate(middlePose.Position);
                cachedThumbBounds[0] = knuckleToMiddleBounds;

                // Middle to tip joint bounds.
                var middleToTipBounds = new Bounds(middlePose.Position, Vector3.zero);
                middleToTipBounds.Encapsulate(tipPose.Position);
                cachedThumbBounds[1] = middleToTipBounds;

                // Update cached bounds entry.
                if (bounds.ContainsKey(TrackedHandBounds.Thumb))
                {
                    bounds[TrackedHandBounds.Thumb] = cachedThumbBounds;
                }
                else
                {
                    bounds.Add(TrackedHandBounds.Thumb, cachedThumbBounds);
                }
            }
        }

        private void UpdateIndexFingerBounds(ref Dictionary<TrackedHandJoint, MixedRealityPose> jointPosesDictionary)
        {
            if (jointPosesDictionary.TryGetValue(TrackedHandJoint.IndexProximal, out var knucklePose) &&
                jointPosesDictionary.TryGetValue(TrackedHandJoint.IndexIntermediate, out var middlePose) &&
                jointPosesDictionary.TryGetValue(TrackedHandJoint.IndexTip, out var tipPose))
            {
                // Index finger bounds include knuckle -> middle and middle -> tip bounds.

                // Knuckle to middle joint bounds.
                var knuckleToMiddleBounds = new Bounds(knucklePose.Position, Vector3.zero);
                knuckleToMiddleBounds.Encapsulate(middlePose.Position);
                cachedIndexFingerBounds[0] = knuckleToMiddleBounds;

                // Middle to tip joint bounds.
                var middleToTipBounds = new Bounds(middlePose.Position, Vector3.zero);
                middleToTipBounds.Encapsulate(tipPose.Position);
                cachedIndexFingerBounds[1] = middleToTipBounds;

                // Update cached bounds entry.
                if (bounds.ContainsKey(TrackedHandBounds.IndexFinger))
                {
                    bounds[TrackedHandBounds.IndexFinger] = cachedIndexFingerBounds;
                }
                else
                {
                    bounds.Add(TrackedHandBounds.IndexFinger, cachedIndexFingerBounds);
                }
            }
        }

        private void UpdateMiddleFingerBounds(ref Dictionary<TrackedHandJoint, MixedRealityPose> jointPosesDictionary)
        {
            if (jointPosesDictionary.TryGetValue(TrackedHandJoint.MiddleProximal, out var knucklePose) &&
                jointPosesDictionary.TryGetValue(TrackedHandJoint.MiddleIntermediate, out var middlePose) &&
                jointPosesDictionary.TryGetValue(TrackedHandJoint.MiddleTip, out var tipPose))
            {
                // Middle finger bounds include knuckle -> middle and middle -> tip bounds.

                // Knuckle to middle joint bounds.
                var knuckleToMiddleBounds = new Bounds(knucklePose.Position, Vector3.zero);
                knuckleToMiddleBounds.Encapsulate(middlePose.Position);
                cachedMiddleFingerBounds[0] = knuckleToMiddleBounds;

                // Middle to tip joint bounds.
                var middleToTipBounds = new Bounds(middlePose.Position, Vector3.zero);
                middleToTipBounds.Encapsulate(tipPose.Position);
                cachedMiddleFingerBounds[1] = middleToTipBounds;

                // Update cached bounds entry.
                if (bounds.ContainsKey(TrackedHandBounds.MiddleFinger))
                {
                    bounds[TrackedHandBounds.MiddleFinger] = cachedMiddleFingerBounds;
                }
                else
                {
                    bounds.Add(TrackedHandBounds.MiddleFinger, cachedMiddleFingerBounds);
                }
            }
        }

        private void UpdateRingFingerBounds(ref Dictionary<TrackedHandJoint, MixedRealityPose> jointPosesDictionary)
        {
            if (jointPosesDictionary.TryGetValue(TrackedHandJoint.RingProximal, out var knucklePose) &&
                jointPosesDictionary.TryGetValue(TrackedHandJoint.RingIntermediate, out var middlePose) &&
                jointPosesDictionary.TryGetValue(TrackedHandJoint.RingTip, out var tipPose))
            {
                // Ring finger bounds include knuckle -> middle and middle -> tip bounds.

                // Knuckle to middle joint bounds.
                var knuckleToMiddleBounds = new Bounds(knucklePose.Position, Vector3.zero);
                knuckleToMiddleBounds.Encapsulate(middlePose.Position);
                cachedRingFingerBounds[0] = knuckleToMiddleBounds;

                // Middle to tip joint bounds.
                var middleToTipBounds = new Bounds(middlePose.Position, Vector3.zero);
                middleToTipBounds.Encapsulate(tipPose.Position);
                cachedRingFingerBounds[1] = middleToTipBounds;

                // Update cached bounds entry.
                if (bounds.ContainsKey(TrackedHandBounds.RingFinger))
                {
                    bounds[TrackedHandBounds.RingFinger] = cachedRingFingerBounds;
                }
                else
                {
                    bounds.Add(TrackedHandBounds.RingFinger, cachedRingFingerBounds);
                }
            }
        }

        private void UpdateLittleFingerBounds(ref Dictionary<TrackedHandJoint, MixedRealityPose> jointPosesDictionary)
        {
            if (jointPosesDictionary.TryGetValue(TrackedHandJoint.LittleProximal, out var knucklePose) &&
                jointPosesDictionary.TryGetValue(TrackedHandJoint.LittleIntermediate, out var middlePose) &&
                jointPosesDictionary.TryGetValue(TrackedHandJoint.LittleTip, out var tipPose))
            {
                // Pinky finger bounds include knuckle -> middle and middle -> tip bounds.

                // Knuckle to middle joint bounds.
                var knuckleToMiddleBounds = new Bounds(knucklePose.Position, Vector3.zero);
                knuckleToMiddleBounds.Encapsulate(middlePose.Position);
                cachedLittleFingerBounds[0] = knuckleToMiddleBounds;

                // Middle to tip joint bounds.
                var middleToTipBounds = new Bounds(middlePose.Position, Vector3.zero);
                middleToTipBounds.Encapsulate(tipPose.Position);
                cachedLittleFingerBounds[1] = middleToTipBounds;

                // Update cached bounds entry.
                if (bounds.ContainsKey(TrackedHandBounds.Pinky))
                {
                    bounds[TrackedHandBounds.Pinky] = cachedLittleFingerBounds;
                }
                else
                {
                    bounds.Add(TrackedHandBounds.Pinky, cachedLittleFingerBounds);
                }
            }
        }
    }
}
