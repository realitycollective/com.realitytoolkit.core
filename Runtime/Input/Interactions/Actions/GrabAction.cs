// Copyright (c) Reality Collective. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace RealityToolkit.Input.Interactions.Actions
{
    public class GrabAction : Action
    {
        /// <inheritdoc/>
        public override void OnStateChanged(InteractionState state)
        {
            if (!Interactable.IsValid)
            {
                return;
            }
        }
    }
}
