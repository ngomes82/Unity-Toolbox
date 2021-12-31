using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class DictionaryExtensions
{
    public static T2 GetSafe<T1,T2>(this Dictionary<T1, T2> dict, T1 key, T2 fallback)
    {
        if (dict.ContainsKey(key))
        {
            return dict[key];
        }
        else
        {
            return fallback;
        }
    }
}
