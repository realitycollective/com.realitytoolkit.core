// Copyright (c) Reality Collective. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using RealityCollective.Definitions.Utilities;
using RealityCollective.ServiceFramework.Services;
using RealityToolkit.Definitions.Devices;
using RealityToolkit.Input.Interfaces;

namespace RealityToolkit.Input.Extensions
{
    /// <summary>
    /// Extensions for the <see cref="InteractionMapping"/> to refactor the generic methods used for raising events.
    /// </summary>
    public static class InteractionMappingsExtensions
    {
        private static IInputService inputSystem = null;

        private static IInputService InputSystem
            => inputSystem ?? (inputSystem = ServiceManager.Instance.GetService<IInputService>());

        /// <summary>
        /// Raise the actions to the input system.
        /// </summary>
        /// <param name="interactionMapping"></param>
        /// <param name="inputSource"></param>
        /// <param name="controllerHandedness"></param>
        public static void RaiseInputAction(this InteractionMapping interactionMapping, IInputSource inputSource, Handedness controllerHandedness)
        {
            var changed = interactionMapping.ControlActivated;
            var updated = interactionMapping.Updated;

            if (changed &&
                (interactionMapping.AxisType == AxisType.Digital ||
                 interactionMapping.AxisType == AxisType.SingleAxis))
            {
                if (interactionMapping.BoolData)
                {
                    InputSystem?.RaiseOnInputDown(inputSource, controllerHandedness, interactionMapping.MixedRealityInputAction);
                }
                else
                {
                    InputSystem?.RaiseOnInputUp(inputSource, controllerHandedness, interactionMapping.MixedRealityInputAction);
                }
            }

            if (updated)
            {
                switch (interactionMapping.AxisType)
                {
                    case AxisType.Digital:
                        InputSystem?.RaiseOnInputPressed(
                            inputSource,
                            controllerHandedness,
                            interactionMapping.MixedRealityInputAction,
                            interactionMapping.BoolData ? 1 : 0);
                        break;
                    case AxisType.SingleAxis:
                        InputSystem?.RaiseOnInputPressed(
                            inputSource,
                            controllerHandedness,
                            interactionMapping.MixedRealityInputAction,
                            interactionMapping.FloatData);
                        break;
                    case AxisType.DualAxis:
                        InputSystem?.RaisePositionInputChanged(
                            inputSource,
                            controllerHandedness,
                            interactionMapping.MixedRealityInputAction,
                            interactionMapping.Vector2Data);
                        break;
                    case AxisType.ThreeDofPosition:
                        InputSystem?.RaisePositionInputChanged(
                            inputSource,
                            controllerHandedness,
                            interactionMapping.MixedRealityInputAction,
                            interactionMapping.PositionData);
                        break;
                    case AxisType.ThreeDofRotation:
                        InputSystem?.RaiseRotationInputChanged(
                            inputSource,
                            controllerHandedness,
                            interactionMapping.MixedRealityInputAction,
                            interactionMapping.RotationData);
                        break;
                    case AxisType.SixDof:
                        InputSystem?.RaisePoseInputChanged(
                            inputSource,
                            controllerHandedness,
                            interactionMapping.MixedRealityInputAction,
                            interactionMapping.PoseData);
                        break;
                }
            }
        }

        /// <summary>
        /// Overload extension to enable getting of an InteractionDefinition of a specific type
        /// </summary>
        /// <param name="input">The InteractionDefinition array reference</param>
        /// <param name="key">The specific DeviceInputType value to query</param>
        public static InteractionMapping GetInteractionByType(this InteractionMapping[] input, DeviceInputType key)
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
        public static bool SupportsInputType(this InteractionMapping[] input, DeviceInputType key)
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