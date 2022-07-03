// Copyright (c) Reality Collective. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Collections.Generic;
using UnityEngine.XR;
using RealityToolkit.Definitions.Controllers.Hands;
using RealityToolkit.Definitions.Utilities;

namespace RealityToolkit.Interfaces.InputSystem.Providers.Controllers.Hands
{
    public interface IUnityXRHandJointDataProvider
    {
        /// <summary>
        /// Updates hand joint data and writes it into <paramref name="jointPoses"/>.
        /// </summary>
        /// <param name="inputDevice">The <see cref="InputDevice"/> to read hand joint data for.</param>
        /// <param name="jointPoses">Dictionary populated with updated hand joint information.</param>
        void UpdateHandJoints(InputDevice inputDevice, Dictionary<XRHandJoint, MixedRealityPose> jointPoses);
    }
}
