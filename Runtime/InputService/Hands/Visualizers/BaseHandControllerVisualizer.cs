// Copyright (c) Reality Collective. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using RealityToolkit.Input.Controllers;
using System;
using UnityEngine;

namespace RealityToolkit.Input.Hands.Visualizers
{
    /// <summary>
    /// Base <see cref="Interfaces.Handlers.IControllerVisualizer"/> for <see cref="Interfaces.Controllers.IController"/>
    /// visualizations that resemble a hand-like appearance.
    /// </summary>
    public abstract class BaseHandControllerVisualizer : BaseControllerVisualizer
    {
        protected IHandJointTransformProvider jointTransformProvider;
        protected int jointCount;

        protected virtual void Awake()
        {
            jointCount = Enum.GetNames(typeof(HandJoint)).Length;

            if (!TryGetComponent(out jointTransformProvider))
            {
                Debug.LogError($"{GetType().Name} requires an {nameof(IHandJointTransformProvider)} on the {nameof(UnityEngine.GameObject)}.", this);
                return;
            }
        }
    }
}
