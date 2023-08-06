// Copyright (c) Reality Collective. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using RealityToolkit.Input.Definitions;
using RealityToolkit.Input.Interfaces;
using UnityEvents = UnityEngine.EventSystems;

namespace RealityToolkit.EventDatum.Input
{
    /// <summary>
    /// Describes a pointer event that involves a tap, click, or touch.
    /// </summary>
    public class PointerEventData : BaseInputEventData
    {
        /// <summary>
        /// Pointer for the Input Event
        /// </summary>
        public IPointer Pointer { get; private set; }

        /// <inheritdoc />
        public PointerEventData(UnityEvents.EventSystem eventSystem) : base(eventSystem) { }

        /// <summary>
        /// Used to initialize/reset the event and populate the data.
        /// </summary>
        /// <param name="pointer"></param>
        /// <param name="inputAction"></param>
        /// <param name="inputSource"></param>
        public void Initialize(IPointer pointer, InputAction inputAction, IInputSource inputSource = null)
        {
            BaseInitialize(inputSource ?? pointer.InputSource, inputAction);
            Pointer = pointer;
        }
    }
}
