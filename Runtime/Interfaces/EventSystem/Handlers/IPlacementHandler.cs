// Copyright (c) Reality Collective. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using RealityToolkit.EventDatum;

namespace RealityToolkit.Interfaces.Events.Handlers
{
    /// <summary>
    /// Interface to implement reacting to placement of objects.
    /// </summary>
    public interface IPlacementHandler : IEventHandler
    {
        void OnPlacingStarted(PlacementEventData eventData);

        void OnPlacingCompleted(PlacementEventData eventData);
    }
}