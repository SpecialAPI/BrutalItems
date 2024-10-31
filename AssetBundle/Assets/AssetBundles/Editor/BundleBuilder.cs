using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public static class BundleBuilder
{
    public static Dictionary<BuildTarget, string> Builds = new Dictionary<BuildTarget, string>
    {
        { BuildTarget.StandaloneWindows, "Windows" },
        { BuildTarget.StandaloneLinux, "Linux" },
        { BuildTarget.StandaloneOSX, "MacOS" }
    };

    [MenuItem("Assets/Build AssetBundles")]
	public static void BuildBundles()
    {
        foreach(var kvp in Builds)
        {
            var path = "Assets/AssetBundles/" + kvp.Value;

            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            BuildPipeline.BuildAssetBundles(path, BuildAssetBundleOptions.None, kvp.Key);
        }
    }
}
