// Copyright (c) Reality Collective. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using RealityCollective.ServiceFramework.Attributes;
using RealityCollective.ServiceFramework.Definitions.Platforms;
using RealityCollective.ServiceFramework.Services;
using RealityToolkit.CameraService.Definitions;
using RealityToolkit.CameraService.Interfaces;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

namespace RealityToolkit.CameraService
{
    /// <summary>
    /// The Reality Toolkit's default implementation of the <see cref="ICameraService"/>.
    /// </summary>
    [RuntimePlatform(typeof(AllPlatforms))]
    [System.Runtime.InteropServices.Guid("5C656EE3-FE7C-4FB3-B3EE-DF3FC0D0973D")]
    public class CameraService : BaseServiceWithConstructor, ICameraService
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="name">The service display name.</param>
        /// <param name="priority">The service initialization priority.</param>
        /// <param name="profile">The service configuration profile.</param>
        public CameraService(string name, uint priority, CameraServiceProfile profile)
            : base(name, priority) { }

        private static readonly List<XRDisplaySubsystem> xrDisplaySubsystems = new List<XRDisplaySubsystem>();
        public const string DefaultXRCameraRigName = "XRCameraRig";

        /// <inheritdoc />
        public override uint Priority => 0;

        /// <inheritdoc />
        public ICameraRig CameraRig => CameraServiceModule != null ? CameraServiceModule.CameraRig : null;

        /// <inheritdoc />
        public ICameraServiceModule CameraServiceModule { get; private set; }

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
        public override void Initialize()
        {
            var cameraServiceModules = ServiceManager.Instance.GetServices<ICameraServiceModule>();
            Debug.Assert(cameraServiceModules.Count > 0, $"There must be an active {nameof(ICameraServiceModule)}. Please check your {nameof(CameraServiceProfile)} configuration.");
            Debug.Assert(cameraServiceModules.Count < 2, $"There should only ever be one active {nameof(ICameraServiceModule)}. Please check your {nameof(CameraServiceProfile)} configuration.");
            CameraServiceModule = cameraServiceModules[0];
        }
    }
}