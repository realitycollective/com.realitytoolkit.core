// Copyright (c) Reality Collective. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using RealityToolkit.Interfaces.InputSystem.Controllers.Hands;
using System.Collections.Generic;
using UnityEngine;
using XRTK.Definitions.Controllers.Hands;
using XRTK.Definitions.Utilities;

namespace RealityToolkit.Services.InputSystem.Controllers.UnityXR
{
    /// <summary>
    /// Calculates and maintains <see cref="IHandController"/> bounds.
    /// </summary>
    public class HandControllerBoundsProvider
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="handController">The <see cref="IHandController"/> to provide bounds for.</param>
        /// <param name="handBoundsLOD">The requested detail level for provided bounds.</param>
        public HandControllerBoundsProvider(IHandController handController, HandBoundsLOD handBoundsLOD)
        {
            this.handController = handController;
            this.handBoundsLOD = handBoundsLOD;
        }

        private readonly Bounds[] cachedPalmBounds = new Bounds[4];
        private readonly Bounds[] cachedThumbBounds = new Bounds[2];
        private readonly Bounds[] cachedIndexFingerBounds = new Bounds[2];
        private readonly Bounds[] cachedMiddleFingerBounds = new Bounds[2];
        private readonly Bounds[] cachedRingFingerBounds = new Bounds[2];
        private readonly Bounds[] cachedLittleFingerBounds = new Bounds[2];
        private readonly Dictionary<TrackedHandBounds, Bounds[]> bounds = new Dictionary<TrackedHandBounds, Bounds[]>();
        private IHandController handController;
        private readonly HandBoundsLOD handBoundsLOD;

        /// <summary>
        /// Updates hand bounds.
        /// </summary>
        public void UpdateBounds(ref Dictionary<TrackedHandJoint, MixedRealityPose> jointPoses)
        {
            if (handBoundsLOD == HandBoundsLOD.Low)
            {
                UpdateHandBounds(ref jointPoses);
            }
            else if (handBoundsLOD == HandBoundsLOD.High)
            {
                UpdatePalmBounds();
                UpdateThumbBounds();
                UpdateIndexFingerBounds();
                UpdateMiddleFingerBounds();
                UpdateRingFingerBounds();
                UpdateLittleFingerBounds();
            }
        }

        /// <summary>
        /// Get the hands bounds of a given type, if they are available.
        /// </summary>
        /// <param name="handBounds">The requested hand bounds.</param>
        /// <param name="bounds">The bounds if available.</param>
        /// <returns>True, if bounds available.</returns>
        public bool TryGetBounds(TrackedHandBounds handBounds, out Bounds[] newBounds)
        {
            if (bounds.ContainsKey(handBounds))
            {
                newBounds = bounds[handBounds];
                return true;
            }

            newBounds = null;
            return false;
        }

        private void UpdatePalmBounds()
        {
            if (handController.TryGetJointPose(TrackedHandJoint.LittleMetacarpal, out var pinkyMetacarpalPose, Space.World) &&
                handController.TryGetJointPose(TrackedHandJoint.LittleProximal, out var pinkyKnucklePose, Space.World) &&
                handController.TryGetJointPose(TrackedHandJoint.RingMetacarpal, out var ringMetacarpalPose, Space.World) &&
                handController.TryGetJointPose(TrackedHandJoint.RingProximal, out var ringKnucklePose, Space.World) &&
                handController.TryGetJointPose(TrackedHandJoint.MiddleMetacarpal, out var middleMetacarpalPose, Space.World) &&
                handController.TryGetJointPose(TrackedHandJoint.MiddleProximal, out var middleKnucklePose, Space.World) &&
                handController.TryGetJointPose(TrackedHandJoint.IndexMetacarpal, out var indexMetacarpalPose, Space.World) &&
                handController.TryGetJointPose(TrackedHandJoint.IndexProximal, out var indexKnucklePose, Space.World))
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

        private void UpdateHandBounds(ref Dictionary<TrackedHandJoint, MixedRealityPose> jointPoses)
        {
            if (handController.TryGetJointPose(TrackedHandJoint.Palm, out var palmPose))
            {
                var newHandBounds = new Bounds(palmPose.Position, Vector3.zero);

                foreach (var kvp in jointPoses)
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

        private void UpdateThumbBounds()
        {
            if (handController.TryGetJointPose(TrackedHandJoint.ThumbMetacarpal, out var knucklePose, Space.World) &&
                handController.TryGetJointPose(TrackedHandJoint.ThumbProximal, out var middlePose, Space.World) &&
                handController.TryGetJointPose(TrackedHandJoint.ThumbTip, out var tipPose, Space.World))
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
            if (handController.TryGetJointPose(TrackedHandJoint.IndexProximal, out var knucklePose, Space.World) &&
                handController.TryGetJointPose(TrackedHandJoint.IndexIntermediate, out var middlePose, Space.World) &&
                handController.TryGetJointPose(TrackedHandJoint.IndexTip, out var tipPose, Space.World))
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
            if (handController.TryGetJointPose(TrackedHandJoint.MiddleProximal, out var knucklePose, Space.World) &&
                handController.TryGetJointPose(TrackedHandJoint.MiddleIntermediate, out var middlePose, Space.World) &&
                handController.TryGetJointPose(TrackedHandJoint.MiddleTip, out var tipPose, Space.World))
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
            if (handController.TryGetJointPose(TrackedHandJoint.RingProximal, out var knucklePose, Space.World) &&
                handController.TryGetJointPose(TrackedHandJoint.RingIntermediate, out var middlePose, Space.World) &&
                handController.TryGetJointPose(TrackedHandJoint.RingTip, out var tipPose, Space.World))
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
            if (handController.TryGetJointPose(TrackedHandJoint.LittleProximal, out var knucklePose, Space.World) &&
                handController.TryGetJointPose(TrackedHandJoint.LittleIntermediate, out var middlePose, Space.World) &&
                handController.TryGetJointPose(TrackedHandJoint.LittleTip, out var tipPose, Space.World))
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
