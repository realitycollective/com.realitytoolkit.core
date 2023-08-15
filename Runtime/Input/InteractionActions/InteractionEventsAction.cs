// Copyright (c) Reality Collective. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using RealityToolkit.Input.Events;
using UnityEngine;

namespace RealityToolkit.Input.InteractionActions
{
    /// <summary>
    /// A utility <see cref="IInteractionAction"/> that provides events for each kind of
    /// interaction on the <see cref="Interactables.IInteractable"/> that you can use to hook up
    /// events in the inspector or via code.
    /// </summary>
    [DisallowMultipleComponent]
    public class InteractionEventsAction : BaseInteractionAction
    {
        [Space]
        [SerializeField]
        private InteractionEvent FirstFocusEntered = null;

        [SerializeField]
        private InteractionEvent FocusEntered = null;

        [SerializeField]
        private InteractionExitEvent FocusExited = null;

        [SerializeField]
        private InteractionExitEvent LastFocusExited = null;

        [Space]
        [SerializeField]
        private InteractionEvent FirstSelectEntered = null;

        [SerializeField]
        private InteractionEvent SelectEntered = null;

        [SerializeField]
        private InteractionExitEvent SelectExited = null;

        [SerializeField]
        private InteractionExitEvent LastSelectExited = null;

        [Space]
        [SerializeField]
        private InteractionEvent FirstGrabEntered = null;

        [SerializeField]
        private InteractionEvent GrabEntered = null;

        [SerializeField]
        private InteractionExitEvent GrabExited = null;

        [SerializeField]
        private InteractionExitEvent LastGrabExited = null;

        /// <inheritdoc/>
        public override void OnFirstFocusEntered(InteractionEventArgs eventArgs) => FirstFocusEntered?.Invoke(eventArgs);

        /// <inheritdoc/>
        public override void OnFocusEntered(InteractionEventArgs eventArgs) => FocusEntered?.Invoke(eventArgs);

        /// <inheritdoc/>
        public override void OnFocusExited(InteractionExitEventArgs eventArgs) => FocusExited?.Invoke(eventArgs);

        /// <inheritdoc/>
        public override void OnLastFocusExited(InteractionExitEventArgs eventArgs) => LastFocusExited?.Invoke(eventArgs);

        /// <inheritdoc/>
        public override void OnFirstSelectEntered(InteractionEventArgs eventArgs) => FirstSelectEntered?.Invoke(eventArgs);

        /// <inheritdoc/>
        public override void OnSelectEntered(InteractionEventArgs eventArgs) => SelectEntered?.Invoke(eventArgs);

        /// <inheritdoc/>
        public override void OnSelectExited(InteractionExitEventArgs eventArgs) => SelectExited?.Invoke(eventArgs);

        /// <inheritdoc/>
        public override void OnLastSelectExited(InteractionExitEventArgs eventArgs) => LastSelectExited?.Invoke(eventArgs);

        /// <inheritdoc/>
        public override void OnFirstGrabEntered(InteractionEventArgs eventArgs) => FirstGrabEntered?.Invoke(eventArgs);

        /// <inheritdoc/>
        public override void OnGrabEntered(InteractionEventArgs eventArgs) => GrabEntered?.Invoke(eventArgs);

        /// <inheritdoc/>
        public override void OnGrabExited(InteractionExitEventArgs eventArgs) => GrabExited?.Invoke(eventArgs);

        /// <inheritdoc/>
        public override void OnLastGrabExited(InteractionExitEventArgs eventArgs) => LastGrabExited?.Invoke(eventArgs);
    }
}
