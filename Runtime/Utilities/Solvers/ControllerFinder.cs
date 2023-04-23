﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using RealityCollective.Definitions.Utilities;
using RealityCollective.ServiceFramework.Services;
using RealityToolkit.EventDatum.Input;
using RealityToolkit.Input.Interfaces;
using RealityToolkit.Input.Interfaces.Controllers;
using RealityToolkit.Input.Interfaces.Handlers;
using UnityEngine;

namespace RealityToolkit.Utilities.Solvers
{
    /// <summary>
    /// ControllerFinder is a base class providing simple event handling for getting/releasing MotionController Transforms.
    /// </summary>
    public abstract class ControllerFinder : MonoBehaviour, ISourceStateHandler
    {
        [SerializeField]
        [Tooltip("The handedness of the controller that should be found.")]
        private Handedness handedness = Handedness.None;

        /// <summary>
        /// The handedness of the controller that should be found.
        /// </summary>
        public Handedness Handedness
        {
            get => handedness;
            set
            {
                // We need to refresh which controller we're attached to if we switch handedness.
                if (handedness != value)
                {
                    handedness = value;
                    RefreshControllerTransform();
                }
            }
        }

        /// <summary>
        /// The Transform of the currently found controller.
        /// </summary>
        protected Transform ControllerTransform;

        #region MonoBehaviour Implementation

        protected virtual void OnEnable()
        {
            // Look if the controller has loaded.
            RefreshControllerTransform();
        }

        #endregion MonoBehaviour Implementation

        #region IMixedRealitySourceStateHandler Implementation

        public void OnSourceDetected(SourceStateEventData eventData)
        {
            if (eventData.Controller?.ControllerHandedness == handedness)
            {
                AddControllerTransform(eventData.Controller);
            }
        }

        public void OnSourceLost(SourceStateEventData eventData)
        {
            if (eventData.Controller?.ControllerHandedness == handedness)
            {
                RemoveControllerTransform();
            }
        }

        #endregion IMixedRealitySourceStateHandler Implementation

        /// <summary>
        /// Looks to see if the controller model already exists and registers it if so.
        /// </summary>
        protected virtual void TryAndAddControllerTransform()
        {
            // Look if the controller was already loaded. This could happen if the
            // GameObject was instantiated at runtime and the model loaded event has already fired.
            if (!ServiceManager.Instance.TryGetService<IInputService>(out var inputSystem))
            {
                // The InputSystem could not be found.
                return;
            }

            foreach (var controller in inputSystem.DetectedControllers)
            {
                if (controller.ControllerHandedness == handedness)
                {
                    AddControllerTransform(controller);
                    return;
                }
            }
        }

        /// <summary>
        /// Starts to track the passed in controller's transform, assuming it meets the previously set handedness criteria.
        /// </summary>
        /// <param name="newController">The new controller to be tracked.</param>
        protected virtual void AddControllerTransform(IController newController)
        {
            if (newController.ControllerHandedness == handedness && newController.Visualizer != null && newController.Visualizer.GameObject.transform != null && !newController.Visualizer.GameObject.transform.Equals(ControllerTransform))
            {
                ControllerTransform = newController.Visualizer.GameObject.transform;

                OnControllerFound();
            }
        }

        /// <summary>
        /// Remove whichever controller is currently tracked, if any.
        /// </summary>
        protected virtual void RemoveControllerTransform()
        {
            if (ControllerTransform != null)
            {
                OnControllerLost();

                ControllerTransform = null;
            }
        }

        /// <summary>
        /// Remove whichever controller is currently tracked, if any, and try to add a new one based on existing sources.
        /// </summary>
        protected virtual void RefreshControllerTransform()
        {
            if (ControllerTransform != null)
            {
                RemoveControllerTransform();
            }

            TryAndAddControllerTransform();
        }

        /// <summary>
        /// Override this method to act when the correct controller is actually found.
        /// This provides similar functionality to overriding AddControllerTransform,
        /// without the overhead of needing to check that handedness matches.
        /// </summary>
        protected virtual void OnControllerFound() { }

        /// <summary>
        /// Override this method to act when the correct controller is actually lost.
        /// This provides similar functionality to overriding AddControllerTransform,
        /// without the overhead of needing to check that handedness matches.
        /// </summary>
        protected virtual void OnControllerLost() { }
    }
}