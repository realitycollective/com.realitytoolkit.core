// Copyright (c) Reality Collective. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;

namespace RealityToolkit.Utilities
{
    /// <summary>
    /// Sets global shader variables relating to calibration space transforms
    /// </summary>
    public class CalibrationSpace : MonoBehaviour
    {
        private static readonly int CalibrationSpaceWorldToLocal = Shader.PropertyToID("CalibrationSpaceWorldToLocal");
        private static readonly int CalibrationSpaceLocalToWorld = Shader.PropertyToID("CalibrationSpaceLocalToWorld");

        private void Update()
        {
            Shader.SetGlobalMatrix(CalibrationSpaceWorldToLocal, transform.worldToLocalMatrix);
            Shader.SetGlobalMatrix(CalibrationSpaceLocalToWorld, transform.localToWorldMatrix);
        }
    }
}
