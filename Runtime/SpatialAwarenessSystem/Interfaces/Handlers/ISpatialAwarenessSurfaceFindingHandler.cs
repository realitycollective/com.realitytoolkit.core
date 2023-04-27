// Copyright (c) Reality Collective. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using RealityToolkit.EventDatum.SpatialAwareness;
using UnityEngine.EventSystems;

namespace RealityToolkit.SpatialAwareness.Interfaces.Handlers
{
    /// <summary>
    /// The event handler for all Spatial Awareness Surface Finding Events.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface ISpatialAwarenessSurfaceFindingHandler<T> : IEventSystemHandler
    {
        /// <summary>
        /// Called when the spatial awareness surface finding subsystem adds a new planar surface.
        /// </summary>
        /// <param name="eventData">Data describing the event.</param>
        void OnSurfaceAdded(SpatialAwarenessEventData<T> eventData);

        /// <summary>
        /// Called when the spatial awareness surface finding subsystem updates an existing planar surface.
        /// </summary>
        /// <param name="eventData">Data describing the event.</param>
        void OnSurfaceUpdated(SpatialAwarenessEventData<T> eventData);

        /// <summary>
        /// Called when the spatial awareness surface finding subsystem removes an existing planar surface.
        /// </summary>
        /// <param name="eventData">Data describing the event.</param>
        void OnSurfaceRemoved(SpatialAwarenessEventData<T> eventData);
    }
}
