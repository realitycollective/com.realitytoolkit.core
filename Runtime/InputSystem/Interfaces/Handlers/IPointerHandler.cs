// Copyright (c) Reality Collective. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using RealityToolkit.EventDatum.Input;
using UnityEngine.EventSystems;

namespace RealityToolkit.Input.Interfaces.Handlers
{
    /// <summary>
    /// Interface to implement to react to simple pointer UnityEngine.Input.
    /// </summary>
    public interface IPointerHandler : IEventSystemHandler
    {
        /// <summary>
        /// When a pointer down event is raised, this method is used to pass along the event data to the input handler.
        /// </summary>
        /// <param name="eventData"></param>
        void OnPointerDown(EventDatum.Input.PointerEventData eventData);

        /// <summary>
        /// When a pointer up event is raised, this method is used to pass along the event data to the input handler.
        /// </summary>
        /// <param name="eventData"></param>
        void OnPointerUp(EventDatum.Input.PointerEventData eventData);

        /// <summary>
        /// When a pointer clicked event is raised, this method is used to pass along the event data to the input handler.
        /// </summary>
        /// <param name="eventData"></param>
        void OnPointerClicked(EventDatum.Input.PointerEventData eventData);
    }
}