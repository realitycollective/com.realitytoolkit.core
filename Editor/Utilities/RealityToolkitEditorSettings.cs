// Copyright (c) Reality Collective. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.IO;
using UnityEditor;
using UnityEditor.Build;
using UnityEngine;

namespace RealityToolkit.Editor.Utilities
{
    /// <summary>
    /// Sets Force Text Serialization and visible meta files in all projects that use the Reality Toolkit.
    /// </summary>
    [InitializeOnLoad]
    public class RealityToolkitEditorSettings : IActiveBuildTargetChanged
    {
        private static readonly string ignoreKey = $"{Application.productName}_XRTK_Editor_IgnoreSettingsPrompts";
        private static readonly string sessionKey = $"{Application.productName}_XRTK_Editor_ShownSettingsPrompts";
        private static readonly string visibleMetaVersionControlMode = "Visible Meta Files";

        /// <summary>
        /// Constructor.
        /// </summary>
        static RealityToolkitEditorSettings()
        {
            if (!Application.isBatchMode)
            {
                EditorApplication.delayCall += CheckSettings;
            }
        }

        /// <summary>
        /// Check the Reality Toolkit's settings.
        /// </summary>
        public static void CheckSettings()
        {
            EditorPrefs.SetBool($"{Application.productName}_XRTK", true);

            if (Application.isPlaying ||
                EditorPrefs.GetBool(ignoreKey, false) ||
                !SessionState.GetBool(sessionKey, true))
            {
                return;
            }

            SessionState.SetBool(sessionKey, false);

            var message = "The Reality Toolkit needs to apply the following settings to your project:\n\n";

            var forceTextSerialization = EditorSettings.serializationMode == SerializationMode.ForceText;

            if (!forceTextSerialization)
            {
                message += "- Force Text Serialization\n";
            }

#if UNITY_2020_1_OR_NEWER
            var visibleMetaFiles = VersionControlSettings.mode.Equals(visibleMetaVersionControlMode);
#else
            var visibleMetaFiles = EditorSettings.externalVersionControl.Equals(visibleMetaVersionControlMode);
#endif

            if (!visibleMetaFiles)
            {
                message += "- Visible meta files\n";
            }

            if (EditorUserBuildSettings.selectedBuildTargetGroup == BuildTargetGroup.WSA)
            {
                message += "- Enable Shared Depth Buffer in the XR SDK Settings\n";
            }

            message += "\nWould you like to make these changes?\n\n";

            if (!forceTextSerialization || !visibleMetaFiles)
            {
                var choice = EditorUtility.DisplayDialogComplex("Apply Reality Toolkit Default Settings?", message, "Apply", "Ignore", "Later");

                switch (choice)
                {
                    case 0:
                        EditorSettings.serializationMode = SerializationMode.ForceText;
#if UNITY_2020_1_OR_NEWER
                        VersionControlSettings.mode = visibleMetaVersionControlMode;
#else
                        EditorSettings.externalVersionControl = visibleMetaVersionControlMode;
#endif
                        AssetDatabase.SaveAssets();

                        if (!EditorApplication.isUpdating)
                        {
                            AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);
                        }
                        break;
                    case 1:
                        EditorPrefs.SetBool(ignoreKey, true);
                        break;
                    case 2:
                        break;
                }
            }
        }

        #region IActiveBuildTargetChanged Implementation

        /// <inheritdoc />
        public int callbackOrder => 0;

        /// <inheritdoc />
        void IActiveBuildTargetChanged.OnActiveBuildTargetChanged(BuildTarget previousTarget, BuildTarget newTarget)
        {
            SessionState.SetBool(sessionKey, true);
            CheckSettings();
        }

        #endregion IActiveBuildTargetChanged Implementation

        private static string settingsFilePath;
        private static byte[] settingsBackup;

        private static void ImportCallback(string packageName)
        {
            // Restore backup of TMP Settings from byte[]
            File.WriteAllBytes(settingsFilePath, settingsBackup);
            AssetDatabase.Refresh();
            AssetDatabase.importPackageCompleted -= ImportCallback;
        }
    }
}
