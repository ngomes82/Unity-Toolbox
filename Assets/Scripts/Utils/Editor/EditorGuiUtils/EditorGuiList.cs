#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;
using System;
using UnityEditor.IMGUI.Controls;

public class EditorGuiList<V>
{
    private const int EDITOR_AVG_PIXELS_PER_CHARACTER = 8;

    public event Action    OnRequestedAddItem;
    public event Action<int, V> OnItemSelected;
    public event Action<V> OnItemRemoved;


    Vector2 scrollListPosition;
    public V currentlySelectedData { get; private set; }
    List<V> listToEdit;
    bool readOnly = false;
    string HeaderLabel = string.Empty;
    string tooltip = string.Empty;

    SearchField searchField;
    private string searchString = string.Empty;
    public int width { get; private set; }

    public EditorGuiList(List<V> toEdit, string argHeaderLabel = "", string argTooltip = "", bool argReadOnly = false)
    {
        HeaderLabel = argHeaderLabel;
        readOnly = argReadOnly;
        currentlySelectedData = default (V);
        listToEdit = toEdit;
        tooltip = argTooltip;
        searchField = new SearchField();

        int longestString = 0;
        foreach(var item in toEdit)
        {
            longestString = Mathf.Max(item.ToString().Length, longestString);
        }

        width = Mathf.Max(250, longestString * EDITOR_AVG_PIXELS_PER_CHARACTER);
    }

    public void DrawGUI()
    {
        int scrollviewWidth = width;
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

        scrollListPosition = EditorGUILayout.BeginScrollView(scrollListPosition, false, true,
                                                            GUIStyle.none, GUI.skin.verticalScrollbar,
                                                            GUI.skin.scrollView, GUILayout.Width(scrollviewWidth));

        List<V> finalListToDisplay = new List<V>();

        for (int i = 0; i < listToEdit.Count; i++)
        {
            var itemStr = listToEdit[i].ToString();

            if (listToEdit[i].Equals(currentlySelectedData)
                || itemStr.FuzzyContains(searchString)
                || string.IsNullOrWhiteSpace(searchString))
            {
                finalListToDisplay.Add(listToEdit[i]);
            }
        }

        for (int i=0; i < finalListToDisplay.Count; i++)
        {
            var baseColor = GUI.color;
            var data = finalListToDisplay[i];

            if (data.Equals(currentlySelectedData))
            {
                GUI.color = Color.yellow;
            }

            if (GUILayout.Button(data.ToString(), GUILayout.Width(buttonWidth), GUILayout.Height(buttonHeight)))
            {
                currentlySelectedData = data;

                OnItemSelected?.Invoke(i, currentlySelectedData);

                GUI.FocusControl("");
            }

            GUI.color = baseColor;
        }
        EditorGUILayout.EndScrollView();
        

        if (!readOnly)
        {
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("+", GUILayout.Width(scrollviewWidth / 2)))
            {
                OnRequestedAddItem?.Invoke();

                GUI.FocusControl("");

                scrollListPosition.y = listToEdit.Count * buttonHeight;

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
                if (currentlySelectedData != null)
                {
                    listToEdit.Remove(currentlySelectedData);
                    OnItemRemoved?.Invoke(currentlySelectedData);
                    currentlySelectedData = default(V);

                    GUI.FocusControl("");
                }
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndVertical();
        }
    }

    public void AddItem(V newItemToAdd)
    {
        listToEdit.Add(newItemToAdd);
    }
}
#endif