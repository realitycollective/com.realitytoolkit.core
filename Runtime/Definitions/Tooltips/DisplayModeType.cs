﻿// Copyright (c) Reality Collective. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace RealityToolkit.Definitions.ToolTips
{
    /// <summary>
    /// Enum describing the display mode of a ToolTip.
    /// </summary>
    public enum DisplayModeType
    {
        /// <summary>
        /// No state to have from Manager
        /// </summary>
        None = 0,
        /// <summary>
        /// Tips are always on
        /// </summary>
        On,
        /// <summary>
        /// Looking at Object Activates tip (Object must be interactive)
        /// </summary>
        OnFocus,
        /// <summary>
        /// Tips are always off
        /// </summary>
        Off
    }
}
