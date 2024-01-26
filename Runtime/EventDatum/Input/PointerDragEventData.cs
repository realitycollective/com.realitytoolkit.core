// Copyright (c) Reality Collective. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using RealityToolkit.Input.Definitions;
using RealityToolkit.Input.Interactors;
using RealityToolkit.Input.Interfaces;
using UnityEngine;
using UnityEngine.EventSystems;

namespace RealityToolkit.EventDatum.Input
{
    /// <summary>
    /// Describes a <see cref="PointerEventData"/> with dragging data.
    /// </summary>
    public class PointerDragEventData : PointerEventData
    {
        /// <summary>
        /// The distance this pointer has been dragged since the last event was raised.
        /// </summary>
        public Vector3 DragDelta { get; private set; }

        /// <inheritdoc />
        public PointerDragEventData(EventSystem eventSystem) : base(eventSystem) { }

        /// <summary>
        /// Used to initialize/reset the event and populate the data.
        /// </summary>
        /// <param name="pointer"></param>
        /// <param name="inputAction"></param>
        /// <param name="dragDelta"></param>
        /// <param name="inputSource"></param>
        public void Initialize(IInteractor pointer, InputAction inputAction, Vector3 dragDelta, IInputSource inputSource = null)
        {
            Initialize(pointer, inputAction, inputSource);
            DragDelta = dragDelta;
        }
    }
}