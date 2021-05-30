using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class TransformExtensions
{
    public static Transform SetPosition(this Transform transform, float? x = null, float? y = null, float? z = null)
    {
        Vector3 newPos = transform.position;

        if (x.HasValue) newPos.x = x.Value;
        if (y.HasValue) newPos.y = y.Value;
        if (z.HasValue) newPos.z = z.Value;
        
        transform.position = newPos;

        return transform;
    }
}
