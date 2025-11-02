// Copyright (c) Reality Collective. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using RealityCollective.Utilities.Extensions;
using RealityToolkit.Definitions.Utilities;
using UnityEngine.Rendering;

namespace RealityToolkit.Utilities
{
    public static class RenderPipelineUtilities
    {
        private const string urpAssetTypeName = "UniversalRenderPipelineAsset";
        private const string hdrpAssetTypeName = "HDRenderPipelineAsset";

        /// <summary>
        /// Gets the <see cref="UnityRenderPipeline"/> used by the project.
        /// </summary>
        /// <returns>The <see cref="UnityRenderPipeline"/> used by the project.</returns>
        public static UnityRenderPipeline GetActiveRenderingPipeline()
        {
            var renderPipelineAsset = GraphicsSettings.currentRenderPipeline;

            if (renderPipelineAsset.IsNull())
            {
                return UnityRenderPipeline.Legacy;
            }

            return renderPipelineAsset.GetType().Name switch
            {
                urpAssetTypeName => UnityRenderPipeline.UniversalRenderPipeline,
                hdrpAssetTypeName => UnityRenderPipeline.HighDefinitionRenderPipeline,
                _ => UnityRenderPipeline.Custom,
            };
        }
    }
}