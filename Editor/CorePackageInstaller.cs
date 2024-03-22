// Copyright (c) Reality Collective. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using RealityCollective.Editor.Extensions;
using RealityCollective.Editor.Utilities;
using RealityCollective.Extensions;
using RealityCollective.ServiceFramework;
using RealityCollective.ServiceFramework.Definitions;
using RealityCollective.ServiceFramework.Editor;
using RealityCollective.ServiceFramework.Editor.Packages;
using RealityCollective.ServiceFramework.Services;
using RealityToolkit.Editor.Utilities;
using System.IO;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR && !UNITY_2021_1_OR_NEWER
using SceneManagement = UnityEditor.Experimental.SceneManagement;
#elif UNITY_EDITOR
using SceneManagement = UnityEditor.SceneManagement;
#endif

namespace RealityToolkit.Editor
{
    /// <summary>
    /// Reality Toolkit core package installer.
    /// </summary>
    [InitializeOnLoad]
    internal static class CorePackageInstaller
    {
        private const string CORE_PATH_FINDER = "/Editor/Utilities/CorePathFinder.cs";
        private static readonly string defaultPath = $"{RealityToolkitPreferences.ProfileGenerationPath}Core";
        private static readonly string hiddenPath = Path.GetFullPath($"{PathFinderUtility.ResolvePath<IPathFinder>(typeof(CorePathFinder))}{Path.DirectorySeparatorChar}{RealityToolkitPreferences.HIDDEN_PACKAGE_ASSETS_PATH}");
        const string configureMenuItemPath = RealityToolkitPreferences.Editor_Menu_Keyword + "/Configure...";

        static CorePackageInstaller()
        {
            EditorApplication.delayCall += CheckPackage;
        }

        [MenuItem(configureMenuItemPath, true, 0)]
        private static bool CreateToolkitGameObjectValidation()
        {
            return SceneManagement.PrefabStageUtility.GetCurrentPrefabStage() == null;
        }

        /// <summary>
        /// Adds the Reality Toolkit to the active <see cref="ServiceManager"/> configuration.
        /// </summary>
        [MenuItem(
            itemName: configureMenuItemPath,
            menuItem = configureMenuItemPath,
            priority = 0,
            validate = false)]
        public async static void ConfigureToolkitAsync()
        {
            if (Application.isPlaying)
            {
                return;
            }

            var serviceManagerInstance = SetupServiceManager();
            EditorPreferences.Set($"{nameof(CorePackageInstaller)}.Assets", AssetsInstaller.TryInstallAssets(hiddenPath, defaultPath, false, true, false));

            // Why is this here you wonder? Well, I have no idea but for some reason we need to give Unity
            // a bit of time after initializing the manager object and potentially copying over assets etc.
            // Attemping to select the manager right after creating it, causes a bunch of issues with the inspector.
            await Task.Delay(500);

            EditorApplication.delayCall += () =>
            {
                Selection.activeObject = serviceManagerInstance;
            };
        }

        private static ServiceManagerInstance SetupServiceManager()
        {
#if UNITY_2023_1_OR_NEWER
            var serviceManagerInstance = Object.FindFirstObjectByType<ServiceManagerInstance>();
#else
            var serviceManagerInstance = Object.FindObjectOfType<ServiceManagerInstance>();
#endif
            if (serviceManagerInstance.IsNotNull() &&
                serviceManagerInstance.Manager != null &&
                serviceManagerInstance.Manager.ActiveProfile.IsNotNull() &&
                serviceManagerInstance.Manager.IsInitialized)
            {
                Selection.activeObject = serviceManagerInstance;
                return serviceManagerInstance;
            }

            if (serviceManagerInstance.IsNull())
            {
                serviceManagerInstance = new GameObject(nameof(ServiceManagerInstance)).AddComponent<ServiceManagerInstance>();
            }

            if (serviceManagerInstance.Manager == null)
            {
                serviceManagerInstance.InitializeServiceManager();
            }

            if (serviceManagerInstance.Manager.ActiveProfile.IsNull())
            {
                var availableRootProfiles = ScriptableObjectExtensions.GetAllInstances<ServiceProvidersProfile>();
                if (availableRootProfiles == null || availableRootProfiles.Length == 0)
                {
                    var newProfile = ScriptableObject.CreateInstance<ServiceProvidersProfile>().GetOrCreateAsset(
                        RealityToolkitPreferences.DEFAULT_GENERATION_PATH,
                        $"RealityToolkit{nameof(ServiceProvidersProfile)}", false);
                    serviceManagerInstance.Manager.ResetProfile(newProfile);
                }
                else
                {
                    serviceManagerInstance.Manager.ResetProfile(availableRootProfiles[0]);
                }
            }

            serviceManagerInstance.Manager.InitializeServiceManager();
            Debug.Assert(serviceManagerInstance.Manager.IsInitialized);

            return serviceManagerInstance;
        }

        [MenuItem(RealityToolkitPreferences.Editor_Menu_Keyword + "/Packages/Install Core Package Assets...", true, -1)]
        private static bool ImportPackageAssetsValidation()
        {
            return !Directory.Exists($"{defaultPath}{Path.DirectorySeparatorChar}");
        }

        [MenuItem(RealityToolkitPreferences.Editor_Menu_Keyword + "/Packages/Install Core Package Assets...", false, -1)]
        private static void ImportPackageAssets()
        {
            EditorPreferences.Set($"{nameof(CorePackageInstaller)}.Assets", false);
            EditorApplication.delayCall += CheckPackage;
        }

        private static void CheckPackage()
        {
            if (!EditorPreferences.Get($"{nameof(CorePackageInstaller)}.Assets", false))
            {
                EditorPreferences.Set($"{nameof(CorePackageInstaller)}.Assets", AssetsInstaller.TryInstallAssets(hiddenPath, defaultPath, onlyUnityAssets: false));
            }
        }

        #region Core Paths

        /// <summary>
        /// The absolute folder path to the Reality Toolkit in your project.
        /// </summary>
        public static string RTK_Core_AbsoluteFolderPath
        {
            get
            {
                if (string.IsNullOrWhiteSpace(coreAbsoluteFolderPath))
                {
                    coreAbsoluteFolderPath = Path.GetFullPath(RTK_Core_RelativeFolderPath);
                }

                return coreAbsoluteFolderPath.BackSlashes();
            }
        }

        private static string coreAbsoluteFolderPath = string.Empty;

        /// <summary>
        /// The relative folder path to the Reality Toolkit Core module folder in relation to either the "Assets" or "Packages" folders.
        /// </summary>
        public static string RTK_Core_RelativeFolderPath
        {
            get
            {
                if (string.IsNullOrEmpty(coreRelativeFolderPath))
                {
                    coreRelativeFolderPath = PathFinderUtility.ResolvePath(CORE_PATH_FINDER);
                    Debug.Assert(!string.IsNullOrWhiteSpace(coreRelativeFolderPath));
                }

                return coreRelativeFolderPath;
            }
        }

        private static string coreRelativeFolderPath = string.Empty;

        #endregion Core Paths
    }
}
