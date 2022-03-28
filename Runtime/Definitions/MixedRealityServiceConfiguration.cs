// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;
using RealityToolkit.ServiceFramework.Definitions;
using RealityToolkit.ServiceFramework.Interfaces;
using UnityEngine;
using XRTK.Interfaces;
using XRTK.Services;

namespace XRTK.Definitions
{
    /// <inheritdoc cref="MixedRealityServiceConfiguration" />
    public class MixedRealityServiceConfiguration<T> : MixedRealityServiceConfiguration, IMixedRealityServiceConfiguration<T>
        where T : IService
    {
        /// <inheritdoc />
        public MixedRealityServiceConfiguration(IMixedRealityServiceConfiguration configuration)
            : base(configuration.ServiceConfiguration.InstancedType, configuration.ServiceConfiguration.Name, configuration.ServiceConfiguration.Priority, configuration.RuntimePlatforms, configuration.ServiceConfiguration.Profile)
        {
        }

        /// <inheritdoc />
        public MixedRealityServiceConfiguration(SystemType instancedType, string name, uint priority, IReadOnlyList<IMixedRealityPlatform> runtimePlatforms, BaseProfile profile)
            : base(instancedType, name, priority, runtimePlatforms, profile)
        {
        }

        /// <inheritdoc />
        public bool Enabled
            => typeof(IMixedRealitySystem).IsAssignableFrom(typeof(T))
                ? ServiceConfiguration.Profile != null && ServiceConfiguration.Enabled // All IMixedRealitySystems require a profile
                : ServiceConfiguration.Enabled;
    }

    /// <summary>
    /// Defines a <see cref="IMixedRealityService"/> to be registered with the <see cref="MixedRealityToolkit"/>.
    /// </summary>
    [Serializable]
    public class MixedRealityServiceConfiguration : IMixedRealityServiceConfiguration
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="instancedType">The concrete type for the <see cref="IMixedRealityService"/>.</param>
        /// <param name="name">The simple, human readable name for the <see cref="IMixedRealityService"/>.</param>
        /// <param name="priority">The priority this <see cref="IMixedRealityService"/> will be initialized in.</param>
        /// <param name="runtimePlatforms">runtimePlatform">The runtime platform(s) to run this <see cref="IMixedRealityService"/> to run on.</param>
        /// <param name="profile">The <see cref="BaseMixedRealityProfile"/> for <see cref="IMixedRealityService"/>.</param>
        public MixedRealityServiceConfiguration(SystemType instancedType, string name, uint priority, IReadOnlyList<IMixedRealityPlatform> runtimePlatforms, BaseProfile profile)
        {
            ServiceConfiguration = new ServiceConfiguration(instancedType, name, priority, profile);
            
            if (runtimePlatforms != null)
            {
                this.runtimePlatforms = new List<IMixedRealityPlatform>(runtimePlatforms.Count);

                for (int i = 0; i < runtimePlatforms.Count; i++)
                {
                    this.runtimePlatforms.Add(runtimePlatforms[i]);
                }

                platformEntries = new RuntimePlatformEntry(runtimePlatforms);
            }
        }

        public ServiceConfiguration ServiceConfiguration { get; }

        [SerializeField]
        private RuntimePlatformEntry platformEntries = new RuntimePlatformEntry();

        [NonSerialized]
        private List<IMixedRealityPlatform> runtimePlatforms = null;
        
        /// <inheritdoc />
        public IReadOnlyList<IMixedRealityPlatform> RuntimePlatforms
        {
            get
            {
                if (runtimePlatforms == null ||
                    runtimePlatforms.Count == 0 ||
                    runtimePlatforms.Count != platformEntries?.RuntimePlatforms?.Length)
                {
                    runtimePlatforms = new List<IMixedRealityPlatform>();

                    for (int i = 0; i < platformEntries?.RuntimePlatforms?.Length; i++)
                    {
                        var platformType = platformEntries.RuntimePlatforms[i]?.Type;

                        if (platformType == null)
                        {
                            continue;
                        }

                        IMixedRealityPlatform platformInstance;

                        try
                        {
                            platformInstance = Activator.CreateInstance(platformType) as IMixedRealityPlatform;
                        }
                        catch (Exception e)
                        {
                            Debug.LogError(e);
                            continue;
                        }

                        runtimePlatforms.Add(platformInstance);
                    }
                }

                return runtimePlatforms;
            }
        }
    }
}