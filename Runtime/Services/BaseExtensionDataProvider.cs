// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using RealityToolkit.Definitions;
using RealityToolkit.Interfaces;

namespace RealityToolkit.Services
{
    /// <summary>
    /// The base extension data provider implements <see cref="IMixedRealityExtensionDataProvider"/> and provides default properties for all extension data providers.
    /// </summary>
    /// <remarks>
    /// Empty, but reserved for future use, in case additional <see cref="IMixedRealityExtensionDataProvider"/> properties or methods are assigned.
    /// </remarks>
    public abstract class BaseExtensionDataProvider : BaseDataProvider, IMixedRealityExtensionDataProvider
    {
        /// <inheritdoc />
        protected BaseExtensionDataProvider(string name, uint priority, BaseMixedRealityExtensionDataProviderProfile profile, IMixedRealityExtensionService parentService)
            : base(name, priority, profile, parentService)
        {
        }
    }
}
