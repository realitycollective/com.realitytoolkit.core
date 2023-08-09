// Copyright (c) Reality Collective. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using RealityToolkit.Input.Interactions.Interactors;
using RealityToolkit.Interfaces.Events;

namespace RealityToolkit.Input.Interfaces
{
    /// <summary>
    /// Interface for an input source.
    /// An input source is the origin of user input and generally comes from a physical controller, sensor, or other hardware device.
    /// </summary>
    public interface IInputSource : IEventSource
    {
        /// <summary>
        /// Array of pointers associated with this input source.
        /// </summary>
        IPointer[] Pointers { get; }
    }
}
