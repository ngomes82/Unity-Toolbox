using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[Serializable]
public class RangeInt
{

    [SerializeField] int min;
    [SerializeField] int max;

    public int Min { get { return min; } }
    public int Max { get { return max; } }


    public int RandomInclusive { get { return UnityEngine.Random.Range(min, max + 1); } }

    public int RandomExclusive { get { return UnityEngine.Random.Range(min, max); } }

    public RangeInt(){}

    public RangeInt(int min, int max)
    {
        this.min = min;
        this.max = max;
    }
}