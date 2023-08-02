// Copyright (c) Reality Collective. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;

namespace RealityToolkit.Input.Hands
{
    /// <summary>
    /// A <see cref="IHandJointTransformProvider"/> provides a <see cref="UnityEngine.Transform"/>
    /// for a <see cref="HandJoint"/>. It can be used for animiation, when you need to modify joint poses
    /// on a hand rig.
    /// </summary>
    public interface IHandJointTransformProvider
    {
        /// <summary>
        /// Gets the <see cref="Transform"/> representation for <paramref name="joint"/>, if found.
        /// </summary>
        /// <param name="joint">The <see cref="HandJoint"/> to find the <see cref="Transform"/> for.</param>
        /// <returns><c>true</c>, if found..</returns>
        bool TryGetTransform(HandJoint joint, out Transform transform);
    }
}
