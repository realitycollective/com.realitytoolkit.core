﻿// Copyright (c) Reality Collective. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using RealityCollective.Extensions;
using RealityCollective.ServiceFramework.Modules;
using RealityToolkit.CameraService.Definitions;
using RealityToolkit.CameraService.Interfaces;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

namespace RealityToolkit.CameraService.Modules
{
    /// <summary>
    /// Base class for <see cref="ICameraServiceModule"/>s to inherit from.
    /// </summary>
    public abstract class BaseCameraServiceModule : BaseServiceModule, ICameraServiceModule
    {
        /// <inheritdoc />
        public BaseCameraServiceModule(string name, uint priority, BaseCameraServiceModuleProfile profile, ICameraService parentService)
            : base(name, priority, profile, parentService)
        {
            cameraSystem = parentService;

            if (profile.CameraRigType?.Type == null)
            {
                throw new Exception($"{nameof(profile.CameraRigType)} cannot be null!");
            }

            rigPrefab = profile.RigPrefab;
            eyeTextureResolution = profile.EyeTextureResolution;
            isCameraPersistent = profile.IsCameraPersistent;
            cameraRigType = profile.CameraRigType.Type;
            applyQualitySettings = profile.ApplyQualitySettings;

            TrackingType = profile.TrackingType;

            trackingOriginMode = profile.TrackingOriginMode;
            defaultHeadHeight = profile.DefaultHeadHeight;

            nearClipPlaneOpaqueDisplay = profile.NearClipPlaneOpaqueDisplay;
            cameraClearFlagsOpaqueDisplay = profile.CameraClearFlagsOpaqueDisplay;
            backgroundColorOpaqueDisplay = profile.BackgroundColorOpaqueDisplay;
            opaqueQualityLevel = profile.OpaqueQualityLevel;

            nearClipPlaneTransparentDisplay = profile.NearClipPlaneTransparentDisplay;
            cameraClearFlagsTransparentDisplay = profile.CameraClearFlagsTransparentDisplay;
            backgroundColorTransparentDisplay = profile.BackgroundColorTransparentDisplay;
            transparentQualityLevel = profile.TransparentQualityLevel;

            bodyAdjustmentAngle = profile.BodyAdjustmentAngle;
            bodyAdjustmentSpeed = profile.BodyAdjustmentSpeed;
        }

        private readonly ICameraService cameraSystem;
        private readonly GameObject rigPrefab;
        private readonly float eyeTextureResolution;
        private readonly bool isCameraPersistent;
        private readonly Type cameraRigType;
        private readonly bool applyQualitySettings;
        private readonly float nearClipPlaneTransparentDisplay;
        private readonly CameraClearFlags cameraClearFlagsTransparentDisplay;
        private readonly Color backgroundColorTransparentDisplay;
        private readonly int transparentQualityLevel;
        private readonly float nearClipPlaneOpaqueDisplay;
        private readonly CameraClearFlags cameraClearFlagsOpaqueDisplay;
        private readonly Color backgroundColorOpaqueDisplay;
        private readonly int opaqueQualityLevel;
        private readonly float bodyAdjustmentSpeed;
        private readonly double bodyAdjustmentAngle;
        private bool cameraOpaqueLastFrame;
        private static List<XRInputSubsystem> inputSubsystems = new List<XRInputSubsystem>();
        private TrackingOriginModeFlags trackingOriginMode;
        private readonly float defaultHeadHeight;
        private bool trackingOriginInitialized = false;
        private bool trackingOriginInitializing = false;

        /// <inheritdoc />
        public virtual bool IsOpaque
        {
            get
            {
                if (cameraSystem.DisplaySubsystem == null)
                {
                    // When no device is attached we are assuming the display
                    // device is the computer's display, which should be opaque.
                    return true;
                }

                return cameraSystem.DisplaySubsystem.displayOpaque;
            }
        }

        /// <inheritdoc />
        public virtual bool IsStereoscopic => CameraRig.PlayerCamera.stereoEnabled;

        /// <inheritdoc />
        public ICameraRig CameraRig { get; private set; }

        /// <inheritdoc />
        public TrackingType TrackingType { get; }

        /// <inheritdoc />
        public virtual float HeadHeight => CameraRig.CameraTransform.localPosition.y;

        #region IMixedRealitySerivce Implementation

        /// <inheritdoc />
        public override uint Priority => 0;

        /// <inheritdoc />
        public override void Initialize()
        {
            base.Initialize();

            EnsureCameraRigSetup(true);

            if (!Application.isPlaying)
            {
                return;
            }

            // We attempt to initialize the camera tracking origin, which might
            // fail at this point if the subsytems are not ready, in which case,
            // we set a flag to keep trying.
            trackingOriginInitialized = SetupTrackingOrigin();
            trackingOriginInitializing = !trackingOriginInitialized;

            cameraOpaqueLastFrame = IsOpaque;

            if (applyQualitySettings)
            {
                if (IsOpaque)
                {
                    ApplySettingsForOpaqueDisplay();
                }
                else
                {
                    ApplySettingsForTransparentDisplay();
                }
            }

            if (Application.isPlaying)
            {
                XRSettings.eyeTextureResolutionScale = eyeTextureResolution;
            }
        }

        /// <inheritdoc />
        public override void Enable()
        {
            base.Enable();

            EnsureCameraRigSetup(false);

            if (Application.isPlaying &&
                isCameraPersistent)
            {
                CameraRig.PlayerCamera.transform.root.DontDestroyOnLoad();
            }
        }

        /// <inheritdoc />
        public override void Update()
        {
            base.Update();

            if (!Application.isPlaying)
            {
                return;
            }

            if (cameraOpaqueLastFrame != IsOpaque)
            {
                cameraOpaqueLastFrame = IsOpaque;

                if (applyQualitySettings)
                {
                    if (IsOpaque)
                    {
                        ApplySettingsForOpaqueDisplay();
                    }
                    else
                    {
                        ApplySettingsForTransparentDisplay();
                    }
                }
            }

            // We keep trying to initialize the tracking origin,
            // until it worked, because at application launch the
            // subsytems might not be ready yet.
            if (trackingOriginInitializing && !trackingOriginInitialized)
            {
                trackingOriginInitialized = SetupTrackingOrigin();
                trackingOriginInitializing = !trackingOriginInitialized;
            }
        }

        /// <inheritdoc />
        public override void LateUpdate()
        {
            base.LateUpdate();

            if (!Application.isPlaying) { return; }

            SyncRigTransforms();
        }

        /// <inheritdoc />
        public override void Disable()
        {
            base.Disable();

            if (CameraRig == null ||
                CameraRig.GameObject.IsNull())
            {
                return;
            }

            ResetRigTransforms();

            if (!CameraRig.PlayerCamera.IsNull() &&
                !CameraRig.PlayerCamera.transform.IsNull())
            {
                var cameraTransform = CameraRig.PlayerCamera.transform;
                cameraTransform.SetParent(null);
                cameraTransform.position = Vector3.one;
                cameraTransform.rotation = Quaternion.identity;
            }

            if (CameraRig.RigTransform != null)
            {
                CameraRig.RigTransform.gameObject.Destroy();
            }

            if (CameraRig is Component component &&
                component is ICameraRig)
            {
                component.Destroy();
            }
        }

        #endregion IMixedRealitySerivce Implementation

        private void EnsureCameraRigSetup(bool resetCameraToOrigin)
        {
            if (CameraRig == null)
            {
                CameraRig = UnityEngine.Object.FindObjectOfType(cameraRigType) as ICameraRig;
                if (CameraRig == null)
                {
                    CameraRig = UnityEngine.Object.Instantiate(rigPrefab).GetComponent(cameraRigType) as ICameraRig;
                }

                Debug.Assert(CameraRig != null);
            }

            if (!string.Equals(CameraRig.RigTransform.name, CameraService.DefaultXRCameraRigName))
            {
                var previousName = CameraRig.RigTransform.name;
                CameraRig.RigTransform.name = CameraService.DefaultXRCameraRigName;
                Debug.Log($"Rig object {previousName} was renamed to {CameraService.DefaultXRCameraRigName} by {GetType().Name}.");
            }

            if (resetCameraToOrigin)
            {
                CameraRig.RigTransform.position = Vector3.zero;
                CameraRig.CameraTransform.position = Vector3.zero;
            }
        }

        #region Tracking Origin Setup

        private bool SetupTrackingOrigin()
        {
            SubsystemManager.GetInstances(inputSubsystems);

            // We assume the tracking mode to be set, that way
            // when in editor and no subsystems are connected / running
            // we can still keep going and assume everything is ready.
            var trackingOriginModeSet = true;

            if (inputSubsystems.Count != 0)
            {
                for (int i = 0; i < inputSubsystems.Count; i++)
                {
                    if (inputSubsystems[i].SubsystemDescriptor.id == "MockHMD Head Tracking")
                    {
                        UpdateCameraHeightOffset(defaultHeadHeight);
                    }
                    else
                    {
                        var result = SetupTrackingOrigin(inputSubsystems[i]);
                        if (result)
                        {
                            inputSubsystems[i].trackingOriginUpdated -= XRInputSubsystem_OnTrackingOriginUpdated;
                            inputSubsystems[i].trackingOriginUpdated += XRInputSubsystem_OnTrackingOriginUpdated;
                        }
                        trackingOriginModeSet &= result;
                    }
                }
            }
            else
            {
                // No subsystems available, we are probably running in editor without a device
                // connected, position the camera at the configured default offset.
                UpdateCameraHeightOffset(defaultHeadHeight);
            }

            return trackingOriginModeSet;
        }

        private bool SetupTrackingOrigin(XRInputSubsystem subsystem)
        {
            if (subsystem == null)
            {
                return false;
            }

            var trackingOriginModeSet = false;
            var supportedModes = subsystem.GetSupportedTrackingOriginModes();
            var requestedMode = trackingOriginMode;

            if (requestedMode == TrackingOriginModeFlags.Floor)
            {
                if ((supportedModes & (TrackingOriginModeFlags.Floor | TrackingOriginModeFlags.Unknown)) == 0)
                {
                    Debug.LogWarning("Attempting to set the tracking origin to floor, but the device does not support it.");
                }
                else
                {
                    trackingOriginModeSet = subsystem.TrySetTrackingOriginMode(requestedMode);
                }
            }
            else if (requestedMode == TrackingOriginModeFlags.Device)
            {
                if ((supportedModes & (TrackingOriginModeFlags.Device | TrackingOriginModeFlags.Unknown)) == 0)
                {
                    Debug.LogWarning("Attempting to set the camera system tracking origin to device, but the device does not support it.");
                }
                else
                {
                    trackingOriginModeSet = subsystem.TrySetTrackingOriginMode(requestedMode) && subsystem.TryRecenter();
                }
            }

            if (trackingOriginModeSet)
            {
                UpdateTrackingOrigin(subsystem.GetTrackingOriginMode());
            }

            return trackingOriginModeSet;
        }

        private void XRInputSubsystem_OnTrackingOriginUpdated(XRInputSubsystem subsystem) => UpdateTrackingOrigin(subsystem.GetTrackingOriginMode());

        private void UpdateTrackingOrigin(TrackingOriginModeFlags trackingOriginModeFlags)
        {
            trackingOriginMode = trackingOriginModeFlags;

            UpdateCameraHeightOffset(trackingOriginMode == TrackingOriginModeFlags.Device ? defaultHeadHeight : 0.0f);
            ResetRigTransforms();
            SyncRigTransforms();
        }

        #endregion Tracking Origin Setup

        /// <summary>
        /// Updates the camera height offset to the specified value.
        /// </summary>
        protected virtual void UpdateCameraHeightOffset(float heightOffset = 0f)
        {
            CameraRig.CameraTransform.localPosition = new Vector3(
                CameraRig.CameraTransform.localPosition.x,
                heightOffset,
                CameraRig.CameraTransform.localPosition.z);
        }

        /// <summary>
        /// Applies opaque settings from camera profile.
        /// </summary>
        protected virtual void ApplySettingsForOpaqueDisplay()
        {
            CameraRig.PlayerCamera.clearFlags = cameraClearFlagsOpaqueDisplay;
            CameraRig.PlayerCamera.nearClipPlane = nearClipPlaneOpaqueDisplay;
            CameraRig.PlayerCamera.backgroundColor = backgroundColorOpaqueDisplay;
            QualitySettings.SetQualityLevel(opaqueQualityLevel, false);
        }

        /// <summary>
        /// Applies transparent settings from camera profile.
        /// </summary>
        protected virtual void ApplySettingsForTransparentDisplay()
        {
            CameraRig.PlayerCamera.clearFlags = cameraClearFlagsTransparentDisplay;
            CameraRig.PlayerCamera.backgroundColor = backgroundColorTransparentDisplay;
            CameraRig.PlayerCamera.nearClipPlane = nearClipPlaneTransparentDisplay;
            QualitySettings.SetQualityLevel(transparentQualityLevel, false);
        }

        /// <summary>
        /// Resets the <see cref="ICameraRig.RigTransform"/>, <see cref="ICameraRig.CameraTransform"/>,
        /// and <see cref="ICameraRig.BodyTransform"/> poses.
        /// </summary>
        protected virtual void ResetRigTransforms()
        {
            CameraRig.RigTransform.position = Vector3.zero;
            CameraRig.RigTransform.rotation = Quaternion.identity;

            // If the camera is a 2d camera then we can adjust the camera's height to match the head height.
            CameraRig.CameraTransform.position = IsStereoscopic ? Vector3.zero : new Vector3(0f, HeadHeight, 0f);

            CameraRig.CameraTransform.rotation = Quaternion.identity;
            CameraRig.BodyTransform.position = Vector3.zero;
            CameraRig.BodyTransform.rotation = Quaternion.identity;
        }

        /// <summary>
        /// Called each <see cref="LateUpdate"/> to sync the <see cref="ICameraRig.RigTransform"/>,
        /// <see cref="ICameraRig.CameraTransform"/>, and <see cref="ICameraRig.BodyTransform"/> poses.
        /// </summary>
        protected virtual void SyncRigTransforms()
        {
            var cameraLocalPosition = CameraRig.CameraTransform.localPosition;
            var bodyLocalPosition = CameraRig.BodyTransform.localPosition;

            bodyLocalPosition.x = cameraLocalPosition.x;
            bodyLocalPosition.y = cameraLocalPosition.y - Math.Abs(HeadHeight);
            bodyLocalPosition.z = cameraLocalPosition.z;

            CameraRig.BodyTransform.localPosition = bodyLocalPosition;

            var bodyRotation = CameraRig.BodyTransform.rotation;
            var headRotation = CameraRig.CameraTransform.rotation;
            var currentAngle = Mathf.Abs(Quaternion.Angle(bodyRotation, headRotation));

            if (currentAngle > bodyAdjustmentAngle)
            {
                CameraRig.BodyTransform.rotation = Quaternion.Slerp(bodyRotation, headRotation, Time.deltaTime * bodyAdjustmentSpeed);
            }
        }
    }
}