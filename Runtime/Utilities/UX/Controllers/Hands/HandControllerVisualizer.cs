// Copyright (c) Reality Collective. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using RealityToolkit.Interfaces.InputSystem.Controllers.Hands;
using System.Collections.Generic;
using UnityEngine;
using RealityToolkit.Definitions.Controllers.Hands;
using RealityToolkit.Definitions.InputSystem;
using RealityToolkit.Definitions.Utilities;
using RealityToolkit.EventDatum.Input;
using RealityToolkit.Extensions;
using RealityToolkit.Interfaces.InputSystem;
using RealityToolkit.Interfaces.InputSystem.Handlers;
using RealityToolkit.Interfaces.InputSystem.Providers.Controllers.Hands;
using RealityToolkit.Services;
using RealityToolkit.Services.InputSystem.Utilities;
using RealityToolkit.Utilities.UX.Controllers.Hands;

namespace RealityToolkit.Utilities.UX.Controllers.Hands
{
    /// <summary>
    /// Default visualizer for <see cref="IHandController"/>s.
    /// </summary>
    [System.Runtime.InteropServices.Guid("f6654aca-e4c2-4653-8033-1465fe9f2fd1")]
    public class HandControllerVisualizer : ControllerPoseSynchronizer, IMixedRealityControllerVisualizer
    {
        [SerializeField]
        [Tooltip("Visualization prefab instantiated once joint rendering mode is enabled for the first time.")]
        private GameObject jointsModePrefab = null;

        [SerializeField]
        [Tooltip("Visualization prefab instantiated once mesh rendering mode is enabled for the first time.")]
        private GameObject meshModePrefab = null;

        private readonly Dictionary<TrackedHandJoint, Transform> jointTransforms = new Dictionary<TrackedHandJoint, Transform>();
        private HandControllerJointsVisualizer jointsVisualizer;
        private HandControllerMeshVisualizer meshVisualizer;
        private MixedRealityInputSystemProfile inputSystemProfile;
        private IMixedRealityHandControllerDataProvider handControllerDataProvider;
        private BoxCollider handBoundsModeCollider;
        private readonly Dictionary<TrackedHandJoint, CapsuleCollider> fingerBoundsModeColliders = new Dictionary<TrackedHandJoint, CapsuleCollider>();
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

        private void Awake()
        {
            MixedRealityToolkit.TryGetService(out handControllerDataProvider);
            MixedRealityToolkit.TryGetSystemProfile<IMixedRealityInputSystem, MixedRealityInputSystemProfile>(out inputSystemProfile);
        }

        public override void OnInputChanged(InputEventData<MixedRealityPose> eventData)
        {
            base.OnInputChanged(eventData);

            if (eventData.Handedness != Controller?.ControllerHandedness ||
                eventData.SourceId != Controller?.InputSource.SourceId)
            {
                return;
            }

            var handController = (IHandController)Controller;

            // Update the visualizers tracking state.
            TrackingState = handController.TrackingState;

            if (TrackingState == RealityToolkit.Definitions.Devices.TrackingState.Tracked)
            {
                UpdateHandJointTransforms();
                UpdateHandColliders();
                UpdateRendering();
            }
        }

        private void UpdateHandColliders()
        {
            var handPhysicsEnabled = handControllerDataProvider != null ?
                handControllerDataProvider.HandPhysicsEnabled :
                inputSystemProfile.HandControllerSettings.HandPhysicsEnabled;

            if (handPhysicsEnabled)
            {
                var handController = (IHandController)Controller;
                var boundsMode = handControllerDataProvider != null ?
                    handControllerDataProvider.BoundsMode :
                    inputSystemProfile.HandControllerSettings.BoundsMode;

                if (boundsMode == HandBoundsLOD.None)
                {
                    Debug.LogError("Hand physics requires hand bounds to be enabled.");
                    return;
                }

                if (boundsMode == HandBoundsLOD.High)
                {
                    // Make sure to disable other colliders not needed for the fingers mode.
                    DisableHandBounds();

                    if (handController.TryGetBounds(TrackedHandBounds.Thumb, out Bounds[] thumbBounds))
                    {
                        // Thumb bounds are made up of two capsule collider bounds entries.
                        Bounds knuckleToMiddle = thumbBounds[0];
                        Bounds middleToTip = thumbBounds[1];

                        var thumbKnuckleGameObject = GetOrCreateJointTransform(TrackedHandJoint.ThumbMetacarpal).gameObject;
                        var capsuleCollider = GetOrCreateCapsuleCollider(TrackedHandJoint.ThumbMetacarpal, thumbKnuckleGameObject);
                        ConfigureCapsuleCollider(capsuleCollider, knuckleToMiddle, thumbKnuckleGameObject.transform);

                        var thumbMiddleGameObject = GetOrCreateJointTransform(TrackedHandJoint.ThumbProximal).gameObject;
                        capsuleCollider = GetOrCreateCapsuleCollider(TrackedHandJoint.ThumbProximal, thumbMiddleGameObject);
                        ConfigureCapsuleCollider(capsuleCollider, middleToTip, thumbMiddleGameObject.transform);
                    }

                    if (handController.TryGetBounds(TrackedHandBounds.IndexFinger, out Bounds[] indexFingerBounds))
                    {
                        // Index finger bounds are made up of two capsule collider bounds entries.
                        Bounds knuckleToMiddle = indexFingerBounds[0];
                        Bounds middleToTip = indexFingerBounds[1];

                        var indexKnuckleGameObject = GetOrCreateJointTransform(TrackedHandJoint.IndexProximal).gameObject;
                        var capsuleCollider = GetOrCreateCapsuleCollider(TrackedHandJoint.IndexProximal, indexKnuckleGameObject);
                        ConfigureCapsuleCollider(capsuleCollider, knuckleToMiddle, indexKnuckleGameObject.transform);

                        var indexMiddleGameObject = GetOrCreateJointTransform(TrackedHandJoint.IndexIntermediate).gameObject;
                        capsuleCollider = GetOrCreateCapsuleCollider(TrackedHandJoint.IndexIntermediate, indexMiddleGameObject);
                        ConfigureCapsuleCollider(capsuleCollider, middleToTip, indexMiddleGameObject.transform);
                    }

                    if (handController.TryGetBounds(TrackedHandBounds.MiddleFinger, out Bounds[] middleFingerBounds))
                    {
                        // Middle finger bounds are made up of two capsule collider bounds entries.
                        Bounds knuckleToMiddle = middleFingerBounds[0];
                        Bounds middleToTip = middleFingerBounds[1];

                        var middleKnuckleGameObject = GetOrCreateJointTransform(TrackedHandJoint.MiddleProximal).gameObject;
                        var capsuleCollider = GetOrCreateCapsuleCollider(TrackedHandJoint.MiddleProximal, middleKnuckleGameObject);
                        ConfigureCapsuleCollider(capsuleCollider, knuckleToMiddle, middleKnuckleGameObject.transform);

                        var middleMiddleGameObject = GetOrCreateJointTransform(TrackedHandJoint.MiddleIntermediate).gameObject;
                        capsuleCollider = GetOrCreateCapsuleCollider(TrackedHandJoint.MiddleIntermediate, middleMiddleGameObject);
                        ConfigureCapsuleCollider(capsuleCollider, middleToTip, middleMiddleGameObject.transform);
                    }

                    if (handController.TryGetBounds(TrackedHandBounds.RingFinger, out Bounds[] ringFingerBounds))
                    {
                        // Ring finger bounds are made up of two capsule collider bounds entries.
                        Bounds knuckleToMiddle = ringFingerBounds[0];
                        Bounds middleToTip = ringFingerBounds[1];

                        var ringKnuckleGameObject = GetOrCreateJointTransform(TrackedHandJoint.RingProximal).gameObject;
                        var capsuleCollider = GetOrCreateCapsuleCollider(TrackedHandJoint.RingProximal, ringKnuckleGameObject);
                        ConfigureCapsuleCollider(capsuleCollider, knuckleToMiddle, ringKnuckleGameObject.transform);

                        var ringMiddleGameObject = GetOrCreateJointTransform(TrackedHandJoint.RingIntermediate).gameObject;
                        capsuleCollider = GetOrCreateCapsuleCollider(TrackedHandJoint.RingIntermediate, ringMiddleGameObject);
                        ConfigureCapsuleCollider(capsuleCollider, middleToTip, ringMiddleGameObject.transform);
                    }

                    if (handController.TryGetBounds(TrackedHandBounds.Pinky, out Bounds[] pinkyFingerBounds))
                    {
                        // Pinky finger bounds are made up of two capsule collider bounds entries.
                        Bounds knuckleToMiddle = pinkyFingerBounds[0];
                        Bounds middleToTip = pinkyFingerBounds[1];

                        var pinkyKnuckleGameObject = GetOrCreateJointTransform(TrackedHandJoint.LittleProximal).gameObject;
                        var capsuleCollider = GetOrCreateCapsuleCollider(TrackedHandJoint.LittleProximal, pinkyKnuckleGameObject);
                        ConfigureCapsuleCollider(capsuleCollider, knuckleToMiddle, pinkyKnuckleGameObject.transform);

                        var pinkyMiddleGameObject = GetOrCreateJointTransform(TrackedHandJoint.LittleIntermediate).gameObject;
                        capsuleCollider = GetOrCreateCapsuleCollider(TrackedHandJoint.LittleIntermediate, pinkyMiddleGameObject);
                        ConfigureCapsuleCollider(capsuleCollider, middleToTip, pinkyMiddleGameObject.transform);
                    }

                    if (handController.TryGetBounds(TrackedHandBounds.Palm, out Bounds[] palmBounds))
                    {
                        // For the palm we create a composite collider using a capsule collider per
                        // finger for the area metacarpal <-> knuckle.
                        Bounds indexPalmBounds = palmBounds[0];
                        var indexMetacarpalGameObject = GetOrCreateJointTransform(TrackedHandJoint.IndexMetacarpal).gameObject;
                        var capsuleCollider = GetOrCreateCapsuleCollider(TrackedHandJoint.IndexMetacarpal, indexMetacarpalGameObject);
                        ConfigureCapsuleCollider(capsuleCollider, indexPalmBounds, indexMetacarpalGameObject.transform);

                        Bounds middlePalmBounds = palmBounds[1];
                        var middleMetacarpalGameObject = GetOrCreateJointTransform(TrackedHandJoint.MiddleMetacarpal).gameObject;
                        capsuleCollider = GetOrCreateCapsuleCollider(TrackedHandJoint.MiddleMetacarpal, middleMetacarpalGameObject);
                        ConfigureCapsuleCollider(capsuleCollider, middlePalmBounds, middleMetacarpalGameObject.transform);

                        Bounds ringPalmBounds = palmBounds[2];
                        var ringMetacarpalGameObject = GetOrCreateJointTransform(TrackedHandJoint.RingMetacarpal).gameObject;
                        capsuleCollider = GetOrCreateCapsuleCollider(TrackedHandJoint.RingMetacarpal, ringMetacarpalGameObject);
                        ConfigureCapsuleCollider(capsuleCollider, ringPalmBounds, ringMetacarpalGameObject.transform);

                        Bounds pinkyPalmBounds = palmBounds[3];
                        var pinkyMetacarpalGameObject = GetOrCreateJointTransform(TrackedHandJoint.LittleMetacarpal).gameObject;
                        capsuleCollider = GetOrCreateCapsuleCollider(TrackedHandJoint.LittleMetacarpal, pinkyMetacarpalGameObject);
                        ConfigureCapsuleCollider(capsuleCollider, pinkyPalmBounds, pinkyMetacarpalGameObject.transform);
                    }
                }
                else if (boundsMode == HandBoundsLOD.Low)
                {
                    DisableFingerBounds();

                    if (handController.TryGetBounds(TrackedHandBounds.Hand, out Bounds[] handBounds))
                    {
                        // For full hand bounds we'll only get one bounds entry, which is a box
                        // encapsulating the whole hand.
                        Bounds fullHandBounds = handBounds[0];
                        handBoundsModeCollider = GameObject.EnsureComponent<BoxCollider>();
                        handBoundsModeCollider.enabled = true;
                        handBoundsModeCollider.center = fullHandBounds.center;
                        handBoundsModeCollider.size = fullHandBounds.size;
                        handBoundsModeCollider.isTrigger = handControllerDataProvider != null ? handControllerDataProvider.UseTriggers : inputSystemProfile.HandControllerSettings.UseTriggers;
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
            collider.isTrigger = handControllerDataProvider != null ? handControllerDataProvider.UseTriggers : inputSystemProfile.HandControllerSettings.UseTriggers;
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
                collider = forObject.EnsureComponent<CapsuleCollider>();
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

        private void UpdateHandJointTransforms()
        {
            var handController = (IHandController)Controller;

            for (int i = 0; i < HandData.JointCount; i++)
            {
                var handJoint = (TrackedHandJoint)i;
                if (handController.TryGetJointPose(handJoint, out var jointPose))
                {
                    var jointTransform = GetOrCreateJointTransform(handJoint);
                    jointTransform.localPosition = jointPose.Position;
                    jointTransform.localRotation = jointPose.Rotation;
                }
            }
        }

        /// <summary>
        /// Gets the proxy transform for a given tracked hand joint or creates
        /// it if it does not exist yet.
        /// </summary>
        /// <param name="handJoint">The hand joint a transform should be returned for.</param>
        /// <returns>Joint transform.</returns>
        public Transform GetOrCreateJointTransform(TrackedHandJoint handJoint)
        {
            if (jointTransforms.TryGetValue(handJoint, out Transform existingJointTransform))
            {
                existingJointTransform.parent = GameObject.transform;
                existingJointTransform.gameObject.SetActive(true);
                return existingJointTransform;
            }

            Transform jointTransform = new GameObject($"{handJoint}").transform;
            jointTransform.parent = GameObject.transform;
            jointTransforms.Add(handJoint, jointTransform.transform);

            return jointTransform;
        }

        protected void UpdateRendering()
        {
            var renderingMode = handControllerDataProvider != null ?
                handControllerDataProvider.RenderingMode :
                inputSystemProfile.HandControllerSettings.RenderingMode;

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
                        jointsVisualizer = Instantiate(jointsModePrefab, GameObject.transform).GetComponent<HandControllerJointsVisualizer>();
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
                        meshVisualizer = Instantiate(meshModePrefab, GameObject.transform).GetComponent<HandControllerMeshVisualizer>();
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
