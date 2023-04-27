﻿// Copyright (c) Reality Collective. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;

namespace RealityToolkit.Utilities.UX.ToolTips
{
    /// <summary>
    /// Renders a background mesh for a tool tip using a mesh renderer
    /// If the mesh has an offset anchor point you will get odd results
    /// </summary>
    public class ToolTipBackgroundMesh : ToolTipBackground
    {
        [SerializeField]
        [Tooltip("Transform that scale and offset will be applied to.")]
        private Transform backgroundTransform;

        private float depth = 1f;

        // TODO: Unused?
        //private Bounds localContentBounds;

        /// <summary>
        /// Mesh renderer button for mesh background.
        /// </summary>
        public MeshRenderer BackgroundRenderer;

        /// <summary>
        /// Determines whether background of Tooltip is visible.
        /// </summary>
        public bool IsVisible
        {
            set
            {
                if (BackgroundRenderer)
                {
                    BackgroundRenderer.enabled = value;
                }
            }
        }

        /// <summary>
        /// The Transform for the background of the Tooltip.
        /// </summary>
        public Transform BackgroundTransform
        {
            get => backgroundTransform;

            set => backgroundTransform = value;
        }

        protected override void ScaleToFitContent()
        {
            if (BackgroundRenderer != null)
            {
                //Get the local size of the content - this is the scale of the text under the content parent
                Vector3 localContentSize = ToolTipContent.LocalContentSize;
                Vector3 localContentOffset = ToolTipContent.LocalContentOffset;

                //Get the size of the mesh and use this to adjust the local content size on the x / y axis
                //This will accomodate meshes that aren't built to 1,1 scale
                Bounds meshBounds = BackgroundRenderer.GetComponent<MeshFilter>().sharedMesh.bounds;
                localContentSize.x /= meshBounds.size.x;
                localContentSize.y /= meshBounds.size.y;
                localContentSize.z = depth;

                //Don't use the mesh bounds for local content since an offset center may be used for design effect
                // TODO: Unused?
                //if (localContentSize.x > 0 && localContentSize.y > 0)
                //{
                //    localContentBounds = new Bounds(localContentOffset, localContentSize);
                //}
            }
        }

        private void OnDrawGizmos()
        {
            if (Application.isPlaying) { return; }

            if (ToolTipContent == null)
            {
                ToolTipContent = gameObject.GetComponent<ToolTip>();
            }

            if (ToolTipContent == null) { return; }

            ScaleToFitContent();
        }
    }
}
