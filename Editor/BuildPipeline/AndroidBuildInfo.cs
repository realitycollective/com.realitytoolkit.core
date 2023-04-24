// Copyright (c) Reality Collective. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using RealityCollective.ServiceFramework.Attributes;
using RealityCollective.ServiceFramework.Definitions.Platforms;
using RealityCollective.ServiceFramework.Interfaces;
using RealityCollective.ServiceFramework.Services;
using System.Linq;
using UnityEditor;
using UnityEditor.Build.Reporting;

namespace RealityToolkit.Editor.BuildPipeline
{
    [RuntimePlatform(typeof(AndroidPlatform))]
    public class AndroidBuildInfo : BuildInfo
    {
        /// <inheritdoc />
        public override BuildTarget BuildTarget => BuildTarget.Android;

        /// <inheritdoc />
        public override IPlatform BuildPlatform => new AndroidPlatform();

        /// <inheritdoc />
        public override string ExecutableFileExtension => ".apk";

        /// <inheritdoc />
        public override void OnPreProcessBuild(BuildReport report)
        {
            if (!ServiceManager.ActivePlatforms.Contains(BuildPlatform) ||
                EditorUserBuildSettings.activeBuildTarget != BuildTarget)
            {
                return;
            }

            if (VersionCode.HasValue)
            {
                PlayerSettings.Android.bundleVersionCode = VersionCode.Value;
            }
            else
            {
                // Usually version codes are unique and not tied to the usual semver versions
                // see https://developer.android.com/studio/publish/versioning#appversioning
                // versionCode - A positive integer used as an internal version number.
                // This number is used only to determine whether one version is more recent than another,
                // with higher numbers indicating more recent versions. The Android system uses the
                // versionCode value to protect against downgrades by preventing users from installing
                // an APK with a lower versionCode than the version currently installed on their device.
                PlayerSettings.Android.bundleVersionCode++;
            }

            if (BuildPlatform.GetType() == typeof(AndroidPlatform))
            {
                // TODO generate manifest
            }
        }
    }
}
