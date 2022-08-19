// Copyright (c) Reality Collective. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using RealityCollective.ServiceFramework.Interfaces;

namespace RealityToolkit
{
    /// <summary>
    /// Interface definition for the reality toolkit service that activates
    /// the toolkit in a <see cref="RealityCollective.ServiceFramework.ServiceManagerInstance"/>
    /// </summary>
    /// <example>
    /// ServiceManager.Instance.GetService<IRealityToolkit>();
    /// </example>
    public interface IRealityToolkit : IService { }
}
