﻿// Copyright (c) Reality Collective. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using RealityToolkit.EventDatum.Input;
using RealityToolkit.Input.Interfaces;
using RealityToolkit.Utilities.Physics;
using UnityEngine;

namespace RealityToolkit.Input.Interactions.Interactors
{
    /// <summary>
    /// Touch Pointer Implementation.
    /// </summary>
    public class TouchPointer : BaseControllerInteractor, ITouchPointer
    {
        private bool isInteractionEnabled = false;

        /// <inheritdoc />
        public override bool IsInteractionEnabled => isInteractionEnabled;

        /// <inheritdoc />
        public override bool IsFarInteractor => true;

        private int fingerId = -1;

        /// <inheritdoc />
        public int FingerId
        {
            get => fingerId;
            set
            {
                if (fingerId < 0)
                {
                    fingerId = value;
                }
            }
        }

        /// <inheritdoc />
        public Ray TouchRay { get; set; } = default;

        /// <inheritdoc />
        public override void OnPreRaycast()
        {
            if (TryGetPointingRay(out var pointingRay))
            {
                Rays[0].CopyRay(pointingRay, PointerExtent);

                if (RayStabilizer != null)
                {
                    RayStabilizer.UpdateStability(Rays[0].Origin, Rays[0].Direction);
                    Rays[0].CopyRay(RayStabilizer.StableRay, PointerExtent);

                    if (Raycaster.DebugEnabled)
                    {
                        Debug.DrawRay(RayStabilizer.StableRay.origin, RayStabilizer.StableRay.direction * PointerExtent, Color.green);
                    }
                }
                else if (Raycaster.DebugEnabled)
                {
                    Debug.DrawRay(pointingRay.origin, pointingRay.direction * PointerExtent, Color.yellow);
                }
            }
        }

        /// <inheritdoc />
        public override bool TryGetPointerPosition(out Vector3 position)
        {
            position = Vector3.zero;

            if (fingerId < 0) { return false; }

            var playerCamera = Camera.main;
            position = Result.CurrentPointerTarget != null
                ? Result.EndPoint
                : playerCamera.ScreenPointToRay(UnityEngine.Input.GetTouch(FingerId).position).GetPoint(PointerExtent);
            return true;
        }

        /// <inheritdoc />
        public override bool TryGetPointingRay(out Ray pointingRay)
        {
            pointingRay = TouchRay;
            return true;
        }

        /// <inheritdoc />
        public override bool TryGetPointerRotation(out Quaternion rotation)
        {
            rotation = Quaternion.identity;
            return false;
        }

        /// <inheritdoc />
        public override void OnSourceDetected(SourceStateEventData eventData)
        {
            base.OnSourceDetected(eventData);

            if (eventData.InputSource.SourceId == Controller.InputSource.SourceId)
            {
                isInteractionEnabled = true;
            }
        }

        /// <inheritdoc />
        public override void OnSourceLost(SourceStateEventData eventData)
        {
            base.OnSourceLost(eventData);

            if (Controller != null &&
                eventData.Controller != null &&
                eventData.Controller.InputSource.SourceId == Controller.InputSource.SourceId)
            {
                isInteractionEnabled = false;
            }
        }
    }
}