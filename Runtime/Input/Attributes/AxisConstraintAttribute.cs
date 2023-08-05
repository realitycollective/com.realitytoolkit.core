// Copyright (c) Reality Collective. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using RealityCollective.Definitions.Utilities;
using UnityEngine;

namespace RealityToolkit.Input.Attributes
{
    /// <summary>
    /// Use this attribute to better define <see cref="InputAction"/>s.
    /// This will filter the dropdown items to only display actions that match the <see cref="AxisConstraint"/>
    /// </summary>
    /// <example>
    /// <code language="csharp"><![CDATA[
    /// [SerializeField]
    /// [AxisConstraint(AxisType.DualAxis)]
    /// private InputAction testAction = InputAction.None;
    /// ]]></code>
    /// </example>
    public sealed class AxisConstraintAttribute : PropertyAttribute
    {
        public AxisType AxisConstraint { get; }

        public AxisConstraintAttribute(AxisType axisConstraint)
        {
            AxisConstraint = axisConstraint;
        }
    }
}