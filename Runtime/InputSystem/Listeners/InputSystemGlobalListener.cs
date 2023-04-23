// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using RealityCollective.ServiceFramework.Services;
using RealityToolkit.Input.Interfaces;
using System;
using UnityEngine;

namespace RealityToolkit.Input.Listeners
{
    /// <summary>
    /// This component ensures that all input events are forwarded to this <see cref="GameObject"/> when focus or gaze is not required.
    /// </summary>
    public class InputSystemGlobalListener : MonoBehaviour
    {
        private IInputService inputSystem = null;

        protected IInputService InputSystem
            => inputSystem ?? (inputSystem = ServiceManager.Instance.GetService<IInputService>());

        private bool lateInitialize = true;

        protected virtual void OnEnable()
        {
            if (!lateInitialize &&
                ServiceManager.Instance.IsInitialized)
            {
                InputSystem?.Register(gameObject);
            }
        }

        protected virtual async void Start()
        {
            if (lateInitialize)
            {
                try
                {
                    inputSystem = await ServiceManager.Instance.GetServiceAsync<IInputService>();
                }
                catch (Exception e)
                {
                    Debug.LogError(e);
                    return;
                }

                // We've been destroyed during the await.
                if (this == null) { return; }

                lateInitialize = false;
                InputSystem.Register(gameObject);
            }
        }

        protected virtual void OnDisable()
        {
            InputSystem?.Unregister(gameObject);
        }

        protected virtual void OnDestroy()
        {
            InputSystem?.Unregister(gameObject);
        }
    }
}
