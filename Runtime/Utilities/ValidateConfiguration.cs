// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Text;
using RealityToolkit.Definitions.Controllers;
using RealityCollective.ServiceFramework.Services;

#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
#endif

namespace RealityToolkit.Utilities
{
    public static class ValidateConfiguration
    {
        private const string IgnoreKey = "_MixedRealityToolkit_Editor_IgnoreControllerMappingsPrompts";

        /// <summary>
        /// Controller Mapping function to test for a controller mapping
        /// </summary>
        /// <param name="profile"></param>
        /// <param name="mappingTypesToValidate">Array of controller mappings to validate</param>
        /// <param name="prompt">Unit Test helper, to control whether the UI prompt is offered or not</param>
        /// <returns></returns>
        public static bool ValidateControllerProfiles(this BaseMixedRealityControllerServiceModuleProfile profile, Type[] mappingTypesToValidate, bool prompt = true)
        {
#if UNITY_EDITOR
            if (Application.isPlaying || EditorPrefs.GetBool(IgnoreKey, false))
            {
                return false;
            }
#endif //UNITY_EDITOR

            if (ServiceManager.Instance != null && ServiceManager.Instance.HasActiveProfile)
            {
                var errorsFound = false;
                var mappingConfigurationSource = profile.ControllerMappingProfiles;

                if (mappingConfigurationSource != null)
                {
                    if (mappingTypesToValidate != null && mappingTypesToValidate.Length > 0)
                    {
                        var typesValidated = new bool[mappingTypesToValidate.Length];

                        for (int i = 0; i < mappingTypesToValidate.Length; i++)
                        {
                            foreach (var mappingProfile in mappingConfigurationSource)
                            {
                                if (mappingProfile.ControllerType == null) { continue; }

                                if (mappingProfile.ControllerType == mappingTypesToValidate[i])
                                {
                                    typesValidated[i] = true;
                                }
                            }
                        }

                        for (var i = 0; i < typesValidated.Length; i++)
                        {
                            if (!typesValidated[i])
                            {
                                errorsFound = true;
                            }
                        }

                        if (errorsFound)
                        {
                            var errorDescription = new StringBuilder();
                            errorDescription.AppendLine("The following Controller Types were not found in the current Mixed Reality Configuration profile:\n");

                            for (int i = 0; i < typesValidated.Length; i++)
                            {
                                if (!typesValidated[i])
                                {
                                    errorDescription.AppendLine($" [{mappingTypesToValidate[i]}]");
                                }
                            }

                            errorDescription.AppendLine($"\nYou will need to either create a profile and add it to your Controller mappings, or assign the default from the 'DefaultProfiles' folder in the associated package");
#if UNITY_EDITOR
                            if (prompt)
                            {
                                if (EditorUtility.DisplayDialog("Controller Mappings not found", errorDescription.ToString(), "Ignore", "Later"))
                                {
                                    EditorPrefs.SetBool(IgnoreKey, true);
                                }
                            }
#endif //UNITY_EDITOR
                        }
                        else
                        {
                            return true;
                        }
                    }
                }
            }

            return false;
        }
    }
}