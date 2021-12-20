using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class SpriteRendererAssetBundle : MonoBehaviour
{
    public SpriteBundleRef spriteAssetBundleRef;
    public SpriteRenderer spriteRenderer;


    // Start is called before the first frame update
    IEnumerator Start()
    {
        //DEBUG: Need to init asset bundle manager
        yield return AssetBundleManager.Init("https://test-asset-bundles.s3.us-east-2.amazonaws.com");
        //END DEBUG

        var request = spriteAssetBundleRef.CreateAssetLoadRequest();
        var routine = request.LoadAsset();

        while (!request.isDone)
        {
            Debug.Log(request.loadMessage);
            routine.MoveNext();
            yield return null;
        }

        spriteRenderer.sprite = request.asset;
    }
}
