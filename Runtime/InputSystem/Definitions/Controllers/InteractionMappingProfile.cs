// Copyright (c) Reality Collective. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using RealityCollective.ServiceFramework.Definitions;
using RealityToolkit.Definitions.Devices;
using RealityToolkit.Input.Definitions;
using UnityEngine;

namespace RealityToolkit.Definitions.Controllers
{
    public class InteractionMappingProfile : BaseProfile
    {
        [SerializeField]
        private InteractionMapping interactionMapping = new InteractionMapping();

        public InteractionMapping InteractionMapping
        {
            get => interactionMapping;
            internal set => interactionMapping = value;
        }

        [SerializeField]
        private MixedRealityPointerProfile[] pointerProfiles = null;

        /// <summary>
        /// The pointer profiles for this interaction if the interaction is 3 or 6 Dof
        /// </summary>
        public MixedRealityPointerProfile[] PointerProfiles
        {
            get => pointerProfiles;
            internal set => pointerProfiles = value;
        }
    }
}