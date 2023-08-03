// Copyright (c) Reality Collective. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using RealityToolkit.Input.Hands;
using UnityEngine;

namespace RealityToolkit.Utilities.UX.Pointers
{
    /// <summary>
    /// Hand controller near interaction pointer.
    /// </summary>
    public class HandNearPointer : BaseControllerPointer
    {
        private IHandController handController;

        /// <inheritdoc />
        public override bool IsInteractionEnabled => base.IsInteractionEnabled && !HandController.IsPinching;

        /// <summary>
        /// Casted reference to the hand controller driving the pointer.
        /// </summary>
        private IHandController HandController => handController ?? (handController = InitializeHandControllerReference());

        private IHandController InitializeHandControllerReference()
        {
            // This pointer type must only be used with hand controllers.
            if (!(Controller is IHandController controller))
            {
                Debug.LogError($"{nameof(HandNearPointer)} is only for use with {nameof(IHandController)} controllers!", this);
                return null;
            }

            return controller;
        }
    }
}