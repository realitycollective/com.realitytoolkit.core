// Copyright (c) Reality Collective. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using RealityCollective.Extensions;
using RealityToolkit.Definitions.Physics;
using RealityToolkit.Utilities.Physics;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace RealityToolkit.Extensions
{
    /// <summary>
    /// Extension methods for Unity's EventSystem
    /// </summary>
    public static class EventSystemExtensions
    {
        private static readonly List<RaycastResult> RaycastResults = new List<RaycastResult>();
        private static readonly RaycastResultComparer RaycastResultComparer = new RaycastResultComparer();

        /// <summary>
        /// Executes a raycast all and returns the closest element.
        /// Fixes the current issue with Unity's raycast sorting which does not consider separate canvases.
        /// </summary>
        /// <returns>RaycastResult if hit, or an empty RaycastResult if nothing was hit</returns>
        public static RaycastResult Raycast(this EventSystem eventSystem, PointerEventData pointerEventData, LayerMask[] layerMasks)
        {
            eventSystem.RaycastAll(pointerEventData, RaycastResults);
            return PrioritizeRaycastResult(layerMasks);
        }

        /// <summary>
        /// Sorts the available Raycasts in to a priority order for query.
        /// </summary>
        /// <param name="priority">The layer mask priority.</param>
        /// <returns><see cref="RaycastResult"/></returns>
        private static RaycastResult PrioritizeRaycastResult(LayerMask[] priority)
        {
            ComparableRaycastResult maxResult = default;

            for (var i = 0; i < RaycastResults.Count; i++)
            {
                if (RaycastResults[i].gameObject.IsNull()) { continue; }

                var layerMaskIndex = RaycastResults[i].gameObject.layer.FindLayerListIndex(priority);
                if (layerMaskIndex == -1) { continue; }

                var result = new ComparableRaycastResult(RaycastResults[i], layerMaskIndex);

                if (maxResult.RaycastResult.module.IsNull() || RaycastResultComparer.Compare(maxResult, result) < 0)
                {
                    maxResult = result;
                }
            }

            return maxResult.RaycastResult;
        }
    }
}
