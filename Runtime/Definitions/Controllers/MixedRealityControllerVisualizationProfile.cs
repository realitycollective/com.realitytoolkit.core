// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using RealityCollective.Attributes;
using RealityCollective.Definitions.Utilities;
using RealityToolkit.Definitions.InputSystem;
using RealityToolkit.Definitions.Utilities;
using RealityToolkit.Interfaces.InputSystem.Handlers;
using UnityEngine;

namespace RealityToolkit.Definitions.Controllers
{
    [CreateAssetMenu(menuName = "Reality Toolkit/Input System/Controller Visualization Profile", fileName = "MixedRealityControllerVisualizationProfile", order = (int)CreateProfileMenuItemIndices.ControllerVisualization)]
    public class MixedRealityControllerVisualizationProfile : BaseMixedRealityProfile
    {
        [SerializeField]
        [Implements(typeof(IMixedRealityControllerVisualizer), TypeGrouping.ByNamespaceFlat)]
        [Tooltip("The concrete Controller Visualizer component to use on the rendered controller model.")]
        private SystemType controllerVisualizationType = null;

        /// <summary>
        /// The concrete Controller Visualizer component to use on the rendered controller model
        /// </summary>
        public SystemType ControllerVisualizationType
        {
            get => controllerVisualizationType;
            private set => controllerVisualizationType = value;
        }

        [SerializeField]
        [Tooltip("Use the platform SDK to load the default controller models.")]
        private bool useDefaultModels = false;

        /// <summary>
        /// User the controller model loader provided by the SDK, or provide override models.
        /// </summary>
        public bool UseDefaultModels
        {
            get => useDefaultModels;
            private set => useDefaultModels = value;
        }

        [Prefab]
        [SerializeField]
        private GameObject model = null;

        public GameObject LeftHandModel
        {
            get => model;
            private set => model = value;
        }

        [SerializeField]
        [Tooltip("This is the controller pose that this visualization will synchronize it's position and rotation with.")]
        private MixedRealityInputAction pointerPose = MixedRealityInputAction.None;

        /// <summary>
        /// This is the controller pose that this visualization will synchronize it's position and rotation with.
        /// </summary>
        public MixedRealityInputAction PointerPose
        {
            get => pointerPose;
            private set => pointerPose = value;
        }

        [SerializeField]
        [Tooltip("This is the controller pose that this visualization will synchronize it's position and rotation with.")]
        private MixedRealityInputAction alternatePointerPose = MixedRealityInputAction.None;

        /// <summary>
        /// This is the controller pose that this visualization will synchronize it's position and rotation with.
        /// </summary>
        public MixedRealityInputAction AlternatePointerPose
        {
            get => alternatePointerPose;
            private set => alternatePointerPose = value;
        }
    }
}
