// Copyright (c) Reality Collective. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using RealityCollective.Definitions.Utilities;
using RealityCollective.Extensions;
using RealityToolkit.Definitions.Devices;
using RealityToolkit.EventDatum.Input;
using RealityToolkit.Input.Definitions;
using RealityToolkit.Input.Listeners;
using UnityEngine;

namespace RealityToolkit.Input.Controllers
{
    /// <summary>
    /// Waits for a controller to be initialized, then synchronizes its transform position to a specified handedness.
    /// </summary>
    [System.Runtime.InteropServices.Guid("F9E5D87E-78B0-4BD0-AE93-491DFCEE9FA0")]
    public class ControllerPoseSynchronizer : InputServiceGlobalListener, IControllerPoseSynchronizer
    {
        #region IControllerPoseSynchronizer Implementation

        private Transform poseDriver = null;

        /// <inheritdoc />
        public Transform PoseDriver
        {
            get => poseDriver;
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

        /// <summary>
        /// Should this <see cref="GameObject"/> clean itself up when it's controller is lost?
        /// </summary>
        /// <remarks>It's up to the implementation to properly destroy the <see cref="GameObject"/>'s this interface will implement.</remarks>
        public bool DestroyOnSourceLost
        {
            get => destroyOnSourceLost;
            set => destroyOnSourceLost = value;
        }

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

                if (PoseDriver.IsNull())
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
                eventData.Controller.ControllerHandedness == Handedness &&
                destroyOnSourceLost)
            {
                gameObject.Destroy();
            }
        }

        /// <inheritdoc />
        public virtual void OnSourcePoseChanged(SourcePoseEventData<TrackingState> eventData) { }

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
                if (PoseDriver.IsNotNull() &&
                    UseSourcePoseData &&
                    Controller.TrackingState == TrackingState.Tracked)
                {
                    PoseDriver.SetLocalPositionAndRotation(eventData.SourceData.position, eventData.SourceData.rotation);
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
                    PoseAction == eventData.InputAction &&
                    PoseDriver.IsNotNull())
                {
                    PoseDriver.SetLocalPositionAndRotation(eventData.InputData.position, eventData.InputData.rotation);
                }
            }
        }

        #endregion  IInputHandler Implementation
    }
}