// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using RealityToolkit.Definitions.Devices;

namespace RealityToolkit.Extensions
{
    /// <summary>
    /// Extension methods for .Net Collection objects, e.g. Lists, Dictionaries, Arrays
    /// </summary>
    public static class CollectionsExtensions
    {
        /// <summary>
        /// Overload extension to enable getting of an InteractionDefinition of a specific type
        /// </summary>
        /// <param name="input">The InteractionDefinition array reference</param>
        /// <param name="key">The specific DeviceInputType value to query</param>
        public static MixedRealityInteractionMapping GetInteractionByType(this MixedRealityInteractionMapping[] input, DeviceInputType key)
        {
            for (int i = 0; i < input?.Length; i++)
            {
                if (input[i].InputType == key)
                {
                    return input[i];
                }
            }

            return default;
        }

        /// <summary>
        /// Overload extension to enable getting of an InteractionDefinition of a specific type
        /// </summary>
        /// <param name="input">The InteractionDefinition array reference</param>
        /// <param name="key">The specific DeviceInputType value to query</param>
        public static bool SupportsInputType(this MixedRealityInteractionMapping[] input, DeviceInputType key)
        {
            for (int i = 0; i < input.Length; i++)
            {
                if (input[i].InputType == key)
                {
                    return true;
                }
            }

            return false;
        }
    }
}