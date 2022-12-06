// Copyright (c) Reality Collective. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using RealityCollective.Extensions;
using RealityCollective.ServiceFramework.Services;
using RealityToolkit.InputSystem.Interfaces;
using UnityEngine;

namespace RealityToolkit.Utilities
{
    /// <summary>
    /// Helper class for setting up canvases for use in the XRTK.
    /// </summary>
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Canvas))]
    public class CanvasUtility : MonoBehaviour
    {
        [SerializeField]
        private Canvas canvas;

        /// <summary>
        /// The canvas this helper script is targeting.
        /// </summary>
        public Canvas Canvas
        {
            get => canvas;
            set => canvas = value;
        }

        private void OnEnable()
        {
            if (Canvas.IsNull())
            {
                Canvas = GetComponent<Canvas>();
            }

            Debug.Assert(Canvas != null, $"The {nameof(CanvasUtility)} requires a {nameof(Canvas)} component on the game object.");

            if (ServiceManager.IsActiveAndInitialized &&
                ServiceManager.Instance.TryGetService<IMixedRealityInputSystem>(out var inputSystem) &&
                Canvas.isRootCanvas && Canvas.renderMode == RenderMode.WorldSpace)
            {
                Canvas.worldCamera = inputSystem.FocusProvider.UIRaycastCamera;
            }
        }
    }
}
