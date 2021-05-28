using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ListExtensions
{
    //Fisher-Yates Shuffle
    public static void Shuffle<T>(this List<T> toShuffle)
    {
        int n = toShuffle.Count;
        for (int i = 0; i < (n - 1); i++)
        {
            int r = i + Random.Range(0, n - i);
            T t = toShuffle[r];
            toShuffle[r] = toShuffle[i];
            toShuffle[i] = t;
        }
    }

    public static T RandomElement<T>(this List<T> list)
    {
        int index = Random.Range(0, list.Count - 1);
        return list[index];
    }
}
