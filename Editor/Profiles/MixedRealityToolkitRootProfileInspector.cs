// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.﻿

using System.Collections.Generic;
using System.Linq;
using RealityToolkit.Definitions;
using RealityToolkit.Editor.Utilities;
using RealityToolkit.Interfaces.CameraSystem;
using RealityToolkit.Services;
using UnityEditor;
#if UNITY_2021_1_OR_NEWER
using SceneManagement = UnityEditor.SceneManagement;
#else
using SceneManagement = UnityEditor.Experimental.SceneManagement;
#endif
using UnityEngine;
using RealityToolkit.ServiceFramework.Interfaces;
using RealityToolkit.ServiceFramework.Definitions.Platforms;

namespace RealityToolkit.Editor.Profiles
{
    [CustomEditor(typeof(MixedRealityToolkitRootProfile))]
    public class MixedRealityToolkitRootProfileInspector : MixedRealityServiceProfileInspector
    {
        private MixedRealityToolkitRootProfile rootProfile;
        private bool didPromptToConfigure = false;

        private readonly GUIContent profileLabel = new GUIContent("Profile");

        private int platformIndex;
        private readonly List<IPlatform> platforms = new List<IPlatform>();

        private List<IPlatform> Platforms
        {
            get
            {
                if (platforms.Count == 0)
                {
                    foreach (var availablePlatform in MixedRealityToolkit.AvailablePlatforms)
                    {
                        if (availablePlatform is AllPlatforms ||
                            availablePlatform is EditorPlatform ||
                            availablePlatform is CurrentBuildTargetPlatform)
                        {
                            continue;
                        }

                        platforms.Add(availablePlatform);
                    }

                    for (var i = 0; i < platforms.Count; i++)
                    {
                        if (MixedRealityPreferences.CurrentPlatformTarget.GetType() == platforms[i].GetType())
                        {
                            platformIndex = i;
                            break;
                        }
                    }
                }

                return platforms;
            }
        }

        protected override void OnEnable()
        {
            base.OnEnable();

            rootProfile = target as MixedRealityToolkitRootProfile;

            var prefabStage = SceneManagement.PrefabStageUtility.GetCurrentPrefabStage();

            // Create the MixedRealityToolkit object if none exists.
            if (!MixedRealityToolkit.IsInitialized && prefabStage == null && !didPromptToConfigure)
            {
                // Search for all instances, in case we've just hot reloaded the assembly.
                var managerSearch = FindObjectsOfType<MixedRealityToolkit>();

                if (managerSearch.Length == 0)
                {
                    if (EditorUtility.DisplayDialog(
                        "Attention!",
                        "There is no active Mixed Reality Toolkit in your scene!\n\nWould you like to create one now?",
                        "Yes",
                        "Later",
                        DialogOptOutDecisionType.ForThisSession,
                        "XRTK_Prompt_Configure_Scene"))
                    {
                        if (MixedRealityToolkit.TryGetSystem<IMixedRealityCameraSystem>(out var cameraSystem))
                        {
                            var rig = cameraSystem.MainCameraRig.RigTransform;
                            Debug.Assert(rig != null);
                        }

                        MixedRealityToolkit.Instance.ActiveProfile = rootProfile;
                    }
                    else
                    {
                        Debug.LogWarning("No Mixed Reality Toolkit in your scene.");
                        didPromptToConfigure = true;
                    }
                }
            }

            platforms.Clear();
        }

        public override void OnInspectorGUI()
        {
            MixedRealityInspectorUtility.RenderMixedRealityToolkitLogo();
            EditorGUILayout.LabelField("The Mixed Reality Toolkit", MixedRealityInspectorUtility.BoldCenteredHeaderStyle);
            EditorGUILayout.Space();
            EditorGUILayout.Space();
            RenderSystemFields();
        }

        internal void RenderSystemFields()
        {
            var currentPlatform = MixedRealityPreferences.CurrentPlatformTarget;

            for (var i = 0; i < Platforms.Count; i++)
            {
                if (currentPlatform.GetType() == Platforms[i].GetType())
                {
                    platformIndex = i;
                    break;
                }
            }

            EditorGUI.BeginChangeCheck();
            var prevPlatformIndex = platformIndex;
            platformIndex = EditorGUILayout.Popup("Platform Target", platformIndex, Platforms.Select(p => p.Name).ToArray());

            if (EditorGUI.EndChangeCheck())
            {
                for (int i = 0; i < Platforms.Count; i++)
                {
                    if (i == platformIndex)
                    {
                        var platform = Platforms[i];

                        var buildTarget = platform.ValidBuildTargets[0]; // For now just get the highest priority one.

                        if (!EditorUserBuildSettings.SwitchActiveBuildTarget(BuildPipeline.GetBuildTargetGroup(buildTarget), buildTarget))
                        {
                            platformIndex = prevPlatformIndex;
                            Debug.LogWarning($"Failed to switch {platform.Name} active build target to {buildTarget}");
                        }
                        else
                        {
                            MixedRealityPreferences.CurrentPlatformTarget = platform;
                        }
                    }
                }
            }

            RenderConfigurationOptions(true);

            serializedObject.Update();

            EditorGUI.BeginChangeCheck();

            serializedObject.ApplyModifiedProperties();

            if (EditorGUI.EndChangeCheck() &&
                MixedRealityToolkit.IsInitialized &&
                MixedRealityToolkit.Instance.ActiveProfile == rootProfile)
            {
                EditorApplication.delayCall += () => MixedRealityToolkit.Instance.ResetProfile(rootProfile);
            }
        }
    }
}
