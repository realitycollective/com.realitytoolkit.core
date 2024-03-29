﻿// Copyright (c) Reality Collective. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace RealityToolkit.Definitions.Lines
{
    /// <summary>
    /// Defines how a base line data provider will transform its points
    /// </summary>
    public enum LinePointTransformMode
    {
        /// <summary>
        /// Use the local line transform. More reliable but with a performance cost.
        /// </summary>
        UseTransform,
        /// <summary>
        /// Use a matrix. Lines that are not active and enabled will not update point positions.
        /// </summary>
        UseMatrix,
    }
}