﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using RealityToolkit.Definitions.Devices;
using RealityToolkit.Definitions.Utilities;
using RealityToolkit.EventDatum.Input;
using UnityEngine;

namespace RealityToolkit.InputSystem.Interfaces.Handlers
{
    /// <summary>
    /// Interface to implement to react to source 
    /// </summary>
    public interface IMixedRealitySourcePoseHandler : IMixedRealitySourceStateHandler
    {
        /// <summary>
        /// Raised when the source pose tracking state is changed.
        /// </summary>
        /// <param name="eventData"></param>
        void OnSourcePoseChanged(SourcePoseEventData<TrackingState> eventData);

        /// <summary>
        /// Raised when the source position is changed.
        /// </summary>
        void OnSourcePoseChanged(SourcePoseEventData<Vector2> eventData);

        /// <summary>
        /// Raised when the source position is changed.
        /// </summary>
        void OnSourcePoseChanged(SourcePoseEventData<Vector3> eventData);

        /// <summary>
        /// Raised when the source rotation is changed.
        /// </summary>
        void OnSourcePoseChanged(SourcePoseEventData<Quaternion> eventData);

        /// <summary>
        /// Raised when the source pose is changed.
        /// </summary>
        void OnSourcePoseChanged(SourcePoseEventData<MixedRealityPose> eventData);
    }
}