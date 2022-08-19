// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using NUnit.Framework;
using RealityCollective.Editor.Extensions;
using RealityCollective.ServiceFramework.Definitions;
using RealityCollective.ServiceFramework.Services;
using RealityToolkit.LocomotionSystem.Definitions;
using RealityToolkit.LocomotionSystem.Interfaces;
using System.Linq;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace RealityToolkit.Tests
{
    public static class TestUtilities
    {
        public static void InitializeMixedRealityToolkit()
        {
            ServiceManager.Instance.ConfirmInitialized();
        }

        public static void CleanupScene()
        {
            EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects, NewSceneMode.Single);
        }

        public static void InitializeMixedRealityToolkitScene(bool useDefaultProfile)
        {
            // Setup
            CleanupScene();
            Assert.IsTrue(!ServiceManager.Instance.IsInitialized);
            Assert.AreEqual(0, ServiceManager.Instance.ActiveServices.Count);
            InitializeMixedRealityToolkit();

            // Tests
            Assert.IsTrue(ServiceManager.Instance.IsInitialized);
            Assert.IsNotNull(ServiceManager.Instance);
            Assert.IsFalse(ServiceManager.Instance.HasActiveProfile);

            ServiceProvidersProfile configuration;

            if (useDefaultProfile)
            {
                configuration = GetDefaultMixedRealityProfile<ServiceProvidersProfile>();
                ServiceManager.Instance.TryGetServiceProfile<ILocomotionSystem, LocomotionSystemProfile>(out var locomotionSystemProfile);
                Debug.Assert(locomotionSystemProfile != null);
            }
            else
            {
                configuration = ScriptableObject.CreateInstance<ServiceProvidersProfile>();
            }

            Assert.IsTrue(configuration != null, "Failed to find the Default Mixed Reality Root Profile");
            ServiceManager.Instance.ResetProfile(configuration);
            Assert.IsTrue(ServiceManager.Instance.ActiveProfile != null);
            Assert.IsTrue(ServiceManager.Instance.IsInitialized);
        }

        private static T GetDefaultMixedRealityProfile<T>() where T : BaseProfile
        {
            return ScriptableObjectExtensions.GetAllInstances<T>().FirstOrDefault(profile => profile.name.Equals(typeof(T).Name));
        }
    }
}
