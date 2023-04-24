// Copyright (c) Reality Collective. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using RealityCollective.ServiceFramework.Definitions;
using UnityEngine;

namespace RealityToolkit.Input.Processors
{
    [CreateAssetMenu(menuName = "Reality Toolkit/Input System/Processors/Invert Dual Axis", fileName = "InvertDualAxisProcessor", order = (int)CreateProfileMenuItemIndices.Configuration)]
    public class InvertDualAxisProcessor : InputProcessor<Vector2>
    {
        [SerializeField]
        private bool invertX = false;

        public bool InvertX
        {
            get => invertX;
            set => invertX = value;
        }

        [SerializeField]
        private bool invertY = false;

        public bool InvertY
        {
            get => invertY;
            set => invertY = value;
        }

        /// <inheritdoc />
        public override void Process(ref Vector2 value)
        {
            if (invertX)
            {
                value.x *= -1f;
            }

            if (invertY)
            {
                value.y *= -1f;
            }
        }
    }
}