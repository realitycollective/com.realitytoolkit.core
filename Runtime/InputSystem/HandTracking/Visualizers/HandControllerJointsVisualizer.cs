﻿// Copyright (c) Reality Collective. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Collections.Generic;
using UnityEngine;

namespace RealityToolkit.InputSystem.Hands.Visualizers
{
    public class HandControllerJointsVisualizer : MonoBehaviour
    {
        private readonly Dictionary<TrackedHandJoint, GameObject> jointVisualizations = new Dictionary<TrackedHandJoint, GameObject>();
        private DefaultHandControllerVisualizer mainVisualizer;

        [SerializeField]
        [Tooltip("The wrist prefab to use.")]
        private GameObject wristPrefab = null;

        [SerializeField]
        [Tooltip("The joint prefab to use.")]
        private GameObject jointPrefab = null;

        [SerializeField]
        [Tooltip("The joint prefab to use for palm.")]
        private GameObject palmPrefab = null;

        [SerializeField]
        [Tooltip("The joint prefab to use for the index tip (point of interaction.")]
        private GameObject fingertipPrefab = null;

        [SerializeField]
        [Tooltip("Material tint color for index fingertip.")]
        private Color indexFingertipColor = Color.cyan;

        private void OnEnable()
        {
            foreach (var jointVisualization in jointVisualizations)
            {
                jointVisualization.Value.SetActive(true);
            }
        }

        private void OnDisable()
        {
            foreach (var jointVisualization in jointVisualizations)
            {
                jointVisualization.Value.SetActive(false);
            }
        }

        /// <summary>
        /// Updates the joints visuailzation.
        /// </summary>
        /// <param name="mainVisualizer">The managing visuailzer component.</param>
        public void UpdateVisualization(DefaultHandControllerVisualizer mainVisualizer)
        {
            this.mainVisualizer = mainVisualizer;

            for (int i = 0; i < HandData.JointCount; i++)
            {
                var joint = (TrackedHandJoint)i;
                CreateJointVisualizerIfNotExists(joint);
            }
        }

        private void CreateJointVisualizerIfNotExists(TrackedHandJoint handJoint)
        {
            if (jointVisualizations.TryGetValue(handJoint, out GameObject jointObject))
            {
                jointObject.SetActive(true);
                return;
            }

            var prefab = jointPrefab;

            switch (handJoint)
            {
                case TrackedHandJoint.Wrist:
                    prefab = wristPrefab;
                    break;
                case TrackedHandJoint.Palm:
                    prefab = palmPrefab;
                    break;
                case TrackedHandJoint.IndexTip:
                case TrackedHandJoint.MiddleTip:
                case TrackedHandJoint.LittleTip:
                case TrackedHandJoint.RingTip:
                case TrackedHandJoint.ThumbTip:
                    prefab = fingertipPrefab;
                    break;
            }

            if (prefab != null)
            {
                var jointVisualization = Instantiate(prefab, mainVisualizer.GetOrCreateJointTransform(handJoint));

                if (handJoint == TrackedHandJoint.IndexTip)
                {
                    var indexJointRenderer = jointVisualization.GetComponent<Renderer>();
                    if (indexJointRenderer != null)
                    {
                        var indexMaterial = indexJointRenderer.material;
                        indexMaterial.color = indexFingertipColor;
                        indexJointRenderer.material = indexMaterial;
                    }
                }

                jointVisualizations.Add(handJoint, jointVisualization);
            }
        }
    }
}