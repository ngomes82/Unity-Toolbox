using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MathUtils
{
    //Returns N samples of a 1-term mathmatical function over the given range.
    public static List<float> SampleFunc(float start, float end, int nSamples, Func<float, float> func)
    {
        float interval = (end - start) / nSamples;
        List<float> toReturn = new List<float>(nSamples);
        for (int i = 0; i < nSamples; i++)
        {
            toReturn.Add(func(start + (i * interval)));
        }

        return toReturn;
    }
}
