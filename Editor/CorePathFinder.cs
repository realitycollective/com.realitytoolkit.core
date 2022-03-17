// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;
using XRTK.Editor.Utilities;

namespace XRTK.Editor
{
    /// <summary>
    /// Dummy scriptable object used to find the relative path of the com.xrtk.
    /// </summary>
    /// <inheritdoc cref="IPathFinder" />
    public class CorePathFinder : ScriptableObject, IPathFinder
    {
        /// <inheritdoc />
        public string Location => $"/Editor/{nameof(CorePathFinder)}.cs";
    }
}