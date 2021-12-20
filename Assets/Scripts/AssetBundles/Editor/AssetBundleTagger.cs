#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Text.RegularExpressions;

public class AssetBundleTagger : AssetPostprocessor
{
    static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
    {
        foreach (string str in importedAssets)
        {
            TryTagAsset(str);
        }

        for (int i = 0; i < movedAssets.Length; i++)
        {
            TryTagAsset(movedAssets[i]);
        }
    }

    private static void TryTagAsset(string str)
    {
        if (str.Contains("$"))
        {
            string bundleName = Regex.Match(str, "\\$[a-zA-Z0-9]+").Value;
            bundleName = bundleName.Replace("$", "").ToLower();

            var importer = AssetImporter.GetAtPath(str);
            if (importer.assetBundleName != bundleName)
            {
                importer.assetBundleName = bundleName;
            }
        }
    }
}
#endif