// Copyright (c) Reality Collective. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using RealityCollective.Extensions;
using RealityCollective.ServiceFramework.Attributes;
using RealityCollective.ServiceFramework.Definitions.Platforms;
using RealityCollective.ServiceFramework.Modules;
using RealityToolkit.Input.Interactions.Interactables;
using RealityToolkit.Input.Interactions.Interactors;
using RealityToolkit.Input.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace RealityToolkit.Input.Interactions
{
    /// <summary>
    /// Default <see cref="IInteractionServiceModule"/> implementation.
    /// </summary>
    [RuntimePlatform(typeof(AllPlatforms))]
    [System.Runtime.InteropServices.Guid("a25b0eb5-95f9-45ef-8645-3d19838b01ff")]
    public class InteractionServiceModule : BaseServiceModule, IInteractionServiceModule
    {
        /// <inheritdoc />
        public InteractionServiceModule(string name, uint priority, InteractionServiceModuleProfile profile, IInputService parentService)
            : base(name, priority, profile, parentService)
        {
            interactors = new List<IInteractor>();
            interactables = new List<IInteractable>();
            NearInteractionEnabled = profile.NearInteraction;
            FarInteractionEnabled = profile.FarInteraction;
        }

        private readonly List<IInteractor> interactors;
        private readonly List<IInteractable> interactables;

        /// <inheritdoc/>
        public bool NearInteractionEnabled { get; set; }

        /// <inheritdoc/>
        public bool FarInteractionEnabled { get; set; }

        /// <inheritdoc/>
        public IReadOnlyList<IInteractor> Interactors => interactors;

        /// <inheritdoc/>
        public IReadOnlyList<IInteractable> Interactables => interactables;

        /// <inheritdoc/>
        public void Add(IInteractor interactor) => interactors.EnsureListItem(interactor);

        /// <inheritdoc/>
        public void Remove(IInteractor interactor) => interactors.SafeRemoveListItem(interactor);

        /// <inheritdoc/>
        public void Add(IInteractable interactable) => interactables.EnsureListItem(interactable);

        /// <inheritdoc/>
        public void Remove(IInteractable interactable) => interactables.SafeRemoveListItem(interactable);

        /// <inheritdoc/>
        public bool TryGetInteractablesByLabel(string label, out IEnumerable<IInteractable> interactables)
        {
            var results = this.interactables.Where(i => !string.IsNullOrWhiteSpace(label) && string.Equals(i.Label, label));
            if (results.Any())
            {
                interactables = results;
                return true;
            }

            interactables = null;
            return false;
        }

        /// <inheritdoc/>
        public bool TryGetInteractors(IInputSource inputSource, out IReadOnlyList<IInteractor> interactors)
        {
            interactors = this.interactors.Where(i => i.InputSource == inputSource).ToList();
            return interactors != null && interactors.Count > 0;
        }
    }
}
