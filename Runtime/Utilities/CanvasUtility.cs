// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

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

        private void Awake()
        {
            if (Canvas == null)
            {
                Canvas = GetComponent<Canvas>();
            }
        }

        private void Start()
        {
            Debug.Assert(Canvas != null);

            if (ServiceManager.Instance.TryGetService<IMixedRealityInputSystem>(out var inputSystem) &&
                Canvas.isRootCanvas && Canvas.renderMode == RenderMode.WorldSpace)
            {
                Canvas.worldCamera = inputSystem.FocusProvider.UIRaycastCamera;
            }
        }
    }
}
