// Copyright (c) Reality Collective. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using RealityCollective.Extensions;
using RealityCollective.ServiceFramework.Services;
using RealityToolkit.Input.Interfaces;
using RealityToolkit.Utilities;
using UnityEditor;
using UnityEngine;

namespace RealityToolkit.Editor.Utilities
{
    /// <summary>
    /// Canvas editor extension making sure to prompt the user to add the <see cref="CanvasUtility"/> to world spaces canvases,
    /// so input can work.
    /// </summary>
    [CustomEditor(typeof(Canvas))]
    public class CanvasEditorExtension : UnityEditor.Editor
    {
        private static readonly string dialogText = $"In order for the {nameof(IInputService)} to work properly with this world space canvas we'd like to add the {nameof(CanvasUtility)} component to it.";
        private Canvas canvas;
        private bool hasUtility;

        private static bool IsUtilityValid =>
            ServiceManager.IsActiveAndInitialized &&
            ServiceManager.Instance.HasActiveProfile &&
            ServiceManager.Instance.TryGetService<IInputService>(out _);

        private bool CanUpdateSettings
        {
            get
            {
                if (!ServiceManager.IsActiveAndInitialized ||
                    !MixedRealityPreferences.ShowCanvasUtilityPrompt)
                {
                    return false;
                }

                var utility = canvas.GetComponent<CanvasUtility>();

                hasUtility = utility != null;

                return hasUtility && !IsUtilityValid ||
                       !hasUtility && IsUtilityValid;
            }
        }

        private void OnEnable()
        {
            canvas = (Canvas)target;

            if (CanUpdateSettings)
            {
                UpdateCanvasSettings();
            }
        }

        public override void OnInspectorGUI()
        {
            EditorGUI.BeginChangeCheck();
            base.OnInspectorGUI();

            if (EditorGUI.EndChangeCheck() && CanUpdateSettings)
            {
                UpdateCanvasSettings();
            }
        }

        private void UpdateCanvasSettings()
        {
            if (!ServiceManager.Instance.TryGetService<IInputService>(out _))
            {
                return;
            }

            if (IsUtilityValid &&
                canvas.isRootCanvas &&
                canvas.renderMode == RenderMode.WorldSpace)
            {
                var selection = EditorUtility.DisplayDialogComplex("Attention!", dialogText, "OK", "Cancel", "Dismiss Forever");
                switch (selection)
                {
                    case 0:
                        canvas.EnsureComponent<CanvasUtility>();
                        break;
                    case 2:
                        MixedRealityPreferences.ShowCanvasUtilityPrompt = false;
                        break;
                }
            }
        }
    }
}