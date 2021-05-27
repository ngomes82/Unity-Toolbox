using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MathUtils
{
    /// <summary>
    /// Returns N samples of a 1-term mathmatical function over the given range.
    /// </summary>
    /// <param name="start"></param>
    /// <param name="end"></param>
    /// <param name="nSamples"></param>
    /// <param name="func"></param>
    /// <returns></returns>
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

    /// <summary>
    /// Converts a linear index to a 2D index, left to right
    /// </summary>
    /// <param name="index1D"></param>
    /// <param name="width"></param>
    /// <param name="xIndex"></param>
    /// <param name="yIndex"></param>
    public static void Convert1DTo2DArrayIdx(int index1D, int width, out int xIndex, out int yIndex)
    {
        xIndex = index1D % width;
        yIndex = index1D / width;
    }

    /// <summary>
    /// Converts a 2D index into a linear index, left to right
    /// </summary>
    /// <param name="xIndex"></param>
    /// <param name="yIndex"></param>
    /// <param name="width"></param>
    /// <returns></returns>
    public static int Convert2DTo1DArrayIdx(int xIndex, int yIndex, int width)
    {
        return xIndex + (yIndex * width);
    }
}
