// Copyright (c) Reality Collective. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace RealityToolkit.Boundary.Definitions
{
    /// <summary>
    /// Defines available visibility state for platform boundaries used by the
    /// <see cref="Interfaces.IBoundaryService"/>.
    /// </summary>
    public enum BoundaryVisibility
    {
        /// <summary>
        /// The boundaries visibility status is unknown. Platform boundaries visibility
        /// may be unknown e.g. when the platform does not provide an API for XRTK to query
        /// boundary visibility state.
        /// </summary>
        Unknown = 0,
        /// <summary>
        /// The boundaries are hidden.
        /// </summary>
        Hidden,
        /// <summary>
        /// The boundaries are visible.
        /// </summary>
        Visible
    }
}