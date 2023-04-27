// Copyright (c) Reality Collective. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using RealityCollective.Definitions.Utilities;
using RealityCollective.Extensions;
using RealityCollective.ServiceFramework.Modules;
using RealityCollective.ServiceFramework.Services;
using RealityToolkit.Definitions.SpatialObservers;
using RealityToolkit.SpatialAwareness.Definitions;
using RealityToolkit.SpatialAwareness.Interfaces;
using RealityToolkit.SpatialAwareness.Interfaces.SpatialObservers;
using System;
using System.Collections;
using UnityEngine;

namespace RealityToolkit.SpatialAwareness.Modules
{
    /// <summary>
    /// Base <see cref="ISpatialAwarenessServiceModule"/> implementation
    /// </summary>
    public abstract class BaseSpatialObserverServiceModule : BaseServiceModule, ISpatialAwarenessServiceModule
    {
        /// <inheritdoc />
        protected BaseSpatialObserverServiceModule(string name, uint priority, BaseSpatialObserverProfile profile, ISpatialAwarenessService parentService)
            : base(name, priority, profile, parentService)
        {
            if (profile.IsNull())
            {
                profile = ServiceManager.Instance.TryGetServiceProfile<ISpatialAwarenessService, SpatialAwarenessSystemProfile>(out var spatialAwarenessSystemProfile)
                    ? spatialAwarenessSystemProfile.GlobalMeshObserverProfile
                    : throw new ArgumentException($"Unable to get a valid {nameof(SpatialAwarenessSystemProfile)}!");
            }

            if (profile.IsNull())
            {
                throw new ArgumentNullException($"Missing a {profile.GetType().Name} profile for {name}");
            }

            SpatialAwarenessSystem = parentService;
            SourceId = parentService.GenerateNewObserverId();
            StartupBehavior = profile.StartupBehavior;
            UpdateInterval = profile.UpdateInterval;
            PhysicsLayer = profile.PhysicsLayer;
        }

        protected readonly ISpatialAwarenessService SpatialAwarenessSystem;

        #region IService Implementation

        /// <inheritdoc />
        public override void Enable()
        {
            base.Enable();

            SpatialAwarenessSystem.RaiseSpatialAwarenessObserverDetected(this);

            if (StartupBehavior == AutoStartBehavior.AutoStart)
            {
                StartObserving();
            }
        }

        /// <inheritdoc />
        public override void Disable()
        {
            base.Disable();

            StopObserving();

            SpatialAwarenessSystem.RaiseSpatialAwarenessObserverLost(this);
        }

        #endregion IService Implementation

        #region ISpatialObserverDataProvider Implementation

        /// <inheritdoc />
        public AutoStartBehavior StartupBehavior { get; }

        /// <inheritdoc />
        public float UpdateInterval { get; set; }

        /// <inheritdoc />
        public virtual int PhysicsLayer { get; set; }

        /// <inheritdoc />
        public bool IsRunning { get; protected set; }

        /// <inheritdoc />
        public virtual void StartObserving()
        {
            if (!Application.isPlaying) { return; }
            IsRunning = true;
        }

        /// <inheritdoc />
        public virtual void StopObserving()
        {
            if (!Application.isPlaying) { return; }
            IsRunning = false;
        }

        #endregion ISpatialObserverDataProvider Implementation

        #region IEventSource Implementation

        /// <inheritdoc />
        public string SourceName => Name;

        /// <inheritdoc />
        public uint SourceId { get; }

        #endregion IEventSource Implementation

        #region IEquality Implementation

        /// <summary>
        /// Determines if the specified objects are equal.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static bool Equals(ISpatialAwarenessServiceModule left, ISpatialAwarenessServiceModule right)
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
            if (obj is null) { return false; }
            if (ReferenceEquals(this, obj)) { return true; }
            if (obj.GetType() != GetType()) { return false; }

            return Equals((ISpatialAwarenessServiceModule)obj);
        }

        private bool Equals(ISpatialAwarenessServiceModule other)
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