#if UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class JsonFileEditor<T1, T2> where T1 : new() where T2 : Dictionary<string, T1>
{
    private string relativePath;
    public T2 dataContainer;
    public EditorGuiDictionary<T1> dictionaryEditor;

    public JsonFileEditor(string argRelativePath)
    {
        relativePath = argRelativePath;

        var textAsset = AssetDatabase.LoadAssetAtPath<TextAsset>(relativePath);
        dataContainer = textAsset.text.DeserializeJson<T2>();
        dictionaryEditor = new EditorGuiDictionary<T1>(dataContainer);
    }

    public void DrawGUI(Action drawDataGUIFunc)
    {
        EditorGUILayout.BeginHorizontal();

        GUILayout.BeginHorizontal(GUILayout.Width(250f));
        
        dictionaryEditor.DrawGUI();
        
        GUILayout.EndHorizontal();
    
        drawDataGUIFunc();

        EditorGUILayout.EndHorizontal();

        if (GUILayout.Button("Save Data", GUILayout.Height(30f)))
        {
            SaveData();
        }
    }

    private void SaveData()
    {
        var jsonData = dataContainer.SerializeJson();
        File.WriteAllText(relativePath, jsonData);
        AssetDatabase.ImportAsset(relativePath);
    }
}
#endif