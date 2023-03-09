// Copyright (c) Reality Collective. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;
using UnityEngine.UI;

namespace RealityToolkit.Utilities.Extensions
{
    public static class GameObjectExtensions
    {
        /// <summary>
        /// Sets the <see cref="Renderer"/>s and optionally <see cref="Collider"/>s in this <see cref="GameObject"/> to the provided <see cref="bool"/> state.
        /// </summary>
        /// <remarks>If the GameObject or it's children are inactive, they will be set active, but the GameObjects will not be set inactive.</remarks>
        /// <param name="gameObject"></param>
        /// <param name="isActive"></param>
        /// <param name="includeColliders"></param>
        public static void SetCanvasRenderingActive(this GameObject gameObject, bool isActive, bool includeColliders = true)
        {
            foreach (var graphic in gameObject.GetComponentsInChildren<Graphic>())
            {
                graphic.enabled = isActive;
            }
        }
    }
}