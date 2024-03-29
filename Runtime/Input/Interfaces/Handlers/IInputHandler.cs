﻿// Copyright (c) Reality Collective. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using RealityToolkit.EventDatum.Input;
using UnityEngine.EventSystems;

namespace RealityToolkit.Input.Interfaces.Handlers
{
    /// <summary>
    /// Interface to implement for simple generic UnityEngine.Input.
    /// </summary>
    public interface IInputHandler : IEventSystemHandler
    {
        /// <summary>
        /// Input Down updates from Interactions, Keys, or any other simple UnityEngine.Input.
        /// </summary>
        /// <param name="eventData"></param>
        void OnInputDown(InputEventData eventData);

        /// <summary>
        /// Input Up updates from Interactions, Keys, or any other simple UnityEngine.Input.
        /// </summary>
        /// <param name="eventData"></param>
        void OnInputUp(InputEventData eventData);
    }

    /// <summary>
    /// Interface to implement for more complex generic UnityEngine.Input.
    /// </summary>
    /// <typeparam name="T">The type of input to listen for.</typeparam>
    public interface IInputHandler<T> : IEventSystemHandler
    {
        /// <summary>
        /// Raised input event updates from the type of input specified in the interface handler implementation.
        /// </summary>
        /// <remarks>
        /// The <see cref="InputEventData{T}.InputData"/> is the current input data.
        /// </remarks>
        void OnInputChanged(InputEventData<T> eventData);
    }
}