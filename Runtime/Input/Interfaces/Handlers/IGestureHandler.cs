﻿// Copyright (c) Reality Collective. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using RealityToolkit.EventDatum.Input;
using UnityEngine.EventSystems;

namespace RealityToolkit.Input.Interfaces.Handlers
{
    /// <summary>
    /// Interface to implement for generic gesture UnityEngine.Input.
    /// </summary>
    public interface IGestureHandler : IEventSystemHandler
    {
        /// <summary>
        /// Gesture Started Event.
        /// </summary>
        /// <param name="eventData"></param>
        void OnGestureStarted(InputEventData eventData);

        /// <summary>
        /// Gesture Updated Event.
        /// </summary>
        /// <param name="eventData"></param>
        void OnGestureUpdated(InputEventData eventData);

        /// <summary>
        /// Gesture Completed Event.
        /// </summary>
        /// <param name="eventData"></param>
        void OnGestureCompleted(InputEventData eventData);

        /// <summary>
        /// Gesture Canceled Event.
        /// </summary>
        /// <param name="eventData"></param>
        void OnGestureCanceled(InputEventData eventData);
    }

    /// <summary>
    /// Interface to implement for generic gesture UnityEngine.Input.
    /// </summary>
    /// <typeparam name="T">The type of data you want to listen for.</typeparam>
    public interface IGestureHandler<T> : IGestureHandler
    {
        /// <summary>
        /// Gesture Updated Event.
        /// </summary>
        /// <remarks>
        /// The <see cref="InputEventData{T}.InputData"/> for the associated gesture data.
        /// </remarks>
        /// <param name="eventData"></param>
        void OnGestureUpdated(InputEventData<T> eventData);

        /// <summary>
        /// Gesture Completed Event.
        /// </summary>
        /// <remarks>
        /// The <see cref="InputEventData{T}.InputData"/> for the associated gesture data.
        /// </remarks>
        /// <param name="eventData"></param>
        void OnGestureCompleted(InputEventData<T> eventData);
    }
}