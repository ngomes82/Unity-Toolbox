using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class StringExtensions
{
    public static bool Contains(this string source, string value, StringComparison comparisonType)
    {
        return source.IndexOf(value, comparisonType) >= 0;
    }

    public static bool FuzzyContains(this string source, string value, int distance = 1)
    {
        return StringUtils.LevenshteinSubstringDistance(value, source) <= distance;
    }

    public static bool ContainsAll(this string source, string value, char separator, bool fuzzy = false, int distance = 1, bool fuzzyNumbers = false)
    {
        string[] searchStrings = value.Split(separator);
        bool[] containsNumbers = new bool[searchStrings.Length];

        if (fuzzy && !fuzzyNumbers)
        {
            for (int string_index = 0; string_index < searchStrings.Length; ++string_index)
                containsNumbers[string_index] = searchStrings[string_index].Any(c => char.IsDigit(c));
        }

        bool allStringsFound = true;
        for (int string_index = 0; string_index < searchStrings.Length; ++string_index)
        {
            string searchFor = searchStrings[string_index];

            if (!source.Contains(searchFor, StringComparison.OrdinalIgnoreCase))
            {
                if (containsNumbers[string_index] || (fuzzy && !source.FuzzyContains(searchFor, distance)))
                {
                    allStringsFound = false;
                    break;
                }
            }
        }

        return allStringsFound;
    }
}
