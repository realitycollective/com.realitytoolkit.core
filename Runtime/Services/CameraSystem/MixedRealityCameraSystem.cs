// Copyright (c) Reality Collective. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Collections.Generic;
using RealityToolkit.Definitions.CameraSystem;
using RealityToolkit.Interfaces.CameraSystem;
using UnityEngine;
using UnityEngine.XR;

namespace RealityToolkit.Services.CameraSystem
{
    /// <summary>
    /// The Reality Toolkit's default implementation of the <see cref="IMixedRealityCameraSystem"/>.
    /// </summary>
    [System.Runtime.InteropServices.Guid("5C656EE3-FE7C-4FB3-B3EE-DF3FC0D0973D")]
    public class MixedRealityCameraSystem : BaseSystem, IMixedRealityCameraSystem
    {
        /// <inheritdoc />
        public MixedRealityCameraSystem(MixedRealityCameraSystemProfile profile)
            : base(profile) { }

        private static readonly List<XRDisplaySubsystem> xrDisplaySubsystems = new List<XRDisplaySubsystem>();

        /// <inheritdoc />
        public override uint Priority => 0;

        private IMixedRealityCameraDataProvider cameraDataProvider;
        /// <inheritdoc />
        public IMixedRealityCameraDataProvider CameraDataProvider
        {
            get
            {
                if (cameraDataProvider == null && !MixedRealityToolkit.TryGetService(out cameraDataProvider))
                {
                    Debug.LogError($"{nameof(MixedRealityCameraSystem)} requires an active {nameof(IMixedRealityCameraDataProvider)} in the configuration!");
                }

                return cameraDataProvider;
            }
        }

        /// <inheritdoc />
        public IMixedRealityCameraRig MainCameraRig => CameraDataProvider.CameraRig;

        /// <inheritdoc />
        public TrackingType TrackingType
        {
            get
            {
                if (CameraDataProvider != null)
                {
                    return CameraDataProvider.TrackingType;
                }

                // If we can't find the active camera data provider we must rely
                // on whatever the platform default is.
                return TrackingType.Auto;
            }
        }

        private static XRDisplaySubsystem displaySubsystem = null;
        /// <inheritdoc />
        public XRDisplaySubsystem DisplaySubsystem
        {
            get
            {
                if (displaySubsystem != null && displaySubsystem.running)
                {
                    return displaySubsystem;
                }

                displaySubsystem = null;
                SubsystemManager.GetInstances(xrDisplaySubsystems);

                for (var i = 0; i < xrDisplaySubsystems.Count; i++)
                {
                    var xrDisplaySubsystem = xrDisplaySubsystems[i];
                    if (xrDisplaySubsystem.running)
                    {
                        displaySubsystem = xrDisplaySubsystem;
                        break;
                    }
                }

                return displaySubsystem;
            }
        }
    }
}