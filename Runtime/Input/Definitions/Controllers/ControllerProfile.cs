// Copyright (c) Reality Collective. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using RealityCollective.Attributes;
using RealityCollective.Definitions.Utilities;
using RealityCollective.ServiceFramework.Definitions;
using RealityToolkit.Input.Controllers;
using RealityToolkit.Input.Interactions.Interactors;
using RealityToolkit.Input.Interfaces.Controllers;
using UnityEngine;

namespace RealityToolkit.Definitions.Controllers
{
    /// <summary>
    /// Configuration profile for configuring a supported <see cref="IController"/> in the application.
    /// </summary>
    public class ControllerProfile : BaseProfile
    {
        [SerializeField]
        [Implements(typeof(IController), TypeGrouping.ByNamespaceFlat)]
        private SystemType controllerType = null;

        /// <summary>
        /// The type of controller this mapping corresponds to.
        /// </summary>
        public SystemType ControllerType
        {
            get => controllerType;
            set => controllerType = value;
        }

        [SerializeField]
        private Handedness handedness = Handedness.None;

        public Handedness Handedness
        {
            get => handedness;
            set => handedness = value;
        }

        [SerializeField, Tooltip("The prefab spawned to visualize the controller.")]
        private BaseControllerVisualizer controllerPrefab = null;

        /// <summary>
        /// The prefab spawned to visualize the controller.
        /// </summary>
        public BaseControllerVisualizer ControllerPrefab => controllerPrefab;

        [SerializeField]
        private bool useCustomInteractions = true;

        internal bool UseCustomInteractions
        {
            get => useCustomInteractions;
            set => useCustomInteractions = value;
        }

        [SerializeField]
        private InteractionMappingProfile[] interactionMappingProfiles = new InteractionMappingProfile[0];

        /// <summary>
        /// Details the list of available interaction profiles available for the controller.
        /// </summary>
        public InteractionMappingProfile[] InteractionMappingProfiles
        {
            get => interactionMappingProfiles;
            set => interactionMappingProfiles = value;
        }

        [SerializeField, Tooltip("Interactors created for this controller.")]
        private BaseControllerInteractor[] controllerInteractors = null;

        /// <summary>
        /// The <see cref="IControllerInteractor"/>s that will be created for this <see cref="IController"/>.
        /// </summary>
        public BaseControllerInteractor[] ControllerInteractors
        {
            get => controllerInteractors;
            internal set => controllerInteractors = value;
        }

        #region Fields hidden in non-debug inspector

        [SerializeField]
        private Texture2D lightThemeLeftControllerTexture = null;

        public Texture2D LightThemeLeftControllerTexture
        {
            get => lightThemeLeftControllerTexture;
            set => lightThemeLeftControllerTexture = value;
        }

        [SerializeField]
        private Texture2D lightThemeLeftControllerTextureScaled = null;

        public Texture2D LightThemeLeftControllerTextureScaled
        {
            get => lightThemeLeftControllerTextureScaled;
            set => lightThemeLeftControllerTextureScaled = value;
        }

        [SerializeField]
        private Texture2D darkThemeLeftControllerTexture = null;

        public Texture2D DarkThemeLeftControllerTexture
        {
            get => darkThemeLeftControllerTexture;
            set => darkThemeLeftControllerTexture = value;
        }

        [SerializeField]
        private Texture2D darkThemeLeftControllerTextureScaled = null;

        public Texture2D DarkThemeLeftControllerTextureScaled
        {
            get => darkThemeLeftControllerTextureScaled;
            set => darkThemeLeftControllerTextureScaled = value;
        }

        [SerializeField]
        private Texture2D lightThemeRightControllerTexture = null;

        public Texture2D LightThemeRightControllerTexture
        {
            get => lightThemeRightControllerTexture;
            set => lightThemeRightControllerTexture = value;
        }

        [SerializeField]
        private Texture2D lightThemeRightControllerTextureScaled = null;

        public Texture2D LightThemeRightControllerTextureScaled
        {
            get => lightThemeRightControllerTextureScaled;
            set => lightThemeRightControllerTextureScaled = value;
        }

        [SerializeField]
        private Texture2D darkThemeRightControllerTexture = null;

        public Texture2D DarkThemeRightControllerTexture
        {
            get => darkThemeRightControllerTexture;
            set => darkThemeRightControllerTexture = value;
        }

        [SerializeField]
        private Texture2D darkThemeRightControllerTextureScaled = null;

        public Texture2D DarkThemeRightControllerTextureScaled
        {
            get => darkThemeRightControllerTextureScaled;
            set => darkThemeRightControllerTextureScaled = value;
        }

        #endregion
    }
}