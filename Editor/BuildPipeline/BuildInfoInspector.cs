// Copyright (c) Reality Collective. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEditor;
using UnityEditor.Build;

namespace RealityToolkit.Editor.BuildPipeline
{
    [CustomEditor(typeof(BuildInfo), true)]
    public class BuildInfoInspector : UnityEditor.Editor
    {
        private SerializedProperty autoIncrement;
        private SerializedProperty bundleIdentifier;
        private SerializedProperty install;

        protected BuildInfo buildInfo;

        protected virtual void OnEnable()
        {
            autoIncrement = serializedObject.FindProperty(nameof(autoIncrement));
            bundleIdentifier = serializedObject.FindProperty(nameof(bundleIdentifier));
            install = serializedObject.FindProperty(nameof(install));

            buildInfo = (BuildInfo)target;
        }

        /// <inheritdoc />
        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(bundleIdentifier);

            if (EditorGUI.EndChangeCheck())
            {
                var buildTargetGroup = UnityEditor.BuildPipeline.GetBuildTargetGroup(buildInfo.BuildTarget);
#if UNITY_2023_1_OR_NEWER
                var namedBuildTarget = NamedBuildTarget.FromBuildTargetGroup(buildTargetGroup);
                PlayerSettings.SetApplicationIdentifier(namedBuildTarget, bundleIdentifier.stringValue);
#else
                PlayerSettings.SetApplicationIdentifier(buildTargetGroup, bundleIdentifier.stringValue);
#endif
            }

            EditorGUILayout.PropertyField(autoIncrement);
            EditorGUILayout.PropertyField(install);

            serializedObject.ApplyModifiedProperties();
        }
    }
}
