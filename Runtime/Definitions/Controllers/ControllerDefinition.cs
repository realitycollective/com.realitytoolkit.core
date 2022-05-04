// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using RealityToolkit.Definitions.Utilities;

namespace RealityToolkit.Definitions.Controllers
{
    [Serializable]
    public struct ControllerDefinition
    {
        public ControllerDefinition(SystemType controllerType, Handedness handedness = this.Handedness.None, bool useCustomInteractions = false)
        {
            ControllerType = controllerType;
            Handedness = handedness;
            UseCustomInteractions = useCustomInteractions;

            string description = null;

            if (ControllerType?.Type != null)
            {
                if (handedness == this.Handedness.Right || handedness == this.Handedness.Left)
                {
                    description = $"{handedness}{ControllerType.Type.Name}";
                }
                else
                {
                    description = ControllerType.Type.Name;
                }
            }

            Description = description;
        }

        public ControllerDefinition(string description, SystemType controllerType, Handedness handedness = this.Handedness.None, bool useCustomInteractions = false)
            : this(controllerType, handedness, useCustomInteractions)
        {
            Description = description;
        }

        public readonly SystemType ControllerType;

        public readonly string Description;

        public readonly Handedness Handedness;

        public readonly bool UseCustomInteractions;
    }
}