// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using RealityCollective.Extensions;
using RealityToolkit.Definitions.Utilities;
using RealityToolkit.EventDatum.Input;
using RealityToolkit.Interfaces.InputSystem.Controllers.Hands;
using RealityToolkit.Interfaces.InputSystem.Handlers;
using RealityToolkit.Interfaces.InputSystem.Providers.Controllers.Hands;
using UnityEngine;

namespace RealityToolkit.Utilities.UX.Controllers.Hands
{
    /// <summary>
    /// Base hand controller visualizer implementation.
    /// </summary>
    [System.Runtime.InteropServices.Guid("5d844e0b-f913-46b8-bc3b-fa6429e62c60")]
    public class DefaultHandControllerVisualizer : HandControllerVisualizer, IMixedRealityControllerVisualizer
    {
        /// <summary>
        /// If using physics with hand, the actual hand visualization is done
        /// on a companion game object which is connected to the <see cref="GameObject"/>
        /// using a <see cref="FixedJoint"/>. For physics to work properly while maintaining
        /// the platforms controller tracking we cannot attach colliders and a rigidbody to the
        /// <see cref="GameObject"/> since that would cause crazy behaviour on controller movement.
        /// </summary>
        private GameObject PhysicsCompanionGameObject { get; set; }

        private IMixedRealityHandControllerDataProvider handControllerDataProvider;

        /// <summary>
        /// The active hand controller data provider.
        /// </summary>
        protected IMixedRealityHandControllerDataProvider HandControllerDataProvider => handControllerDataProvider ?? (handControllerDataProvider = (IMixedRealityHandControllerDataProvider)Controller.ControllerDataProvider);

        /// <inheritdoc />
        protected override void OnDestroy()
        {
            // In case physics are enabled we need to take destroy the
            // physics game object as well when destroying the hand visualizer.
            if (PhysicsCompanionGameObject != null)
            {
                PhysicsCompanionGameObject.Destroy();
            }

            base.OnDestroy();
        }

        /// <inheritdoc />
        public override void OnInputChanged(InputEventData<MixedRealityPose> eventData)
        {
            if (eventData.Handedness == Controller.ControllerHandedness)
            {
                var handController = (IMixedRealityHandController)Controller;
                TrackingState = handController.TrackingState;

                if (TrackingState == Definitions.Devices.TrackingState.Tracked)
                {
                    // It's important to update physics
                    // configuration first.
                    UpdatePhysicsConfiguration();
                }
            }

            base.OnInputChanged(eventData);
        }

        #region Hand Colliders / Physics

        private void UpdatePhysicsConfiguration()
        {
            if (HandControllerDataProvider.HandPhysicsEnabled)
            {
                // If we are using hand physics, we need to make sure
                // the physics companion is setup properly.
                if (PhysicsCompanionGameObject != null)
                {
                    PhysicsCompanionGameObject.SetActive(true);
                    PhysicsCompanionGameObject.transform.localPosition = GameObject.transform.localPosition;
                    PhysicsCompanionGameObject.transform.localRotation = GameObject.transform.localRotation;
                    return;
                }

                PhysicsCompanionGameObject = new GameObject($"{GameObject.name}_Physics");
                PhysicsCompanionGameObject.transform.SetParent(GameObject.transform.parent, false);
                PhysicsCompanionGameObject.transform.localPosition = GameObject.transform.localPosition;
                PhysicsCompanionGameObject.transform.localRotation = GameObject.transform.localRotation;

                // Setup the kinematic rigidbody on the actual controller game object.
                Rigidbody controllerRigidbody = GameObject.EnsureComponent<Rigidbody>();
                controllerRigidbody.mass = .46f; // 0.46 Kg average human hand weight
                controllerRigidbody.isKinematic = true;
                controllerRigidbody.useGravity = false;
                controllerRigidbody.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;

                // Make the physics proxy a fixed joint rigidbody to the controller
                // and give it an adamantium coated connection so it doesn't break.
                Rigidbody physicsRigidbody = PhysicsCompanionGameObject.EnsureComponent<Rigidbody>();
                physicsRigidbody.mass = .46f; // 0.46 Kg average human hand weight
                physicsRigidbody.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
                FixedJoint fixedJoint = PhysicsCompanionGameObject.EnsureComponent<FixedJoint>();
                fixedJoint.connectedBody = controllerRigidbody;
                fixedJoint.breakForce = float.MaxValue;
                fixedJoint.breakTorque = float.MaxValue;
            }
            else if (PhysicsCompanionGameObject != null)
            {
                PhysicsCompanionGameObject.SetActive(false);
            }
        }

        #endregion
    }
}
