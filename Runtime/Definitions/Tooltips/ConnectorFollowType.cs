﻿// Copyright (c) Reality Collective. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;

namespace RealityToolkit.Definitions.ToolTips
{
    /// <summary>
    /// How does the Tooltip track with its parent object
    /// </summary>
    [Flags]
    public enum ConnectorFollowType
    {
        /// <summary>
        /// The anchor will follow the target - pivot remains unaffected
        /// </summary>
        AnchorOnly = 1 << 0,
        /// <summary>
        /// Anchor and pivot will follow target position, but not rotation
        /// </summary>
        Position = 1 << 1,
        /// <summary>
        /// Anchor and pivot will follow target like it's parented, but only on Y axis
        /// </summary>
        YRotation = 1 << 2,
        /// <summary>
        /// Anchor and pivot will follow target like it's parented
        /// </summary>
        XRotation = 1 << 3,
    }
}