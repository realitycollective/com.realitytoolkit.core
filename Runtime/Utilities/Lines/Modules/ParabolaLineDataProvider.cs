// Copyright (c) Reality Collective. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;

namespace RealityToolkit.Utilities.Lines.DataProviders
{
    /// <summary>
    /// Base Parabola line data provider.
    /// </summary>
    public abstract class ParabolaLineDataProvider : BaseMixedRealityLineDataProvider
    {
        [SerializeField]
        private Pose startPoint = Pose.identity;

        /// <summary>
        /// The Starting point of this line.
        /// </summary>
        /// <remarks>Always located at this <see cref="GameObject"/>'s <see cref="Transform.position"/></remarks>
        public Pose StartPoint => startPoint;

        #region MonoBehaviour Implementation

        protected override void OnValidate()
        {
            base.OnValidate();

            startPoint.position = transform.transform.InverseTransformPoint(LineTransform.position);
        }

        #endregion MonoBehaviour Implementation

        #region Line Data Provider Implementation

        /// <inheritdoc />
        protected override float GetUnClampedWorldLengthInternal()
        {
            // Crude approximation
            // TODO optimize
            float distance = 0f;
            Vector3 last = GetUnClampedPoint(0f);
            for (int i = 1; i < 10; i++)
            {
                Vector3 current = GetUnClampedPoint((float)i / 10);
                distance += Vector3.Distance(last, current);
            }

            return distance;
        }

        /// <inheritdoc />
        protected override Vector3 GetUpVectorInternal(float normalizedLength)
        {
            return transform.up;
        }

        #endregion Line Data Provider Implementation
    }
}