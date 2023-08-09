// Copyright (c) Reality Collective. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using RealityToolkit.Input.Interactions.Interactors;
using UnityEngine;
using UnityEngine.EventSystems;

namespace RealityToolkit.EventDatum.Input
{
    /// <summary>
    /// Describes an Input Event associated with a specific pointer's focus state change.
    /// </summary>
    public class FocusEventData : BaseEventData
    {
        /// <summary>
        /// The pointer associated with this event.
        /// </summary>
        public IInteractor Pointer { get; private set; }

        /// <summary>
        /// The old focused object.
        /// </summary>
        public GameObject OldFocusedObject { get; private set; }

        /// <summary>
        /// The new focused object.
        /// </summary>
        public GameObject NewFocusedObject { get; private set; }

        /// <inheritdoc />
        public FocusEventData(EventSystem eventSystem) : base(eventSystem) { }

        /// <summary>
        /// Used to initialize/reset the event and populate the data.
        /// </summary>
        /// <param name="pointer"></param>
        public void Initialize(IInteractor pointer)
        {
            Reset();
            Pointer = pointer;
        }

        /// <summary>
        /// Used to initialize/reset the event and populate the data.
        /// </summary>
        /// <param name="pointer"></param>
        /// <param name="oldFocusedObject"></param>
        /// <param name="newFocusedObject"></param>
        public void Initialize(IInteractor pointer, GameObject oldFocusedObject, GameObject newFocusedObject)
        {
            Reset();
            Pointer = pointer;
            OldFocusedObject = oldFocusedObject;
            NewFocusedObject = newFocusedObject;
        }
    }
}

