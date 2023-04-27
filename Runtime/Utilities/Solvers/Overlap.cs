// Copyright (c) Reality Collective. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace RealityToolkit.Utilities.Solvers
{
    /// <summary>
    /// Provides a solver that overlaps with the tracked object.
    /// </summary>
    public class Overlap : Solver
    {
        public override void SolverUpdate()
        {
            GoalPosition = SolverHandler.TransformTarget.position;
            GoalRotation = SolverHandler.TransformTarget.rotation;

            UpdateWorkingPositionToGoal();
            UpdateWorkingRotationToGoal();
        }
    }
}