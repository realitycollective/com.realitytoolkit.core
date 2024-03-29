﻿// Copyright (c) Reality Collective. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace RealityToolkit.Definitions.Lines
{
    /// <summary>
    /// Defines the type of interpolation to use when calculating a spline.
    /// </summary>
    public enum InterpolationType
    {
        Bezier = 0,
        CatmullRom,
        Hermite,
    }
}