// Copyright (c) Reality Collective. All rights reserved.
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
    public class InputServiceGlobalListener : MonoBehaviour
    {
        private IInputService inputService = null;

        protected IInputService InputService
            => inputService ?? (inputService = ServiceManager.Instance.GetService<IInputService>());

        private bool lateInitialize = true;

        protected virtual void OnEnable()
        {
            if (!lateInitialize &&
                ServiceManager.Instance.IsInitialized)
            {
                InputService?.Register(gameObject);
            }
        }

        protected virtual async void Start()
        {
            if (lateInitialize)
            {
                try
                {
                    inputService = await ServiceManager.Instance.GetServiceAsync<IInputService>();
                }
                catch (Exception e)
                {
                    Debug.LogError(e);
                    return;
                }

                // We've been destroyed during the await.
                if (this == null) { return; }

                lateInitialize = false;
                InputService.Register(gameObject);
            }
        }

        protected virtual void OnDisable()
        {
            InputService?.Unregister(gameObject);
        }

        protected virtual void OnDestroy()
        {
            InputService?.Unregister(gameObject);
        }
    }
}
