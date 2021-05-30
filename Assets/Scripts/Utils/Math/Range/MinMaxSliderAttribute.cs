using UnityEngine;

public class MinMaxSliderAttribute : PropertyAttribute
{
    public float lowerLimit;
    public float upperLimit;

    public MinMaxSliderAttribute(float lowerLimit, float upperLimit)
    {
        this.lowerLimit = lowerLimit;
        this.upperLimit = upperLimit;
    }
}
