// Copyright (c) Reality Collective. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using RealityCollective.ServiceFramework.Definitions;
using RealityToolkit.Definitions.Devices;
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
    }
}