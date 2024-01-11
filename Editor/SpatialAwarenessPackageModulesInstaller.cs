// Copyright (c) Reality Collective. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using RealityCollective.ServiceFramework.Definitions;
using RealityCollective.ServiceFramework.Editor.Packages;
using RealityCollective.ServiceFramework.Services;
using RealityToolkit.SpatialAwareness.Definitions;
using RealityToolkit.SpatialAwareness.Interfaces;
using RealityToolkit.SpatialAwareness.Interfaces.SpatialObservers;
using System.Linq;
using UnityEditor;

namespace RealityToolkit.Editor
{
    /// <summary>
    /// Installs <see cref="IS"/>s coming from a third party package
    /// into the <see cref="PlayerServiceProfile"/> in the <see cref="ServiceManager.ActiveProfile"/>.
    /// </summary>
    [InitializeOnLoad]
    public sealed class SpatialAwarenessPackageModulesInstaller : IPackageModulesInstaller
    {
        /// <summary>
        /// Statis initalizer for the installer instance.
        /// </summary>
        static SpatialAwarenessPackageModulesInstaller()
        {
            if (Instance == null)
            {
                Instance = new SpatialAwarenessPackageModulesInstaller();
            }

            PackageInstaller.RegisterModulesInstaller(Instance);
        }

        /// <summary>
        /// Internal singleton instance of the installer.
        /// </summary>
        private static SpatialAwarenessPackageModulesInstaller Instance { get; }

        /// <inheritdoc/>
        public bool Install(ServiceConfiguration serviceConfiguration)
        {
            if (!typeof(ISpatialAwarenessServiceModule).IsAssignableFrom(serviceConfiguration.InstancedType.Type))
            {
                // This module installer does not accept the configuration type.
                return false;
            }

            if (!ServiceManager.IsActiveAndInitialized)
            {
                UnityEngine.Debug.LogWarning($"Could not install {serviceConfiguration.InstancedType.Type.Name}.{nameof(ServiceManager)} is not initialized.");
                return false;
            }

            if (!ServiceManager.Instance.HasActiveProfile)
            {
                UnityEngine.Debug.LogWarning($"Could not install {serviceConfiguration.InstancedType.Type.Name}.{nameof(ServiceManager)} has no active profile.");
                return false;
            }

            if (!ServiceManager.Instance.TryGetServiceProfile<ISpatialAwarenessService, SpatialAwarenessSystemProfile>(out var spatialAwarenessServiceProfile))
            {
                UnityEngine.Debug.LogWarning($"Could not install {serviceConfiguration.InstancedType.Type.Name}.{nameof(SpatialAwarenessSystemProfile)} not found.");
                return false;
            }

            // Setup the configuration.
            var typedServiceConfiguration = new ServiceConfiguration<ISpatialAwarenessServiceModule>(serviceConfiguration.InstancedType.Type, serviceConfiguration.Name, serviceConfiguration.Priority, serviceConfiguration.RuntimePlatforms, serviceConfiguration.Profile);

            // Make sure it's not already in the target profile.
            if (spatialAwarenessServiceProfile.ServiceConfigurations.All(sc => sc.InstancedType.Type != serviceConfiguration.InstancedType.Type))
            {
                spatialAwarenessServiceProfile.AddConfiguration(typedServiceConfiguration);
                UnityEngine.Debug.Log($"Successfully installed the {serviceConfiguration.InstancedType.Type.Name} to {spatialAwarenessServiceProfile.name}.");
            }
            else
            {
                UnityEngine.Debug.Log($"Skipped installing the {serviceConfiguration.InstancedType.Type.Name} to {spatialAwarenessServiceProfile.name}. Already installed.");
            }

            return true;
        }
    }
}
