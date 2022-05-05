﻿// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using RealityToolkit.EventDatum.DiagnosticsSystem;

namespace RealityToolkit.Interfaces.DiagnosticsSystem.Handlers
{
    public interface IMixedRealityConsoleDiagnosticsHandler : IMixedRealityDiagnosticsHandler
    {
        /// <summary>
        /// A new log entry was received.
        /// </summary>
        void OnLogReceived(ConsoleEventData eventData);
    }
}
