// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using RealityToolkit.EventDatum.Input;
using UnityEngine.EventSystems;

namespace RealityToolkit.Input.Interfaces.Handlers
{
    /// <summary>
    /// Interface to implement to react to pointer drag UnityEngine.Input.
    /// </summary>
    public interface IPointerDragHandler : IEventSystemHandler
    {
        /// <summary>
        /// When a pointer drag begin event is raised, this method is used to pass along the event data to the input handler.
        /// </summary>
        /// <param name="eventData"></param>
        void OnPointerDragBegin(PointerDragEventData eventData);

        /// <summary>
        /// When a pointer dragged event is raised, this method is used to pass along the event data to the input handler.
        /// </summary>
        /// <param name="eventData"></param>
        void OnPointerDrag(PointerDragEventData eventData);

        /// <summary>
        /// When a pointer drag end event is raised, this method is used to pass along the event data to the input handler.
        /// </summary>
        /// <param name="eventData"></param>
        void OnPointerDragEnd(PointerDragEventData eventData);
    }
}