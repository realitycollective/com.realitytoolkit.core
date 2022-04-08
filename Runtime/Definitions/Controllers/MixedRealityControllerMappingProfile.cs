// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using RealityToolkit.ServiceFramework.Definitions;
using UnityEngine;
using XRTK.Attributes;
using XRTK.Definitions.Utilities;
using XRTK.Interfaces.InputSystem.Controllers;
using TypeGrouping = XRTK.Definitions.Utilities.TypeGrouping;

namespace XRTK.Definitions.Controllers
{
    public class MixedRealityControllerMappingProfile : BaseProfile
    {
        [SerializeField]
        [Implements(typeof(IMixedRealityController), TypeGrouping.ByNamespaceFlat)]
        private SystemType controllerType = null;

        /// <summary>
        /// The type of controller this mapping corresponds to.
        /// </summary>
        public SystemType ControllerType
        {
            get => controllerType;
            internal set => controllerType = value;
        }

        [SerializeField]
        private Handedness handedness = Handedness.None;

        public Handedness Handedness
        {
            get => handedness;
            internal set => handedness = value;
        }

        [SerializeField]
        private MixedRealityControllerVisualizationProfile visualizationProfile = null;

        public MixedRealityControllerVisualizationProfile VisualizationProfile
        {
            get => visualizationProfile;
            internal set => visualizationProfile = value;
        }

        [SerializeField]
        private bool useCustomInteractions = true;

        internal bool UseCustomInteractions
        {
            get => useCustomInteractions;
            set => useCustomInteractions = value;
        }

        [SerializeField]
        private MixedRealityInteractionMappingProfile[] interactionMappingProfiles = new MixedRealityInteractionMappingProfile[0];

        /// <summary>
        /// Details the list of available interaction profiles available for the controller.
        /// </summary>
        public MixedRealityInteractionMappingProfile[] InteractionMappingProfiles
        {
            get => interactionMappingProfiles;
            internal set => interactionMappingProfiles = value;
        }

        #region Fields hidden in non-debug inspector

        [SerializeField]
        private Texture2D lightThemeLeftControllerTexture = null;

        public Texture2D LightThemeLeftControllerTexture
        {
            get => lightThemeLeftControllerTexture;
            internal set => lightThemeLeftControllerTexture = value;
        }

        [SerializeField]
        private Texture2D lightThemeLeftControllerTextureScaled = null;

        public Texture2D LightThemeLeftControllerTextureScaled
        {
            get => lightThemeLeftControllerTextureScaled;
            internal set => lightThemeLeftControllerTextureScaled = value;
        }

        [SerializeField]
        private Texture2D darkThemeLeftControllerTexture = null;

        public Texture2D DarkThemeLeftControllerTexture
        {
            get => darkThemeLeftControllerTexture;
            internal set => darkThemeLeftControllerTexture = value;
        }

        [SerializeField]
        private Texture2D darkThemeLeftControllerTextureScaled = null;

        public Texture2D DarkThemeLeftControllerTextureScaled
        {
            get => darkThemeLeftControllerTextureScaled;
            internal set => darkThemeLeftControllerTextureScaled = value;
        }

        [SerializeField]
        private Texture2D lightThemeRightControllerTexture = null;

        public Texture2D LightThemeRightControllerTexture
        {
            get => lightThemeRightControllerTexture;
            internal set => lightThemeRightControllerTexture = value;
        }

        [SerializeField]
        private Texture2D lightThemeRightControllerTextureScaled = null;

        public Texture2D LightThemeRightControllerTextureScaled
        {
            get => lightThemeRightControllerTextureScaled;
            internal set => lightThemeRightControllerTextureScaled = value;
        }

        [SerializeField]
        private Texture2D darkThemeRightControllerTexture = null;

        public Texture2D DarkThemeRightControllerTexture
        {
            get => darkThemeRightControllerTexture;
            internal set => darkThemeRightControllerTexture = value;
        }

        [SerializeField]
        private Texture2D darkThemeRightControllerTextureScaled = null;

        public Texture2D DarkThemeRightControllerTextureScaled
        {
            get => darkThemeRightControllerTextureScaled;
            internal set => darkThemeRightControllerTextureScaled = value;
        }

        #endregion
    }
}