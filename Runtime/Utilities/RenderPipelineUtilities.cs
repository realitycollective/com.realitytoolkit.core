// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine.Rendering;
using RealityToolkit.Extensions;
using RenderPipeline = RealityToolkit.Definitions.Utilities.RenderPipeline;

namespace RealityToolkit.Utilities
{
    public static class RenderPipelineUtilities
    {
        private const string urpAssetTypeName = "UniversalRenderPipelineAsset";
        private const string hdrpAssetTypeName = "HDRenderPipelineAsset";

        /// <summary>
        /// Gets the <see cref="Definitions.Utilities.RenderPipeline"/> used by the project.
        /// </summary>
        /// <returns>The <see cref="Definitions.Utilities.RenderPipeline"/> used by the project.</returns>
        public static Definitions.Utilities.RenderPipeline GetActiveRenderingPipeline()
        {
            var renderPipelineAsset = GraphicsSettings.renderPipelineAsset;
            if (renderPipelineAsset.IsNull())
            {
                return Definitions.Utilities.RenderPipeline.Legacy;
            }

            switch (renderPipelineAsset.GetType().Name)
            {
                case urpAssetTypeName:
                    return Definitions.Utilities.RenderPipeline.UniversalRenderPipeline;
                case hdrpAssetTypeName:
                    return Definitions.Utilities.RenderPipeline.HighDefinitionRenderPipeline;
            }

            return Definitions.Utilities.RenderPipeline.Custom;
        }
    }
}