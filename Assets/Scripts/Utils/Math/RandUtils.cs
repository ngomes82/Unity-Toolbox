using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public static class RandUtils 
{
    public static bool NextBool(float trueOdds = 0.5f)
    {
        return Random.value < trueOdds;
   }

    /// <summary>
    /// Based on Math for Game Programmers: Noise-Based RNG 
    /// -----------
    /// The 2017 GDC talk by SMU Guildhall's Squirrel Eiserloh
    /// https://www.youtube.com/watch?v=LWFzPP8ZbdU
    /// </summary>
    public static uint QuickRand(uint position, uint seed = 0)
    {
        const uint PRIME_1 = 0xB5297A4D;
        const uint PRIME_2 = 0x68E31DA4;
        const uint PRIME_3 = 0x1B56C4E9;

        uint output = position + 1;
        output *= PRIME_1;
        output += seed;
        output ^= (output >> 8);
        output *= PRIME_2;
        output ^= (output << 8);
        output *= PRIME_3;
        output ^= (output >> 8);

        return output;
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

    /// <summary>
    /// Picks N items from a list without replacement.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="values"></param>
    /// <param name="count"></param>
    /// <returns></returns>
    public static List<T> RandomWithoutReplacement<T>(List<T> values, int count)
    {
        Debug.Assert(count <= values.Count, $"Can't choose {count} from {values.Count} items without replacement.");

        List<T> toReturn = new List<T>(count);
        RandomIntSequence indicies = new RandomIntSequence(0, values.Count);

        for (int i = 0; i < count; i++)
        {
            toReturn.Add(values[indicies.Next()]);
        }

        return toReturn;
    }
}
