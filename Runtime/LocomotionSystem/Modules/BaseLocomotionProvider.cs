// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using RealityCollective.Definitions.Utilities;
using RealityCollective.Extensions;
using RealityCollective.ServiceFramework.Modules;
using RealityCollective.ServiceFramework.Services;
using RealityToolkit.CameraService.Interfaces;
using RealityToolkit.EventDatum.Input;
using RealityToolkit.InputSystem.Definitions;
using RealityToolkit.LocomotionSystem.Definitions;
using RealityToolkit.LocomotionSystem.Interfaces;
using RealityToolkit.Utilities;
using UnityEngine;

namespace RealityToolkit.LocomotionSystem.Modules
{
    public abstract class BaseLocomotionProvider : BaseServiceModule, ILocomotionProvider
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
        public bool IsActive { get; protected set; }

        /// <inheritdoc />
        public MixedRealityInputAction InputAction { get; }

        /// <summary>
        /// Gets the active <see cref="Services.LocomotionSystem.LocomotionSystem"/> instance.
        /// </summary>
        protected virtual LocomotionSystem LocomotionSystem => (LocomotionSystem)ParentService;

        /// <summary>
        /// Gets the player camera <see cref="Transform"/>.
        /// </summary>
        protected virtual Transform CameraTransform
        {
            get
            {
                return ServiceManager.Instance.TryGetService<ICameraService>(out var cameraSystem)
                    ? cameraSystem.CameraRig.CameraTransform
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
                        Debug.Assert(!CameraTransform.parent.IsNull(), $"The {nameof(LocomotionSystem)} expects the camera to be parented under another transform!");
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

            if (IsActive)
            {
                return;
            }

            if (startupBehaviour == AutoStartBehavior.AutoStart || isInitialized)
            {
                IsActive = true;
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

            if (!IsActive)
            {
                return;
            }

            IsActive = false;
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
