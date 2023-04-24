// Copyright (c) Reality Collective. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;

namespace RealityToolkit.Interfaces.Physics
{
    public interface IBaseRayStabilizer
    {
        Vector3 StablePosition { get; }
        Quaternion StableRotation { get; }
        Ray StableRay { get; }
        void UpdateStability(Vector3 position, Quaternion rotation);
        void UpdateStability(Vector3 position, Vector3 direction);
    }
}