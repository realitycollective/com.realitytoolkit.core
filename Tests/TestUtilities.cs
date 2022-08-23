// Copyright (c) Reality Collective. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using NUnit.Framework;
using RealityCollective.ServiceFramework.Definitions;
using RealityCollective.ServiceFramework.Services;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace RealityToolkit.Tests
{
    /// <summary>
    /// Unit testing utilities for the toolkit.
    /// </summary>
    public static class TestUtilities
    {
        /// <summary>
        /// Resets the active scene to a single default scene with a camera and a directional light.
        /// </summary>
        public static void CleanupScene() => EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects, NewSceneMode.Single);

        /// <summary>
        /// Performs a clean up by loading a new empty scene and initializes the toolkit in that scene
        /// for further tests.
        /// </summary>
        public static void InitializeRealityToolkit()
        {
            // Setup
            CleanupScene();
            Assert.IsTrue(ServiceManager.Instance == null);
            var managerGameObject = new GameObject(nameof(ServiceManager));
            new ServiceManager(managerGameObject);
            ServiceManager.Instance.ConfirmInitialized();

            // Tests
            Assert.IsTrue(ServiceManager.Instance.IsInitialized);
            Assert.IsNotNull(ServiceManager.Instance);
            Assert.IsFalse(ServiceManager.Instance.HasActiveProfile);

            ServiceProvidersProfile configuration = ScriptableObject.CreateInstance<ServiceProvidersProfile>();
            Assert.IsTrue(configuration != null, "Failed to create the Default Reality Toolkit Profile");

            ServiceManager.Instance.ResetProfile(configuration);
            Assert.IsTrue(ServiceManager.Instance.ActiveProfile != null);
            Assert.IsTrue(ServiceManager.Instance.IsInitialized);
        }
    }
}
