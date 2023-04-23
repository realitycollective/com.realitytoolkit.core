// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using RealityToolkit.EventDatum.Input;
using UnityEngine.EventSystems;

namespace RealityToolkit.Input.Interfaces.Handlers
{
    /// <summary>
    /// Interface to implement to react to pointer scroll UnityEngine.Input.
    /// </summary>
    public interface IPointerScrollHandler : IEventSystemHandler
    {
        /// <summary>
        /// When a pointer scroll is raised, this method is used to pass along the event data to the input handler.
        /// </summary>
        /// <param name="eventData"></param>
        void OnPointerScroll(PointerScrollEventData eventData);
    }
}