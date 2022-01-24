#if UNITY_EDITOR

using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

public class EditorItemSelectPopup<V> : PopupWindowContent
{
    private EditorGuiList<V> list;
    private Action<int, V> ItemSelectedCallback;

    public EditorItemSelectPopup(List<V> source, Action<int, V> callback)
    {
        list = new EditorGuiList<V>(source, string.Empty, string.Empty, true);
        list.OnItemSelected += List_OnItemSelected;
        ItemSelectedCallback = callback;
    }

    private void List_OnItemSelected(int idx, V obj)
    {
        ItemSelectedCallback(idx, obj);
        editorWindow.Close();
    }

    public override void OnGUI(Rect rect)
    {
        list.DrawGUI();
    }

    public override Vector2 GetWindowSize()
    {
        return new Vector2(list.width, 500);
    }

    public static void Open(List<V> dict, Action<int, V> callback)
    {
        var mousePos = Event.current.mousePosition;
        var rect = new Rect(mousePos, Vector2.zero);
        var popupReference = new EditorItemSelectPopup<V>(dict, callback);
        PopupWindow.Show(rect, popupReference);
    }
}
#endif