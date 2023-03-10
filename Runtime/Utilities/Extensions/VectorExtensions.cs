// Copyright (c) Reality Collective. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;
using UnityEngine.EventSystems;

namespace RealityToolkit.Utilities.Extensions
{
    /// <summary>
    /// Extension methods for Unity's Vector struct
    /// </summary>
    public static class VectorExtensions
    {
        /// <summary>
        /// Determine the move direction based off of the direction provided
        /// </summary>
        /// <param name="direction"></param>
        /// <param name="deadZone"></param>
        /// <returns></returns>
        public static MoveDirection DetermineMoveDirection(this Vector2 direction, float deadZone = 0.6f)
        {
            if (direction.sqrMagnitude < deadZone * deadZone)
            {
                return MoveDirection.None;
            }

            if (Mathf.Abs(direction.x) > Mathf.Abs(direction.y))
            {
                return direction.x > 0 ? MoveDirection.Right : MoveDirection.Left;
            }

            return direction.y > 0 ? MoveDirection.Up : MoveDirection.Down;
        }
    }
}