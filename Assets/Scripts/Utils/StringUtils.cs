using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;

public static class StringUtils
{
    static public int LevenshteinSubstringDistance(string s0, string s1, int minFuzzyLength = 3)
    {
        var subString = s0.ToLower();
        var searchIn = s1.ToLower();

        if (subString.Length <= minFuzzyLength)
            return searchIn.Contains(subString, StringComparison.OrdinalIgnoreCase) ? 0 : int.MaxValue;
        if (searchIn.Length == 0)
            return int.MaxValue;

        int maxLength = Mathf.Max(subString.Length, searchIn.Length) + 1;
        List<int> row0 = new List<int>(maxLength);

        for (int i = 0; i < maxLength; ++i)
            row0.Add(0);

        for (int i = 0; i < subString.Length; ++i)
        {
            List<int> row1 = new List<int>();
            row1.Add(i + 1);

            for (int j = 0; j < searchIn.Length; ++j)
            {
                int cost = (subString[i] != searchIn[j]) ? 1 : 0;
                row1.Add(Mathf.Min(row0[j + 1] + 1, row1[j] + 1, row0[j] + cost));
            }

            row0.Clear();
            row0.AddRange(row1);
        }

        return Mathf.Min(row0.ToArray());
    }

    public static string RandomString(int length)
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        return new string(Enumerable.Repeat(chars, length)
          .Select(s => s[UnityEngine.Random.Range(0, s.Length)]).ToArray());
    }
}
