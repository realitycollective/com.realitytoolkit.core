// Copyright (c) Reality Collective. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using RealityCollective.Definitions.Utilities;
using RealityToolkit.Input.Interfaces.Handlers;
using UnityEngine;

namespace RealityToolkit.Input.Interfaces
{
    /// <summary>
    /// Interface for cursor modifiers that can modify a <see cref="GameObject"/>'s properties.
    /// </summary>
    public interface ICursorModifier : IFocusChangedHandler
    {
        /// <summary>
        /// Transform for which this <see cref="ICursor"/> modifies applies its various properties.
        /// </summary>
        Transform HostTransform { get; set; }

        /// <summary>
        /// How much a <see cref="ICursor"/>'s position should be offset from the surface of the <see cref="GameObject"/> when overlapping.
        /// </summary>
        Vector3 CursorPositionOffset { get; set; }

        /// <summary>
        /// Should the <see cref="ICursor"/> snap to the <see cref="GameObject"/>'s position?
        /// </summary>
        bool SnapCursorPosition { get; set; }

        /// <summary>
        /// Scale of the <see cref="ICursor"/> when looking at this <see cref="GameObject"/>.
        /// </summary>
        Vector3 CursorScaleOffset { get; set; }

        /// <summary>
        /// Direction of the <see cref="ICursor"/> offset.
        /// </summary>
        Vector3 CursorNormalOffset { get; set; }

        /// <summary>
        /// If true, the normal from the pointing vector will be used to orient the <see cref="ICursor"/> instead of the targeted <see cref="GameObject"/>'s normal at point of contact.
        /// </summary>
        bool UseGazeBasedNormal { get; set; }

        /// <summary>
        /// Should the <see cref="ICursor"/> be hidden when this <see cref="GameObject"/> is focused?
        /// </summary>
        bool HideCursorOnFocus { get; set; }

        /// <summary>
        /// <see cref="ICursor"/> animation parameters to set when this <see cref="GameObject"/> is focused. Leave empty for none.
        /// </summary>
        AnimatorParameter[] CursorParameters { get; }

        /// <summary>
        /// Indicates whether the <see cref="ICursor"/> should be visible or not.
        /// </summary>
        /// <returns>True if <see cref="ICursor"/> should be visible, false if not.</returns>
        bool GetCursorVisibility();

        /// <summary>
        /// Returns the <see cref="ICursor"/> position after considering this modifier.
        /// </summary>
        /// <param name="cursor"><see cref="ICursor"/> that is being modified.</param>
        /// <returns>New position for the <see cref="ICursor"/></returns>
        Vector3 GetModifiedPosition(ICursor cursor);

        /// <summary>
        /// Returns the <see cref="ICursor"/> rotation after considering this modifier.
        /// </summary>
        /// <param name="cursor"><see cref="ICursor"/> that is being modified.</param>
        /// <returns>New rotation for the <see cref="ICursor"/></returns>
        Quaternion GetModifiedRotation(ICursor cursor);

        /// <summary>
        /// Returns the <see cref="ICursor"/>'s local scale after considering this modifier.
        /// </summary>
        /// <param name="cursor"><see cref="ICursor"/> that is being modified.</param>
        /// <returns>New local scale for the <see cref="ICursor"/></returns>
        Vector3 GetModifiedScale(ICursor cursor);

        /// <summary>
        /// Returns the modified <see cref="Transform"/> for the <see cref="ICursor"/> after considering this modifier.
        /// </summary>
        /// <param name="cursor">Cursor that is being modified.</param>
        /// <param name="position">Modified position.</param>
        /// <param name="rotation">Modified rotation.</param>
        /// <param name="scale">Modified scale.</param>
        void GetModifiedTransform(ICursor cursor, out Vector3 position, out Quaternion rotation, out Vector3 scale);
    }
}