// Copyright (c) Reality Collective. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace RealityToolkit.Input.Interactors
{
    /// <summary>
    /// A <see cref="IInteractor"/> for direction interactions, such as poking, touching, and grabbing.
    /// </summary>
    public interface IDirectInteractor : IControllerInteractor
    {
        /// <summary>
        /// If set, this <see cref="IDirectInteractor"/> is giving input privilege to <see cref="IPokeInteractor"/>s.
        /// </summary>
        bool PokePrivilege { get; set; }

        /// <summary>
        /// The combined physics and graphics raycast pointer result for
        /// the <see cref="IDirectInteractor"/>.
        /// </summary>
        IDirectInteractorResult DirectResult { get; }
    }
}
