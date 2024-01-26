// Copyright (c) Reality Collective. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using RealityToolkit.Input.Controllers;
using RealityToolkit.Input.Interfaces;
using UnityEngine.EventSystems;

namespace RealityToolkit.EventDatum.Input
{
    /// <summary>
    /// Describes a source change event.
    /// </summary>
    /// <remarks>Source State events do not have an associated <see cref="Definitions.Input.InputAction"/>.</remarks>
    public class SourcePoseEventData<T> : SourceStateEventData
    {
        /// <summary>
        /// The new position of the input source.
        /// </summary>
        public T SourceData { get; private set; }

        /// <inheritdoc />
        public SourcePoseEventData(EventSystem eventSystem) : base(eventSystem) { }

        /// <summary>
        /// Populates the event with data.
        /// </summary>
        /// <param name="inputSource"></param>
        /// <param name="controller"></param>
        /// <param name="data"></param>
        public void Initialize(IInputSource inputSource, IController controller, T data)
        {
            Initialize(inputSource, controller);
            SourceData = data;
        }
    }
}