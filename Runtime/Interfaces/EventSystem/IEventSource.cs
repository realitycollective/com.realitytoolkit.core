﻿// Copyright (c) Reality Collective. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Collections;

namespace RealityToolkit.Interfaces.Events
{
    /// <summary>
    /// Interface to implement an event source.
    /// </summary>
    public interface IEventSource : IEqualityComparer
    {
        /// <summary>
        /// The Unique Source Id of this Event Source.
        /// </summary>
        uint SourceId { get; }

        /// <summary>
        /// The Name of this Event Source.
        /// </summary>
        string SourceName { get; }
    }
}