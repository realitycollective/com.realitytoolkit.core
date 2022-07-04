// Copyright (c) Reality Collective. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using RealityToolkit.Definitions.Controllers.Hands;
using RealityToolkit.Definitions.Devices;
using RealityToolkit.Interfaces.InputSystem.Controllers.Hands;
using System.Collections.Generic;
using UnityEngine;

namespace RealityToolkit.Services.InputSystem.Controllers.Hands
{
    public class HandBoundsPostProcessor : BaseHandPostProcessor
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="handController">The <see cref="IHandController"/> to post process <see cref="HandData"/> for.</param>
        /// <param name="handControllerSettings">Configuration to use when post processing information for the <see cref="IHandController"/>.</param>
        public HandBoundsPostProcessor(IHandController handController, HandControllerSettings handControllerSettings)
            : base(handController, handControllerSettings) { }

        private readonly Bounds[] cachedPalmBounds = new Bounds[4];
        private readonly Bounds[] cachedThumbBounds = new Bounds[2];
        private readonly Bounds[] cachedIndexFingerBounds = new Bounds[2];
        private readonly Bounds[] cachedMiddleFingerBounds = new Bounds[2];
        private readonly Bounds[] cachedRingFingerBounds = new Bounds[2];
        private readonly Bounds[] cachedLittleFingerBounds = new Bounds[2];
        private readonly Dictionary<TrackedHandBounds, Bounds[]> bounds = new Dictionary<TrackedHandBounds, Bounds[]>();

        /// <inheritdoc />
        public override HandData PostProcess(HandData handData)
        {
            if (handData.TrackingState == TrackingState.Tracked)
            {
                if (Settings.BoundsMode == HandBoundsLOD.Low)
                {
                    UpdateHandBounds();
                }
                else if (Settings.BoundsMode == HandBoundsLOD.High)
                {
                    UpdatePalmBounds();
                    UpdateThumbBounds();
                    UpdateIndexFingerBounds();
                    UpdateMiddleFingerBounds();
                    UpdateRingFingerBounds();
                    UpdateLittleFingerBounds();
                }
            }

            handData.Bounds = bounds;
            return handData;
        }

        private void UpdatePalmBounds()
        {
            if (Hand.TryGetJointPose(XRHandJoint.LittleMetacarpal, out var pinkyMetacarpalPose, Space.World) &&
                Hand.TryGetJointPose(XRHandJoint.LittleProximal, out var pinkyKnucklePose, Space.World) &&
                Hand.TryGetJointPose(XRHandJoint.RingMetacarpal, out var ringMetacarpalPose, Space.World) &&
                Hand.TryGetJointPose(XRHandJoint.RingProximal, out var ringKnucklePose, Space.World) &&
                Hand.TryGetJointPose(XRHandJoint.MiddleMetacarpal, out var middleMetacarpalPose, Space.World) &&
                Hand.TryGetJointPose(XRHandJoint.MiddleProximal, out var middleKnucklePose, Space.World) &&
                Hand.TryGetJointPose(XRHandJoint.IndexMetacarpal, out var indexMetacarpalPose, Space.World) &&
                Hand.TryGetJointPose(XRHandJoint.IndexProximal, out var indexKnucklePose, Space.World))
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

        private void UpdateHandBounds()
        {
            if (Hand.TryGetJointPose(XRHandJoint.Palm, out var palmPose))
            {
                var newHandBounds = new Bounds(palmPose.Position, Vector3.zero);

                foreach (var kvp in jointPoses)
                {
                    if (kvp.Key == XRHandJoint.Palm)
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

        private void UpdateThumbBounds()
        {
            if (Hand.TryGetJointPose(XRHandJoint.ThumbMetacarpal, out var knucklePose, Space.World) &&
                Hand.TryGetJointPose(XRHandJoint.ThumbProximal, out var middlePose, Space.World) &&
                Hand.TryGetJointPose(XRHandJoint.ThumbTip, out var tipPose, Space.World))
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

        private void UpdateIndexFingerBounds()
        {
            if (Hand.TryGetJointPose(XRHandJoint.IndexProximal, out var knucklePose, Space.World) &&
                Hand.TryGetJointPose(XRHandJoint.IndexIntermediate, out var middlePose, Space.World) &&
                Hand.TryGetJointPose(XRHandJoint.IndexTip, out var tipPose, Space.World))
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

        private void UpdateMiddleFingerBounds()
        {
            if (Hand.TryGetJointPose(XRHandJoint.MiddleProximal, out var knucklePose, Space.World) &&
                Hand.TryGetJointPose(XRHandJoint.MiddleIntermediate, out var middlePose, Space.World) &&
                Hand.TryGetJointPose(XRHandJoint.MiddleTip, out var tipPose, Space.World))
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

        private void UpdateRingFingerBounds()
        {
            if (Hand.TryGetJointPose(XRHandJoint.RingProximal, out var knucklePose, Space.World) &&
                Hand.TryGetJointPose(XRHandJoint.RingIntermediate, out var middlePose, Space.World) &&
                Hand.TryGetJointPose(XRHandJoint.RingTip, out var tipPose, Space.World))
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

        private void UpdateLittleFingerBounds()
        {
            if (Hand.TryGetJointPose(XRHandJoint.LittleProximal, out var knucklePose, Space.World) &&
                Hand.TryGetJointPose(XRHandJoint.LittleIntermediate, out var middlePose, Space.World) &&
                Hand.TryGetJointPose(XRHandJoint.LittleTip, out var tipPose, Space.World))
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
