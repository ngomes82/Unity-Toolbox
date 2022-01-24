#if UNITY_EDITOR

using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

public class KeySelectPopup<V> : PopupWindowContent where V : new()
{
    private EditorGuiDictionary<V> dict;
    private Action<string> KeySelectedCallback;

    public KeySelectPopup(Dictionary<string,V> source, Action<string> callback)
    {
        dict = new EditorGuiDictionary<V>(source, string.Empty, string.Empty, true);
        dict.OnKeySelected += Dict_OnKeySelected;
        KeySelectedCallback = callback;
    }

    private void Dict_OnKeySelected(string obj)
    {
        KeySelectedCallback(obj);
        editorWindow.Close();
    }

    public override void OnGUI(Rect rect)
    {
        dict.DrawGUI();
    }

    public override Vector2 GetWindowSize()
    {
        return new Vector2(250, 500);
    }

    public static void Open(Dictionary<string, V> dict, Action<string> callback)
    {
        var mousePos = Event.current.mousePosition;
        var rect = new Rect(mousePos, Vector2.zero);
        var popupReference = new KeySelectPopup<V>(dict, callback);
        PopupWindow.Show(rect, popupReference);
    }
}
#endif