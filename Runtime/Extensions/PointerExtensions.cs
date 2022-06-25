// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using RealityToolkit.Definitions.Utilities;
using RealityToolkit.Interfaces.InputSystem;
using UnityEngine;

namespace RealityToolkit.Extensions
{
    /// <summary>
    /// Extensions for the <see cref="IMixedRealityPointer"/> to provice additional data for position and rotation.
    /// </summary>
    public static class PointerExtensions 
    {
        public static Vector3 GetPosition(this IMixedRealityPointer pointer)
        {
            if (pointer.TryGetPointerPosition(out var position))
            {
                return position;
            }
            return Vector3.zero;
        }

        public static Quaternion GetRotation(this IMixedRealityPointer pointer)
        {
            if (pointer.TryGetPointerRotation(out var rotation))
            {
                return rotation;
            }
            return Quaternion.identity;
        }

        public static MixedRealityPose GetPose(this IMixedRealityPointer pointer)
        {
            var position = pointer.GetPosition();
            var rotation = pointer.GetRotation();
            return new MixedRealityPose(position, rotation);
        }
    }
}
