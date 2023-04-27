// Copyright (c) Reality Collective. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using RealityToolkit.Interfaces.Events;
using UnityEngine;
using UnityEngine.EventSystems;

namespace RealityToolkit.EventDatum
{
    /// <summary>
    /// Describes placement of objects events.
    /// </summary>
    public class PlacementEventData : GenericBaseEventData
    {
        /// <summary>
        /// The game object that is being placed.
        /// </summary>
        public GameObject ObjectBeingPlaced { get; private set; }

        /// <inheritdoc />
        public PlacementEventData(EventSystem eventSystem) : base(eventSystem) { }

        /// <summary>
        /// Populates the event with data.
        /// </summary>
        /// <param name="eventSource"></param>
        /// <param name="objectBeingPlaced"></param>
        public void Initialize(IEventSource eventSource, GameObject objectBeingPlaced)
        {
            BaseInitialize(eventSource);
            ObjectBeingPlaced = objectBeingPlaced;
        }
    }
}
