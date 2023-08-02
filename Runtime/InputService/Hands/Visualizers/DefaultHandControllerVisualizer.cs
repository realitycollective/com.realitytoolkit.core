// Copyright (c) Reality Collective. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using RealityCollective.Extensions;
using RealityToolkit.Definitions.Controllers.Hands;
using RealityToolkit.EventDatum.Input;
using RealityToolkit.Input.Controllers;
using RealityToolkit.Input.Interfaces.Modules;
using System.Collections.Generic;
using UnityEngine;

namespace RealityToolkit.Input.Hands
{
    /// <summary>
    /// Base hand controller visualizer implementation.
    /// </summary>
    [System.Runtime.InteropServices.Guid("5d844e0b-f913-46b8-bc3b-fa6429e62c60")]
    public class DefaultHandControllerVisualizer : BaseControllerVisualizer
    {
        private readonly Dictionary<HandJoint, Transform> jointTransforms = new Dictionary<HandJoint, Transform>();
        private readonly Dictionary<HandJoint, CapsuleCollider> fingerBoundsModeColliders = new Dictionary<HandJoint, CapsuleCollider>();
        private BoxCollider handBoundsModeCollider;
        private const float fingerColliderRadius = .007f;
        private const int capsuleColliderZAxis = 2;
        private HandControllerJointsVisualizer jointsVisualizer;
        private HandControllerMeshVisualizer meshVisualizer;

        [SerializeField]
        [Tooltip("Visualization prefab instantiated once joint rendering mode is enabled for the first time.")]
        private GameObject jointsModePrefab = null;

        [SerializeField]
        [Tooltip("Visualization prefab instantiated once mesh rendering mode is enabled for the first time.")]
        private GameObject meshModePrefab = null;

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
        protected IHandControllerServiceModule HandControllerDataProvider => handControllerDataProvider ?? (handControllerDataProvider = (IHandControllerServiceModule)Controller.ServiceModule);

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
        public override void OnInputChanged(InputEventData<Pose> eventData)
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
                // It's important to update physics
                // configuration first.
                UpdatePhysicsConfiguration();

                UpdateHandJointTransforms();
                UpdateHandColliders();
                UpdateRendering();
            }
        }

        private void UpdateHandJointTransforms()
        {
            var handController = (IHandController)Controller;

            for (int i = 0; i < HandData.JointCount; i++)
            {
                var handJoint = (HandJoint)i;
                if (handController.TryGetJointPose(handJoint, out var jointPose))
                {
                    var jointTransform = GetOrCreateJointTransform(handJoint);
                    jointTransform.localPosition = jointPose.position;
                    jointTransform.localRotation = jointPose.rotation;
                }
            }
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

                        var thumbKnuckleGameObject = GetOrCreateJointTransform(HandJoint.ThumbMetacarpal).gameObject;
                        var capsuleCollider = GetOrCreateCapsuleCollider(HandJoint.ThumbMetacarpal, thumbKnuckleGameObject);
                        ConfigureCapsuleCollider(capsuleCollider, knuckleToMiddle, thumbKnuckleGameObject.transform);

                        var thumbMiddleGameObject = GetOrCreateJointTransform(HandJoint.ThumbProximal).gameObject;
                        capsuleCollider = GetOrCreateCapsuleCollider(HandJoint.ThumbProximal, thumbMiddleGameObject);
                        ConfigureCapsuleCollider(capsuleCollider, middleToTip, thumbMiddleGameObject.transform);
                    }

                    if (handController.TryGetBounds(TrackedHandBounds.IndexFinger, out Bounds[] indexFingerBounds))
                    {
                        // Index finger bounds are made up of two capsule collider bounds entries.
                        Bounds knuckleToMiddle = indexFingerBounds[0];
                        Bounds middleToTip = indexFingerBounds[1];

                        var indexKnuckleGameObject = GetOrCreateJointTransform(HandJoint.IndexProximal).gameObject;
                        var capsuleCollider = GetOrCreateCapsuleCollider(HandJoint.IndexProximal, indexKnuckleGameObject);
                        ConfigureCapsuleCollider(capsuleCollider, knuckleToMiddle, indexKnuckleGameObject.transform);

                        var indexMiddleGameObject = GetOrCreateJointTransform(HandJoint.IndexIntermediate).gameObject;
                        capsuleCollider = GetOrCreateCapsuleCollider(HandJoint.IndexIntermediate, indexMiddleGameObject);
                        ConfigureCapsuleCollider(capsuleCollider, middleToTip, indexMiddleGameObject.transform);
                    }

                    if (handController.TryGetBounds(TrackedHandBounds.MiddleFinger, out Bounds[] middleFingerBounds))
                    {
                        // Middle finger bounds are made up of two capsule collider bounds entries.
                        Bounds knuckleToMiddle = middleFingerBounds[0];
                        Bounds middleToTip = middleFingerBounds[1];

                        var middleKnuckleGameObject = GetOrCreateJointTransform(HandJoint.MiddleProximal).gameObject;
                        var capsuleCollider = GetOrCreateCapsuleCollider(HandJoint.MiddleProximal, middleKnuckleGameObject);
                        ConfigureCapsuleCollider(capsuleCollider, knuckleToMiddle, middleKnuckleGameObject.transform);

                        var middleMiddleGameObject = GetOrCreateJointTransform(HandJoint.MiddleIntermediate).gameObject;
                        capsuleCollider = GetOrCreateCapsuleCollider(HandJoint.MiddleIntermediate, middleMiddleGameObject);
                        ConfigureCapsuleCollider(capsuleCollider, middleToTip, middleMiddleGameObject.transform);
                    }

                    if (handController.TryGetBounds(TrackedHandBounds.RingFinger, out Bounds[] ringFingerBounds))
                    {
                        // Ring finger bounds are made up of two capsule collider bounds entries.
                        Bounds knuckleToMiddle = ringFingerBounds[0];
                        Bounds middleToTip = ringFingerBounds[1];

                        var ringKnuckleGameObject = GetOrCreateJointTransform(HandJoint.RingProximal).gameObject;
                        var capsuleCollider = GetOrCreateCapsuleCollider(HandJoint.RingProximal, ringKnuckleGameObject);
                        ConfigureCapsuleCollider(capsuleCollider, knuckleToMiddle, ringKnuckleGameObject.transform);

                        var ringMiddleGameObject = GetOrCreateJointTransform(HandJoint.RingIntermediate).gameObject;
                        capsuleCollider = GetOrCreateCapsuleCollider(HandJoint.RingIntermediate, ringMiddleGameObject);
                        ConfigureCapsuleCollider(capsuleCollider, middleToTip, ringMiddleGameObject.transform);
                    }

                    if (handController.TryGetBounds(TrackedHandBounds.Pinky, out Bounds[] pinkyFingerBounds))
                    {
                        // Pinky finger bounds are made up of two capsule collider bounds entries.
                        Bounds knuckleToMiddle = pinkyFingerBounds[0];
                        Bounds middleToTip = pinkyFingerBounds[1];

                        var pinkyKnuckleGameObject = GetOrCreateJointTransform(HandJoint.LittleProximal).gameObject;
                        var capsuleCollider = GetOrCreateCapsuleCollider(HandJoint.LittleProximal, pinkyKnuckleGameObject);
                        ConfigureCapsuleCollider(capsuleCollider, knuckleToMiddle, pinkyKnuckleGameObject.transform);

                        var pinkyMiddleGameObject = GetOrCreateJointTransform(HandJoint.LittleIntermediate).gameObject;
                        capsuleCollider = GetOrCreateCapsuleCollider(HandJoint.LittleIntermediate, pinkyMiddleGameObject);
                        ConfigureCapsuleCollider(capsuleCollider, middleToTip, pinkyMiddleGameObject.transform);
                    }

                    if (handController.TryGetBounds(TrackedHandBounds.Palm, out Bounds[] palmBounds))
                    {
                        // For the palm we create a composite collider using a capsule collider per
                        // finger for the area metacarpal <-> knuckle.
                        Bounds indexPalmBounds = palmBounds[0];
                        var indexMetacarpalGameObject = GetOrCreateJointTransform(HandJoint.IndexMetacarpal).gameObject;
                        var capsuleCollider = GetOrCreateCapsuleCollider(HandJoint.IndexMetacarpal, indexMetacarpalGameObject);
                        ConfigureCapsuleCollider(capsuleCollider, indexPalmBounds, indexMetacarpalGameObject.transform);

                        Bounds middlePalmBounds = palmBounds[1];
                        var middleMetacarpalGameObject = GetOrCreateJointTransform(HandJoint.MiddleMetacarpal).gameObject;
                        capsuleCollider = GetOrCreateCapsuleCollider(HandJoint.MiddleMetacarpal, middleMetacarpalGameObject);
                        ConfigureCapsuleCollider(capsuleCollider, middlePalmBounds, middleMetacarpalGameObject.transform);

                        Bounds ringPalmBounds = palmBounds[2];
                        var ringMetacarpalGameObject = GetOrCreateJointTransform(HandJoint.RingMetacarpal).gameObject;
                        capsuleCollider = GetOrCreateCapsuleCollider(HandJoint.RingMetacarpal, ringMetacarpalGameObject);
                        ConfigureCapsuleCollider(capsuleCollider, ringPalmBounds, ringMetacarpalGameObject.transform);

                        Bounds pinkyPalmBounds = palmBounds[3];
                        var pinkyMetacarpalGameObject = GetOrCreateJointTransform(HandJoint.LittleMetacarpal).gameObject;
                        capsuleCollider = GetOrCreateCapsuleCollider(HandJoint.LittleMetacarpal, pinkyMetacarpalGameObject);
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

        private CapsuleCollider GetOrCreateCapsuleCollider(HandJoint trackedHandJoint, GameObject forObject)
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

        /// <summary>
        /// Gets the proxy transform for a given tracked hand joint or creates
        /// it if it does not exist yet.
        /// </summary>
        /// <param name="handJoint">The hand joint a transform should be returned for.</param>
        /// <returns>Joint transform.</returns>
        public Transform GetOrCreateJointTransform(HandJoint handJoint)
        {
            if (jointTransforms.TryGetValue(handJoint, out Transform existingJointTransform))
            {
                existingJointTransform.parent = HandVisualizationGameObject.transform;
                existingJointTransform.gameObject.SetActive(true);
                return existingJointTransform;
            }

            Transform jointTransform = new GameObject($"{handJoint}").transform;
            jointTransform.parent = HandVisualizationGameObject.transform;
            jointTransforms.Add(handJoint, jointTransform.transform);

            return jointTransform;
        }

        private void UpdateRendering()
        {
            var renderingMode = HandControllerDataProvider.RenderingMode;
            if (renderingMode != HandRenderingMode.None)
            {
                var handController = (IHandController)Controller;
                HandMeshData handMeshData = HandMeshData.Empty;

                // Fallback to joints rendering if mesh data is not available.
                if (renderingMode == HandRenderingMode.Mesh &&
                    !handController.TryGetHandMeshData(out handMeshData))
                {
                    renderingMode = HandRenderingMode.Joints;
                }

                if (renderingMode == HandRenderingMode.Joints)
                {
                    if (meshVisualizer != null)
                    {
                        meshVisualizer.gameObject.SetActive(false);
                    }

                    if (jointsVisualizer == null)
                    {
                        jointsVisualizer = Instantiate(jointsModePrefab, HandVisualizationGameObject.transform).GetComponent<HandControllerJointsVisualizer>();
                    }

                    jointsVisualizer.gameObject.SetActive(true);
                    jointsVisualizer.UpdateVisualization(this);
                }
                else if (renderingMode == HandRenderingMode.Mesh)
                {
                    if (jointsVisualizer != null)
                    {
                        jointsVisualizer.gameObject.SetActive(false);
                    }

                    if (meshVisualizer == null)
                    {
                        meshVisualizer = Instantiate(meshModePrefab, HandVisualizationGameObject.transform).GetComponent<HandControllerMeshVisualizer>();
                    }

                    meshVisualizer.gameObject.SetActive(true);
                    meshVisualizer.UpdateVisualization(handMeshData);
                }
            }
            else
            {
                if (jointsVisualizer != null)
                {
                    jointsVisualizer.gameObject.SetActive(false);
                }

                if (meshVisualizer != null)
                {
                    meshVisualizer.gameObject.SetActive(false);
                }
            }
        }
    }
}
