// Copyright (c) Reality Collective. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;

namespace RealityToolkit.Utilities.Lines.DataProviders
{
    /// <summary>
    /// A simple line with two points.
    /// </summary>
    public class SimpleLineDataProvider : BaseLineDataProvider
    {
        [SerializeField]
        private Pose startPoint = Pose.identity;

        /// <summary>
        /// The Starting point of this line.
        /// </summary>
        /// <remarks>Always located at this <see cref="GameObject"/>'s <see cref="Transform.position"/></remarks>
        public Pose StartPoint => startPoint;

        [SerializeField]
        [Tooltip("The point where this line will end.\nNote: Start point is always located at the GameObject's transform position.")]
        private Pose endPoint = new Pose(Vector3.right, Quaternion.identity);

        /// <summary>
        /// The point where this line will end.
        /// </summary>
        public Pose EndPoint
        {
            get => endPoint;
            set => endPoint = value;
        }

        #region MonoBehaviour Implementation

        protected override void OnValidate()
        {
            base.OnValidate();

            if (endPoint.position == startPoint.position)
            {
                endPoint.position = transform.InverseTransformPoint(LineTransform.position) + Vector3.right;
            }

            startPoint.position = transform.transform.InverseTransformPoint(LineTransform.position);
        }

        #endregion MonoBehaviour Implementation

        #region Line Data Provider Implementation

        /// <inheritdoc />
        public override int PointCount => 2;

        /// <inheritdoc />
        protected override Vector3 GetPointInternal(int pointIndex)
        {
            switch (pointIndex)
            {
                case 0:
                    return startPoint.position;
                case 1:
                    return endPoint.position;
                default:
                    Debug.LogError("Invalid point index");
                    return Vector3.zero;
            }
        }

        /// <inheritdoc />
        protected override void SetPointInternal(int pointIndex, Vector3 point)
        {
            switch (pointIndex)
            {
                case 0:
                    startPoint.position = point;
                    break;
                case 1:
                    endPoint.position = point;
                    break;
                default:
                    Debug.LogError("Invalid point index");
                    break;
            }
        }

        /// <inheritdoc />
        protected override Vector3 GetPointInternal(float normalizedDistance)
        {
            return Vector3.Lerp(startPoint.position, endPoint.position, normalizedDistance);
        }

        /// <inheritdoc />
        protected override float GetUnClampedWorldLengthInternal()
        {
            return Vector3.Distance(startPoint.position, endPoint.position);
        }

        /// <inheritdoc />
        protected override Vector3 GetUpVectorInternal(float normalizedLength)
        {
            return transform.up;
        }

        #endregion Line Data Provider Implementation
    }
}