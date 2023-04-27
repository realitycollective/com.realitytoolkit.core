// Copyright (c) Reality Collective. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using RealityToolkit.EventDatum.Input;
using RealityToolkit.Input.Definitions;
using RealityToolkit.Input.Interfaces;
using RealityToolkit.Input.Interfaces.Controllers.Hands;
using UnityEngine;

namespace RealityToolkit.Utilities.UX.Pointers
{
    /// <summary>
    /// Extends the simple line pointer for drawing lines from the input source origin to the current pointer position,
    /// by adding visual state
    /// </summary>
    public class HandSpatialPointer : LinePointer
    {
        private IPointer nearPointer;
        private IHandController handController;

        [SerializeField]
        private Transform pointerPoseTransform = null;

        [SerializeField]
        [Tooltip("Local offset for the pointer pose transform when not pinching.")]
        private Vector3 offsetStart = Vector3.zero;

        [SerializeField]
        [Tooltip("Local offset for the pointer pose transform when pinching.")]
        private Vector3 offsetEnd = Vector3.zero;

        /// <inheritdoc />
        public override bool IsInteractionEnabled =>
            base.IsInteractionEnabled &&
            (IsNearPointerIdle &&
            HandController != null &&
            HandController.IsPointing) ||
            (IsSelectPressed && Result.CurrentPointerTarget != null);

        /// <summary>
        /// Gets the near pointer attached to the hand.
        /// </summary>
        private IPointer NearPointer => nearPointer ?? (nearPointer = InitializeNearPointerReference());

        /// <summary>
        /// Casted reference to the hand controller driving the pointer.
        /// </summary>
        private IHandController HandController => handController ?? (handController = InitializeHandControllerReference());

        /// <summary>
        /// Is the near pointer in an idle state where it's not
        /// interacting with anything and not targeting anything?
        /// </summary>
        private bool IsNearPointerIdle => NearPointer == null || NearPointer.Result?.CurrentPointerTarget == null || !NearPointer.IsInteractionEnabled;

        private IHandController InitializeHandControllerReference()
        {
            // This pointer type must only be used with hand controllers.
            if (!(Controller is IHandController controller))
            {
                Debug.LogError($"{nameof(HandSpatialPointer)} is only for use with {nameof(IHandController)} controllers!");
                return null;
            }

            return controller;
        }

        private IPointer InitializeNearPointerReference()
        {
            for (int i = 0; i < Controller.InputSource.Pointers.Length; i++)
            {
                var pointer = Controller.InputSource.Pointers[i];

                if (pointer.PointerId != PointerId && pointer is HandNearPointer)
                {
                    return pointer;
                }
            }

            return null;
        }

        /// <inheritdoc />
        public override InteractionMode InteractionMode => InteractionMode.Far;

        /// <inheritdoc />
        public override void OnInputChanged(InputEventData<Pose> eventData)
        {
            base.OnInputChanged(eventData);

            if (IsInteractionEnabled)
            {
                pointerPoseTransform.gameObject.SetActive(true);
                var pinchScale = Mathf.Clamp(1 - HandController.PinchStrength, .5f, 1f);
                pointerPoseTransform.localScale = new Vector3(pinchScale, pinchScale, 1f);
                pointerPoseTransform.localPosition = Vector3.Slerp(offsetStart, offsetEnd, HandController.PinchStrength);
            }
            else
            {
                pointerPoseTransform.gameObject.SetActive(false);
            }
        }
    }
}