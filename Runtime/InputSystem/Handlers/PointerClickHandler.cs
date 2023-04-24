// Copyright (c) Reality Collective. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using RealityToolkit.EventDatum.Input;
using RealityToolkit.Input.Definitions;
using RealityToolkit.Input.Interfaces.Handlers;
using UnityEngine;

namespace RealityToolkit.Input.Handlers
{
    /// <summary>
    /// This component handles pointer clicks from all types of input sources.<para/>
    /// i.e. a primary mouse button click, motion controller selection press, or hand tap.
    /// </summary>
    public class PointerClickHandler : BaseInputHandler, IPointerHandler
    {
        [SerializeField]
        [Tooltip("The input actions to be recognized on pointer up.")]
        private InputActionEventPair onPointerUpActionEvent = null;

        [SerializeField]
        [Tooltip("The input actions to be recognized on pointer down.")]
        private InputActionEventPair onPointerDownActionEvent = null;

        [SerializeField]
        [Tooltip("The input actions to be recognized on pointer clicked.")]
        private InputActionEventPair onPointerClickedActionEvent = null;

        #region IMixedRealityPointerHandler Implementation

        /// <inheritdoc />
        public void OnPointerDown(PointerEventData eventData)
        {
            if (onPointerDownActionEvent.InputAction == InputAction.None) { return; }

            if (onPointerDownActionEvent.InputAction == eventData.InputAction)
            {
                onPointerDownActionEvent.UnityEvent.Invoke();
            }
        }

        /// <inheritdoc />
        public void OnPointerUp(PointerEventData eventData)
        {
            if (onPointerUpActionEvent.InputAction == InputAction.None) { return; }

            if (onPointerUpActionEvent.InputAction == eventData.InputAction)
            {
                onPointerUpActionEvent.UnityEvent.Invoke();
            }
        }

        /// <inheritdoc />
        public void OnPointerClicked(PointerEventData eventData)
        {
            if (onPointerClickedActionEvent.InputAction == InputAction.None) { return; }

            if (onPointerClickedActionEvent.InputAction == eventData.InputAction)
            {
                onPointerClickedActionEvent.UnityEvent.Invoke();
            }
        }

        #endregion IMixedRealityPointerHandler Implementation
    }
}
