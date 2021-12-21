using Newtonsoft.Json;
using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;


public class LoadBundleRequest
{
    public string bundleName;

    public AssetBundle assetBundle = null;

    public string loadMessage = "Loading Start";
    public bool isDone = false;

    public IEnumerator LoadBundle()
    {
        if (!AssetBundleManager.loadedBundles.ContainsKey(bundleName))
        {
            var bundleFilePath = $"{AssetBundleManager.ASSET_BUNDLE_DOWNLOAD_FOLDER}/{bundleName}";

            if (!AssetBundleManager.IsDownloadedBundleValid(bundleName))
            {
                var www = AssetBundleManager.requestCache.CreateHttpGetBundleRequest(bundleName);

                while (!www.isDone)
                {
                    loadMessage = $"Downloading Bundle: {bundleName} ({www.downloadProgress})";
                    yield return null;
                }

                if (www.result == UnityWebRequest.Result.Success)
                {
                    if (!Directory.Exists(AssetBundleManager.ASSET_BUNDLE_DOWNLOAD_FOLDER))
                    {
                        Directory.CreateDirectory(AssetBundleManager.ASSET_BUNDLE_DOWNLOAD_FOLDER);
                    }

                    File.WriteAllBytes(bundleFilePath, www.downloadHandler.data);

                    AssetBundleManager.clientBundleHashes[bundleName] = AssetBundleManager.serverBundleHashes[bundleName];
                    File.WriteAllText($"{AssetBundleManager.ASSET_BUNDLE_DOWNLOAD_FOLDER}/{AssetBundleManager.ASSET_BUNDLE_HASH_FILE}", JsonConvert.SerializeObject(AssetBundleManager.clientBundleHashes));

                    AssetBundleCreateRequest loadBundle = AssetBundleManager.requestCache.CreateLoadBundleFromMemoryRequest(bundleName, www.downloadHandler.data);
                    AssetBundleManager.requestCache.ReleaseHttpGetBundleRequest(bundleName);

                    while (!loadBundle.isDone)
                    {
                        loadMessage = $"Loading Bundle: {bundleName} ({loadBundle.progress})";
                        yield return null;
                    }

                    AssetBundleManager.loadedBundles[bundleName] = loadBundle.assetBundle;
                    AssetBundleManager.requestCache.ReleaseLoadBundleRequest(bundleName);
                }
                else
                {
                    AssetBundleManager.requestCache.ReleaseHttpGetBundleRequest(bundleName);
                    Debug.LogException(new System.Exception($"HTTP GET ERROR [{www.responseCode}] ({www.url}) -- While downloading bundle: {bundleName} -- error:{www.error}"));
                }
            }

            if (!AssetBundleManager.loadedBundles.ContainsKey(bundleName) && File.Exists(bundleFilePath))
            {
                var loadBundle = AssetBundleManager.requestCache.CreateLoadBundleFromFileRequest(bundleName);

                while (!loadBundle.isDone)
                {
                    loadMessage = $"Loading Bundle: {bundleName} ({loadBundle.progress})";
                    yield return null;
                }

                AssetBundleManager.loadedBundles[bundleName] = loadBundle.assetBundle;
                AssetBundleManager.requestCache.ReleaseLoadBundleRequest(bundleName);
            }
        }

        if (AssetBundleManager.loadedBundles.ContainsKey(bundleName))
        {
            assetBundle = AssetBundleManager.loadedBundles[bundleName];
        }
        else
        {
            Debug.LogException(new System.Exception($"BUNDLE ERROR: Failed to load bundle {bundleName}"));
        }

        isDone = true;
    }

}

public class LoadAssetRequest<T> where T : UnityEngine.Object
{
    public string bundleName;
    public string assetName;
    public string loadMessage = "Loading Start";
    public bool isDone = false;
    public T asset = null;

    public IEnumerator LoadAsset()
    {
        var loadBundleRequest = AssetBundleManager.CreateLoadBundleRequest(bundleName);
        var routine = loadBundleRequest.LoadBundle();

        while(!loadBundleRequest.isDone)
        {
            routine.MoveNext();
            yield return null;
        }

        if (loadBundleRequest.assetBundle != null)
        {
            var bundle = loadBundleRequest.assetBundle;

            if (bundle != null)
            {
                var loadAsset = bundle.LoadAssetAsync<T>(assetName);
                
                while(!loadAsset.isDone)
                {
                    loadMessage = $"Loading Asset: {assetName} ({loadAsset.progress})";
                    yield return null;
                }
                
                asset = loadAsset.asset as T;

                if(asset == null)
                {
                    Debug.LogException(new System.Exception($"BUNDLE ERROR: Failed to load asset {bundleName}::{assetName}"));
                }

            }
        }

        isDone = true;
    }
}