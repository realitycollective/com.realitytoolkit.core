// Copyright (c) xRealityLabs. All rights reserved.

using System.IO;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEditor.UnityLinker;

namespace xRealityLabs.XRFoundation.Editor
{
    public class LinkXmlInstaller : IUnityLinkerProcessor
    {
        int IOrderedCallback.callbackOrder => 0;

        string IUnityLinkerProcessor.GenerateAdditionalLinkXmlFile(BuildReport report, UnityLinkerBuildPipelineData data)
        {
            // This is pretty ugly, but it was the only thing I could think of in order to reliably get the path to link.xml
            const string linkXmlGuid = "a5e451326b91fd345b67f663721b67ad"; // copied from link.xml.meta
            var assetPath = AssetDatabase.GUIDToAssetPath(linkXmlGuid);
            // assets paths are relative to the unity project root, but they don't correspond to actual folders for
            // Packages that are embedded. I.e. it won't work if a package is installed as a git submodule
            // So resolve it to an absolute path:
            return Path.GetFullPath(assetPath);
        }

        void IUnityLinkerProcessor.OnBeforeRun(BuildReport report, UnityLinkerBuildPipelineData data)
        {
        }

        void IUnityLinkerProcessor.OnAfterRun(BuildReport report, UnityLinkerBuildPipelineData data)
        {
        }
    }
}