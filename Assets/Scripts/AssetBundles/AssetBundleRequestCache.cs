using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class AssetBundleRequestCache
{
    private static Dictionary<string, UnityWebRequest> downloadBundleRequestMap = new Dictionary<string, UnityWebRequest>();
    private static Dictionary<string, int> downloadBundleRefCount = new Dictionary<string, int>();

    private static Dictionary<string, AssetBundleCreateRequest> loadBundleRequestMap = new Dictionary<string, AssetBundleCreateRequest>();
    private static Dictionary<string, int> loadBundleRefCount = new Dictionary<string, int>();

    public AssetBundleCreateRequest CreateLoadBundleFromFileRequest(string bundleName, string bundleFilePath)
    {
        if (!loadBundleRequestMap.ContainsKey(bundleName))
        {
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