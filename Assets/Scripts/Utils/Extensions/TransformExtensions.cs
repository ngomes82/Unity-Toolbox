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

    public static Transform SetEulerAngles(this Transform transform, float? x = null, float? y = null, float? z = null)
    {
        Vector3 newRot = transform.eulerAngles;

        if (x.HasValue) newRot.x = x.Value;
        if (y.HasValue) newRot.y = y.Value;
        if (z.HasValue) newRot.z = z.Value;

        transform.eulerAngles = newRot;

        return transform;
    }

    public static Transform SetLocalPosition(this Transform transform, float? x = null, float? y = null, float? z = null)
    {
        Vector3 newPos = transform.localPosition;

        if (x.HasValue) newPos.x = x.Value;
        if (y.HasValue) newPos.y = y.Value;
        if (z.HasValue) newPos.z = z.Value;

        transform.localPosition = newPos;

        return transform;
    }
    public static Transform SetLocalScale(this Transform transform, float? x = null, float? y = null, float? z = null)
    {
        Vector3 newscale = transform.localScale;

        if (x.HasValue) newscale.x = x.Value;
        if (y.HasValue) newscale.y = y.Value;
        if (z.HasValue) newscale.z = z.Value;

        transform.localScale = newscale;

        return transform;
    }

    public static Transform SetLocalEulerAngles(this Transform transform, float? x = null, float? y = null, float? z = null)
    {
        Vector3 newRot = transform.localEulerAngles;

        if (x.HasValue) newRot.x = x.Value;
        if (y.HasValue) newRot.y = y.Value;
        if (z.HasValue) newRot.z = z.Value;

        transform.localEulerAngles = newRot;

        return transform;
    }
}
