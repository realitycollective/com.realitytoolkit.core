// Copyright (c) Reality Collective. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using RealityToolkit.Input.Interactables;

namespace RealityToolkit.Input.Interactors
{
    /// <summary>
    /// <see cref="IDirectInteractor"/> that interacts with <see cref="IInteractable"/>s that by touching them.
    /// </summary>
    public class PokeInteractor : NearInteractor, IPokeInteractor
    {
        private void Update()
        {
            if (Controller == null || Controller.Visualizer == null)
            {
                return;
            }

            transform.SetPositionAndRotation(Controller.Visualizer.PokePose.position, Controller.Visualizer.PokePose.rotation);
        }
    }
}
