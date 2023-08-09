// Copyright (c) Reality Collective. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using RealityCollective.ServiceFramework.Services;
using RealityToolkit.Input.Interactions.Interactors;
using RealityToolkit.Input.Interfaces;
using System;
using System.Collections;

namespace RealityToolkit.Input.InputSources
{
    /// <summary>
    /// Base class for input sources that don't inherit from MonoBehaviour.
    /// <remarks>This base class does not support adding or removing pointers, because many will never
    /// pass pointers in their constructors and will fall back to either the Gaze or Mouse Pointer.</remarks>
    /// </summary>
    public class BaseGenericInputSource : IInputSource
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="pointers"></param>
        public BaseGenericInputSource(string name, IPointer[] pointers = null)
        {
            if (ServiceManager.Instance.TryGetService<IInputService>(out var inputService))
            {
                SourceId = inputService.GenerateNewSourceId();
                SourceName = name;

                Pointers = pointers;
                if (Pointers == null && inputService.GazeProvider != null)
                {
                    Pointers = new[] { inputService.GazeProvider.GazePointer };
                }
            }
            else
            {
                throw new ArgumentException($"Failed to find a valid {nameof(IInputService)}!");
            }
        }

        /// <inheritdoc />
        public uint SourceId { get; }

        /// <inheritdoc />
        public string SourceName { get; }

        /// <inheritdoc />
        public virtual IPointer[] Pointers { get; }

        #region IEquality Implementation

        public static bool Equals(IInputSource left, IInputSource right)
        {
            return left.Equals(right);
        }

        /// <inheritdoc />
        bool IEqualityComparer.Equals(object left, object right)
        {
            return left.Equals(right);
        }

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) { return false; }
            if (ReferenceEquals(this, obj)) { return true; }
            if (obj.GetType() != GetType()) { return false; }

            return Equals((IInputSource)obj);
        }

        private bool Equals(IInputSource other)
        {
            return other != null && SourceId == other.SourceId && string.Equals(SourceName, other.SourceName);
        }

        /// <inheritdoc />
        int IEqualityComparer.GetHashCode(object obj)
        {
            return obj.GetHashCode();
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = 0;
                hashCode = (hashCode * 397) ^ (int)SourceId;
                hashCode = (hashCode * 397) ^ (SourceName != null ? SourceName.GetHashCode() : 0);
                return hashCode;
            }
        }

        #endregion IEquality Implementation
    }
}
