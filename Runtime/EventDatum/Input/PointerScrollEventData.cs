// Copyright (c) Reality Collective. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using RealityToolkit.Input.Definitions;
using RealityToolkit.Input.Interfaces;
using UnityEngine;
using UnityEngine.EventSystems;

namespace RealityToolkit.EventDatum.Input
{
    /// <summary>
    /// Describes a <see cref="PointerEventData"/> with scroll data.
    /// </summary>
    public class PointerScrollEventData : PointerEventData
    {
        /// <summary>
        /// The distance this pointer has been scrolled since the last event was raised.
        /// </summary>
        public Vector2 ScrollDelta { get; private set; }

        /// <inheritdoc />
        public PointerScrollEventData(EventSystem eventSystem) : base(eventSystem) { }

        /// <summary>
        /// Used to initialize/reset the event and populate the data.
        /// </summary>
        /// <param name="pointer"></param>
        /// <param name="inputAction"></param>
        /// <param name="scrollDelta"></param>
        /// <param name="inputSource"></param>
        public void Initialize(IPointer pointer, InputAction inputAction, Vector2 scrollDelta, IInputSource inputSource = null)
        {
            Initialize(pointer, inputAction, inputSource);
            ScrollDelta = scrollDelta;
        }
    }
}