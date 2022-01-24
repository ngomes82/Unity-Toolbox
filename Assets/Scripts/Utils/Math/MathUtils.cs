using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MathUtils
{
    public static readonly float E = (float)System.Math.E;

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
    /// Converts from cartesian to 45-isometric coordinate space
    /// </summary>
    /// <param name="input"></param>
    /// <param name="output"></param>
    public static void CartToIso(Vector2 input, out Vector2 output)
    {
        output.x = input.x - input.y;
        output.y = (input.x + input.y) * 0.5f;
    }

    /// <summary>
    /// Converts from 45-isometric to cartesian coordinate space
    /// </summary>
    /// <param name="input"></param>
    /// <param name="output"></param>
    public static void IsoToCart(Vector2 input, out Vector2 output)
    {
        output.x = (2f * input.y + input.x) * 0.5f;
        output.y = (2f * input.y - input.x) * 0.5f;
    }

    /// <summary>
    /// Converts from cartesian to polar coordinate space. Angle is returned in radians.
    /// </summary>
    /// <param name="input"></param>
    /// <param name="radius"></param>
    /// <param name="angle"> In Radians</param>
    public static void CartToPolar(Vector2 input, out float radius, out float angle)
    {
        radius = input.magnitude;
        angle  = Mathf.Atan2(input.y, input.x);
    }

    /// <summary>
    /// Converts from cartesian to polar coordinate space. Angle is in radians.
    /// </summary>
    /// <param name="radius"></param>
    /// <param name="angle"></param>
    /// <param name="output"></param>
    public static void PolarToCart(float radius, float angle, out Vector2 output)
    {
        output.x = radius * Mathf.Cos(angle);
        output.y = radius * Mathf.Sin(angle);
    }

    public static Vector2 CalculateCenter(params Vector2[] points)
    {
        Vector2 sum = Vector2.zero;
        for(int i=0; i < points.Length; i++)
        {
            sum += points[i];
        }

        return sum / points.Length;
    }

    public static Vector3 CalculateCenter(params Vector3[] points)
    {
        Vector3 sum = Vector3.zero;
        for (int i = 0; i < points.Length; i++)
        {
            sum += points[i];
        }

        return sum / points.Length;
    }

    /// <summary>
    /// Calculates the pdf at X for for a normal distribution with a given mean and standard deviation
    /// </summary>
    /// <param name="x"></param>
    /// <param name="mean"></param>
    /// <param name="std"></param>
    /// <returns></returns>
    public static float NormalDist(float x, float mean, float std)
    {
        return (1f / (std * Mathf.Sqrt(2f * Mathf.PI))) * Mathf.Pow(E, (-0.5f * Mathf.Pow(((x - mean) / std), 2f)));
    }

    /// <summary>
    /// Calculates the pdf at X for a normal distribution and normalizes it within 0-1 range (using mean normalization)
    /// </summary>
    /// <param name="x"></param>
    /// <param name="mean"></param>
    /// <param name="std"></param>
    /// <returns></returns>
    public static float NormalDistNormalized(float x, float mean, float std)
    {
        float meanVal = MathUtils.NormalDist(mean, mean, std);
        float value = MathUtils.NormalDist(x, mean, std);
        float normalizedVal = (value / meanVal) * 0.5f;

        if (x > mean)
        {
            normalizedVal = 1f - normalizedVal;
        }

        return normalizedVal;
    }
}
