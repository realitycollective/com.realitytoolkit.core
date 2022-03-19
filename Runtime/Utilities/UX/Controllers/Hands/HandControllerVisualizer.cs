// Copyright (c) Reality Collective. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using RealityToolkit.Interfaces.InputSystem.Controllers.Hands;
using System.Collections.Generic;
using UnityEngine;
using XRTK.Definitions.Controllers.Hands;
using XRTK.Definitions.InputSystem;
using XRTK.Interfaces.InputSystem;
using XRTK.Interfaces.InputSystem.Handlers;
using XRTK.Interfaces.InputSystem.Providers.Controllers.Hands;
using XRTK.Services;
using XRTK.Services.InputSystem.Utilities;
using XRTK.Utilities.UX.Controllers.Hands;

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
                inputSystemProfile.RenderingMode;

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
