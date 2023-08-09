// Copyright (c) Reality Collective. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using RealityCollective.Attributes;
using RealityCollective.ServiceFramework.Definitions;
using RealityToolkit.Input.Interactions.Interactors;
using UnityEngine;

namespace RealityToolkit.Input.Definitions
{
    /// <summary>
    /// Configuration profile settings for setting up controller pointers.
    /// </summary>
    public class PointerProfile : BaseProfile
    {
        [SerializeField]
        [Prefab(typeof(IPointer))]
        private GameObject pointerPrefab = null;

        public GameObject PointerPrefab => pointerPrefab;
    }
}