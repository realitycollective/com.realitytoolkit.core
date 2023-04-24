// Copyright (c) Reality Collective. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using RealityToolkit.Input.Definitions;
using RealityToolkit.Input.Interfaces;
using RealityToolkit.Input.Interfaces.Controllers;
using UnityEngine.EventSystems;

namespace RealityToolkit.EventDatum.Input
{
    /// <summary>
    /// Describes an source state event that has a source id.
    /// </summary>
    /// <remarks>Source State events do not have an associated <see cref="Definitions.InputSystem.MixedRealityInputAction"/>.</remarks>
    public class SourceStateEventData : BaseInputEventData
    {
        public IController Controller { get; private set; }

        /// <inheritdoc />
        public SourceStateEventData(EventSystem eventSystem) : base(eventSystem) { }

        /// <summary>
        /// Populates the event with data.
        /// </summary>
        /// <param name="inputSource"></param>
        /// <param name="controller"></param>
        public void Initialize(IInputSource inputSource, IController controller)
        {
            // NOTE: Source State events do not have an associated Input Action.
            BaseInitialize(inputSource, InputAction.None);
            Controller = controller;
        }
    }
}
