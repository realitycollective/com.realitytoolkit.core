// Copyright (c) Reality Collective. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using RealityToolkit.Definitions.Physics;
using RealityToolkit.Utilities.Lines.DataProviders;
using RealityToolkit.Utilities.Lines.Renderers;
using UnityEngine;
using UnityEngine.Serialization;

namespace RealityToolkit.Utilities.UX.Pointers
{
    /// <summary>
    /// A simple line pointer for drawing lines from the input source origin to the current pointer position.
    /// </summary>
    public class LinePointer : BaseControllerPointer
    {
        [SerializeField]
        [FormerlySerializedAs("LineColorSelected")]
        private Gradient lineColorSelected = new Gradient();

        protected Gradient LineColorSelected
        {
            get => lineColorSelected;
            set => lineColorSelected = value;
        }

        [SerializeField]
        [FormerlySerializedAs("LineColorValid")]
        private Gradient lineColorValid = new Gradient();

        protected Gradient LineColorValid
        {
            get => lineColorValid;
            set => lineColorValid = value;
        }

        [SerializeField]
        [FormerlySerializedAs("LineColorInvalid")]
        private Gradient lineColorInvalid = new Gradient();

        protected Gradient LineColorInvalid
        {
            get => lineColorInvalid;
            set => lineColorInvalid = value;
        }

        [SerializeField]
        [FormerlySerializedAs("LineColorNoTarget")]
        private Gradient lineColorNoTarget = new Gradient();

        protected Gradient LineColorNoTarget
        {
            get => lineColorNoTarget;
            set => lineColorNoTarget = value;
        }

        [SerializeField]
        [FormerlySerializedAs("LineColorLockFocus")]
        private Gradient lineColorLockFocus = new Gradient();

        protected Gradient LineColorLockFocus
        {
            get => lineColorLockFocus;
            set => lineColorLockFocus = value;
        }

        [Range(2, 50)]
        [SerializeField]
        [FormerlySerializedAs("LineCastResolution")]
        [Tooltip("This setting has a high performance cost. Values above 20 are not recommended.")]
        private int lineCastResolution = 10;

        protected int LineCastResolution
        {
            get => lineCastResolution;
            set => lineCastResolution = value;
        }

        [SerializeField]
        private BaseLineDataProvider lineBase;

        /// <summary>
        /// The Line Data Provider driving this pointer.
        /// </summary>
        public BaseLineDataProvider LineBase => lineBase;

        [SerializeField]
        [Tooltip("If no line renderers are specified, this array will be auto-populated on startup.")]
        private BaseLineRenderer[] lineRenderers;

        /// <summary>
        /// The current line renderers that this pointer is utilizing.
        /// </summary>
        /// <remarks>
        /// If no line renderers are specified, this array will be auto-populated on startup.
        /// </remarks>
        public BaseLineRenderer[] LineRenderers
        {
            get => lineRenderers;
            set => lineRenderers = value;
        }

        private void CheckInitialization()
        {
            if (lineBase == null)
            {
                lineBase = GetComponent<BaseLineDataProvider>();
            }

            if (lineBase == null)
            {
                Debug.LogError($"No Mixed Reality Line Data Provider found on {gameObject.name}. Did you forget to add a Line Data provider?");
            }

            if (lineBase != null && (lineRenderers == null || lineRenderers.Length == 0))
            {
                lineRenderers = lineBase.GetComponentsInChildren<BaseLineRenderer>();
            }

            if (lineRenderers == null || lineRenderers.Length == 0)
            {
                Debug.LogError($"No Mixed Reality Line Renderers found on {gameObject.name}. Did you forget to add a Mixed Reality Line Renderer?");
            }
        }

        #region MonoBehaviour Implementation

        protected virtual void OnValidate()
        {
            CheckInitialization();
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            CheckInitialization();
        }

        #endregion MonoBehaviour Implementation

        #region IPointer Implementation

        /// <inheritdoc />
        public override void OnPreRaycast()
        {
            Debug.Assert(lineBase != null);

            lineBase.UpdateMatrix();

            if (RayStabilizer != null)
            {
                RayStabilizer.UpdateStability(Rays[0].Origin, Rays[0].Direction);
                Rays[0].CopyRay(RayStabilizer.StableRay, PointerExtent);
            }

            TryGetPointerPosition(out var pointerPosition);
            TryGetPointerRotation(out var pointerRotation);

            // Set our first and last points
            lineBase.FirstPoint = pointerPosition;

            if (IsFocusLocked && Result.CurrentPointerTarget != null)
            {
                if (SyncedTarget != null)
                {
                    // Now raycast out like nothing happened so we can get an updated pointer position.
                    lineBase.LastPoint = pointerPosition + pointerRotation * (Vector3.forward * PointerExtent);
                }
                else
                {
                    // Set the line to the locked position.
                    lineBase.LastPoint = Result.EndPoint;
                }
            }
            else
            {
                lineBase.LastPoint = pointerPosition + pointerRotation * (Vector3.forward * PointerExtent);
            }

            // Make sure our array will hold
            if (Rays == null || Rays.Length != lineCastResolution)
            {
                Rays = new RayStep[lineCastResolution];
            }

            var stepSize = 1f / Rays.Length;
            var lastPoint = lineBase.GetUnClampedPoint(0f);

            for (int i = 0; i < Rays.Length; i++)
            {
                var currentPoint = lineBase.GetUnClampedPoint(stepSize * (i + 1));
                Rays[i].UpdateRayStep(ref lastPoint, ref currentPoint);
                lastPoint = currentPoint;
            }
        }

        /// <inheritdoc />
        public override void OnPostRaycast()
        {
            if (!IsInteractionEnabled)
            {
                lineBase.enabled = false;

                if (BaseCursor != null)
                {
                    BaseCursor.IsVisible = false;
                }

                return;
            }

            base.OnPostRaycast();

            Gradient lineColor;

            lineBase.enabled = true;

            if (BaseCursor != null)
            {
                BaseCursor.IsVisible = true;
            }

            // The distance the ray travels through the world before it hits something.
            // Measured in world-units (as opposed to normalized distance).
            float clearWorldLength;

            // Used to ensure the line doesn't extend beyond the cursor
            float cursorOffsetWorldLength = BaseCursor?.SurfaceCursorDistance ?? 0f;

            // If we hit something
            if (Result?.CurrentPointerTarget != null)
            {
                clearWorldLength = Result.RayDistance;
                lineColor = IsSelectPressed || IsGrabPressed ? lineColorSelected : lineColorValid;
            }
            else
            {
                clearWorldLength = PointerExtent;
                lineColor = IsSelectPressed || IsGrabPressed ? lineColorSelected : lineColorNoTarget;
            }

            if (IsFocusLocked)
            {
                lineColor = lineColorLockFocus;
            }

            int maxClampLineSteps = lineCastResolution;

            for (var i = 0; i < lineRenderers.Length; i++)
            {
                var lineRenderer = lineRenderers[i];
                // Renderers are enabled by default if line is enabled
                lineRenderer.enabled = true;
                maxClampLineSteps = Mathf.Max(maxClampLineSteps, lineRenderer.LineStepCount);
                lineRenderer.LineColor = lineColor;
            }

            // If focus is locked, we're sticking to the target
            // So don't clamp the world length
            if (IsFocusLocked && Result.CurrentPointerTarget != null)
            {
                if (SyncedTarget != null)
                {
                    if (Result.GrabPoint == Vector3.zero)
                    {
                        LineBase.LastPoint = Result.EndPoint;
                    }
                    else
                    {
                        LineBase.LastPoint = Result.GrabPoint;
                    }
                }

                float cursorOffsetLocalLength = LineBase.GetNormalizedLengthFromWorldLength(cursorOffsetWorldLength);
                LineBase.LineEndClamp = 1 - cursorOffsetLocalLength;
            }
            else
            {
                // Otherwise clamp the line end by the clear distance
                float clearLocalLength = lineBase.GetNormalizedLengthFromWorldLength(clearWorldLength - cursorOffsetWorldLength, maxClampLineSteps);
                lineBase.LineEndClamp = clearLocalLength;
            }
        }

        #endregion IPointer Implementation
    }
}