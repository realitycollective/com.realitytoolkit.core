// Copyright (c) Reality Collective. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using RealityCollective.Editor.Extensions;
using RealityCollective.Editor.Utilities;
using RealityCollective.Extensions;
using RealityCollective.ServiceFramework;
using RealityCollective.ServiceFramework.Definitions;
using RealityCollective.ServiceFramework.Services;
using RealityToolkit.Editor.Utilities;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace RealityToolkit.Editor
{
    /// <summary>
    /// Reality Toolkit core package installer.
    /// </summary>
    [InitializeOnLoad]
    internal static class CorePackageInstaller
    {
        private static readonly string defaultPath = $"{MixedRealityPreferences.ProfileGenerationPath}Core";
        private static readonly string hiddenPath = Path.GetFullPath($"{PathFinderUtility.ResolvePath<IPathFinder>(typeof(CorePathFinder))}{Path.DirectorySeparatorChar}{MixedRealityPreferences.HIDDEN_PACKAGE_ASSETS_PATH}");

        static CorePackageInstaller()
        {
            EditorApplication.delayCall += CheckPackage;
        }

        /// <summary>
        /// Adds the Reality Toolkit to the active <see cref="RealityCollective.ServiceFramework.Services.ServiceManager"/> configuration.
        /// </summary>
        [MenuItem(
            itemName: MixedRealityPreferences.Editor_Menu_Keyword + "/Configure...",
            menuItem = MixedRealityPreferences.Editor_Menu_Keyword + "/Configure...",
            priority = 0,
            validate = false)]
        public static void ConfigureToolkit()
        {
            // When configuring the toolkit, we have to assume first the core assets have not
            // been installed to the project yet.
            EditorPreferences.Set($"{nameof(CorePackageInstaller)}.Assets", false);

            var serviceManagerInstance = Object.FindObjectOfType<ServiceManagerInstance>();
            if (serviceManagerInstance.IsNull())
            {
                serviceManagerInstance = SetupServiceManagerInstance();
            }

            var serviceProvidersProfile = serviceManagerInstance.ServiceProvidersProfile;
            if (serviceProvidersProfile.IsNull())
            {
                serviceProvidersProfile = SetupNewServiceProvidersProfile();
                serviceManagerInstance.Manager.ActiveProfile = serviceProvidersProfile;
            }

            // Now that the service framework is in place, we simply install the core package.
            CheckPackageWithoutDialog();

            // Ping the service manager instance object to trigger initialization
            // and mark the scene it is in as dirty to indicate there's changes to be saved.
            EditorApplication.delayCall += RefreshScene;
        }

        private static ServiceManagerInstance SetupServiceManagerInstance()
        {
            var gameObject = new GameObject(nameof(ServiceManagerInstance));
            return gameObject.AddComponent<ServiceManagerInstance>();
        }

        private static ServiceProvidersProfile SetupNewServiceProvidersProfile()
        {
            return ScriptableObject.CreateInstance<ServiceProvidersProfile>().GetOrCreateAsset(
                MixedRealityPreferences.DEFAULT_GENERATION_PATH,
                nameof(ServiceProvidersProfile), true);
        }

        private static void RefreshScene()
        {
            EditorApplication.delayCall -= RefreshScene;
            ServiceManager.Instance.InitializeServiceManager();
            Selection.activeGameObject = Object.FindObjectOfType<ServiceManagerInstance>().gameObject;
            EditorSceneManager.MarkSceneDirty(Selection.activeGameObject.scene);
        }

        [MenuItem(MixedRealityPreferences.Editor_Menu_Keyword + "/Packages/Install Core Package Assets...", true, -1)]
        private static bool ImportPackageAssetsValidation()
        {
            return !Directory.Exists($"{defaultPath}{Path.DirectorySeparatorChar}");
        }

        [MenuItem(MixedRealityPreferences.Editor_Menu_Keyword + "/Packages/Install Core Package Assets...", false, -1)]
        private static void ImportPackageAssets()
        {
            EditorPreferences.Set($"{nameof(CorePackageInstaller)}.Assets", false);
            EditorApplication.delayCall += CheckPackage;
        }

        private static void CheckPackageWithoutDialog()
        {
            if (!EditorPreferences.Get($"{nameof(CorePackageInstaller)}.Assets", false))
            {
                EditorPreferences.Set($"{nameof(CorePackageInstaller)}.Assets", PackageInstaller.TryInstallAssets(hiddenPath, defaultPath, false, true));
            }
        }

        private static void CheckPackage()
        {
            if (!EditorPreferences.Get($"{nameof(CorePackageInstaller)}.Assets", false))
            {
                EditorPreferences.Set($"{nameof(CorePackageInstaller)}.Assets", PackageInstaller.TryInstallAssets(hiddenPath, defaultPath));
            }
        }
    }
}
