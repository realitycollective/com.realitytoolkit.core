// Copyright (c) Reality Collective. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using RealityToolkit.Input.Interactions.Interactables;
using RealityToolkit.Input.Interactions.Interactors;
using RealityToolkit.Input.Interfaces;
using RealityToolkit.Input.Interfaces.Modules;
using System.Collections.Generic;

namespace RealityToolkit.Input.Interactions
{
    /// <summary>
    /// An <see cref="IInputServiceModule"/> that provides events and APIs useful for managing
    /// interactions between <see cref="IInteractable"/>s and <see cref="IInteractor"/>s.
    /// </summary>
    public interface IInteractionServiceModule : IInputServiceModule
    {
        /// <summary>
        /// Gets or sets whether near interaction should work or not.
        /// </summary>
        bool NearInteractionEnabled { get; set; }

        /// <summary>
        /// Gets or sets whether far interaction should work or not.
        /// </summary>
        bool FarInteractionEnabled { get; set; }

        /// <summary>
        /// Available <see cref="IInteractor"/>s in the scene.
        /// </summary>
        IReadOnlyList<IInteractor> Interactors { get; }

        /// <summary>
        /// Available <see cref="IInteractable"/>s in the scene.
        /// </summary>
        IReadOnlyList<IInteractable> Interactables { get; }

        /// <summary>
        /// Adds an <see cref="IInteractor"/> to the service's registry.
        /// </summary>
        /// <param name="interactor">The <see cref="IInteractor"/> to add.</param>
        void Add(IInteractor interactor);

        /// <summary>
        /// Removes an <see cref="IInteractor"/> from the service's registry.
        /// </summary>
        /// <param name="interactor">The <see cref="IInteractor"/> to remove.</param>
        void Remove(IInteractor interactor);

        /// <summary>
        /// Adds an <see cref="IInteractable"/> to the service's registry.
        /// </summary>
        /// <param name="interactable">The <see cref="IInteractable"/> to add.</param>
        void Add(IInteractable interactable);

        /// <summary>
        /// Removes an <see cref="IInteractable"/> from the service's registry.
        /// </summary>
        /// <param name="interactable">The <see cref="IInteractable"/> to remove.</param>
        void Remove(IInteractable interactable);

        /// <summary>
        /// Gets all known <see cref="IInteractable"/>s that have the <paramref name="label"/> provided.
        /// </summary>
        /// <param name="label">The label to look for.</param>
        /// <param name="interactables">Collection of <see cref="IInteractable"/>s with the requested label.</param>
        /// <returns><c>true, if any <see cref="IInteractable"/>s were found.</c></returns>
        bool TryGetInteractablesByLabel(string label, out IEnumerable<IInteractable> interactables);

        /// <summary>
        /// Gets the <see cref="IInteractor"/>s for the <paramref name="inputSource"/>.
        /// </summary>
        /// <param name="inputSource">The <see cref="IInputSource"/> to find the <see cref="IInteractor"/>s for.</param>
        /// <param name="interactors">The <see cref="IInteractor"/>s found.</param>
        /// <returns><c>true</c>, if <see cref="IInteractor"/>s were found.</returns>
        bool TryGetInteractors(IInputSource inputSource, out IReadOnlyList<IInteractor> interactors);
    }
}
