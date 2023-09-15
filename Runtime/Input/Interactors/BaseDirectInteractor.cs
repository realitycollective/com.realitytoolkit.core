// Copyright (c) Reality Collective. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using RealityCollective.Extensions;
using RealityToolkit.Input.Definitions;

namespace RealityToolkit.Input.Interactors
{
    /// <summary>
    /// Abstract base implementation for <see cref="IDirectInteractor"/>s.
    /// </summary>
    public abstract class BaseDirectInteractor : BaseControllerInteractor, IDirectInteractor
    {
        protected readonly DirectInteractorResult directResult = new DirectInteractorResult();

        /// <inheritdoc />
        public override bool IsFarInteractor => false;

        /// <inheritdoc />
        public IDirectInteractorResult DirectResult => directResult;

        /// <inheritdoc />
        /// <remarks>
        /// All <see cref="BaseDirectInteractor"/>s cannot give privilege to <see cref="IDirectInteractor"/>s,
        /// since they are a <see cref="IDirectInteractor"/> themselves.
        /// </remarks>
        public override bool DirectPrivilege
        {
            get => false;
            set { }
        }

        /// <inheritdoc />
        public bool PokePrivilege { get; set; }

        /// <inheritdoc />
        public override bool IsInteractionEnabled
        {
            get
            {
                if (PokePrivilege)
                {
                    return false;
                }

                return base.IsInteractionEnabled;
            }
        }

        /// <inheritdoc />
        /// <remarks>
        /// For direct interactors we must check for the direct interaction result.
        /// Base implementation will only check for raycast based results.
        /// </remarks>
        protected override void OnRaisePointerDown(InputAction inputAction)
        {
            if (DirectResult.CurrentTarget.IsNotNull() && IsInteractionEnabled)
            {
                InputService.RaisePointerDown(this, inputAction);
            }
        }

        /// <inheritdoc />
        /// <remarks>
        /// For direct interactors we must check for the direct interaction result.
        /// Base implementation will only check for raycast based results.
        /// </remarks>
        protected override void OnRaisePointerClicked(InputAction inputAction)
        {
            if (DirectResult.CurrentTarget.IsNotNull() && IsInteractionEnabled)
            {
                InputService.RaisePointerClicked(this, inputAction);
            }
        }
    }
}
