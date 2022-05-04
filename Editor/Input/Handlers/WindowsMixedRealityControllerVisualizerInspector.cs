﻿using RealityToolkit.Utilities.UX.Controllers;
using UnityEditor;

namespace RealityToolkit.Editor.Input.Handlers
{
    [CustomEditor(typeof(WindowsMixedRealityControllerVisualizer))]
    public class WindowsMixedRealityControllerVisualizerInspector : DefaultMixedRealityControllerVisualizerInspector
    {
        private SerializedProperty touchpadTouchVisualizer;

        protected override void OnEnable()
        {
            base.OnEnable();

            touchpadTouchVisualizer = serializedObject.FindProperty("touchpadTouchVisualizer");
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            EditorGUILayout.PropertyField(touchpadTouchVisualizer);
        }
    }
}