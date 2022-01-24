#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;
using System;
using UnityEditor.IMGUI.Controls;

public class EditorGuiDictionary<V> where V : new()
{
    public event Action<string> OnKeySelected;
    public event Action<string, string> OnKeyNameChanged;
    public event Action<string> OnKeyAdded;
    public event Action<string> OnKeyRemoved;


    Vector2 gearKeyScrollPos;
    public string currentlySelectedKey { get; private set; }
    Dictionary<string, V> dictionaryToEdit;

    bool readOnly = false;
    string HeaderLabel = string.Empty;
    string tooltip = string.Empty;

    SearchField searchField;
    private string searchString = string.Empty;

    public EditorGuiDictionary(Dictionary<string, V> toEdit, string argHeaderLabel = "", string argTooltip = "", bool argReadOnly = false)
    {
        HeaderLabel = argHeaderLabel;
        readOnly = argReadOnly;
        currentlySelectedKey = string.Empty;
        dictionaryToEdit = toEdit;
        searchField = new SearchField();
        tooltip = argTooltip;
    }

    public void DrawGUI()
    {
        int scrollviewWidth = 250;
        //int scrollViewHeight = 300;
        int buttonWidth = scrollviewWidth;
        int buttonHeight = 20;

        EditorGUILayout.BeginVertical();

        if (!string.IsNullOrEmpty(HeaderLabel))
        {
            EditorGUILayout.LabelField(new GUIContent(HeaderLabel, tooltip));
        }

        EditorGUILayout.Space(5f);

        var rect = GUILayoutUtility.GetRect(0, 0, 18, 18, GUILayout.Width(scrollviewWidth));
        searchString = searchField.OnGUI(rect, searchString);
        

        gearKeyScrollPos = EditorGUILayout.BeginScrollView(gearKeyScrollPos, false, true, 
                                                            GUIStyle.none, GUI.skin.verticalScrollbar,
                                                            GUI.skin.scrollView, GUILayout.Width(scrollviewWidth));

        List<string> finalListToDisplay = new List<string>();
        foreach(var data in dictionaryToEdit)
        {
            if (data.Key == currentlySelectedKey
                || data.Key.FuzzyContains(searchString)
                || string.IsNullOrWhiteSpace(searchString))
            {
                finalListToDisplay.Add(data.Key);
            }
        }


        foreach (var Key in finalListToDisplay)
        {
            var baseColor = GUI.color;

            if (Key.Equals(currentlySelectedKey))
            {
                GUI.color = Color.yellow;
            }

            if (GUILayout.Button(Key.ToString(), GUILayout.Width(buttonWidth), GUILayout.Height(buttonHeight)))
            {
                currentlySelectedKey = Key;
                OnKeySelected?.Invoke(currentlySelectedKey);
                GUI.FocusControl("");
            }

            GUI.color = baseColor;
        }
        EditorGUILayout.EndScrollView();
        

        if (!readOnly)
        {
            string newKeyName = EditorGUILayout.TextField(currentlySelectedKey, GUILayout.Width(scrollviewWidth));
            if (newKeyName != currentlySelectedKey)
            {
                if (dictionaryToEdit.ContainsKey(newKeyName))
                {
                    newKeyName += "_" + RandomString(4);
                    GUI.FocusControl("");
                }

                var data = dictionaryToEdit[currentlySelectedKey];
                dictionaryToEdit.Remove(currentlySelectedKey);
                dictionaryToEdit.Add(newKeyName, data);

                OnKeyNameChanged?.Invoke(newKeyName, currentlySelectedKey);

                currentlySelectedKey = newKeyName;
            }

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("+", GUILayout.Width(scrollviewWidth / 2)))
            {
                var newKey = "new_key_" + RandomString(6);
                currentlySelectedKey = newKey;
                dictionaryToEdit.Add(newKey, new V());
                OnKeyAdded?.Invoke(newKey);

                GUI.FocusControl("");

                gearKeyScrollPos.y = dictionaryToEdit.Keys.Count * buttonHeight;

            }

            bool deletePressed = false;
            if (Event.current.type == EventType.KeyDown)
            {
                if (Event.current.keyCode == KeyCode.Delete)
                {
                    deletePressed = true;
                }
            }

            if (GUILayout.Button("-", GUILayout.Width(scrollviewWidth / 2)) || deletePressed)
            {
                if (!string.IsNullOrEmpty(currentlySelectedKey) && dictionaryToEdit.ContainsKey(currentlySelectedKey))
                {
                    dictionaryToEdit.Remove(currentlySelectedKey);
                    OnKeyRemoved?.Invoke(currentlySelectedKey);
                    currentlySelectedKey = string.Empty;

                    GUI.FocusControl("");
                }
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndVertical();
        }
    }


    

    public static string RandomString(int length)
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        return new string(Enumerable.Repeat(chars, length)
          .Select(s => s[UnityEngine.Random.Range(0, s.Length)]).ToArray());
    }
}
#endif