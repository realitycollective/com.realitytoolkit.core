﻿// Copyright (c) Reality Collective. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using RealityCollective.Editor.Extensions;
using RealityCollective.Editor.Utilities;
using RealityCollective.Extensions;
using RealityCollective.ServiceFramework.Definitions.Platforms;
using RealityCollective.ServiceFramework.Interfaces;
using RealityCollective.ServiceFramework.Services;
using RealityCollective.Utilities.Editor;
using RealityToolkit.Editor.Utilities.SymbolicLinks;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace RealityToolkit.Editor
{
    public static class RealityToolkitPreferences
    {
        public const string Editor_Menu_Keyword = ToolkitPreferences.Editor_Menu_Keyword + "/Reality Toolkit";

        private static readonly string[] Package_Keywords = { "Reality", "Toolkit", "Mixed", "Augmented", "Virtual" };

        #region Ignore startup settings prompt

        private static readonly GUIContent IgnoreContent = new GUIContent("Ignore settings prompt on startup", "Prevents settings dialog popup from showing on startup.\n\nThis setting applies to all projects using the RealityToolkit.");
        private static readonly string IgnoreKey = $"{Application.productName}_RealityToolkit_Editor_IgnoreSettingsPrompts";
        private static bool ignorePrefLoaded;
        private static bool ignoreSettingsPrompt;

        /// <summary>
        /// Should the settings prompt show on startup?
        /// </summary>
        public static bool IgnoreSettingsPrompt
        {
            get
            {
                if (!ignorePrefLoaded)
                {
                    ignoreSettingsPrompt = EditorPrefs.GetBool(IgnoreKey, false);
                    ignorePrefLoaded = true;
                }

                return ignoreSettingsPrompt;
            }
            set => EditorPrefs.SetBool(IgnoreKey, ignoreSettingsPrompt = value);
        }

        #endregion Ignore startup settings prompt

        #region Show Canvas Utility Prompt

        private static readonly GUIContent CanvasUtilityContent = new GUIContent("Canvas world space utility dialogs", "Enable or disable the dialog popups for the world space canvas settings.\n\nThis setting only applies to the currently running project.");
        private const string CANVAS_KEY = "EnableCanvasUtilityDialog";
        private static bool isCanvasUtilityPrefLoaded;
        private static bool showCanvasUtilityPrompt;

        /// <summary>
        /// Should the <see cref="Canvas"/> utility dialog show when updating the <see cref="RenderMode"/> settings on that component?
        /// </summary>
        public static bool ShowCanvasUtilityPrompt
        {
            get
            {
                if (!isCanvasUtilityPrefLoaded)
                {
                    showCanvasUtilityPrompt = EditorPreferences.Get(CANVAS_KEY, true);
                    isCanvasUtilityPrefLoaded = true;
                }

                return showCanvasUtilityPrompt;
            }
            set => EditorPreferences.Set(CANVAS_KEY, showCanvasUtilityPrompt = value);
        }

        #endregion Show Canvas Utility Prompt

        #region Custom Profile Generation Path

        /// <summary>
        /// The hidden assets path for each package.
        /// </summary>
        public const string HIDDEN_PACKAGE_ASSETS_PATH = "Assets~";

        private static readonly GUIContent GeneratedProfilePathContent = new GUIContent("New Generated Profiles Default Path:", "When generating new profiles, their files are saved in this location.");
        private const string PROFILE_GENERATION_PATH_KEY = "_RealityToolkit_Editor_Profile_Generation_Path";
        public const string DEFAULT_GENERATION_PATH = "Assets/RealityToolkit.Generated/";
        private static string profileGenerationPath;
        private static bool isProfilePathPrefLoaded;

        /// <summary>
        /// The path where all profile files are created by default.
        /// </summary>
        public static string ProfileGenerationPath
        {
            get
            {
                if (!isProfilePathPrefLoaded ||
                    string.IsNullOrWhiteSpace(profileGenerationPath))
                {
                    profileGenerationPath = EditorPreferences.Get(PROFILE_GENERATION_PATH_KEY, DEFAULT_GENERATION_PATH);
                    isProfilePathPrefLoaded = true;
                }

                return profileGenerationPath;
            }
            set
            {
                var newPath = value;
                var root = Path.GetFullPath(Application.dataPath).ForwardSlashes();

                if (!newPath.Contains(root))
                {
                    Debug.LogWarning("Path must be in the Assets folder");
                    newPath = DEFAULT_GENERATION_PATH;
                }

                newPath = newPath.Replace(root, "Assets");
                if (!newPath.EndsWith("/"))
                {
                    newPath += "/";
                }

                EditorPreferences.Set(PROFILE_GENERATION_PATH_KEY, profileGenerationPath = newPath);
            }
        }

        #endregion Custom Profile Generation Path

        #region Symbolic Link Preferences

        private static bool isSymbolicLinkSettingsPathLoaded;
        private static string symbolicLinkSettingsPath = string.Empty;

        /// <summary>
        /// The path to the symbolic link settings found for this project.
        /// </summary>
        public static string SymbolicLinkSettingsPath
        {
            get
            {
                if (!isSymbolicLinkSettingsPathLoaded)
                {
                    symbolicLinkSettingsPath = EditorPreferences.Get("_SymbolicLinkSettingsPath", string.Empty);
                    isSymbolicLinkSettingsPathLoaded = true;
                }

                if (!EditorApplication.isUpdating &&
                    string.IsNullOrEmpty(symbolicLinkSettingsPath))
                {
                    symbolicLinkSettingsPath = AssetDatabase
                        .FindAssets($"t:{nameof(SymbolicLinkSettings)}")
                        .Select(AssetDatabase.GUIDToAssetPath)
                        .OrderBy(x => x)
                        .FirstOrDefault();
                }

                return symbolicLinkSettingsPath;
            }
            set => EditorPreferences.Set("_SymbolicLinkSettingsPath", symbolicLinkSettingsPath = value);
        }

        private static bool isAutoLoadSymbolicLinksLoaded;
        private static bool autoLoadSymbolicLinks = true;

        /// <summary>
        /// Should the project automatically load symbolic links?
        /// </summary>
        public static bool AutoLoadSymbolicLinks
        {
            get
            {
                if (!isAutoLoadSymbolicLinksLoaded)
                {
                    autoLoadSymbolicLinks = EditorPreferences.Get("_AutoLoadSymbolicLinks", true);
                    isAutoLoadSymbolicLinksLoaded = true;
                }

                return autoLoadSymbolicLinks;
            }
            set
            {
                EditorPreferences.Set("_AutoLoadSymbolicLinks", autoLoadSymbolicLinks = value);

                if (autoLoadSymbolicLinks && SymbolicLinker.Settings.IsNull())
                {
                    ScriptableObject.CreateInstance(nameof(SymbolicLinkSettings)).GetOrCreateAsset();
                }
            }
        }

        #endregion Symbolic Link Preferences

        #region Debug Symbolic Links

        private static readonly GUIContent DebugSymbolicContent = new GUIContent("Debug symbolic linking", "Enable or disable the debug information for symbolic linking.\n\nThis setting only applies to the currently running project.");
        private const string SYMBOLIC_DEBUG_KEY = "EnablePackageDebug";
        private static bool isSymbolicDebugPrefLoaded;
        private static bool debugSymbolicInfo;

        /// <summary>
        /// Enabled debugging info for the symbolic linking.
        /// </summary>
        public static bool DebugSymbolicInfo
        {
            get
            {
                if (!isSymbolicDebugPrefLoaded)
                {
                    debugSymbolicInfo = EditorPreferences.Get(SYMBOLIC_DEBUG_KEY, Application.isBatchMode);
                    isSymbolicDebugPrefLoaded = true;
                }

                return debugSymbolicInfo;
            }
            set => EditorPreferences.Set(SYMBOLIC_DEBUG_KEY, debugSymbolicInfo = value);
        }

        #endregion Debug Symbolic Links

        #region Current Platform Target

        private static bool isCurrentPlatformPreferenceLoaded;

        private static IPlatform currentPlatformTarget = null;

        /// <summary>
        /// The current <see cref="IPlatform"/> target.
        /// </summary>
        public static IPlatform CurrentPlatformTarget
        {
            get
            {
                if (!isCurrentPlatformPreferenceLoaded || currentPlatformTarget == null)
                {
                    isCurrentPlatformPreferenceLoaded = true;

                    if (TypeExtensions.TryResolveType(EditorPreferences.Get(nameof(CurrentPlatformTarget), Guid.Empty.ToString()), out var platform))
                    {
                        foreach (var availablePlatform in ServiceManager.AvailablePlatforms)
                        {
                            if (availablePlatform is AllPlatforms ||
                                availablePlatform is EditorPlatform ||
                                availablePlatform is CurrentBuildTargetPlatform)
                            {
                                continue;
                            }

                            if (availablePlatform.GetType() == platform)
                            {
                                currentPlatformTarget = availablePlatform;
                                break;
                            }
                        }
                    }

                    if (currentPlatformTarget == null)
                    {
                        var possibleBuildTargets = new List<IPlatform>();

                        foreach (var availablePlatform in ServiceManager.AvailablePlatforms)
                        {
                            if (availablePlatform is AllPlatforms ||
                                availablePlatform is EditorPlatform ||
                                availablePlatform is CurrentBuildTargetPlatform)
                            {
                                continue;
                            }

                            foreach (var buildTarget in availablePlatform.ValidBuildTargets)
                            {
                                if (EditorUserBuildSettings.activeBuildTarget == buildTarget)
                                {
                                    possibleBuildTargets.Add(availablePlatform);
                                }
                            }
                        }

                        Debug.Assert(possibleBuildTargets.Count > 0);

                        currentPlatformTarget = possibleBuildTargets.Count == 1
                            ? possibleBuildTargets[0]
                            : possibleBuildTargets.FirstOrDefault(p => p.PlatformOverrides == null ||
                                                                       p.PlatformOverrides.Length == 0);
                        return currentPlatformTarget;
                    }
                }

                return currentPlatformTarget;
            }
            set
            {
                if (value is AllPlatforms ||
                    value is EditorPlatform ||
                    value is CurrentBuildTargetPlatform)
                {
                    return;
                }

                currentPlatformTarget = value;

                EditorPreferences.Set(nameof(CurrentPlatformTarget),
                    currentPlatformTarget != null
                        ? currentPlatformTarget.GetType().GUID.ToString()
                        : Guid.Empty.ToString());
            }
        }

        #endregion Current Platform Target

        [SettingsProvider]
        private static SettingsProvider Preferences()
        {
            return new SettingsProvider("Preferences/RealityToolkit", SettingsScope.User, Package_Keywords)
            {
                label = "RealityToolkit",
                guiHandler = OnPreferencesGui,
                keywords = new HashSet<string>(Package_Keywords)
            };
        }

        private static void OnPreferencesGui(string searchContext)
        {
            var prevLabelWidth = EditorGUIUtility.labelWidth;
            EditorGUIUtility.labelWidth = 200f;

            #region Ignore Settings Preference

            EditorGUI.BeginChangeCheck();
            ignoreSettingsPrompt = EditorGUILayout.Toggle(IgnoreContent, IgnoreSettingsPrompt);

            if (EditorGUI.EndChangeCheck())
            {
                IgnoreSettingsPrompt = ignoreSettingsPrompt;
            }

            #endregion Ignore Settings Preference

            #region Show Canvas Prompt Preference

            EditorGUI.BeginChangeCheck();
            showCanvasUtilityPrompt = EditorGUILayout.Toggle(CanvasUtilityContent, ShowCanvasUtilityPrompt);

            if (EditorGUI.EndChangeCheck())
            {
                ShowCanvasUtilityPrompt = showCanvasUtilityPrompt;
            }

            if (!ShowCanvasUtilityPrompt)
            {
                EditorGUILayout.HelpBox("Be aware that if a Canvas needs to receive input events it is required to have the CanvasUtility attached or the Focus Provider's UIRaycast Camera assigned to the canvas' camera reference.", MessageType.Warning);
            }

            #endregion Show Canvas Prompt Preference

            #region Generated Profile path Preference

            EditorGUILayout.LabelField(GeneratedProfilePathContent);
            EditorGUILayout.LabelField(ProfileGenerationPath);

            if (GUILayout.Button("Choose a new default path"))
            {
                ProfileGenerationPath = EditorUtility.OpenFolderPanel("Default Profile Generation Location", profileGenerationPath, string.Empty);
            }

            #endregion Generated Profile path Preference

            #region Script Reloading Preference

            EditorGUI.BeginChangeCheck();
            var scriptLock = EditorGUILayout.Toggle("Is Script Reloading locked?", EditorAssemblyReloadManager.LockReloadAssemblies);

            if (EditorGUI.EndChangeCheck())
            {
                EditorAssemblyReloadManager.LockReloadAssemblies = scriptLock;
            }

            #endregion Script Reloading Preference

            #region Symbolic Links Preferences

            EditorGUI.BeginChangeCheck();
            autoLoadSymbolicLinks = EditorGUILayout.Toggle("Auto Load Symbolic Links", AutoLoadSymbolicLinks);

            if (EditorGUI.EndChangeCheck())
            {
                AutoLoadSymbolicLinks = autoLoadSymbolicLinks;

                if (AutoLoadSymbolicLinks)
                {
                    EditorApplication.delayCall += () => SymbolicLinker.RunSync();
                }
            }

            EditorGUI.BeginChangeCheck();
            var symbolicLinkSettings = EditorGUILayout.ObjectField("Symbolic Link Settings", SymbolicLinker.Settings, typeof(SymbolicLinkSettings), false) as SymbolicLinkSettings;

            if (EditorGUI.EndChangeCheck())
            {
                if (symbolicLinkSettings != null)
                {
                    var shouldSync = string.IsNullOrEmpty(SymbolicLinkSettingsPath);
                    SymbolicLinkSettingsPath = AssetDatabase.GetAssetPath(symbolicLinkSettings);
                    SymbolicLinker.Settings = AssetDatabase.LoadAssetAtPath<SymbolicLinkSettings>(SymbolicLinkSettingsPath);

                    if (shouldSync)
                    {
                        EditorApplication.delayCall += () => SymbolicLinker.RunSync();
                    }
                }
                else
                {
                    SymbolicLinkSettingsPath = string.Empty;
                    SymbolicLinker.Settings = null;
                }
            }

            EditorGUI.BeginChangeCheck();
            debugSymbolicInfo = EditorGUILayout.Toggle(DebugSymbolicContent, DebugSymbolicInfo);

            if (EditorGUI.EndChangeCheck())
            {
                DebugSymbolicInfo = debugSymbolicInfo;
            }

            #endregion Symbolic Links Preferences

            EditorGUIUtility.labelWidth = prevLabelWidth;
        }
    }
}
