using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.IO;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class AssetBundleManager
{
#if UNITY_EDITOR
    public const string KEY_EDITOR_BUNDLE_LOAD_MODE = "BundleLoadMode";

    public enum EditorBundleLoadMode
    {
        Simulation = 0,   // Load tagged assets directly from editor using Unity's AssetDatabase.
        Local = 1,        // Load from locally built asset bundles that exist in Build/AssetBundles - ignore server bundles.
        Server = 2        // Behave like a live build, download new or missing asset bundles from server, load downloaded bundles.
    }
#endif

    public enum AssetServerEnvironment
    {
        Dev,
        Stag,
        Prod
    }

    public static AssetServerEnvironment serverEnvironment = AssetServerEnvironment.Dev;

    public static Dictionary<string, string> serverBundleHashes = new Dictionary<string, string>();
    public static Dictionary<string, string> clientBundleHashes = new Dictionary<string, string>();
    
    public static Dictionary<string, AssetBundle> loadedBundles = new Dictionary<string, AssetBundle>();

    public static AssetBundleRequestCache requestCache = new AssetBundleRequestCache();


    private static string rootAssetBundleUrl         = "https://your-fallback-url-goes-here.com";
    public const string ASSET_BUNDLE_HASH_FILE       = "bundleHashes.json";
    public const string ASSET_BUNDLE_DOWNLOAD_FOLDER = "DownloadedBundles";


    public static IEnumerator Init(string _rootAssetBundleUrl)
    {
        rootAssetBundleUrl = _rootAssetBundleUrl;

        var clientHashesFilepath = $"{ASSET_BUNDLE_DOWNLOAD_FOLDER}/{ASSET_BUNDLE_HASH_FILE}";
        if(File.Exists(clientHashesFilepath))
        {
            clientBundleHashes = JsonConvert.DeserializeObject<Dictionary<string, string>>(File.ReadAllText(clientHashesFilepath));
        }

        UnityWebRequest www = UnityWebRequest.Get($"{GetRemoteBundleUrl()}{ASSET_BUNDLE_HASH_FILE}");
        
        yield return www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.Success)
        {
            serverBundleHashes = JsonConvert.DeserializeObject<Dictionary<string, string>>(www.downloadHandler.text);
        }
        else
        {
            Debug.LogException(new System.Exception($"HTTP ERROR [{www.responseCode}] ({www.url}) Response -- error:{www.error} data: {www.downloadHandler.text}"));
        }
    }

    public static LoadBundleRequest CreateLoadBundleRequest(string _bundleName)
    {
        return new LoadBundleRequest()
        {
            bundleName = _bundleName
        };
    }

    public static LoadAssetRequest<T> CreateLoadAssetRequest<T>(string _bundleName, string _assetName) where T: UnityEngine.Object
    {
        return new LoadAssetRequest<T>()
        {
            bundleName = _bundleName,
            assetName  = _assetName
        };
    }

    public static bool IsDownloadedBundleValid(string bundleName)
    {
        var bundleFilePath = $"{ASSET_BUNDLE_DOWNLOAD_FOLDER}/{bundleName}";
        return File.Exists(bundleFilePath) && clientBundleHashes.ContainsKey(bundleName) && clientBundleHashes[bundleName] == serverBundleHashes[bundleName];
    }

    public static void UnloadBundle(string bundleName, bool forceUnloadAssets = true)
    {
        if (AssetBundleManager.loadedBundles.ContainsKey(bundleName))
        {
            AssetBundleManager.loadedBundles[bundleName].Unload(forceUnloadAssets);
            AssetBundleManager.loadedBundles.Remove(bundleName);
        }
    }

    public static void UnloadAllAssetBundles(bool forceUnloadAssets = true)
    {
        AssetBundle.UnloadAllAssetBundles(forceUnloadAssets);
        AssetBundleManager.loadedBundles.Clear();
    }

    public static string GetRemoteBundleUrl()
    {
        RuntimePlatform platform = Application.platform;

#if UNITY_EDITOR
        platform = GetRuntimePlatformFromBuildTarget(EditorUserBuildSettings.activeBuildTarget);
#endif

        return $"{rootAssetBundleUrl}/{serverEnvironment}/{Application.version}/{platform}/";
    }


#if UNITY_EDITOR
    public static string GetBundleBuildDir()
    {
        string outputPath = $"Build/AssetBundles/{ AssetBundleManager.GetRuntimePlatformFromBuildTarget(EditorUserBuildSettings.activeBuildTarget) }";

        if (!Directory.Exists(outputPath))
        {
            Directory.CreateDirectory(outputPath);
        }

        return outputPath;
    }

    public static RuntimePlatform GetRuntimePlatformFromBuildTarget(BuildTarget target)
    {
        Dictionary<BuildTarget, RuntimePlatform> buildTargetToRuntimePlatformMap = new Dictionary<BuildTarget, RuntimePlatform>()
        {
            {BuildTarget.iOS, RuntimePlatform.IPhonePlayer },
            {BuildTarget.Android, RuntimePlatform.Android }
        };

        return buildTargetToRuntimePlatformMap[target];
    }
#endif
}

