// Copyright (c) Reality Collective. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using RealityCollective.Definitions.Utilities;
using RealityCollective.Extensions;
using RealityToolkit.Definitions.Devices;
using RealityToolkit.EventDatum.Input;
using RealityToolkit.Input.Definitions;
using RealityToolkit.Input.Interfaces.Controllers;
using RealityToolkit.Input.Interfaces.Handlers;
using RealityToolkit.Input.Listeners;
using UnityEngine;

namespace RealityToolkit.Services.Input.Utilities
{
    /// <summary>
    /// Waits for a controller to be initialized, then synchronizes its transform position to a specified handedness.
    /// </summary>
    [System.Runtime.InteropServices.Guid("F9E5D87E-78B0-4BD0-AE93-491DFCEE9FA0")]
    public class ControllerPoseSynchronizer : InputSystemGlobalListener, IControllerPoseSynchronizer
    {
        #region IControllerPoseSynchronizer Implementation

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
        /// The current tracking state of the assigned <see cref="IController"/>
        /// </summary>
        protected TrackingState TrackingState { get; set; } = TrackingState.NotTracked;

        private IController controller;

        /// <inheritdoc />
        public virtual IController Controller
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
        private InputAction poseAction = InputAction.None;

        /// <inheritdoc />
        public InputAction PoseAction
        {
            get => poseAction;
            set => poseAction = value;
        }

        #endregion IControllerPoseSynchronizer Implementation

        #region ISourcePoseHandler Implementation

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
        public virtual void OnSourcePoseChanged(SourcePoseEventData<Pose> eventData)
        {
            if (eventData.SourceId == Controller?.InputSource.SourceId)
            {
                if (!PoseDriver.IsNull() &&
                    UseSourcePoseData &&
                    TrackingState == TrackingState.Tracked)
                {
                    PoseDriver.localPosition = eventData.SourceData.position;
                    PoseDriver.localRotation = eventData.SourceData.rotation;
                }
            }
        }

        #endregion ISourcePoseHandler Implementation

        #region IInputHandler Implementation

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
                    PoseAction == eventData.InputAction)
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
                    PoseAction == eventData.InputAction)
                {
                    IsTracked = true;
                    TrackingState = TrackingState.Tracked;
                    transform.localRotation = eventData.InputData;
                }
            }
        }

        /// <inheritdoc />
        public virtual void OnInputChanged(InputEventData<Pose> eventData)
        {
            if (eventData.SourceId == Controller?.InputSource.SourceId)
            {
                if (!UseSourcePoseData &&
                    PoseAction == eventData.InputAction)
                {
                    IsTracked = true;
                    TrackingState = TrackingState.Tracked;

                    if (PoseDriver != null)
                    {
                        PoseDriver.localPosition = eventData.InputData.position;
                        PoseDriver.localRotation = eventData.InputData.rotation;
                    }
                }
            }
        }

        #endregion  IInputHandler Implementation
    }
}