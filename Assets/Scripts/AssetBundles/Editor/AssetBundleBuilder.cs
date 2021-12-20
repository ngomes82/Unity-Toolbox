#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using System;
using System.Collections;
using System.IO;
using System.Collections.Generic;
using Newtonsoft.Json;

public class AssetBundleBuilder : EditorWindow
{
    [MenuItem("Tools/Asset Bundle Builder")]
    static void ShowWindow()
    {
        var window = EditorWindow.GetWindow(typeof(AssetBundleBuilder));
        window.maxSize = new Vector2(500f, 500f);
        window.minSize = window.maxSize;
    }

    void OnGUI()
    {

        if (GUILayout.Button("Build All"))
        {
            string outputPath = GetBundleBuildDir();

            var manifest = BuildPipeline.BuildAssetBundles(outputPath, BuildAssetBundleOptions.ChunkBasedCompression, EditorUserBuildSettings.activeBuildTarget);

            var bundleHashMap = new Dictionary<string, string>();
            var assetBundles = manifest.GetAllAssetBundles();
            for(int i=0; i < assetBundles.Length; i++)
            {
                bundleHashMap[assetBundles[i]] = manifest.GetAssetBundleHash(assetBundles[i]).ToString();
            }

            File.WriteAllText(GetBundleBuildDir() + "/bundleHashes.json", JsonConvert.SerializeObject(bundleHashMap) );
        }

        if (GUILayout.Button("Upload All"))
        {
            //HTTP PUT bundles up on s3 or firestore
            //https://medium.com/xrlo-extended-reality-lowdown/uploading-to-aws-from-unity-5e7de2c80fce
        }
    }

    private string GetBundleBuildDir()
    {
        string outputPath = $"Build/AssetBundles/{ AssetBundleManager.GetRuntimePlatformFromBuildTarget(EditorUserBuildSettings.activeBuildTarget) }";

        if (!Directory.Exists(outputPath))
        {
            Directory.CreateDirectory(outputPath);
        }

        return outputPath;
    }
}
#endif