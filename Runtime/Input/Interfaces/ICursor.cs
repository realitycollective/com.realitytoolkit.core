// Copyright (c) Reality Collective. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using RealityToolkit.Input.Interactions.Interactors;
using RealityToolkit.Input.Interfaces.Handlers;
using UnityEngine;

namespace RealityToolkit.Input.Interfaces
{
    /// <summary>
    /// Cursor Interface for handling input events and setting visibility.
    /// </summary>
    public interface ICursor : IFocusChangedHandler, ISourceStateHandler, IPointerHandler
    {
        /// <summary>
        /// The <see cref="IInteractor"/> this <see cref="ICursor"/> is associated with.
        /// </summary>
        IInteractor Pointer { get; set; }

        /// <summary>
        /// Surface distance to place the cursor off of the surface at
        /// </summary>
        float SurfaceCursorDistance { get; }

        /// <summary>
        /// Position of the <see cref="ICursor"/>.
        /// </summary>
        Vector3 Position { get; }

        /// <summary>
        /// Rotation of the <see cref="ICursor"/>.
        /// </summary>
        Quaternion Rotation { get; }

        /// <summary>
        /// Local scale of the <see cref="ICursor"/>.
        /// </summary>
        Vector3 LocalScale { get; }

        /// <summary>
        /// Gets or sets whether the <see cref="ICursor"/> is visible or not.
        /// </summary>
        bool IsVisible { get; set; }

        /// <summary>
        /// Sets the visibility of the <see cref="ICursor"/> when the source is detected.
        /// </summary>
        bool SetVisibilityOnSourceDetected { get; set; }

        /// <summary>
        /// Returns the <see cref="ICursor"/>'s <see cref="GameObject"/> reference.
        /// </summary>
        /// <returns>The <see cref="GameObject"/> this <see cref="ICursor"/> component is attached to.</returns>
        GameObject GameObjectReference { get; }
    }
}
