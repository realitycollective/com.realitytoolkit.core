// Copyright (c) Reality Collective. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using RealityCollective.Extensions;
using RealityToolkit.Definitions.Utilities;
using RealityToolkit.EventDatum.Input;
using RealityToolkit.InputSystem.Interfaces.Handlers;
using RealityToolkit.Services.InputSystem.Utilities;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace RealityToolkit.InputSystem.Hands.Visualizers
{
    /// <summary>
    /// Base hand controller visualizer implementation.
    /// </summary>
    [System.Runtime.InteropServices.Guid("5d844e0b-f913-46b8-bc3b-fa6429e62c60")]
    public class DefaultHandControllerVisualizer : ControllerPoseSynchronizer, IMixedRealityControllerVisualizer
    {
        private HandSkeleton skeleton;
        private readonly Dictionary<TrackedHandJoint, CapsuleCollider> fingerBoundsModeColliders = new Dictionary<TrackedHandJoint, CapsuleCollider>();
        private BoxCollider handBoundsModeCollider;
        private const float fingerColliderRadius = .007f;
        private const int capsuleColliderZAxis = 2;

        /// <inheritdoc />
        public GameObject GameObject
        {
            get
            {
                try
                {
                    return gameObject;
                }
                catch
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// If using physics with hand, the actual hand visualization is done
        /// on a companion game object which is connected to the <see cref="GameObject"/>
        /// using a <see cref="FixedJoint"/>. For physics to work properly while maintaining
        /// the platforms controller tracking we cannot attach colliders and a rigidbody to the
        /// <see cref="GameObject"/> since that would cause crazy behaviour on controller movement.
        /// </summary>
        private GameObject PhysicsCompanionGameObject { get; set; }

        /// <summary>
        /// The actual game object that is parent to all controller visualization of this hand controller.
        /// </summary>
        public GameObject HandVisualizationGameObject => HandControllerDataProvider.HandPhysicsEnabled ? PhysicsCompanionGameObject : GameObject;

        private IHandControllerServiceModule handControllerDataProvider;

        /// <summary>
        /// The active <see cref="IHandControllerServiceModule"/>.
        /// </summary>
        protected IHandControllerServiceModule HandControllerDataProvider => handControllerDataProvider ?? (handControllerDataProvider = (IHandControllerServiceModule)Controller.ControllerDataProvider);

        /// <inheritdoc />
        protected override void OnDestroy()
        {
            // In case physics are enabled we need to take destroy the
            // physics game object as well when destroying the hand visualizer.
            if (PhysicsCompanionGameObject != null)
            {
                PhysicsCompanionGameObject.Destroy();
            }

            base.OnDestroy();
        }

        /// <inheritdoc />
        public override void OnInputChanged(InputEventData<MixedRealityPose> eventData)
        {
            base.OnInputChanged(eventData);

            if (eventData.Handedness != Controller.ControllerHandedness)
            {
                return;
            }

            var handController = (IHandController)Controller;

            // Update the visualizers tracking state.
            TrackingState = handController.TrackingState;

            if (TrackingState == RealityToolkit.Definitions.Devices.TrackingState.Tracked)
            {
                if (skeleton.IsNull())
                {
                    skeleton = GameObject.EnsureComponent<HandSkeleton>();
                    skeleton.Create();
                }

                // It's important to update physics
                // configuration first.
                UpdatePhysicsConfiguration();

                UpdateHandJointTransforms();
                UpdateHandColliders();
            }
        }

        private void UpdateHandJointTransforms()
        {
            var handController = (IHandController)Controller;

            for (int i = 0; i < HandData.JointCount; i++)
            {
                var handJoint = (TrackedHandJoint)i;
                if (handController.TryGetJointPose(handJoint, out var jointPose))
                {
                    UpdateJointPose(handJoint, jointPose);
                }
            }
        }

        /// <summary>
        /// Updates the <paramref name="pose"/> of the <see cref="Transform"/> representing
        /// <paramref name="trackedHandJoint"/>.
        /// </summary>
        /// <param name="trackedHandJoint">The <see cref="TrackedHandJoint"/> to target.</param>
        /// <param name="pose">The new <see cref="MixedRealityPose"/> of the <paramref name="trackedHandJoint"/>.</param>
        /// <exception cref="ArgumentOutOfRangeException">In case of unknown <see cref="TrackedHandJoint"/>.</exception>
        private void UpdateJointPose(TrackedHandJoint trackedHandJoint, MixedRealityPose pose)
        {
            Transform target = null;
            switch (trackedHandJoint)
            {
                case TrackedHandJoint.Wrist:
                    target = skeleton.Wrist;
                    break;
                case TrackedHandJoint.Palm:
                    target = skeleton.Palm;
                    break;
                case TrackedHandJoint.ThumbMetacarpal:
                    target = skeleton.ThumbMetacarpal;
                    break;
                case TrackedHandJoint.ThumbProximal:
                    target = skeleton.ThumbProximal;
                    break;
                case TrackedHandJoint.ThumbDistal:
                    target = skeleton.ThumbDistal;
                    break;
                case TrackedHandJoint.ThumbTip:
                    target = skeleton.ThumbTip;
                    break;
                case TrackedHandJoint.IndexMetacarpal:
                    target = skeleton.IndexMetacarpal;
                    break;
                case TrackedHandJoint.IndexProximal:
                    target = skeleton.IndexProximal;
                    break;
                case TrackedHandJoint.IndexIntermediate:
                    target = skeleton.IndexIntermediate;
                    break;
                case TrackedHandJoint.IndexDistal:
                    target = skeleton.IndexDistal;
                    break;
                case TrackedHandJoint.IndexTip:
                    target = skeleton.IndexTip;
                    break;
                case TrackedHandJoint.MiddleMetacarpal:
                    target = skeleton.MiddleMetacarpal;
                    break;
                case TrackedHandJoint.MiddleProximal:
                    target = skeleton.MiddleProximal;
                    break;
                case TrackedHandJoint.MiddleIntermediate:
                    target = skeleton.MiddleIntermediate;
                    break;
                case TrackedHandJoint.MiddleDistal:
                    target = skeleton.MiddleDistal;
                    break;
                case TrackedHandJoint.MiddleTip:
                    target = skeleton.MiddleTip;
                    break;
                case TrackedHandJoint.RingMetacarpal:
                    target = skeleton.RingMetacarpal;
                    break;
                case TrackedHandJoint.RingProximal:
                    target = skeleton.RingProximal;
                    break;
                case TrackedHandJoint.RingIntermediate:
                    target = skeleton.RingIntermediate;
                    break;
                case TrackedHandJoint.RingDistal:
                    target = skeleton.RingDistal;
                    break;
                case TrackedHandJoint.RingTip:
                    target = skeleton.RingTip;
                    break;
                case TrackedHandJoint.LittleMetacarpal:
                    target = skeleton.LittleMetacarpal;
                    break;
                case TrackedHandJoint.LittleProximal:
                    target = skeleton.LittleProximal;
                    break;
                case TrackedHandJoint.LittleIntermediate:
                    target = skeleton.LittleIntermediate;
                    break;
                case TrackedHandJoint.LittleDistal:
                    target = skeleton.LittleDistal;
                    break;
                case TrackedHandJoint.LittleTip:
                    target = skeleton.LittleTip;
                    break;
                default:
                    throw new ArgumentOutOfRangeException($"Unknown {nameof(TrackedHandJoint)}.{trackedHandJoint} does not exist in the {nameof(HandSkeleton)}");
            }

            target.localPosition = pose.Position;
            target.localRotation = pose.Rotation;
        }

        #region Hand Colliders / Physics

        private void UpdatePhysicsConfiguration()
        {
            if (HandControllerDataProvider.HandPhysicsEnabled)
            {
                // If we are using hand physics, we need to make sure
                // the physics companion is setup properly.
                if (PhysicsCompanionGameObject != null)
                {
                    PhysicsCompanionGameObject.SetActive(true);
                    PhysicsCompanionGameObject.transform.localPosition = GameObject.transform.localPosition;
                    PhysicsCompanionGameObject.transform.localRotation = GameObject.transform.localRotation;
                    return;
                }

                PhysicsCompanionGameObject = new GameObject($"{GameObject.name}_Physics");
                PhysicsCompanionGameObject.transform.SetParent(GameObject.transform.parent, false);
                PhysicsCompanionGameObject.transform.localPosition = GameObject.transform.localPosition;
                PhysicsCompanionGameObject.transform.localRotation = GameObject.transform.localRotation;

                // Setup the kinematic rigidbody on the actual controller game object.
                Rigidbody controllerRigidbody = GameObject.GetOrAddComponent<Rigidbody>();
                controllerRigidbody.mass = .46f; // 0.46 Kg average human hand weight
                controllerRigidbody.isKinematic = true;
                controllerRigidbody.useGravity = false;
                controllerRigidbody.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;

                // Make the physics proxy a fixed joint rigidbody to the controller
                // and give it an adamantium coated connection so it doesn't break.
                Rigidbody physicsRigidbody = PhysicsCompanionGameObject.GetOrAddComponent<Rigidbody>();
                physicsRigidbody.mass = .46f; // 0.46 Kg average human hand weight
                physicsRigidbody.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
                FixedJoint fixedJoint = PhysicsCompanionGameObject.GetOrAddComponent<FixedJoint>();
                fixedJoint.connectedBody = controllerRigidbody;
                fixedJoint.breakForce = float.MaxValue;
                fixedJoint.breakTorque = float.MaxValue;
            }
            else if (PhysicsCompanionGameObject != null)
            {
                PhysicsCompanionGameObject.SetActive(false);
            }
        }

        private void UpdateHandColliders()
        {
            if (HandControllerDataProvider.HandPhysicsEnabled)
            {
                var handController = (IHandController)Controller;

                if (HandControllerDataProvider.BoundsMode == HandBoundsLOD.High)
                {
                    // Make sure to disable other colliders not needed for the fingers mode.
                    DisableHandBounds();

                    if (handController.TryGetBounds(TrackedHandBounds.Thumb, out Bounds[] thumbBounds))
                    {
                        // Thumb bounds are made up of two capsule collider bounds entries.
                        Bounds knuckleToMiddle = thumbBounds[0];
                        Bounds middleToTip = thumbBounds[1];

                        var thumbKnuckleGameObject = skeleton.ThumbMetacarpal.gameObject;
                        var capsuleCollider = GetOrCreateCapsuleCollider(TrackedHandJoint.ThumbMetacarpal, thumbKnuckleGameObject);
                        ConfigureCapsuleCollider(capsuleCollider, knuckleToMiddle, thumbKnuckleGameObject.transform);

                        var thumbMiddleGameObject = skeleton.ThumbProximal.gameObject;
                        capsuleCollider = GetOrCreateCapsuleCollider(TrackedHandJoint.ThumbProximal, thumbMiddleGameObject);
                        ConfigureCapsuleCollider(capsuleCollider, middleToTip, thumbMiddleGameObject.transform);
                    }

                    if (handController.TryGetBounds(TrackedHandBounds.IndexFinger, out Bounds[] indexFingerBounds))
                    {
                        // Index finger bounds are made up of two capsule collider bounds entries.
                        Bounds knuckleToMiddle = indexFingerBounds[0];
                        Bounds middleToTip = indexFingerBounds[1];

                        var indexKnuckleGameObject = skeleton.IndexProximal.gameObject;
                        var capsuleCollider = GetOrCreateCapsuleCollider(TrackedHandJoint.IndexProximal, indexKnuckleGameObject);
                        ConfigureCapsuleCollider(capsuleCollider, knuckleToMiddle, indexKnuckleGameObject.transform);

                        var indexMiddleGameObject = skeleton.IndexIntermediate.gameObject;
                        capsuleCollider = GetOrCreateCapsuleCollider(TrackedHandJoint.IndexIntermediate, indexMiddleGameObject);
                        ConfigureCapsuleCollider(capsuleCollider, middleToTip, indexMiddleGameObject.transform);
                    }

                    if (handController.TryGetBounds(TrackedHandBounds.MiddleFinger, out Bounds[] middleFingerBounds))
                    {
                        // Middle finger bounds are made up of two capsule collider bounds entries.
                        Bounds knuckleToMiddle = middleFingerBounds[0];
                        Bounds middleToTip = middleFingerBounds[1];

                        var middleKnuckleGameObject = skeleton.MiddleProximal.gameObject;
                        var capsuleCollider = GetOrCreateCapsuleCollider(TrackedHandJoint.MiddleProximal, middleKnuckleGameObject);
                        ConfigureCapsuleCollider(capsuleCollider, knuckleToMiddle, middleKnuckleGameObject.transform);

                        var middleMiddleGameObject = skeleton.MiddleIntermediate.gameObject;
                        capsuleCollider = GetOrCreateCapsuleCollider(TrackedHandJoint.MiddleIntermediate, middleMiddleGameObject);
                        ConfigureCapsuleCollider(capsuleCollider, middleToTip, middleMiddleGameObject.transform);
                    }

                    if (handController.TryGetBounds(TrackedHandBounds.RingFinger, out Bounds[] ringFingerBounds))
                    {
                        // Ring finger bounds are made up of two capsule collider bounds entries.
                        Bounds knuckleToMiddle = ringFingerBounds[0];
                        Bounds middleToTip = ringFingerBounds[1];

                        var ringKnuckleGameObject = skeleton.RingProximal.gameObject;
                        var capsuleCollider = GetOrCreateCapsuleCollider(TrackedHandJoint.RingProximal, ringKnuckleGameObject);
                        ConfigureCapsuleCollider(capsuleCollider, knuckleToMiddle, ringKnuckleGameObject.transform);

                        var ringMiddleGameObject = skeleton.RingIntermediate.gameObject;
                        capsuleCollider = GetOrCreateCapsuleCollider(TrackedHandJoint.RingIntermediate, ringMiddleGameObject);
                        ConfigureCapsuleCollider(capsuleCollider, middleToTip, ringMiddleGameObject.transform);
                    }

                    if (handController.TryGetBounds(TrackedHandBounds.Pinky, out Bounds[] pinkyFingerBounds))
                    {
                        // Pinky finger bounds are made up of two capsule collider bounds entries.
                        Bounds knuckleToMiddle = pinkyFingerBounds[0];
                        Bounds middleToTip = pinkyFingerBounds[1];

                        var pinkyKnuckleGameObject = skeleton.LittleProximal.gameObject;
                        var capsuleCollider = GetOrCreateCapsuleCollider(TrackedHandJoint.LittleProximal, pinkyKnuckleGameObject);
                        ConfigureCapsuleCollider(capsuleCollider, knuckleToMiddle, pinkyKnuckleGameObject.transform);

                        var pinkyMiddleGameObject = skeleton.LittleIntermediate.gameObject;
                        capsuleCollider = GetOrCreateCapsuleCollider(TrackedHandJoint.LittleIntermediate, pinkyMiddleGameObject);
                        ConfigureCapsuleCollider(capsuleCollider, middleToTip, pinkyMiddleGameObject.transform);
                    }

                    if (handController.TryGetBounds(TrackedHandBounds.Palm, out Bounds[] palmBounds))
                    {
                        // For the palm we create a composite collider using a capsule collider per
                        // finger for the area metacarpal <-> knuckle.
                        Bounds indexPalmBounds = palmBounds[0];
                        var indexMetacarpalGameObject = skeleton.IndexMetacarpal.gameObject;
                        var capsuleCollider = GetOrCreateCapsuleCollider(TrackedHandJoint.IndexMetacarpal, indexMetacarpalGameObject);
                        ConfigureCapsuleCollider(capsuleCollider, indexPalmBounds, indexMetacarpalGameObject.transform);

                        Bounds middlePalmBounds = palmBounds[1];
                        var middleMetacarpalGameObject = skeleton.MiddleMetacarpal.gameObject;
                        capsuleCollider = GetOrCreateCapsuleCollider(TrackedHandJoint.MiddleMetacarpal, middleMetacarpalGameObject);
                        ConfigureCapsuleCollider(capsuleCollider, middlePalmBounds, middleMetacarpalGameObject.transform);

                        Bounds ringPalmBounds = palmBounds[2];
                        var ringMetacarpalGameObject = skeleton.RingMetacarpal.gameObject;
                        capsuleCollider = GetOrCreateCapsuleCollider(TrackedHandJoint.RingMetacarpal, ringMetacarpalGameObject);
                        ConfigureCapsuleCollider(capsuleCollider, ringPalmBounds, ringMetacarpalGameObject.transform);

                        Bounds pinkyPalmBounds = palmBounds[3];
                        var pinkyMetacarpalGameObject = skeleton.LittleMetacarpal.gameObject;
                        capsuleCollider = GetOrCreateCapsuleCollider(TrackedHandJoint.LittleMetacarpal, pinkyMetacarpalGameObject);
                        ConfigureCapsuleCollider(capsuleCollider, pinkyPalmBounds, pinkyMetacarpalGameObject.transform);
                    }
                }
                else if (HandControllerDataProvider.BoundsMode == HandBoundsLOD.Low)
                {
                    DisableFingerBounds();

                    if (handController.TryGetBounds(TrackedHandBounds.Hand, out Bounds[] handBounds))
                    {
                        // For full hand bounds we'll only get one bounds entry, which is a box
                        // encapsulating the whole hand.
                        Bounds fullHandBounds = handBounds[0];
                        handBoundsModeCollider = HandVisualizationGameObject.GetOrAddComponent<BoxCollider>();
                        handBoundsModeCollider.enabled = true;
                        handBoundsModeCollider.center = fullHandBounds.center;
                        handBoundsModeCollider.size = fullHandBounds.size;
                        handBoundsModeCollider.isTrigger = HandControllerDataProvider.UseTriggers;
                    }
                }
            }
        }

        private void ConfigureCapsuleCollider(CapsuleCollider collider, Bounds bounds, Transform jointTransform)
        {
            collider.radius = fingerColliderRadius;
            collider.direction = capsuleColliderZAxis;
            collider.height = bounds.size.magnitude;
            collider.center = jointTransform.InverseTransformPoint(bounds.center);
            collider.isTrigger = HandControllerDataProvider.UseTriggers;
            collider.enabled = true;
        }

        private CapsuleCollider GetOrCreateCapsuleCollider(TrackedHandJoint trackedHandJoint, GameObject forObject)
        {
            CapsuleCollider collider;
            if (fingerBoundsModeColliders.ContainsKey(trackedHandJoint))
            {
                collider = fingerBoundsModeColliders[trackedHandJoint];
            }
            else
            {
                collider = forObject.GetOrAddComponent<CapsuleCollider>();
                fingerBoundsModeColliders.Add(trackedHandJoint, collider);
            }

            return collider;
        }

        private void DisableFingerBounds()
        {
            foreach (var item in fingerBoundsModeColliders)
            {
                item.Value.enabled = false;
            }
        }

        private void DisableHandBounds()
        {
            if (handBoundsModeCollider != null)
            {
                handBoundsModeCollider.enabled = false;
            }
        }

        #endregion
    }
}
