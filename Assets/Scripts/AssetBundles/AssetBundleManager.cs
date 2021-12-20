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
    public static Dictionary<string, string> serverBundleHashes = new Dictionary<string, string>();
    public static Dictionary<string, string> clientBundleHashes = new Dictionary<string, string>();
    
    public static Dictionary<string, AssetBundle> loadedBundles = new Dictionary<string, AssetBundle>();
    
    private static Dictionary<string, UnityWebRequest> downloadBundleRequestMap       = new Dictionary<string, UnityWebRequest>();
    private static Dictionary<string, int> downloadBundleRefCount                     = new Dictionary<string, int>();

    private static Dictionary<string, AssetBundleCreateRequest> loadBundleRequestMap  = new Dictionary<string, AssetBundleCreateRequest>();
    private static Dictionary<string, int> loadBundleRefCount                         = new Dictionary<string, int>();


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

    public static LoadAssetRequest<T> CreateAssetLoadRequest<T>(string _bundleName, string _assetName) where T: UnityEngine.Object
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

    public static AssetBundleCreateRequest CreateLoadBundleFromFileRequest(string bundleName)
    {
        if (!AssetBundleManager.loadBundleRequestMap.ContainsKey(bundleName))
        {
            var bundleFilePath = $"{AssetBundleManager.ASSET_BUNDLE_DOWNLOAD_FOLDER}/{bundleName}";
            AssetBundleManager.loadBundleRequestMap[bundleName] = AssetBundle.LoadFromFileAsync(bundleFilePath);
            AssetBundleManager.loadBundleRefCount[bundleName] = 1;
        }
        else
        {
            AssetBundleManager.loadBundleRefCount[bundleName] += 1;
        }

        return AssetBundleManager.loadBundleRequestMap[bundleName];
    }

    public static AssetBundleCreateRequest CreateLoadBundleFromMemoryRequest(string bundleName, byte[] data)
    {
        if (!AssetBundleManager.loadBundleRequestMap.ContainsKey(bundleName))
        {
            AssetBundleManager.loadBundleRequestMap[bundleName] = AssetBundle.LoadFromMemoryAsync(data);
            AssetBundleManager.loadBundleRefCount[bundleName] = 1;
        }
        else
        {
            AssetBundleManager.loadBundleRefCount[bundleName] += 1;
        }

        return AssetBundleManager.loadBundleRequestMap[bundleName];
    }

    public static void ReleaseLoadBundleRequest(string bundleName)
    {
        AssetBundleManager.loadBundleRefCount[bundleName] -= 1;

        if( AssetBundleManager.loadBundleRefCount[bundleName] <= 0)
        {
            AssetBundleManager.loadBundleRequestMap.Remove(bundleName);
            AssetBundleManager.loadBundleRefCount.Remove(bundleName);
        }
    }

    public static UnityWebRequest CreateHttpGetBundleRequest(string bundleName)
    {
        if (!AssetBundleManager.downloadBundleRequestMap.ContainsKey(bundleName))
        {
            var webUrl = AssetBundleManager.GetRemoteBundleUrl() + bundleName;
            var newWebRequest = UnityWebRequest.Get(webUrl);
            newWebRequest.SendWebRequest();
            AssetBundleManager.downloadBundleRequestMap[bundleName] = newWebRequest;
            AssetBundleManager.downloadBundleRefCount[bundleName] = 1;
        }
        else
        {
            AssetBundleManager.downloadBundleRefCount[bundleName] += 1;
        }

        return AssetBundleManager.downloadBundleRequestMap[bundleName];
    }

    public static void ReleaseHttpGetBundleRequest(string bundleName)
    {
        AssetBundleManager.downloadBundleRefCount[bundleName] -= 1;

        if(AssetBundleManager.downloadBundleRefCount[bundleName] <= 0)
        {
            AssetBundleManager.downloadBundleRequestMap[bundleName].Dispose();
            AssetBundleManager.downloadBundleRequestMap.Remove(bundleName);

            AssetBundleManager.downloadBundleRefCount.Remove(bundleName);
        }
    }

    private static string GetRemoteBundleUrl()
    {
        RuntimePlatform platform = Application.platform;

#if UNITY_EDITOR
        platform = GetRuntimePlatformFromBuildTarget(EditorUserBuildSettings.activeBuildTarget);
#endif

        return $"{rootAssetBundleUrl}/{Application.version}/{platform}/";
    }


#if UNITY_EDITOR
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
