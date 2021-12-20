#if UNITY_EDITOR
using System.Reflection;
using UnityEditor;
using UnityEngine;

public static class AssetBundleRefPropertyDrawer 
{
   public static void DrawPropertyDrawer<AssetBundleRefType, AssetType>(Rect pos, FieldInfo fieldInfo, SerializedProperty property) 
        where AssetBundleRefType : class, IAssetBundleRef where AssetType : UnityEngine.Object
    {
       var assetBundleRef = fieldInfo.GetValue(property.serializedObject.targetObject) as AssetBundleRefType;
       var path = AssetDatabase.GUIDToAssetPath(assetBundleRef.AssetGuid);
       var obj = AssetDatabase.LoadAssetAtPath<AssetType>(path);

        var newObj = EditorGUI.ObjectField(pos, typeof(AssetType).ToString().Replace("UnityEngine.", ""), obj, typeof(AssetType), false);

        if(newObj != obj)
        {
            assetBundleRef.UpdateRefs(newObj);
            EditorUtility.SetDirty(property.serializedObject.targetObject);
        }
    }
}

[CustomPropertyDrawer(typeof(SpriteBundleRef))]
public class SpriteBundleRefPropertyDrawer : PropertyDrawer
{
    public override void OnGUI(Rect pos, SerializedProperty property, GUIContent label)
    {
        AssetBundleRefPropertyDrawer.DrawPropertyDrawer<SpriteBundleRef, Sprite>(pos, fieldInfo, property);
    }
}

[CustomPropertyDrawer(typeof(TextAssetBundleRef))]
public class TextAssetBundleRefPropertyDrawer : PropertyDrawer
{
    public override void OnGUI(Rect pos, SerializedProperty property, GUIContent label)
    {
        AssetBundleRefPropertyDrawer.DrawPropertyDrawer<TextAssetBundleRef, TextAsset>(pos, fieldInfo, property);
    }
}

[CustomPropertyDrawer(typeof(GameObjectBundleRef))]
public class GameObjectBundleRefPropertyDrawer : PropertyDrawer
{
    public override void OnGUI(Rect pos, SerializedProperty property, GUIContent label)
    {
        AssetBundleRefPropertyDrawer.DrawPropertyDrawer<GameObjectBundleRef, GameObject>(pos, fieldInfo, property);
    }
}
#endif