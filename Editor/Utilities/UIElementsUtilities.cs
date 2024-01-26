// Copyright (c) Reality Collective. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine.UIElements;

namespace RealityToolkit.Editor.Utilities
{
    /// <summary>
    /// Utilities for creating UI Toolkit (UIElements) based custom inspectors.
    /// </summary>
    public static class UIElementsUtilities
    {
        private const int defaultVerticalSpace = 10;

        /// <summary>
        /// Creates an empty <see cref="VisualElement"/> to create vertical space.
        /// </summary>
        /// <param name="height">Space in pixels. Defaults to <c>10</c>.</param>
        /// <returns></returns>
        public static VisualElement Space(int height = defaultVerticalSpace)
        {
            var spacer = new VisualElement();
            spacer.style.height = height;
            return spacer;
        }
    }
}
