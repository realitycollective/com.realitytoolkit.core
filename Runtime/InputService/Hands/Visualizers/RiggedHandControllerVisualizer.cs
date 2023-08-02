// Copyright (c) Reality Collective. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using RealityToolkit.Input.Controllers;
using UnityEngine;

namespace RealityToolkit.Input.Hands.Visualizers
{
    [System.Runtime.InteropServices.Guid("f7dc4217-86da-4540-a2e4-3e721994e7c8")]
    public class RiggedHandControllerVisualizer : BaseControllerVisualizer
    {
        private IHandJointTransformProvider jointTransformProvider;

        private void Awake()
        {
            if (!TryGetComponent(out jointTransformProvider))
            {
                Debug.LogError($"{GetType().Name} requires an {nameof(IHandJointTransformProvider)} on the {nameof(GameObject)}.", this);
                return;
            }
        }
    }
}
