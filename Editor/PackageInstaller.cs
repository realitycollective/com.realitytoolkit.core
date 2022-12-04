// Copyright (c) Reality Collective. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using RealityCollective.Editor.Extensions;
using RealityCollective.Editor.Utilities;
using RealityCollective.Extensions;
using RealityCollective.ServiceFramework.Definitions;
using RealityCollective.ServiceFramework.Services;
using RealityToolkit.BoundarySystem.Interfaces;
using RealityToolkit.CameraSystem.Definitions;
using RealityToolkit.CameraSystem.Interfaces;
using RealityToolkit.Definitions;
using RealityToolkit.Definitions.BoundarySystem;
using RealityToolkit.InputSystem.Definitions;
using RealityToolkit.InputSystem.Interfaces;
using RealityToolkit.InputSystem.Interfaces.Modules;
using RealityToolkit.LocomotionSystem.Interfaces;
using RealityToolkit.SpatialAwarenessSystem.Definitions;
using RealityToolkit.SpatialAwarenessSystem.Interfaces;
using RealityToolkit.SpatialAwarenessSystem.Interfaces.SpatialObservers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace RealityToolkit.Editor
{
    public static class PackageInstaller
    {
        private static string ProjectRootPath => Directory.GetParent(Application.dataPath).FullName.BackSlashes();

        /// <summary>
        /// Attempt to copy any assets found in the source path into the project.
        /// </summary>
        /// <param name="sourcePath">The source path of the assets to be installed. This should typically be from a hidden upm package folder marked with a "~".</param>
        /// <param name="destinationPath">The destination path, typically inside the projects "Assets" directory.</param>
        /// <param name="regenerateGuids">Should the guids for the copied assets be regenerated?</param>
        /// <param name="skipDialog">If set, assets and configuration is installed without prompting the user.</param>
        /// <returns>Returns true if the profiles were successfully copies, installed, and added to the <see cref="MixedRealityToolkitRootProfile"/>.</returns>
        public static bool TryInstallAssets(string sourcePath, string destinationPath, bool regenerateGuids = false, bool skipDialog = false)
            => TryInstallAssets(new Dictionary<string, string> { { sourcePath, destinationPath } }, regenerateGuids, skipDialog);

        /// <summary>
        /// Attempt to copy any assets found in the source path into the project.
        /// </summary>
        /// <param name="installationPaths">The assets paths to be installed. Key is the source path of the assets to be installed. This should typically be from a hidden upm package folder marked with a "~". Value is the destination.</param>
        /// <param name="regenerateGuids">Should the guids for the copied assets be regenerated?</param>
        /// <param name="skipDialog">If set, assets and configuration is installed without prompting the user.</param>
        /// <returns>Returns true if the profiles were successfully copies, installed, and added to the <see cref="MixedRealityToolkitRootProfile"/>.</returns>
        public static bool TryInstallAssets(Dictionary<string, string> installationPaths, bool regenerateGuids = false, bool skipDialog = false)
        {
            var anyFail = false;
            var newInstall = true;
            var installedAssets = new List<string>();
            var installedDirectories = new List<string>();

            foreach (var installationPath in installationPaths)
            {
                var sourcePath = installationPath.Key.BackSlashes();
                var destinationPath = installationPath.Value.BackSlashes();
                installedDirectories.Add(destinationPath);

                if (Directory.Exists(destinationPath))
                {
                    newInstall = false;
                    EditorUtility.DisplayProgressBar("Verifying assets...", $"{sourcePath} -> {destinationPath}", 0);

                    installedAssets.AddRange(UnityFileHelper.GetUnityAssetsAtPath(destinationPath));

                    for (int i = 0; i < installedAssets.Count; i++)
                    {
                        EditorUtility.DisplayProgressBar("Verifying assets...", Path.GetFileNameWithoutExtension(installedAssets[i]), i / (float)installedAssets.Count);
                        installedAssets[i] = installedAssets[i].Replace($"{ProjectRootPath}{Path.DirectorySeparatorChar}", string.Empty).BackSlashes();
                    }

                    EditorUtility.ClearProgressBar();
                }
                else
                {
                    var destinationDirectory = Path.GetFullPath(destinationPath);

                    // Check if directory or symbolic link exists before attempting to create it
                    if (!Directory.Exists(destinationDirectory) &&
                        !File.Exists(destinationDirectory))
                    {
                        Directory.CreateDirectory(destinationDirectory);
                    }

                    EditorUtility.DisplayProgressBar("Copying assets...", $"{sourcePath} -> {destinationPath}", 0);

                    var copiedAssets = UnityFileHelper.GetUnityAssetsAtPath(sourcePath);

                    for (var i = 0; i < copiedAssets.Count; i++)
                    {
                        EditorUtility.DisplayProgressBar("Copying assets...", Path.GetFileNameWithoutExtension(copiedAssets[i]), i / (float)copiedAssets.Count);

                        try
                        {
                            copiedAssets[i] = CopyAsset(sourcePath, copiedAssets[i], destinationPath);
                        }
                        catch (Exception e)
                        {
                            Debug.LogError(e);
                            anyFail = true;
                        }
                    }

                    if (!anyFail)
                    {
                        installedAssets.AddRange(copiedAssets);
                    }

                    EditorUtility.ClearProgressBar();
                }
            }

            if (anyFail)
            {
                foreach (var installedDirectory in installedDirectories)
                {
                    try
                    {
                        if (Directory.Exists(installedDirectory))
                        {
                            Directory.Delete(installedDirectory);
                        }
                    }
                    catch (Exception e)
                    {
                        Debug.LogError(e);
                    }
                }
            }

            if (newInstall && regenerateGuids)
            {
                GuidRegenerator.RegenerateGuids(installedDirectories);
            }

            if (!Application.isBatchMode)
            {
                EditorApplication.delayCall += () =>
                {
                    AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);
                    EditorApplication.delayCall += () =>
                        AddConfigurations(installedAssets, skipDialog);
                };
            }

            EditorUtility.ClearProgressBar();
            return true;
        }

        private static void AddConfigurations(List<string> profiles, bool skipDialog = false)
        {
            ServiceProvidersProfile rootProfile;

            if (ServiceManager.IsActiveAndInitialized)
            {
                rootProfile = ServiceManager.Instance.ActiveProfile;
            }
            else
            {
                var availableRootProfiles = ScriptableObjectExtensions.GetAllInstances<ServiceProvidersProfile>();
                rootProfile = availableRootProfiles.Length > 0 ? availableRootProfiles[0] : null;
            }

            // Only if a root profile is available at all it makes sense to display the
            // platform configuration import dialog. If the user does not have a root profile yet,
            // for whatever reason, there is nothing we can do here.
            if (rootProfile.IsNull())
            {
                EditorUtility.DisplayDialog("Attention!", "Each service and service module in the platform configuration will need to be manually registered as no existing Service Framework Instance was found.\nUse the Platform Installer in the Profiles folder for the package once a Service Manager has been configured.", "OK");
                return;
            }

            Selection.activeObject = null;

            foreach (var profile in profiles.Where(x => x.EndsWith(".asset")))
            {
                var platformConfigurationProfile = AssetDatabase.LoadAssetAtPath<MixedRealityPlatformServiceConfigurationProfile>(profile);

                if (platformConfigurationProfile.IsNull()) { continue; }

                if (skipDialog || EditorUtility.DisplayDialog("We found a new Platform Configuration",
                    $"We found the {platformConfigurationProfile.name.ToProperCase()}. Would you like to add this platform configuration to your {rootProfile.name}?",
                    "Yes, Absolutely!",
                    "later"))
                {
                    InstallConfiguration(platformConfigurationProfile, rootProfile);
                }
            }
        }

        private static string CopyAsset(this string rootPath, string sourceAssetPath, string destinationPath)
        {
            sourceAssetPath = sourceAssetPath.BackSlashes();
            destinationPath = $"{destinationPath}{sourceAssetPath.Replace(Path.GetFullPath(rootPath), string.Empty)}".BackSlashes();
            destinationPath = Path.Combine(ProjectRootPath, destinationPath).BackSlashes();

            if (!File.Exists(destinationPath))
            {
                if (!Directory.Exists(Directory.GetParent(destinationPath).FullName))
                {
                    Directory.CreateDirectory(Directory.GetParent(destinationPath).FullName);
                }

                try
                {
                    File.Copy(sourceAssetPath, destinationPath);
                }
                catch
                {
                    Debug.LogError($"$Failed to copy asset!\n{sourceAssetPath}\n{destinationPath}");
                    throw;
                }
            }

            return destinationPath.Replace($"{ProjectRootPath}{Path.DirectorySeparatorChar}", string.Empty);
        }

        /// <summary>
        /// Installs the provided <see cref="MixedRealityServiceConfiguration"/> in the provided <see cref="MixedRealityToolkitRootProfile"/>.
        /// </summary>
        /// <param name="platformConfigurationProfile">The platform configuration to install.</param>
        /// <param name="rootProfile">The root profile to install the </param>
        public static void InstallConfiguration(MixedRealityPlatformServiceConfigurationProfile platformConfigurationProfile, ServiceProvidersProfile rootProfile)
        {
            if (ServiceManager.Instance == null ||
                ServiceManager.Instance.ActiveProfile.IsNull())
            {
                Debug.LogError($"Cannot install service configurations. There is no active {nameof(ServiceManager)} or it does not have a valid profile.");
                return;
            }

            var didInstallConfigurations = false;
            var configurationsAlreadyInstalled = false;
            foreach (var configuration in platformConfigurationProfile.Configurations)
            {
                var configurationType = configuration.InstancedType.Type;

                if (configurationType == null)
                {
                    Debug.LogError($"Failed to find a valid {nameof(configuration.InstancedType)} for {configuration.Name}!");
                    continue;
                }

                switch (configurationType)
                {
                    case Type _ when typeof(IMixedRealityCameraSystem).IsAssignableFrom(configurationType):
                        if (!ServiceManager.Instance.TryGetService<IMixedRealityCameraSystem>(out _))
                        {
                            ServiceManager.Instance.ActiveProfile.AddConfiguration(new ServiceConfiguration<IMixedRealityCameraSystem>(configuration));
                            EditorUtility.SetDirty(ServiceManager.Instance.ActiveProfile);
                            didInstallConfigurations = true;
                        }
                        else 
                        {
                            configurationsAlreadyInstalled = true;
                        }
                        break;

                    case Type _ when typeof(IMixedRealityInputSystem).IsAssignableFrom(configurationType):
                        if (!ServiceManager.Instance.TryGetService<IMixedRealityInputSystem>(out _))
                        {
                            ServiceManager.Instance.ActiveProfile.AddConfiguration(new ServiceConfiguration<IMixedRealityInputSystem>(configuration));
                            EditorUtility.SetDirty(ServiceManager.Instance.ActiveProfile);
                            didInstallConfigurations = true;
                        }
                        else
                        {
                            configurationsAlreadyInstalled = true;
                        }
                        break;

                    case Type _ when typeof(ILocomotionSystem).IsAssignableFrom(configurationType):
                        if (!ServiceManager.Instance.TryGetService<ILocomotionSystem>(out _))
                        {
                            ServiceManager.Instance.ActiveProfile.AddConfiguration(new ServiceConfiguration<ILocomotionSystem>(configuration));
                            EditorUtility.SetDirty(ServiceManager.Instance.ActiveProfile);
                            didInstallConfigurations = true;
                        }
                        else
                        {
                            configurationsAlreadyInstalled = true;
                        }
                        break;

                    case Type _ when typeof(IMixedRealityBoundarySystem).IsAssignableFrom(configurationType):
                        if (!ServiceManager.Instance.TryGetService<IMixedRealityBoundarySystem>(out _))
                        {
                            ServiceManager.Instance.ActiveProfile.AddConfiguration(new ServiceConfiguration<IMixedRealityBoundarySystem>(configuration));
                            EditorUtility.SetDirty(ServiceManager.Instance.ActiveProfile);
                            didInstallConfigurations = true;
                        }
                        else
                        {
                            configurationsAlreadyInstalled = true;
                        }
                        break;

                    case Type _ when typeof(IMixedRealitySpatialAwarenessSystem).IsAssignableFrom(configurationType):
                        if (!ServiceManager.Instance.TryGetService<IMixedRealitySpatialAwarenessSystem>(out _))
                        {
                            ServiceManager.Instance.ActiveProfile.AddConfiguration(new ServiceConfiguration<IMixedRealitySpatialAwarenessSystem>(configuration));
                            EditorUtility.SetDirty(ServiceManager.Instance.ActiveProfile);
                            didInstallConfigurations = true;
                        }
                        else
                        {
                            configurationsAlreadyInstalled = true;
                        }
                        break;

                    case Type _ when typeof(IMixedRealityCameraServiceModule).IsAssignableFrom(configurationType):
                        if (ServiceManager.Instance.TryGetServiceProfile<IMixedRealityCameraSystem, MixedRealityCameraSystemProfile>(out var cameraSystemProfile, rootProfile))
                        {
                            var cameraDataProviderConfiguration = new ServiceConfiguration<IMixedRealityCameraServiceModule>(configuration);

                            if (cameraSystemProfile.ServiceConfigurations.Any(serviceConfiguration => serviceConfiguration.InstancedType.Type == cameraDataProviderConfiguration.InstancedType.Type))
                            {
                                configurationsAlreadyInstalled = true;
                            }
                            else if (cameraSystemProfile.ServiceConfigurations.All(serviceConfiguration => serviceConfiguration.InstancedType.Type != cameraDataProviderConfiguration.InstancedType.Type))
                            {
                                Debug.Log($"Added {configuration.Name} to {rootProfile.name}");
                                cameraSystemProfile.AddConfiguration(cameraDataProviderConfiguration);
                                EditorUtility.SetDirty(cameraSystemProfile);
                                didInstallConfigurations = true;
                            }
                        }
                        break;

                    case Type _ when typeof(IMixedRealityInputServiceModule).IsAssignableFrom(configurationType):
                        if (ServiceManager.Instance.TryGetServiceProfile<IMixedRealityInputSystem, MixedRealityInputSystemProfile>(out var inputSystemProfile, rootProfile))
                        {
                            var inputDataProviderConfiguration = new ServiceConfiguration<IMixedRealityInputServiceModule>(configuration);

                            if (inputSystemProfile.ServiceConfigurations.Any(serviceConfiguration => serviceConfiguration.InstancedType.Type == inputDataProviderConfiguration.InstancedType.Type))
                            {
                                configurationsAlreadyInstalled = true;
                            }
                            else if (inputSystemProfile.ServiceConfigurations.All(serviceConfiguration => serviceConfiguration.InstancedType.Type != inputDataProviderConfiguration.InstancedType.Type))
                            {
                                Debug.Log($"Added {configuration.Name} to {rootProfile.name}");
                                inputSystemProfile.AddConfiguration(inputDataProviderConfiguration);
                                EditorUtility.SetDirty(inputSystemProfile);
                                didInstallConfigurations = true;
                            }
                        }
                        break;

                    case Type _ when typeof(IMixedRealitySpatialAwarenessServiceModule).IsAssignableFrom(configurationType):
                        if (ServiceManager.Instance.TryGetServiceProfile<IMixedRealitySpatialAwarenessSystem, MixedRealitySpatialAwarenessSystemProfile>(out var spatialAwarenessSystemProfile, rootProfile))
                        {
                            var spatialObserverConfiguration = new ServiceConfiguration<IMixedRealitySpatialAwarenessServiceModule>(configuration);

                            if (spatialAwarenessSystemProfile.ServiceConfigurations.Any(serviceConfiguration => serviceConfiguration.InstancedType.Type == spatialObserverConfiguration.InstancedType.Type))
                            {
                                configurationsAlreadyInstalled = true;
                            }
                            else if (spatialAwarenessSystemProfile.ServiceConfigurations.All(serviceConfiguration => serviceConfiguration.InstancedType.Type != spatialObserverConfiguration.InstancedType.Type))
                            {
                                Debug.Log($"Added {configuration.Name} to {rootProfile.name}");
                                spatialAwarenessSystemProfile.AddConfiguration(spatialObserverConfiguration);
                                EditorUtility.SetDirty(spatialAwarenessSystemProfile);
                                didInstallConfigurations = true;
                            }
                        }
                        break;
                    case Type _ when typeof(IMixedRealityBoundaryServiceModule).IsAssignableFrom(configurationType):
                        if (ServiceManager.Instance.TryGetServiceProfile<IMixedRealityBoundarySystem, MixedRealityBoundaryProfile>(out var boundarySystemProfile, rootProfile))
                        {
                            var boundarySystemConfiguration = new ServiceConfiguration<IMixedRealityBoundaryServiceModule>(configuration);

                            if (boundarySystemProfile.ServiceConfigurations.Any(serviceConfiguration => serviceConfiguration.InstancedType.Type != boundarySystemConfiguration.InstancedType.Type))
                            {
                                configurationsAlreadyInstalled = true;
                            }
                            else if (boundarySystemProfile.ServiceConfigurations.All(serviceConfiguration => serviceConfiguration.InstancedType.Type != boundarySystemConfiguration.InstancedType.Type))
                            {
                                Debug.Log($"Added {configuration.Name} to {rootProfile.name}");
                                boundarySystemProfile.AddConfiguration(boundarySystemConfiguration);
                                EditorUtility.SetDirty(boundarySystemProfile);
                                didInstallConfigurations = true;
                            }
                        }
                        break;
                }
            }

            AssetDatabase.SaveAssets();
            EditorApplication.delayCall += () => AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);

            if (didInstallConfigurations || configurationsAlreadyInstalled)
            {
                ServiceManager.Instance.ResetProfile(ServiceManager.Instance.ActiveProfile);
            }
            else
            {
                Debug.LogError("Unable to install configuration as the corresponding services were not available\nIs the Toolkit configured?");
            }
        }
    }
}
