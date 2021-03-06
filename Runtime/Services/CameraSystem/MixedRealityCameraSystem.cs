// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Collections.Generic;
using RealityToolkit.Definitions.CameraSystem;
using RealityToolkit.Interfaces.CameraSystem;
using RealityToolkit.Utilities;
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
        private readonly HashSet<IMixedRealityCameraDataProvider> cameraDataProviders = new HashSet<IMixedRealityCameraDataProvider>();

        /// <inheritdoc />
        public override uint Priority => 0;

        /// <inheritdoc />
        public override void Destroy()
        {
            base.Destroy();
            Debug.Assert(cameraDataProviders.Count == 0, "Failed to clean up camera data provider references!");
        }

        /// <inheritdoc />
        public IReadOnlyCollection<IMixedRealityCameraDataProvider> CameraDataProviders => cameraDataProviders;

        private IMixedRealityCameraRig mainCameraRig = null;
        /// <inheritdoc />
        public IMixedRealityCameraRig MainCameraRig
        {
            get
            {
                if (mainCameraRig == null)
                {
                    foreach (var dataProvider in cameraDataProviders)
                    {
                        if (dataProvider.CameraRig.PlayerCamera == CameraCache.Main)
                        {
                            mainCameraRig = dataProvider.CameraRig;
                        }
                    }
                }

                return mainCameraRig;
            }
        }

        /// <inheritdoc />
        public TrackingType TrackingType
        {
            get
            {
                foreach (var dataProvider in cameraDataProviders)
                {
                    if (dataProvider.CameraRig.PlayerCamera == CameraCache.Main)
                    {
                        return dataProvider.TrackingType;
                    }
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

        /// <inheritdoc />
        public void RegisterCameraDataProvider(IMixedRealityCameraDataProvider dataProvider) => cameraDataProviders.Add(dataProvider);

        /// <inheritdoc />
        public void UnRegisterCameraDataProvider(IMixedRealityCameraDataProvider dataProvider) => cameraDataProviders.Remove(dataProvider);
    }
}