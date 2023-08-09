// Copyright (c) Reality Collective. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using RealityToolkit.EventDatum.Input;
using RealityToolkit.Input.Interactions.Interactors;
using RealityToolkit.Input.Interfaces.Handlers;
using RealityToolkit.Input.Interfaces.Modules;
using UnityEngine;

namespace RealityToolkit.Input.Interfaces
{
    /// <summary>
    /// Implements the Focus Provider for handling focus of pointers.
    /// </summary>
    public interface IFocusProvider : ISourceStateHandler, IInputServiceModule
    {
        /// <summary>
        /// Maximum distance at which all pointers can collide with a <see cref="GameObject"/>, unless it has an override extent.
        /// </summary>
        float GlobalPointingExtent { get; }

        /// <summary>
        /// The Physics Layers, in prioritized order, that are used to determine the <see cref="IPointerResult.CurrentPointerTarget"/> when raycasting.
        /// </summary>
        LayerMask[] GlobalPointerRaycastLayerMasks { get; }

        /// <summary>
        /// The Camera the <see cref="UnityEngine.EventSystems.EventSystem"/> uses to raycast against.
        /// </summary>
        /// <remarks>Every uGUI <see cref="Canvas"/> in your scene should use this camera as its event camera.</remarks>
        Camera UIRaycastCamera { get; }

        /// <summary>
        /// Gets the currently focused object for the pointing source.
        /// </summary>
        /// <remarks>If the pointing source is not registered, then the Gaze's Focused <see cref="GameObject"/> is returned.</remarks>
        /// <param name="pointingSource"></param>
        /// <returns>Currently Focused Object.</returns>
        GameObject GetFocusedObject(IInteractor pointingSource);

        /// <summary>
        /// Gets the currently focused object for the pointing source.
        /// </summary>
        /// <param name="pointer"></param>
        /// <param name="focusDetails"></param>
        bool TryGetFocusDetails(IInteractor pointer, out IPointerResult focusDetails);

        /// <summary>
        /// Get the Graphic Event Data for the specified pointing source.
        /// </summary>
        /// <param name="pointer">The pointer who's graphic event data we're looking for.</param>
        /// <param name="graphicInputEventData">The graphic event data for the specified pointer</param>
        /// <returns>True, if graphic event data exists.</returns>
        bool TryGetSpecificPointerGraphicEventData(IInteractor pointer, out GraphicInputEventData graphicInputEventData);

        /// <summary>
        /// Generate a new unique pointer id.
        /// </summary>
        /// <returns></returns>
        uint GenerateNewPointerId();

        /// <summary>
        /// Checks if the pointer is registered with the Focus Manager.
        /// </summary>
        /// <param name="pointer"></param>
        /// <returns>True, if registered, otherwise false.</returns>
        bool IsPointerRegistered(IInteractor pointer);

        /// <summary>
        /// Registers the pointer with the Focus Manager.
        /// </summary>
        /// <param name="pointer"></param>
        /// <returns>True, if the pointer was registered, false if the pointer was previously registered.</returns>
        bool RegisterPointer(IInteractor pointer);

        /// <summary>
        /// Unregisters the pointer with the Focus Manager.
        /// </summary>
        /// <param name="pointer"></param>
        /// <returns>True, if the pointer was unregistered, false if the pointer was not registered.</returns>
        bool UnregisterPointer(IInteractor pointer);
    }
}