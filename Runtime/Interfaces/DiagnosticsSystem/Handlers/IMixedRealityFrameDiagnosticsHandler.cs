﻿// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using RealityToolkit.EventDatum.DiagnosticsSystem;

namespace RealityToolkit.Interfaces.DiagnosticsSystem.Handlers
{
    public interface IMixedRealityFrameDiagnosticsHandler : IMixedRealityDiagnosticsHandler
    {
        /// <summary>
        /// Raised when the <see cref="IMixedRealityDiagnosticsSystem"/> frame rate has changed.
        /// </summary>
        void OnFrameRateChanged(FrameEventData eventData);

        /// <summary>
        /// A frame target was missed.
        /// </summary>
        void OnFrameMissed(FrameEventData eventData);
    }
}
