// Copyright (c) Reality Collective. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using RealityToolkit.Definitions.Controllers.Hands;
using RealityToolkit.Definitions.InputSystem;
using RealityToolkit.Definitions.Utilities;
using RealityToolkit.EventDatum.Input;
using RealityToolkit.Interfaces.InputSystem;
using RealityToolkit.Interfaces.InputSystem.Controllers.Hands;
using RealityToolkit.Interfaces.InputSystem.Handlers;
using RealityToolkit.Services;
using RealityToolkit.Services.InputSystem.Utilities;
using System.Collections.Generic;
using UnityEngine;

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

        private readonly Dictionary<XRHandJoint, Transform> jointTransforms = new Dictionary<XRHandJoint, Transform>();
        private HandControllerJointsVisualizer jointsVisualizer;
        private HandControllerMeshVisualizer meshVisualizer;
        private MixedRealityInputSystemProfile inputSystemProfile;
        private BoxCollider handBoundsModeCollider;
        private readonly Dictionary<XRHandJoint, CapsuleCollider> fingerBoundsModeColliders = new Dictionary<XRHandJoint, CapsuleCollider>();
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
            if (!MixedRealityToolkit.TryGetSystemProfile<IMixedRealityInputSystem, MixedRealityInputSystemProfile>(out inputSystemProfile))
            {
                Debug.LogError($"The {nameof(HandControllerVisualizer)} requires a valid {nameof(MixedRealityInputSystemProfile)} to work.");
                return;
            }
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
                UpdateRendering();
            }
        }

        private void UpdateHandJointTransforms()
        {
            var handController = (IHandController)Controller;

            for (int i = 0; i < HandData.JointCount; i++)
            {
                var handJoint = (XRHandJoint)i;
                if (handController.TryGetJointPose(handJoint, out var jointPose))
                {
                    var jointTransform = GetOrCreateJointTransform(handJoint);
                    jointTransform.position = jointPose.Position;
                    jointTransform.rotation = jointPose.Rotation;
                }
            }
        }

        /// <summary>
        /// Gets the proxy transform for a given tracked hand joint or creates
        /// it if it does not exist yet.
        /// </summary>
        /// <param name="handJoint">The hand joint a transform should be returned for.</param>
        /// <returns>Joint transform.</returns>
        public Transform GetOrCreateJointTransform(XRHandJoint handJoint)
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
            var renderingMode = inputSystemProfile.HandControllerSettings.RenderingMode;
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
