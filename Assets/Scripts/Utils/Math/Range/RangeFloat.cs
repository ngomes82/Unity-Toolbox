using System;
using UnityEngine;
using Random = UnityUtils.Random;

[Serializable]
public class RangeFloat
{

    [SerializeField] float min;
    [SerializeField] float max;


    public float Min { get { return min; } }
    public float Max { get { return max; } }

    public float RandomInclusive { get { return Random.Instance.Range(min, max); } }

    public RangeFloat(){ }


    public RangeFloat(float argMin, float argMax)
    {
        this.min = argMin;
        this.max = argMax;
    }

    public float Lerp(float t)
    {
        return Mathf.Lerp(min, max, t);
    }

    public float LerpUnclamped(float t)
    {
        return Mathf.LerpUnclamped(min, max, t);
    }

    public float InverseLerp(float value)
    {
        return Mathf.InverseLerp(min, max, value);
    }
}