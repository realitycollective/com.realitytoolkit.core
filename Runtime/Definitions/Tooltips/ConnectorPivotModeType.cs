﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace RealityToolkit.Definitions.ToolTips
{
    /// <summary>
    /// Controls how the tooltip will pivot relative to the camera/other objects.
    /// </summary>
    public enum ConnectorPivotModeType
    {
        /// <summary>
        /// Tooltip pivot will be set manually
        /// </summary>
        Manual = 0,
        /// <summary>
        /// Tooltip pivot will be set relative to object/camera based on specified direction and line length
        /// </summary>
        Automatic,
    }
}