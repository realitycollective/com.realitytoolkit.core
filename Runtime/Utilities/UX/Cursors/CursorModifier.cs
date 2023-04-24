﻿// Copyright (c) Reality Collective. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using RealityCollective.Definitions.Utilities;
using RealityCollective.ServiceFramework.Services;
using RealityToolkit.EventDatum.Input;
using RealityToolkit.Input.Interfaces;
using RealityToolkit.Input.Interfaces.Handlers;
using UnityEngine;

namespace RealityToolkit.Utilities.UX.Cursors
{
    /// <summary>
    /// Component that can be added to any <see cref="GameObject"/> with a <see cref="Collider"/> to Modifies either the <see cref="ICursor"/> reacts when focused by a <see cref="IPointer"/>.
    /// </summary>
    public class CursorModifier : MonoBehaviour, ICursorModifier
    {
        #region ICursorModifier Implementation

        [SerializeField]
        [Tooltip("Transform for which this cursor modifier applies its various properties.")]
        private Transform hostTransform;

        /// <inheritdoc />
        public Transform HostTransform
        {
            get
            {
                if (hostTransform == null)
                {
                    hostTransform = transform;
                }

                return hostTransform;
            }
            set => hostTransform = value;
        }

        [SerializeField]
        [Tooltip("How much a cursor should be offset from the surface of the object when overlapping.")]
        private Vector3 cursorPositionOffset = Vector3.zero;

        /// <inheritdoc />
        public Vector3 CursorPositionOffset
        {
            get => cursorPositionOffset;
            set => cursorPositionOffset = value;
        }

        [SerializeField]
        [Tooltip("Should the cursor snap to the GameObject?")]
        private bool snapCursorPosition = false;

        /// <inheritdoc />
        public bool SnapCursorPosition
        {
            get => snapCursorPosition;
            set => snapCursorPosition = value;
        }

        [Tooltip("Scale of the cursor when looking at this GameObject.")]
        [SerializeField]
        private Vector3 cursorScaleOffset = Vector3.one;

        /// <inheritdoc />
        public Vector3 CursorScaleOffset
        {
            get => cursorScaleOffset;
            set => cursorScaleOffset = value;
        }

        [SerializeField]
        [Tooltip("Direction of the cursor offset.")]
        private Vector3 cursorNormalOffset = Vector3.back;

        /// <inheritdoc />
        public Vector3 CursorNormalOffset
        {
            get => cursorNormalOffset;
            set => cursorNormalOffset = value;
        }

        [SerializeField]
        [Tooltip("If true, the normal from the pointing vector will be used to orient the cursor instead of the targeted object's normal at point of contact.")]
        private bool useGazeBasedNormal = false;

        /// <inheritdoc />
        public bool UseGazeBasedNormal
        {
            get => useGazeBasedNormal;
            set => useGazeBasedNormal = value;
        }

        [SerializeField]
        [Tooltip("Should the cursor be hiding when this object is focused?")]
        private bool hideCursorOnFocus = false;

        /// <inheritdoc />
        public bool HideCursorOnFocus
        {
            get => hideCursorOnFocus;
            set => hideCursorOnFocus = value;
        }

        [SerializeField]
        [Tooltip("Cursor animation parameters to set when this object is focused. Leave empty for none.")]
        private AnimatorParameter[] cursorParameters = null;

        /// <inheritdoc />
        public AnimatorParameter[] CursorParameters => cursorParameters;

        /// <inheritdoc />
        public bool GetCursorVisibility() => HideCursorOnFocus;

        /// <inheritdoc />
        public Vector3 GetModifiedPosition(ICursor cursor)
        {
            Debug.Assert(gameObject.activeInHierarchy);

            if (SnapCursorPosition)
            {
                // Snap if the targeted object has a cursor modifier that supports snapping
                return HostTransform.position + HostTransform.TransformVector(CursorPositionOffset);
            }

            if (cursor.Pointer == null)
            {
                Debug.LogError($"{cursor.GameObjectReference.name} has no pointer set in it's cursor component!");
                return Vector3.zero;
            }

            if (ServiceManager.Instance.TryGetService<IInputService>(out var inputService) &&
                inputService.FocusProvider.TryGetFocusDetails(cursor.Pointer, out var focusDetails))
            {
                // Else, consider the modifiers on the cursor modifier, but don't snap
                return focusDetails.EndPoint + HostTransform.TransformVector(CursorPositionOffset);
            }

            return Vector3.zero;
        }

        /// <inheritdoc />
        public Quaternion GetModifiedRotation(ICursor cursor)
        {
            Debug.Assert(gameObject.activeInHierarchy);
            var lastStep = cursor.Pointer.Rays[cursor.Pointer.Rays.Length - 1];
            var forward = UseGazeBasedNormal ? -lastStep.Direction : HostTransform.rotation * CursorNormalOffset;

            // Determine the cursor forward rotation
            return forward.magnitude > 0
                    ? Quaternion.LookRotation(forward, Vector3.up)
                    : cursor.Rotation;
        }

        /// <inheritdoc />
        public Vector3 GetModifiedScale(ICursor cursor)
        {
            Debug.Assert(gameObject.activeInHierarchy);
            return CursorScaleOffset;
        }

        /// <inheritdoc />
        public void GetModifiedTransform(ICursor cursor, out Vector3 position, out Quaternion rotation, out Vector3 scale)
        {
            Debug.Assert(gameObject.activeInHierarchy);
            position = GetModifiedPosition(cursor);
            rotation = GetModifiedRotation(cursor);
            scale = GetModifiedScale(cursor);
        }

        #endregion ICursorModifier Implementation

        #region IFocusChangedHandler Implementation

        /// <inheritdoc />
        void IFocusChangedHandler.OnBeforeFocusChange(FocusEventData eventData)
        {
            if (eventData.NewFocusedObject == gameObject)
            {
                eventData.Pointer.CursorModifier = this;
            }

            if (eventData.OldFocusedObject == gameObject)
            {
                eventData.Pointer.CursorModifier = null;
            }
        }

        /// <inheritdoc />
        void IFocusChangedHandler.OnFocusChanged(FocusEventData eventData) { }

        #endregion IFocusChangedHandler Implementation

        #region MonoBehaviour Implementation

        private void OnValidate()
        {
            Debug.Assert(HostTransform.GetComponent<Collider>() != null, $"A collider component is required on {hostTransform.gameObject.name} for the cursor modifier component on {gameObject.name} to function properly.");
        }

        private void OnDisable()
        {
            if (ServiceManager.Instance.TryGetService<IInputService>(out var inputService))
            {
                foreach (var inputSource in inputService.DetectedInputSources)
                {
                    foreach (var pointer in inputSource.Pointers)
                    {
                        if (pointer.CursorModifier != null &&
                            pointer.CursorModifier.HostTransform == HostTransform)
                        {
                            pointer.CursorModifier = null;
                        }
                    }
                }
            }
        }

        #endregion MonoBehaviour Implementaiton
    }
}
