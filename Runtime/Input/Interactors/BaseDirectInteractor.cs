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
        public IDirectInteractorResult DirectResult => directResult;

        /// <inheritdoc />
        protected override void OnRaisePointerDown(InputAction inputAction)
        {
            // Only if we have a target, we want to raise input down.
            if (DirectResult.CurrentTarget.IsNotNull())
            {
                base.OnRaisePointerDown(inputAction);
            }
        }
    }
}
