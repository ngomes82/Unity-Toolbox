using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.IO;

#if UNITY_EDITOR
using UnityEditor;
#endif
public enum AssetServerEnvironment
{
    Dev,
    Stag,
    Prod
}

public class AssetBundleManager
{
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

    public static string GetRemoteBundleUrl()
    {
        RuntimePlatform platform = Application.platform;

#if UNITY_EDITOR
        platform = GetRuntimePlatformFromBuildTarget(EditorUserBuildSettings.activeBuildTarget);
#endif

        return $"{rootAssetBundleUrl}/{serverEnvironment}/{Application.version}/{platform}/";
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


public class AssetBundleRequestCache
{
    private static Dictionary<string, UnityWebRequest> downloadBundleRequestMap = new Dictionary<string, UnityWebRequest>();
    private static Dictionary<string, int> downloadBundleRefCount = new Dictionary<string, int>();

    private static Dictionary<string, AssetBundleCreateRequest> loadBundleRequestMap = new Dictionary<string, AssetBundleCreateRequest>();
    private static Dictionary<string, int> loadBundleRefCount = new Dictionary<string, int>();

    public AssetBundleCreateRequest CreateLoadBundleFromFileRequest(string bundleName)
    {
        if (!loadBundleRequestMap.ContainsKey(bundleName))
        {
            var bundleFilePath = $"{AssetBundleManager.ASSET_BUNDLE_DOWNLOAD_FOLDER}/{bundleName}";
            loadBundleRequestMap[bundleName] = AssetBundle.LoadFromFileAsync(bundleFilePath);
            loadBundleRefCount[bundleName] = 1;
        }
        else
        {
            loadBundleRefCount[bundleName] += 1;
        }

        return loadBundleRequestMap[bundleName];
    }

    public AssetBundleCreateRequest CreateLoadBundleFromMemoryRequest(string bundleName, byte[] data)
    {
        if (!loadBundleRequestMap.ContainsKey(bundleName))
        {
            loadBundleRequestMap[bundleName] = AssetBundle.LoadFromMemoryAsync(data);
            loadBundleRefCount[bundleName] = 1;
        }
        else
        {
            loadBundleRefCount[bundleName] += 1;
        }

        return loadBundleRequestMap[bundleName];
    }

    public void ReleaseLoadBundleRequest(string bundleName)
    {
        loadBundleRefCount[bundleName] -= 1;

        if (loadBundleRefCount[bundleName] <= 0)
        {
            loadBundleRequestMap.Remove(bundleName);
            loadBundleRefCount.Remove(bundleName);
        }
    }

    public UnityWebRequest CreateHttpGetBundleRequest(string bundleName)
    {
        if (!downloadBundleRequestMap.ContainsKey(bundleName))
        {
            var webUrl = AssetBundleManager.GetRemoteBundleUrl() + bundleName;
            var newWebRequest = UnityWebRequest.Get(webUrl);
            newWebRequest.SendWebRequest();
            downloadBundleRequestMap[bundleName] = newWebRequest;
            downloadBundleRefCount[bundleName] = 1;
        }
        else
        {
            downloadBundleRefCount[bundleName] += 1;
        }

        return downloadBundleRequestMap[bundleName];
    }

    public void ReleaseHttpGetBundleRequest(string bundleName)
    {
        downloadBundleRefCount[bundleName] -= 1;

        if (downloadBundleRefCount[bundleName] <= 0)
        {
            downloadBundleRequestMap[bundleName].Dispose();
            downloadBundleRequestMap.Remove(bundleName);

            downloadBundleRefCount.Remove(bundleName);
        }
    }
}