// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using RealityToolkit.Definitions.InputSystem;
using RealityToolkit.Definitions.LocomotionSystem;
using RealityCollective.Definitions.Utilities;
using RealityToolkit.EventDatum.Input;
using RealityToolkit.Interfaces.CameraSystem;
using RealityToolkit.Interfaces.LocomotionSystem;
using RealityToolkit.Utilities;
using UnityEngine;
using RealityCollective.Extensions;

namespace RealityToolkit.Services.LocomotionSystem.Providers
{
    public abstract class BaseLocomotionProvider : BaseDataProvider, ILocomotionProvider
    {
        /// <inheritdoc />
        public BaseLocomotionProvider(string name, uint priority, BaseLocomotionProviderProfile profile, ILocomotionSystem parentService)
            : base(name, priority, profile, parentService)
        {
            startupBehaviour = profile.StartupBehaviour;
            InputAction = profile.InputAction;
        }

        private readonly AutoStartBehavior startupBehaviour;
        private bool isInitialized;

        /// <inheritdoc />
        public MixedRealityInputAction InputAction { get; }

        /// <inheritdoc />
        public bool IsEnabled { get; private set; }

        /// <summary>
        /// Gets the active <see cref="Services.LocomotionSystem.LocomotionSystem"/> instance.
        /// </summary>
        protected virtual Services.LocomotionSystem.LocomotionSystem LocomotionSystem => (Services.LocomotionSystem.LocomotionSystem)ParentService;

        /// <summary>
        /// Gets the player camera <see cref="Transform"/>.
        /// </summary>
        protected virtual Transform CameraTransform
        {
            get
            {
                return MixedRealityToolkit.TryGetSystem<IMixedRealityCameraSystem>(out var cameraSystem)
                    ? cameraSystem.MainCameraRig.CameraTransform
                    : CameraCache.Main.transform;
            }
        }

        /// <summary>
        /// Gets the target <see cref="Transform"/> for locomotion.
        /// </summary>
        protected virtual Transform LocomotionTargetTransform
        {
            get
            {
                if (LocomotionSystem.LocomotionTargetOverride.IsNull() ||
                    !LocomotionSystem.LocomotionTargetOverride.enabled)
                {
                    if (Debug.isDebugBuild)
                    {
                        Debug.Assert(!CameraTransform.parent.IsNull(), $"The {nameof(Services.LocomotionSystem.LocomotionSystem)} expects the camera to be parented under another transform!");
                    }

                    return CameraTransform.parent;
                }

                return LocomotionSystem.LocomotionTargetOverride.transform;
            }
        }

        /// <inheritdoc />
        public override void Enable()
        {
            base.Enable();

            if (IsEnabled)
            {
                return;
            }

            if (startupBehaviour == AutoStartBehavior.AutoStart || isInitialized)
            {
                IsEnabled = true;
                LocomotionSystem.OnLocomotionProviderEnabled(this);
            }
            else
            {
                Disable();
            }

            isInitialized = true;
        }

        /// <inheritdoc />
        public override void Disable()
        {
            base.Disable();

            if (!IsEnabled)
            {
                return;
            }

            IsEnabled = false;
            LocomotionSystem.OnLocomotionProviderDisabled(this);
        }

        /// <inheritdoc />
        public virtual void OnTeleportTargetRequested(LocomotionEventData eventData) { }

        /// <inheritdoc />
        public virtual void OnTeleportStarted(LocomotionEventData eventData) { }

        /// <inheritdoc />
        public virtual void OnTeleportCompleted(LocomotionEventData eventData) { }

        /// <inheritdoc />
        public virtual void OnTeleportCanceled(LocomotionEventData eventData) { }

        /// <inheritdoc />
        public virtual void OnInputChanged(InputEventData<float> eventData) { }

        /// <inheritdoc />
        public virtual void OnInputChanged(InputEventData<Vector2> eventData) { }

        /// <inheritdoc />
        public virtual void OnInputDown(InputEventData eventData) { }

        /// <inheritdoc />
        public virtual void OnInputUp(InputEventData eventData) { }
    }
}
