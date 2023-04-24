// Copyright (c) Reality Collective. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using RealityToolkit.EventDatum;
using UnityEngine.EventSystems;

namespace RealityToolkit.Interfaces.Events.Handlers
{
    /// <summary>
    /// Interface to implement generic events.
    /// </summary>
    public interface IEventHandler : IEventSystemHandler
    {
        void OnEventRaised(GenericBaseEventData eventData);
    }
}