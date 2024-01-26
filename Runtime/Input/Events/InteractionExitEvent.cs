// Copyright (c) Reality Collective. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using UnityEngine.Events;

namespace RealityToolkit.Input.Events
{
    /// <summary>
    /// <see cref="UnityEvent"/> invoked when an interaction ends.
    /// </summary>
    [Serializable]
    public class InteractionExitEvent : UnityEvent<InteractionExitEventArgs> { }
}
