// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using NUnit.Framework;
using RealityToolkit.Definitions;
using RealityToolkit.Definitions.LocomotionSystem;
using RealityToolkit.Editor.Extensions;
using RealityToolkit.Interfaces.LocomotionSystem;
using RealityToolkit.ServiceFramework.Definitions;
using RealityToolkit.Services;
using System.Linq;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace RealityToolkit.Tests
{
    public static class TestUtilities
    {
        public static void InitializeMixedRealityToolkit()
        {
            MixedRealityToolkit.ConfirmInitialized();
        }

        public static void CleanupScene()
        {
            EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects, NewSceneMode.Single);
        }

        public static void InitializeMixedRealityToolkitScene(bool useDefaultProfile)
        {
            // Setup
            CleanupScene();
            Assert.IsTrue(!MixedRealityToolkit.IsInitialized, "Toolkit was initialised when it was not meant to be");
            Assert.AreEqual(0, MixedRealityToolkit.ActiveSystems.Count, "There were services registered with the toolkit when there should be none");
            var instance = MixedRealityToolkit.Instance;
            InitializeMixedRealityToolkit();

            // Tests
            Assert.IsTrue(MixedRealityToolkit.IsInitialized, "Toolkit was NOT initialised when it should be");
            Assert.IsNotNull(MixedRealityToolkit.Instance, "No instance of the toolkit found");
            Assert.IsFalse(MixedRealityToolkit.HasActiveProfile, "A profile was found registered to the toolkit when none should be");

            MixedRealityToolkitRootProfile configuration;

            if (useDefaultProfile)
            {
                configuration = GetDefaultMixedRealityProfile<MixedRealityToolkitRootProfile>();
                MixedRealityToolkit.TryGetSystemProfile<ILocomotionSystem, LocomotionSystemProfile>(out var locomotionSystemProfile);
                Debug.Assert(locomotionSystemProfile != null);
            }
            else
            {
                configuration = ScriptableObject.CreateInstance<MixedRealityToolkitRootProfile>();
            }

            Assert.IsTrue(configuration != null, "Failed to find the Default Mixed Reality Root Profile");
            MixedRealityToolkit.Instance.ResetProfile(configuration);
            Assert.IsTrue(MixedRealityToolkit.Instance.ActiveProfile != null, "No profile was found after the toolkit was reset with a profile");
            Assert.IsTrue(MixedRealityToolkit.IsInitialized, "Toolkit was NOT initialised when it should be after receiving configuration");
        }

        private static T GetDefaultMixedRealityProfile<T>() where T : BaseProfile
        {
            return ScriptableObjectExtensions.GetAllInstances<T>().FirstOrDefault(profile => profile.name.Equals(typeof(T).Name));
        }
    }
}
