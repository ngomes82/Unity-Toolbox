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

    /// <summary>
    /// Picks a random item from a list given a second list with weights and the sum of the weights.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="items"></param>
    /// <param name="weights"></param>
    /// <param name="sum"></param>
    /// <returns></returns>
    public static T ChooseWithWeights<T>(List<T> items, List<float> weights, float sum)
    {
        Debug.Assert(items.Count == weights.Count, $"Items and weights are not the same sizes: {items.Count} != {weights.Count} ");

        int index = ChooseWeightedArrayIndex(weights, sum);
        return items[index];
    }

    /// <summary>
    /// Picks a random index given a list of weights and the sum of weights.
    /// </summary>
    /// <param name="weights"></param>
    /// <param name="sum"></param>
    /// <returns></returns>
    public static int ChooseWeightedArrayIndex(List<float> weights, float sum)
    {
        float randWeight = UnityEngine.Random.Range(0f, sum);
        for (int i = 0; i < weights.Count; i++)
        {
            randWeight -= weights[i];
            if (randWeight < 0f)
            {
                return i;
            }
        }

        return weights.Count - 1;
    }
}
