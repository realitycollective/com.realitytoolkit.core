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
    /// <remarks>
    /// Consider implementing a custom <see cref="BaseInteractionAction"/> instead.
    /// </remarks>
    [DisallowMultipleComponent]
    public class InteractionEventsAction : BaseInteractionAction
    {
        [Space]
        [SerializeField]
        private InteractionEvent activated = null;

        [SerializeField]
        private InteractionExitEvent deactivated = null;

        [Space]
        [SerializeField]
        private InteractionEvent firstFocusEntered = null;

        [SerializeField]
        private InteractionEvent focusEntered = null;

        [SerializeField]
        private InteractionExitEvent focusExited = null;

        [SerializeField]
        private InteractionExitEvent lastFocusExited = null;

        [Space]
        [SerializeField]
        private InteractionEvent firstSelectEntered = null;

        [SerializeField]
        private InteractionEvent selectEntered = null;

        [SerializeField]
        private InteractionExitEvent selectExited = null;

        [SerializeField]
        private InteractionExitEvent lastSelectExited = null;

        [Space]
        [SerializeField]
        private InteractionEvent firstGrabEntered = null;

        [SerializeField]
        private InteractionEvent grabEntered = null;

        [SerializeField]
        private InteractionExitEvent grabExited = null;

        [SerializeField]
        private InteractionExitEvent lastGrabExited = null;

        /// <inheritdoc cref="OnActivated(InteractionEventArgs)"/>
        public InteractionEvent Activated => activated;

        /// <inheritdoc cref="OnDeactivated(InteractionExitEventArgs)"/>
        public InteractionExitEvent Deactivated => deactivated;

        /// <inheritdoc cref="OnFirstFocusEntered(InteractionEventArgs)"/>
        public InteractionEvent FirstFocusEntered => firstFocusEntered;

        /// <inheritdoc cref="OnFocusEntered(InteractionEventArgs)"/>
        public InteractionEvent FocusEntered => focusEntered;

        /// <inheritdoc cref="OnFocusExited(InteractionExitEventArgs)"/>
        public InteractionExitEvent FocusExited => focusExited;

        /// <inheritdoc cref="OnLastFocusExited(InteractionExitEventArgs)"/>
        public InteractionExitEvent LastFocusExited => lastFocusExited;

        /// <inheritdoc cref="OnFirstSelectEntered(InteractionEventArgs)"/>
        public InteractionEvent FirstSelectEntered => firstSelectEntered;

        /// <inheritdoc cref="OnSelectEntered(InteractionEventArgs)"/>
        public InteractionEvent SelectEntered => selectEntered;

        /// <inheritdoc cref="OnSelectExited(InteractionExitEventArgs)"/>
        public InteractionExitEvent SelectExited => selectExited;

        /// <inheritdoc cref="OnLastSelectExited(InteractionExitEventArgs)"/>
        public InteractionExitEvent LastSelectExited => lastSelectExited;

        /// <inheritdoc cref="OnFirstGrabEntered(InteractionEventArgs)"/>
        public InteractionEvent FirstGrabEntered => firstGrabEntered;

        /// <inheritdoc cref="OnGrabEntered(InteractionEventArgs)"/>
        public InteractionEvent GrabEntered => grabEntered;

        /// <inheritdoc cref="OnGrabExited(InteractionExitEventArgs)"/>
        public InteractionExitEvent GrabExited => grabExited;

        /// <inheritdoc cref="OnLastGrabExited(InteractionExitEventArgs)"/>
        public InteractionExitEvent LastGrabExited => lastGrabExited;

        /// <inheritdoc/>
        protected override void OnActivated(InteractionEventArgs eventArgs) => Activated?.Invoke(eventArgs);

        /// <inheritdoc/>
        protected override void OnDeactivated(InteractionExitEventArgs eventArgs) => Deactivated?.Invoke(eventArgs);

        /// <inheritdoc/>
        protected override void OnFirstFocusEntered(InteractionEventArgs eventArgs) => FirstFocusEntered?.Invoke(eventArgs);

        /// <inheritdoc/>
        protected override void OnFocusEntered(InteractionEventArgs eventArgs) => FocusEntered?.Invoke(eventArgs);

        /// <inheritdoc/>
        protected override void OnFocusExited(InteractionExitEventArgs eventArgs) => FocusExited?.Invoke(eventArgs);

        /// <inheritdoc/>
        protected override void OnLastFocusExited(InteractionExitEventArgs eventArgs) => LastFocusExited?.Invoke(eventArgs);

        /// <inheritdoc/>
        protected override void OnFirstSelectEntered(InteractionEventArgs eventArgs) => FirstSelectEntered?.Invoke(eventArgs);

        /// <inheritdoc/>
        protected override void OnSelectEntered(InteractionEventArgs eventArgs) => SelectEntered?.Invoke(eventArgs);

        /// <inheritdoc/>
        protected override void OnSelectExited(InteractionExitEventArgs eventArgs) => SelectExited?.Invoke(eventArgs);

        /// <inheritdoc/>
        protected override void OnLastSelectExited(InteractionExitEventArgs eventArgs) => LastSelectExited?.Invoke(eventArgs);

        /// <inheritdoc/>
        protected override void OnFirstGrabEntered(InteractionEventArgs eventArgs) => FirstGrabEntered?.Invoke(eventArgs);

        /// <inheritdoc/>
        protected override void OnGrabEntered(InteractionEventArgs eventArgs) => GrabEntered?.Invoke(eventArgs);

        /// <inheritdoc/>
        protected override void OnGrabExited(InteractionExitEventArgs eventArgs) => GrabExited?.Invoke(eventArgs);

        /// <inheritdoc/>
        protected override void OnLastGrabExited(InteractionExitEventArgs eventArgs) => LastGrabExited?.Invoke(eventArgs);
    }
}
