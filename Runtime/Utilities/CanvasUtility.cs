// Copyright (c) Reality Collective. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using RealityCollective.Extensions;
using RealityCollective.ServiceFramework.Services;
using RealityToolkit.Input.Interfaces;
using UnityEngine;

namespace RealityToolkit.Utilities
{
    /// <summary>
    /// Utility component for setting up <see cref="UnityEngine.Canvas"/>es for use with the
    /// <see cref="IInputService"/>.
    /// </summary>
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Canvas))]
    public class CanvasUtility : MonoBehaviour
    {
        [SerializeField]
        private Canvas canvas;

        private void OnEnable()
        {
            if (canvas.IsNull())
            {
                canvas = GetComponent<Canvas>();
            }

            Debug.Assert(canvas.IsNotNull(), $"The {nameof(CanvasUtility)} requires a {nameof(Canvas)} component on the game object.");

            if (ServiceManager.IsActiveAndInitialized &&
                ServiceManager.Instance.TryGetService<IInputService>(out var inputSystem) &&
                canvas.isRootCanvas && canvas.renderMode == RenderMode.WorldSpace)
            {
                canvas.worldCamera = inputSystem.FocusProvider.UIRaycastCamera;
            }
        }
    }
}
