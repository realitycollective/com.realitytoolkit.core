// Copyright (c) Reality Collective. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using RealityCollective.ServiceFramework.Definitions;
using UnityEngine;

namespace RealityToolkit.Definitions.Controllers
{
    /// <summary>
    /// Provides additional configuration options for controller service modules.
    /// </summary>
    public abstract class BaseControllerServiceModuleProfile : BaseProfile
    {
        [SerializeField]
        private bool hasSetupDefaults = false;

        /// <summary>
        /// Has the default mappings been initialized?
        /// </summary>
        protected bool HasSetupDefaults => hasSetupDefaults;

        [SerializeField]
        private ControllerProfile[] controllerMappingProfiles = new ControllerProfile[0];

        public ControllerProfile[] ControllerMappingProfiles
        {
            get => controllerMappingProfiles;
            internal set => controllerMappingProfiles = value;
        }

        public abstract ControllerDefinition[] GetDefaultControllerOptions();
    }
}