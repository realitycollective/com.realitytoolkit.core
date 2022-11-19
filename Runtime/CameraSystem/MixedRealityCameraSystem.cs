// Copyright (c) Reality Collective. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using RealityCollective.ServiceFramework.Attributes;
using RealityCollective.ServiceFramework.Definitions.Platforms;
using RealityCollective.ServiceFramework.Services;
using RealityToolkit.CameraSystem.Definitions;
using RealityToolkit.CameraSystem.Interfaces;
using RealityToolkit.Utilities;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

namespace RealityToolkit.CameraSystem
{
    /// <summary>
    /// The Reality Toolkit's default implementation of the <see cref="IMixedRealityCameraSystem"/>.
    /// </summary>
    [RuntimePlatform(typeof(AllPlatforms))]
    [System.Runtime.InteropServices.Guid("5C656EE3-FE7C-4FB3-B3EE-DF3FC0D0973D")]
    public class MixedRealityCameraSystem : BaseServiceWithConstructor, IMixedRealityCameraSystem
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="name">The service display name.</param>
        /// <param name="priority">The service initialization priority.</param>
        /// <param name="profile">The service configuration profile.</param>
        public MixedRealityCameraSystem(string name, uint priority, MixedRealityCameraSystemProfile profile)
            : base(name, priority) { }

        private static readonly List<XRDisplaySubsystem> xrDisplaySubsystems = new List<XRDisplaySubsystem>();
        private readonly HashSet<IMixedRealityCameraServiceModule> cameraDataProviders = new HashSet<IMixedRealityCameraServiceModule>();

        public const string DefaultXRCameraRigName = "XRCameraRig";

        /// <inheritdoc />
        public override uint Priority => 0;

        /// <inheritdoc />
        public IReadOnlyCollection<IMixedRealityCameraServiceModule> CameraDataProviders => cameraDataProviders;

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
        public override void Destroy()
        {
            base.Destroy();
            Debug.Assert(cameraDataProviders.Count == 0, "Failed to clean up camera data provider references!");
        }

        /// <inheritdoc />
        public void RegisterCameraDataProvider(IMixedRealityCameraServiceModule dataProvider) => cameraDataProviders.Add(dataProvider);

        /// <inheritdoc />
        public void UnRegisterCameraDataProvider(IMixedRealityCameraServiceModule dataProvider) => cameraDataProviders.Remove(dataProvider);
    }
}