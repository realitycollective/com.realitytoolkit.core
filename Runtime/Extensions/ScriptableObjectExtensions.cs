﻿// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace XRTK.Extensions
{
    /// <summary>
    /// Extensions for <see cref="ScriptableObject"/>s
    /// </summary>
    public static class ScriptableObjectExtensions
    {

#if UNITY_EDITOR

        /// <summary>
        /// Creates, saves, and then optionally selects a new asset for the target <see cref="ScriptableObject"/>.
        /// </summary>
        /// <param name="scriptableObject"><see cref="ScriptableObject"/> you want to create an asset file for.</param>
        /// <param name="ping">The new asset should be selected and opened in the inspector.</param>
        public static ScriptableObject CreateAsset(this ScriptableObject scriptableObject, bool ping = true)
        {
            return CreateAsset(scriptableObject, null, ping);
        }

        /// <summary>
        /// Creates, saves, and then opens a new asset for the target <see cref="ScriptableObject"/>.
        /// </summary>
        /// <param name="scriptableObject"><see cref="ScriptableObject"/> you want to create an asset file for.</param>
        /// <param name="path">Optional path for the new asset.</param>
        /// <param name="ping">The new asset should be selected and opened in the inspector.</param>
        public static ScriptableObject CreateAsset(this ScriptableObject scriptableObject, string path, bool ping = true)
        {
            return CreateAsset(scriptableObject, path, null, ping);
        }

        /// <summary>
        /// Creates, saves, and then opens a new asset for the target <see cref="ScriptableObject"/>.
        /// </summary>
        /// <param name="scriptableObject"><see cref="ScriptableObject"/> you want to create an asset file for.</param>
        /// <param name="path">Optional path for the new asset.</param>
        /// <param name="fileName">Optional filename for the new asset.</param>
        /// <param name="ping">The new asset should be selected and opened in the inspector.</param>
        public static ScriptableObject CreateAsset(this ScriptableObject scriptableObject, string path, string fileName, bool ping)
        {
            var name = string.IsNullOrEmpty(fileName) ? $"{scriptableObject.GetType().Name}" : fileName;

            name = name.Replace(" ", string.Empty);

            path = path.Replace(".asset", string.Empty);

            if (!string.IsNullOrWhiteSpace(Path.GetExtension(path)))
            {
                var subtractedPath = path.Substring(path.LastIndexOf("/", StringComparison.Ordinal));
                path = path.Replace(subtractedPath, string.Empty);
            }

            if (!Directory.Exists(Path.GetFullPath(path)))
            {
                Directory.CreateDirectory(Path.GetFullPath(path));
            }

            path = path.Replace($"{Directory.GetParent(Application.dataPath).FullName}\\", string.Empty);

            string assetPathAndName = AssetDatabase.GenerateUniqueAssetPath($"{path}/{name}.asset");

            AssetDatabase.CreateAsset(scriptableObject, assetPathAndName);
            AssetDatabase.SaveAssets();

            if (!EditorApplication.isUpdating)
            {
                AssetDatabase.Refresh();
            }

            scriptableObject = AssetDatabase.LoadAssetAtPath<ScriptableObject>(assetPathAndName);

            if (ping)
            {
                EditorApplication.delayCall += () =>
                {
                    EditorUtility.FocusProjectWindow();
                    EditorGUIUtility.PingObject(scriptableObject);
                    Selection.activeObject = scriptableObject;
                };
            }

            Debug.Assert(scriptableObject != null);

            return scriptableObject;
        }

        /// <summary>
        /// Attempts to find the asset associated to the instance of the <see cref="ScriptableObject"/>, if none is found a new asset is created.
        /// </summary>
        /// <param name="scriptableObject"><see cref="ScriptableObject"/> you want to create an asset file for.</param>
        /// <param name="ping">The new asset should be selected and opened in the inspector.</param>
        public static ScriptableObject GetOrCreateAsset(this ScriptableObject scriptableObject, bool ping = true)
        {
            return GetOrCreateAsset(scriptableObject, null, ping);
        }

        /// <summary>
        /// Attempts to find the asset associated to the instance of the <see cref="ScriptableObject"/>, if none is found a new asset is created.
        /// </summary>
        /// <param name="scriptableObject"><see cref="ScriptableObject"/> you want to create an asset file for.</param>
        /// <param name="path">Optional path for the new asset.</param>
        /// <param name="ping">The new asset should be selected and opened in the inspector.</param>
        public static ScriptableObject GetOrCreateAsset(this ScriptableObject scriptableObject, string path, bool ping = true)
        {
            return GetOrCreateAsset(scriptableObject, path, null, ping);
        }

        /// <summary>
        /// Attempts to find the asset associated to the instance of the <see cref="ScriptableObject"/>, if none is found a new asset is created.
        /// </summary>
        /// <param name="scriptableObject"><see cref="ScriptableObject"/> you want get or create an asset file for.</param>
        /// <param name="path">Optional path for the new asset.</param>
        /// <param name="fileName">Optional filename for the new asset.</param>
        /// <param name="ping">The new asset should be selected and opened in the inspector.</param>
        public static ScriptableObject GetOrCreateAsset(this ScriptableObject scriptableObject, string path, string fileName, bool ping)
        {
            return !AssetDatabase.TryGetGUIDAndLocalFileIdentifier(scriptableObject, out var guid, out long _)
                ? scriptableObject.CreateAsset(path, fileName, ping)
                : AssetDatabase.LoadAssetAtPath<ScriptableObject>(AssetDatabase.GUIDToAssetPath(guid));
        }

        /// <summary>
        /// Gets all the scriptable object instances in the project.
        /// </summary>
        /// <typeparam name="T">The Type of <see cref="ScriptableObject"/> you're wanting to find instances of.</typeparam>
        /// <returns>An Array of instances for the type.</returns>
        public static T[] GetAllInstances<T>() where T : ScriptableObject
        {
            // FindAssets uses tags check documentation for more info
            var guids = AssetDatabase.FindAssets($"t:{typeof(T).Name}");
            var instances = new T[guids.Length];

            for (int i = 0; i < guids.Length; i++)
            {
                instances[i] = AssetDatabase.LoadAssetAtPath<T>(AssetDatabase.GUIDToAssetPath(guids[i]));
            }

            return instances;
        }
#endif
    }
}