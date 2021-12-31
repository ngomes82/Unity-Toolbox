using System;
using System.Collections;
using System.IO;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

//https://medium.com/@pierrickbignet/managing-assetbundles-assets-references-in-unity-705db5338785

public interface IAssetBundleRef
{
    string BundleName { get; }
    string AssetName { get; }
    string AssetGuid { get; }

#if UNITY_EDITOR
    void UpdateRefs(UnityEngine.Object newObj);
#endif
}

public class AssetBundleRef<T> : IAssetBundleRef where T : UnityEngine.Object
{
    [SerializeField]
    private string bundleName;

    [SerializeField]
    private string assetName;

    [SerializeField]
    private string assetGuid;

    public string BundleName => bundleName;

    public string AssetName => assetName;

    public string AssetGuid => assetGuid;

#if UNITY_EDITOR
    public void UpdateRefs(UnityEngine.Object _assetReference)
    {
        if (_assetReference != null)
        {
            var assetPath = AssetDatabase.GetAssetPath(_assetReference);
            assetGuid = AssetDatabase.AssetPathToGUID(assetPath);
            var bundleName = AssetImporter.GetAtPath(assetPath).assetBundleName;
            this.bundleName = bundleName;
            assetName = Path.GetFileNameWithoutExtension(assetPath);
        }
        else
        {
            assetName = null;
            bundleName = null;
            assetGuid = null;
        }
    }
#endif

    public LoadAssetRequest<T> CreateAssetLoadRequest()
    {
        return AssetBundleManager.CreateLoadAssetRequest<T>(bundleName, assetName);
    }
}

[Serializable]
public class SpriteBundleRef : AssetBundleRef<Sprite>{}

[Serializable]
public class TextAssetBundleRef : AssetBundleRef<TextAsset>{}

[Serializable]
public class GameObjectBundleRef : AssetBundleRef<GameObject> { }