﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using RealityToolkit.Definitions.Devices;
using RealityToolkit.Definitions.InputSystem;
using RealityToolkit.Definitions.Utilities;
using RealityToolkit.EventDatum.Input;
using RealityToolkit.Interfaces.InputSystem.Controllers;
using RealityToolkit.Interfaces.InputSystem.Handlers;
using RealityToolkit.Services.InputSystem.Listeners;
using UnityEngine;
using RealityToolkit.Extensions;

namespace RealityToolkit.Services.InputSystem.Utilities
{
    /// <summary>
    /// Waits for a controller to be initialized, then synchronizes its transform position to a specified handedness.
    /// </summary>
    [System.Runtime.InteropServices.Guid("F9E5D87E-78B0-4BD0-AE93-491DFCEE9FA0")]
    public class ControllerPoseSynchronizer : InputSystemGlobalListener, IMixedRealityControllerPoseSynchronizer
    {
        #region IMixedRealityControllerPoseSynchronizer Implementation

        private Transform poseDriver = null;

        /// <inheritdoc />
        public Transform PoseDriver
        {
            get
            {
                try
                {
                    return poseDriver;
                }
                catch
                {
                    return null;
                }
            }
            set => poseDriver = value;
        }

        [SerializeField]
        [Tooltip("The handedness this controller should synchronize with.")]
        private Handedness handedness = Handedness.Left;

        /// <inheritdoc />
        public Handedness Handedness
        {
            get => handedness;
            protected set => handedness = value;
        }

        [SerializeField]
        [Tooltip("Should this GameObject clean itself up when it's controller is lost?")]
        private bool destroyOnSourceLost = true;

        /// <inheritdoc />
        public bool DestroyOnSourceLost
        {
            get => destroyOnSourceLost;
            set => destroyOnSourceLost = value;
        }

        /// <summary>
        /// Is the controller this Synchronizer is registered to currently tracked?
        /// </summary>
        public bool IsTracked { get; protected set; } = false;

        /// <summary>
        /// The current tracking state of the assigned <see cref="IMixedRealityController"/>
        /// </summary>
        protected TrackingState TrackingState { get; set; } = TrackingState.NotTracked;

        private IMixedRealityController controller;

        /// <inheritdoc />
        public virtual IMixedRealityController Controller
        {
            get => controller;
            set
            {
                handedness = value.ControllerHandedness;
                controller = value;
                gameObject.name = $"{handedness}_{gameObject.name}";

                if (PoseDriver == null)
                {
                    PoseDriver = transform;
                }
            }
        }

        [SerializeField]
        [Tooltip("Should the Transform's position be driven from the source pose or from input handler?")]
        private bool useSourcePoseData = true;

        /// <inheritdoc />
        public bool UseSourcePoseData
        {
            get => useSourcePoseData;
            set => useSourcePoseData = value;
        }

        [SerializeField]
        [Tooltip("The input action that will drive the Transform's pose, position, or rotation.")]
        private MixedRealityInputAction poseAction = MixedRealityInputAction.None;

        /// <inheritdoc />
        public MixedRealityInputAction PoseAction
        {
            get => poseAction;
            set => poseAction = value;
        }

        #endregion IMixedRealityControllerPoseSynchronizer Implementation

        #region IMixedRealitySourcePoseHandler Implementation

        /// <inheritdoc />
        public virtual void OnSourceDetected(SourceStateEventData eventData) { }

        /// <inheritdoc />
        public virtual void OnSourceLost(SourceStateEventData eventData)
        {
            if (eventData.SourceId == Controller?.InputSource.SourceId &&
                eventData.Controller.ControllerHandedness == Handedness)
            {
                IsTracked = false;
                TrackingState = TrackingState.NotTracked;

                if (destroyOnSourceLost)
                {
                    gameObject.Destroy();
                }
            }
        }

        /// <inheritdoc />
        public virtual void OnSourcePoseChanged(SourcePoseEventData<TrackingState> eventData)
        {
            if (eventData.SourceId == Controller?.InputSource.SourceId)
            {
                IsTracked = eventData.SourceData == TrackingState.Tracked;
                TrackingState = eventData.SourceData;
            }
        }

        /// <inheritdoc />
        public virtual void OnSourcePoseChanged(SourcePoseEventData<Vector2> eventData) { }

        /// <inheritdoc />
        public virtual void OnSourcePoseChanged(SourcePoseEventData<Vector3> eventData) { }

        /// <inheritdoc />
        public virtual void OnSourcePoseChanged(SourcePoseEventData<Quaternion> eventData) { }

        /// <inheritdoc />
        public virtual void OnSourcePoseChanged(SourcePoseEventData<MixedRealityPose> eventData)
        {
            if (eventData.SourceId == Controller?.InputSource.SourceId)
            {
                if (!PoseDriver.IsNull() &&
                    UseSourcePoseData &&
                    TrackingState == TrackingState.Tracked)
                {
                    PoseDriver.localPosition = eventData.SourceData.Position;
                    PoseDriver.localRotation = eventData.SourceData.Rotation;
                }
            }
        }

        #endregion IMixedRealitySourcePoseHandler Implementation

        #region IMixedRealityInputHandler Implementation

        /// <inheritdoc />
        public virtual void OnInputUp(InputEventData eventData) { }

        /// <inheritdoc />
        public virtual void OnInputDown(InputEventData eventData) { }

        /// <inheritdoc />
        public virtual void OnInputChanged(InputEventData<float> eventData) { }

        /// <inheritdoc />
        public virtual void OnInputChanged(InputEventData<Vector2> eventData) { }

        /// <inheritdoc />
        public virtual void OnInputChanged(InputEventData<Vector3> eventData)
        {
            if (eventData.SourceId == Controller?.InputSource.SourceId)
            {
                if (!UseSourcePoseData &&
                    PoseAction == eventData.MixedRealityInputAction)
                {
                    IsTracked = true;
                    TrackingState = TrackingState.Tracked;
                    transform.localPosition = eventData.InputData;
                }
            }
        }

        /// <inheritdoc />
        public virtual void OnInputChanged(InputEventData<Quaternion> eventData)
        {
            if (eventData.SourceId == Controller?.InputSource.SourceId)
            {
                if (!UseSourcePoseData &&
                    PoseAction == eventData.MixedRealityInputAction)
                {
                    IsTracked = true;
                    TrackingState = TrackingState.Tracked;
                    transform.localRotation = eventData.InputData;
                }
            }
        }

        /// <inheritdoc />
        public virtual void OnInputChanged(InputEventData<MixedRealityPose> eventData)
        {
            if (eventData.SourceId == Controller?.InputSource.SourceId)
            {
                if (!UseSourcePoseData &&
                    PoseAction == eventData.MixedRealityInputAction)
                {
                    IsTracked = true;
                    TrackingState = TrackingState.Tracked;

                    if (PoseDriver != null)
                    {
                        PoseDriver.localPosition = eventData.InputData.Position;
                        PoseDriver.localRotation = eventData.InputData.Rotation;
                    }
                }
            }
        }

        #endregion  IMixedRealityInputHandler Implementation
    }
}