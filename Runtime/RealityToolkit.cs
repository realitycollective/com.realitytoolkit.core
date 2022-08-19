// Copyright (c) Reality Collective. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using RealityCollective.Extensions;
using RealityCollective.ServiceFramework.Services;
using RealityToolkit.Utilities;
using UnityEngine;
using UnityEngine.EventSystems;

namespace RealityToolkit
{
    [System.Runtime.InteropServices.Guid("d80d0819-42ff-4e7b-a917-887b788394db")]
    public sealed class RealityToolkit : BaseServiceWithConstructor, IRealityToolkit
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="name">The service display name.</param>
        /// <param name="priority">The service initialization priority.</param>
        /// <param name="profile">The service configuration profile.</param>
        public RealityToolkit(string name, uint priority, RealityToolkitProfile profile)
            : base(name, priority) { }

        /// <inheritdoc />
        public override void Initialize()
        {
            EnsureToolkitRequirements();
        }

        private static void EnsureToolkitRequirements()
        {
            // We need at least one instance of the event system to be active.
            EnsureEventSystemSetup();
        }

        private static void EnsureEventSystemSetup()
        {
            var eventSystems = Object.FindObjectsOfType<EventSystem>();
            if (eventSystems.Length == 0)
            {
                CameraCache.Main.gameObject.EnsureComponent<EventSystem>();
                Debug.Log($"There was no {nameof(EventSystem)} in the scene. The {nameof(RealityToolkit)} requires one and added it to the main camera.");
            }
            else if (eventSystems.Length > 1)
            {
                Debug.LogError($"There is more than one {nameof(EventSystem)} active in the scene. Please make sure only one instance of it exists as it may cause errors.");
            }
        }
    }
}
