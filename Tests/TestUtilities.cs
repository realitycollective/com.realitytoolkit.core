// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using NUnit.Framework;
using System.Linq;
using RealityToolkit.ServiceFramework.Definitions;
using UnityEditor.SceneManagement;
using UnityEngine;
using XRTK.Definitions;
using XRTK.Definitions.LocomotionSystem;
using XRTK.Interfaces.LocomotionSystem;
using XRTK.Editor.Extensions;
using XRTK.Services;

namespace XRTK.Tests
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
            Assert.IsTrue(!MixedRealityToolkit.IsInitialized);
            Assert.AreEqual(0, MixedRealityToolkit.ActiveSystems.Count);
            InitializeMixedRealityToolkit();

            // Tests
            Assert.IsTrue(MixedRealityToolkit.IsInitialized);
            Assert.IsNotNull(MixedRealityToolkit.Instance);
            Assert.IsFalse(MixedRealityToolkit.HasActiveProfile);

            ServiceManagerRootProfile configuration;

            if (useDefaultProfile)
            {
                configuration = GetDefaultMixedRealityProfile<ServiceManagerRootProfile>();
                MixedRealityToolkit.TryGetSystemProfile<ILocomotionSystem, LocomotionSystemProfile>(out var locomotionSystemProfile);
                Debug.Assert(locomotionSystemProfile != null);
            }
            else
            {
                configuration = ScriptableObject.CreateInstance<ServiceManagerRootProfile>();
            }

            Assert.IsTrue(configuration != null, "Failed to find the Default Mixed Reality Root Profile");
            MixedRealityToolkit.Instance.ResetProfile(configuration);
            Assert.IsTrue(MixedRealityToolkit.Instance.ActiveProfile != null);  //SJ - Why is the Active Profile Null?
            Assert.IsTrue(MixedRealityToolkit.IsInitialized);
        }

        private static T GetDefaultMixedRealityProfile<T>() where T : BaseProfile
        {
            return ScriptableObjectExtensions.GetAllInstances<T>().FirstOrDefault(profile => profile.name.Equals(typeof(T).Name));
        }
    }
}
