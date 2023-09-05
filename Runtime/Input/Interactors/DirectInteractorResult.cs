// Copyright (c) Reality Collective. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;

namespace RealityToolkit.Input.Interactors
{
    /// <summary>
    /// Default <see cref="IDirectInteractor"/> implementation.
    /// </summary>
    public class DirectInteractorResult : IDirectInteractorResult
    {
        /// <inheritdoc/>
        public GameObject PreviousTarget { get; private set; }

        /// <inheritdoc/>
        public GameObject CurrentTarget { get; private set; }

        /// <inheritdoc/>
        public float Distance { get; private set; }

        /// <inheritdoc/>
        public Vector3 Direction { get; private set; }

        /// <inheritdoc/>
        public void UpdateHit(IDirectInteractor interactor, GameObject hitObject)
        {
            PreviousTarget = CurrentTarget;
            CurrentTarget = hitObject;

            var direction = hitObject.transform.position - interactor.GameObject.transform.position;
            Distance = Vector3.Magnitude(direction);
            Direction = direction.normalized;
        }

        /// <inheritdoc/>
        public void Clear()
        {
            PreviousTarget = CurrentTarget;
            CurrentTarget = null;
            Distance = 0f;
            Direction = Vector3.zero;
        }
    }
}
