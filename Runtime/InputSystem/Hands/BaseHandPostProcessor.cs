// Copyright (c) Reality Collective. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace RealityToolkit.InputSystem.Hands
{
    /// <summary>
    /// Base class for all <see cref="IHandDataPostProcessor"/> implementations.
    /// Makes sure all implementations implement the required constructor.
    /// </summary>
    public abstract class BaseHandPostProcessor : IHandDataPostProcessor
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="hand">The <see cref="IHandController"/> to post process <see cref="HandData"/> for.</param>
        /// <param name="settings">Configuration to use when post processing information for the <see cref="IHandController"/>.</param>
        public BaseHandPostProcessor(IHandController hand, HandControllerSettings settings)
        {
            Hand = hand;
            Settings = settings;
        }

        /// <summary>
        /// The <see cref="IHandController"/> to post process <see cref="HandData"/> for.
        /// </summary>
        protected IHandController Hand { get; private set; }

        /// <summary>
        /// Configuration to use when post processing information for the <see cref="IHandController"/>.
        /// </summary>
        protected HandControllerSettings Settings { get; private set; }

        /// <inheritdoc/>
        public abstract HandData PostProcess(HandData handData);
    }
}
