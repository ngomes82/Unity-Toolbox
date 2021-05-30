using UnityEngine;

public class BoundsAttribute : PropertyAttribute
{
    public float lowerLimit;
    public float upperLimit;

    public BoundsAttribute(float lowerLimit, float upperLimit)
    {
        this.lowerLimit = lowerLimit;
        this.upperLimit = upperLimit;
    }
}
