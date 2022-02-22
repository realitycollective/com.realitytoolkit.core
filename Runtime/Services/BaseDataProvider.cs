// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using RealityToolkit.ServiceFramework.Interfaces;
using XRTK.Definitions;
using XRTK.Interfaces;

namespace XRTK.Services
{
    /// <summary>
    /// The base data provider implements <see cref="IMixedRealityDataProvider"/> and provides default properties for all data providers.
    /// </summary>
    public abstract class BaseDataProvider : RealityToolkit.ServiceFramework.Services.BaseServiceWithConstructor, IServiceDataProvider
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="name">The name of the service.</param>
        /// <param name="priority">The priority of the service.</param>
        /// <param name="profile">The optional <see cref="BaseMixedRealityProfile"/> for the data provider.</param>
        /// <param name="parentService">The <see cref="IMixedRealityService"/> that this <see cref="IMixedRealityDataProvider"/> is assigned to.</param>
        protected BaseDataProvider(string name, uint priority, BaseMixedRealityProfile profile, IService parentService) : base(name, priority)
        {
            ParentService = parentService ?? throw new ArgumentNullException($"{nameof(parentService)} cannot be null");
        }

        /// <inheritdoc />
        public IService ParentService { get; }

        private uint priority;

        /// <inheritdoc />
        public override uint Priority
        {
            get => priority + ParentService.Priority;
            protected set => priority = value;
        }
    }
}