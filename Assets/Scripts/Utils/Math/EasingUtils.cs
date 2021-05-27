using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

static public class EasingUtils
{
    public delegate double EasingFunction(double t, double b, double c, double d);


    #region FunctionTable
    public enum Type
    {
        Linear = 0,

        // In
        InQuad,
        InSin,
        InExpo,
        InCirc,
        InCube,
        InQuart,
        InQuint,
        InBack,
        InElastic,
        InBounce,

        // Out
        OutQuad,
        OutSin,
        OutExpo,
        OutCirc,
        OutCube,
        OuQuart,
        OutQuint,
        OutBack,
        OutElastic,
        OutBounce,

        // In/Out
        InOutQuad,
        InOutSin,
        InOutExpo,
        InOutCirc,
        InOutCube,
        InOutQuart,
        InOutQuint,
        InOutBack,
        InOutElastic,
        InOutBounce,

        // Out/In
        OutInQuad,
        OutInSin,
        OutInExpo,
        OutInCirc,
        OutInCube,
        OutInQuart,
        OutInQuint,
        OutInBack,
        OutInElastic,
        OutInBounce
    }

    static private EasingFunction[] _table =
    {
            Linear,

            // In
            InQuad,
            InSin,
            InExpo,
            InCirc,
            InCube,
            InQuart,
            InQuint,
            InBack,
            InElastic,
            InBounce,

            // Out
            OutQuad,
            OutSin,
            OutExpo,
            OutCirc,
            OutCube,
            OutQuart,
            OutQuint,
            OutBack,
            OutElastic,
            OutBounce,

            // In/Out
            InOutQuad,
            InOutSin,
            InOutExpo,
            InOutCirc,
            InOutCube,
            InOutQuart,
            InOutQuint,
            InOutBack,
            InOutElastic,
            InOutBounce,

            // Out/In
            OutInQuad,
            OutInSin,
            OutInExpo,
            OutInCirc,
            OutInCube,
            OutInQuart,
            OutInQuint,
            OutInBack,
            OutInElastic,
            OutInBounce
        };
    #endregion

    public static float Ease(Type function, double t, double b, double c, double d)
    {
        EasingFunction func = Get(function);
        return (float)func(t, b, c, d);
    }

    public static Vector3 Ease(Type function, double t, Vector3 b, Vector3 c, double d)
    {
        EasingFunction func = Get(function);
        return Ease(func, t, b, c, d);
    }

    public static Vector3 Ease(EasingFunction function, double t, Vector3 b, Vector3 c, double d)
    {
        return new Vector3((float)function(t, b.x, c.x, d), (float)function(t, b.y, c.y, d), (float)function(t, b.z, c.z, d));
    }

    static public EasingFunction Get(Type type)
    {
        return _table[(int)type];
    }

    /// <summary>
    /// A simple Easing envelope for tracking parameters.
    /// </summary>
    public class Envelope
    {
        public Type Type = Type.Linear;
        public EasingFunction Function = null;
        public double StartValue = 0.0f;
        public double EndValue = 0.0f;
        public double Duration = 0.0f;

        public Envelope() { }

        public Envelope(Type type, double startValue, double endValue, double duration)
        {
            Set(type, startValue, endValue, duration);
        }

        public void Set(Type type, double startValue, double endValue, double duration)
        {
            Type = type;
            Function = Get(Type);
            StartValue = startValue;
            EndValue = endValue;
            Duration = duration;
        }

        public void SetAbsolute(Type type, double startValue, double endValue, double duration)
        {
            Type = type;
            Function = Get(Type);
            StartValue = startValue;
            EndValue = endValue - StartValue;
            Duration = duration;
        }

        public double Process(double time)
        {
            return Function(time, StartValue, EndValue, Duration);
        }
    }

    /// <summary>
    /// Simple Bezier curve envelope with an easing function for translating time/duration to parametric space for ease.
    /// </summary>
    [Serializable]
    public class BezierEnvelope
    {
        public Vector3 P0;
        public Vector3 P1;
        public Vector3 P2;
        public Vector3 P3;

        EasingUtils.Envelope Easing;

        public BezierEnvelope(EasingUtils.Type type, double duration, Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3)
        {
            Easing = new EasingUtils.Envelope(type, 0.0f, 1.0f, duration);
            P0 = p0;
            P1 = p1;
            P2 = p2;
            P3 = p3;
        }

        public Vector3 Process(double time)
        {
            float t = (float)Easing.Process(time);

            float u = 1.0f - t;
            float tt = t * t;
            float uu = u * u;
            float uuu = uu * u;
            float ttt = tt * t;

            Vector3 p = uuu * P0; //first term
            p += 3 * uu * t * P1; //second term
            p += 3 * u * tt * P2; //third term
            p += ttt * P3; //fourth term

            return p;
        }

    }

    #region Functions
    /// <summary>
    /// Linear function with no easing.
    /// </summary>
    /// <param name="t">Current time (in seconds)</param>
    /// <param name="b">Starting Value</param>
    /// <param name="c">Final Value</param>
    /// <param name="d">Duration (in seconds)</param>
    /// <returns>The value at time t.</returns>
    public static double Linear(double t, double b, double c, double d)
    {
        return c * t / d + b;
    }

    #region In
    /// <summary>
    /// Easing function for a quadratic (t^2) easing in: 
    /// accelerating from zero velocity.
    /// </summary>
    /// <param name="t">Current time (in seconds)</param>
    /// <param name="b">Starting Value</param>
    /// <param name="c">Final Value</param>
    /// <param name="d">Duration (in seconds)</param>
    /// <returns>The value at time t.</returns>
    public static double InQuad(double t, double b, double c, double d)
    {
        return c * (t /= d) * t + b;
    }

    /// <summary>
    /// Easing function for a sinusoidal (sin(t)) easing in: 
    /// accelerating from zero velocity.
    /// </summary>
    /// <param name="t">Current time (in seconds)</param>
    /// <param name="b">Starting Value</param>
    /// <param name="c">Final Value</param>
    /// <param name="d">Duration (in seconds)</param>
    /// <returns>The value at time t.</returns>
    public static double InSin(double t, double b, double c, double d)
    {
        return -c * Math.Cos(t / d * (Math.PI / 2)) + c + b;
    }

    /// <summary>
    /// Easing function for an exponential (2^t) easing in: 
    /// accelerating from zero velocity.
    /// </summary>
    /// <param name="t">Current time (in seconds)</param>
    /// <param name="b">Starting Value</param>
    /// <param name="c">Final Value</param>
    /// <param name="d">Duration (in seconds)</param>
    /// <returns>The value at time t.</returns>
    public static double InExpo(double t, double b, double c, double d)
    {
        return (t == 0) ? b : c * Math.Pow(2, 10 * (t / d - 1)) + b;
    }

    /// <summary>
    /// Easing function for a circular (sqrt(1-t^2)) easing in: 
    /// accelerating from zero velocity.
    /// </summary>
    /// <param name="t">Current time (in seconds)</param>
    /// <param name="b">Starting Value</param>
    /// <param name="c">Final Value</param>
    /// <param name="d">Duration (in seconds)</param>
    /// <returns>The value at time t.</returns>
    public static double InCirc(double t, double b, double c, double d)
    {
        return -c * (Math.Sqrt(1 - (t /= d) * t) - 1) + b;
    }

    /// <summary>
    /// Easing function for a cubic (t^3) easing in: 
    /// accelerating from zero velocity.
    /// </summary>
    /// <param name="t">Current time (in seconds)</param>
    /// <param name="b">Starting Value</param>
    /// <param name="c">Final Value</param>
    /// <param name="d">Duration (in seconds)</param>
    /// <returns>The value at time t.</returns>
    public static double InCube(double t, double b, double c, double d)
    {
        return c * (t /= d) * t * t + b;
    }

    /// <summary>
    /// Easing function for a quartic (t^4) easing in: 
    /// accelerating from zero velocity.
    /// </summary>
    /// <param name="t">Current time (in seconds)</param>
    /// <param name="b">Starting Value</param>
    /// <param name="c">Final Value</param>
    /// <param name="d">Duration (in seconds)</param>
    /// <returns>The value at time t.</returns>
    public static double InQuart(double t, double b, double c, double d)
    {
        return c * (t /= d) * t * t * t + b;
    }

    /// <summary>
    /// Easing function for a quintic (t^5) easing in: 
    /// accelerating from zero velocity.
    /// </summary>
    /// <param name="t">Current time (in seconds)</param>
    /// <param name="b">Starting Value</param>
    /// <param name="c">Final Value</param>
    /// <param name="d">Duration (in seconds)</param>
    /// <returns>The value at time t.</returns>
    public static double InQuint(double t, double b, double c, double d)
    {
        return c * (t /= d) * t * t * t * t + b;
    }

    /// <summary>
    /// Easing function for a back (overshooting cubic easing: (s+1)*t^3 - s*t^2) easing in: 
    /// accelerating from zero velocity.
    /// </summary>
    /// <param name="t">Current time (in seconds)</param>
    /// <param name="b">Starting Value</param>
    /// <param name="c">Final Value</param>
    /// <param name="d">Duration (in seconds)</param>
    /// <returns>The value at time t.</returns>
    public static double InBack(double t, double b, double c, double d)
    {
        return c * (t /= d) * t * ((1.70158 + 1) * t - 1.70158) + b;
    }

    /// <summary>
    /// Easing function for an elastic (exponentially decaying sine wave) easing in: 
    /// accelerating from zero velocity.
    /// </summary>
    /// <param name="t">Current time (in seconds)</param>
    /// <param name="b">Starting Value</param>
    /// <param name="c">Final Value</param>
    /// <param name="d">Duration (in seconds)</param>
    /// <returns>The value at time t.</returns>
    public static double InElastic(double t, double b, double c, double d)
    {
        if ((t /= d) == 1)
            return b + c;

        double p = d * .3;
        double s = p / 4;

        return -(c * Math.Pow(2, 10 * (t -= 1)) * Math.Sin((t * d - s) * (2 * Math.PI) / p)) + b;
    }

    /// <summary>
    /// Easing function for a bounce (exponentially decaying parabolic bounce) easing in: 
    /// accelerating from zero velocity.
    /// </summary>
    /// <param name="t">Current time (in seconds)</param>
    /// <param name="b">Starting Value</param>
    /// <param name="c">Final Value</param>
    /// <param name="d">Duration (in seconds)</param>
    /// <returns>The value at time t.</returns>
    public static double InBounce(double t, double b, double c, double d)
    {
        return c - OutBounce(d - t, 0, c, d) + b;
    }
    #endregion

    #region Out
    /// <summary>
    /// Easing function for a quadratic (t^2) easing out: 
    /// decelerating from zero velocity.
    /// </summary>
    /// <param name="t">Current time (in seconds)</param>
    /// <param name="b">Starting Value</param>
    /// <param name="c">Final Value</param>
    /// <param name="d">Duration (in seconds)</param>
    /// <returns>The value at time t.</returns>
    public static double OutQuad(double t, double b, double c, double d)
    {
        return -c * (t /= d) * (t - 2) + b;
    }

    /// <summary>
    /// Easing function for a sinusoidal (sin(t)) easing out: 
    /// decelerating from zero velocity.
    /// </summary>
    /// <param name="t">Current time (in seconds)</param>
    /// <param name="b">Starting Value</param>
    /// <param name="c">Final Value</param>
    /// <param name="d">Duration (in seconds)</param>
    /// <returns>The value at time t.</returns>
    public static double OutSin(double t, double b, double c, double d)
    {
        return c * Math.Sin(t / d * (Math.PI / 2)) + b;
    }

    /// <summary>
    /// Easing function for an exponential (2^t) easing out: 
    /// decelerating from zero velocity.
    /// </summary>
    /// <param name="t">Current time (in seconds)</param>
    /// <param name="b">Starting Value</param>
    /// <param name="c">Final Value</param>
    /// <param name="d">Duration (in seconds)</param>
    /// <returns>The value at time t.</returns>
    public static double OutExpo(double t, double b, double c, double d)
    {
        return (t == d) ? b + c : c * (-Math.Pow(2, -10 * t / d) + 1) + b;
    }

    /// <summary>
    /// Easing function for a circular (sqrt(1-t^2)) easing out: 
    /// decelerating from zero velocity.
    /// </summary>
    /// <param name="t">Current time (in seconds)</param>
    /// <param name="b">Starting Value</param>
    /// <param name="c">Final Value</param>
    /// <param name="d">Duration (in seconds)</param>
    /// <returns>The value at time t.</returns>
    public static double OutCirc(double t, double b, double c, double d)
    {
        return c * Math.Sqrt(1 - (t = t / d - 1) * t) + b;
    }

    /// <summary>
    /// Easing function for a cubic (t^3) easing out: 
    /// decelerating from zero velocity.
    /// </summary>
    /// <param name="t">Current time (in seconds)</param>
    /// <param name="b">Starting Value</param>
    /// <param name="c">Final Value</param>
    /// <param name="d">Duration (in seconds)</param>
    /// <returns>The value at time t.</returns>
    public static double OutCube(double t, double b, double c, double d)
    {
        return c * ((t = t / d - 1) * t * t + 1) + b;
    }

    /// <summary>
    /// Easing function for a quartic (t^4) easing out: 
    /// decelerating from zero velocity.
    /// </summary>
    /// <param name="t">Current time (in seconds)</param>
    /// <param name="b">Starting Value</param>
    /// <param name="c">Final Value</param>
    /// <param name="d">Duration (in seconds)</param>
    /// <returns>The value at time t.</returns>
    public static double OutQuart(double t, double b, double c, double d)
    {
        return -c * ((t = t / d - 1) * t * t * t - 1) + b;
    }

    /// <summary>
    /// Easing function for a quintic (t^5) easing out: 
    /// decelerating from zero velocity.
    /// </summary>
    /// <param name="t">Current time (in seconds)</param>
    /// <param name="b">Starting Value</param>
    /// <param name="c">Final Value</param>
    /// <param name="d">Duration (in seconds)</param>
    /// <returns>The value at time t.</returns>
    public static double OutQuint(double t, double b, double c, double d)
    {
        return c * ((t = t / d - 1) * t * t * t * t + 1) + b;
    }

    /// <summary>
    /// Easing function for a back (overshooting cubic easing: (s+1)*t^3 - s*t^2) easing out: 
    /// decelerating from zero velocity.
    /// </summary>
    /// <param name="t">Current time (in seconds)</param>
    /// <param name="b">Starting Value</param>
    /// <param name="c">Final Value</param>
    /// <param name="d">Duration (in seconds)</param>
    /// <returns>The value at time t.</returns>
    public static double OutBack(double t, double b, double c, double d)
    {
        return c * ((t = t / d - 1) * t * ((1.70158 + 1) * t + 1.70158) + 1) + b;
    }

    /// <summary>
    /// Easing function for an elastic (exponentially decaying sine wave) easing out: 
    /// decelerating from zero velocity.
    /// </summary>
    /// <param name="t">Current time (in seconds)</param>
    /// <param name="b">Starting Value</param>
    /// <param name="c">Final Value</param>
    /// <param name="d">Duration (in seconds)</param>
    /// <returns>The value at time t.</returns>
    public static double OutElastic(double t, double b, double c, double d)
    {
        if ((t /= d) == 1)
            return b + c;

        double p = d * .3;
        double s = p / 4;

        return (c * Math.Pow(2, -10 * t) * Math.Sin((t * d - s) * (2 * Math.PI) / p) + c + b);
    }

    /// <summary>
    /// Easing function for a bounce (exponentially decaying parabolic bounce) easing out: 
    /// decelerating from zero velocity.
    /// </summary>
    /// <param name="t">Current time (in seconds)</param>
    /// <param name="b">Starting Value</param>
    /// <param name="c">Final Value</param>
    /// <param name="d">Duration (in seconds)</param>
    /// <returns>The value at time t.</returns>
    public static double OutBounce(double t, double b, double c, double d)
    {
        if ((t /= d) < (1 / 2.75))
            return c * (7.5625 * t * t) + b;
        else if (t < (2 / 2.75))
            return c * (7.5625 * (t -= (1.5 / 2.75)) * t + .75) + b;
        else if (t < (2.5 / 2.75))
            return c * (7.5625 * (t -= (2.25 / 2.75)) * t + .9375) + b;
        else
            return c * (7.5625 * (t -= (2.625 / 2.75)) * t + .984375) + b;
    }
    #endregion

    #region InOut
    /// <summary>
    /// Easing function for a quadratic (t^2) easing in/out: 
    /// acceleration until halfway, then deceleration.
    /// </summary>
    /// <param name="t">Current time (in seconds)</param>
    /// <param name="b">Starting Value</param>
    /// <param name="c">Final Value</param>
    /// <param name="d">Duration (in seconds)</param>
    /// <returns>The value at time t.</returns>
    public static double InOutQuad(double t, double b, double c, double d)
    {
        if ((t /= d / 2) < 1)
            return c / 2 * t * t + b;

        return -c / 2 * ((--t) * (t - 2) - 1) + b;
    }

    /// <summary>
    /// Easing function for a sinusoidal (sin(t)) easing in/out: 
    /// acceleration until halfway, then deceleration.
    /// </summary>
    /// <param name="t">Current time (in seconds)</param>
    /// <param name="b">Starting Value</param>
    /// <param name="c">Final Value</param>
    /// <param name="d">Duration (in seconds)</param>
    /// <returns>The value at time t.</returns>
    public static double InOutSin(double t, double b, double c, double d)
    {
        if ((t /= d / 2) < 1)
            return c / 2 * (Math.Sin(Math.PI * t / 2)) + b;

        return -c / 2 * (Math.Cos(Math.PI * --t / 2) - 2) + b;
    }

    /// <summary>
    /// Easing function for an exponential (2^t) easing in/out: 
    /// acceleration until halfway, then deceleration.
    /// </summary>
    /// <param name="t">Current time (in seconds)</param>
    /// <param name="b">Starting Value</param>
    /// <param name="c">Final Value</param>
    /// <param name="d">Duration (in seconds)</param>
    /// <returns>The value at time t.</returns>
    public static double InOutExpo(double t, double b, double c, double d)
    {
        if (t == 0)
            return b;

        if (t == d)
            return b + c;

        if ((t /= d / 2) < 1)
            return c / 2 * Math.Pow(2, 10 * (t - 1)) + b;

        return c / 2 * (-Math.Pow(2, -10 * --t) + 2) + b;
    }

    /// <summary>
    /// Easing function for a circular (sqrt(1-t^2)) easing in/out: 
    /// acceleration until halfway, then deceleration.
    /// </summary>
    /// <param name="t">Current time (in seconds)</param>
    /// <param name="b">Starting Value</param>
    /// <param name="c">Final Value</param>
    /// <param name="d">Duration (in seconds)</param>
    /// <returns>The value at time t.</returns>
    public static double InOutCirc(double t, double b, double c, double d)
    {
        if ((t /= d / 2) < 1)
            return -c / 2 * (Math.Sqrt(1 - t * t) - 1) + b;

        return c / 2 * (Math.Sqrt(1 - (t -= 2) * t) + 1) + b;
    }

    /// <summary>
    /// Easing function for a cubic (t^3) easing in/out: 
    /// acceleration until halfway, then deceleration.
    /// </summary>
    /// <param name="t">Current time (in seconds)</param>
    /// <param name="b">Starting Value</param>
    /// <param name="c">Final Value</param>
    /// <param name="d">Duration (in seconds)</param>
    /// <returns>The value at time t.</returns>
    public static double InOutCube(double t, double b, double c, double d)
    {
        if ((t /= d / 2) < 1)
            return c / 2 * t * t * t + b;

        return c / 2 * ((t -= 2) * t * t + 2) + b;
    }

    /// <summary>
    /// Easing function for a quartic (t^4) easing in/out: 
    /// acceleration until halfway, then deceleration.
    /// </summary>
    /// <param name="t">Current time (in seconds)</param>
    /// <param name="b">Starting Value</param>
    /// <param name="c">Final Value</param>
    /// <param name="d">Duration (in seconds)</param>
    /// <returns>The value at time t.</returns>
    public static double InOutQuart(double t, double b, double c, double d)
    {
        if ((t /= d / 2) < 1)
            return c / 2 * t * t * t * t + b;

        return -c / 2 * ((t -= 2) * t * t * t - 2) + b;
    }

    /// <summary>
    /// Easing function for a quintic (t^5) easing in/out: 
    /// acceleration until halfway, then deceleration.
    /// </summary>
    /// <param name="t">Current time (in seconds)</param>
    /// <param name="b">Starting Value</param>
    /// <param name="c">Final Value</param>
    /// <param name="d">Duration (in seconds)</param>
    /// <returns>The value at time t.</returns>
    public static double InOutQuint(double t, double b, double c, double d)
    {
        if ((t /= d / 2) < 1)
            return c / 2 * t * t * t * t * t + b;
        return c / 2 * ((t -= 2) * t * t * t * t + 2) + b;
    }

    /// <summary>
    /// Easing function for a back (overshooting cubic easing: (s+1)*t^3 - s*t^2) easing in/out: 
    /// acceleration until halfway, then deceleration.
    /// </summary>
    /// <param name="t">Current time (in seconds)</param>
    /// <param name="b">Starting Value</param>
    /// <param name="c">Final Value</param>
    /// <param name="d">Duration (in seconds)</param>
    /// <returns>The value at time t.</returns>
    public static double InOutBack(double t, double b, double c, double d)
    {
        double s = 1.70158;
        if ((t /= d / 2) < 1)
            return c / 2 * (t * t * (((s *= (1.525)) + 1) * t - s)) + b;
        return c / 2 * ((t -= 2) * t * (((s *= (1.525)) + 1) * t + s) + 2) + b;
    }

    /// <summary>
    /// Easing function for an elastic (exponentially decaying sine wave) easing in/out: 
    /// acceleration until halfway, then deceleration.
    /// </summary>
    /// <param name="t">Current time (in seconds)</param>
    /// <param name="b">Starting Value</param>
    /// <param name="c">Final Value</param>
    /// <param name="d">Duration (in seconds)</param>
    /// <returns>The value at time t.</returns>
    public static double InOutElastic(double t, double b, double c, double d)
    {
        if ((t /= d / 2) == 2)
            return b + c;

        double p = d * (.3 * 1.5);
        double s = p / 4;

        if (t < 1)
            return -.5 * (c * Math.Pow(2, 10 * (t -= 1)) * Math.Sin((t * d - s) * (2 * Math.PI) / p)) + b;
        return c * Math.Pow(2, -10 * (t -= 1)) * Math.Sin((t * d - s) * (2 * Math.PI) / p) * .5 + c + b;
    }

    /// <summary>
    /// Easing function for a bounce (exponentially decaying parabolic bounce) easing in/out: 
    /// acceleration until halfway, then deceleration.
    /// </summary>
    /// <param name="t">Current time (in seconds)</param>
    /// <param name="b">Starting Value</param>
    /// <param name="c">Final Value</param>
    /// <param name="d">Duration (in seconds)</param>
    /// <returns>The value at time t.</returns>
    public static double InOutBounce(double t, double b, double c, double d)
    {
        if (t < d / 2)
            return InBounce(t * 2, 0, c, d) * .5 + b;
        else
            return OutBounce(t * 2 - d, 0, c, d) * .5 + c * .5 + b;
    }
    #endregion

    #region OutIn
    /// <summary>
    /// Easing function for a quadratic (t^2) easing out/in: 
    /// deceleration until halfway, then acceleration.
    /// </summary>
    /// <param name="t">Current time (in seconds)</param>
    /// <param name="b">Starting Value</param>
    /// <param name="c">Final Value</param>
    /// <param name="d">Duration (in seconds)</param>
    /// <returns>The value at time t.</returns>
    public static double OutInQuad(double t, double b, double c, double d)
    {
        if (t < d / 2)
            return OutQuad(t * 2, b, c / 2, d);

        return InQuad((t * 2) - d, b + c / 2, c / 2, d);
    }

    /// <summary>
    /// Easing function for a sinusoidal (sin(t)) easing in/out: 
    /// deceleration until halfway, then acceleration.
    /// </summary>
    /// <param name="t">Current time (in seconds)</param>
    /// <param name="b">Starting Value</param>
    /// <param name="c">Final Value</param>
    /// <param name="d">Duration (in seconds)</param>
    /// <returns>The value at time t.</returns>
    public static double OutInSin(double t, double b, double c, double d)
    {
        if (t < d / 2)
            return OutSin(t * 2, b, c / 2, d);

        return InSin((t * 2) - d, b + c / 2, c / 2, d);
    }

    /// <summary>
    /// Easing function for an exponential (2^t) easing out/in: 
    /// deceleration until halfway, then acceleration.
    /// </summary>
    /// <param name="t">Current time (in seconds)</param>
    /// <param name="b">Starting Value</param>
    /// <param name="c">Final Value</param>
    /// <param name="d">Duration (in seconds)</param>
    /// <returns>The value at time t.</returns>
    public static double OutInExpo(double t, double b, double c, double d)
    {
        if (t < d / 2)
            return OutExpo(t * 2, b, c / 2, d);

        return InExpo((t * 2) - d, b + c / 2, c / 2, d);
    }

    /// <summary>
    /// Easing function for a circular (sqrt(1-t^2)) easing out/in: 
    /// acceleration until halfway, then deceleration.
    /// </summary>
    /// <param name="t">Current time (in seconds)</param>
    /// <param name="b">Starting Value</param>
    /// <param name="c">Final Value</param>
    /// <param name="d">Duration (in seconds)</param>
    /// <returns>The value at time t.</returns>
    public static double OutInCirc(double t, double b, double c, double d)
    {
        if (t < d / 2)
            return OutCirc(t * 2, b, c / 2, d);

        return InCirc((t * 2) - d, b + c / 2, c / 2, d);
    }

    /// <summary>
    /// Easing function for a cubic (t^3) easing out/in: 
    /// deceleration until halfway, then acceleration.
    /// </summary>
    /// <param name="t">Current time (in seconds)</param>
    /// <param name="b">Starting Value</param>
    /// <param name="c">Final Value</param>
    /// <param name="d">Duration (in seconds)</param>
    /// <returns>The value at time t.</returns>
    /// <returns>The correct value.</returns>
    public static double OutInCube(double t, double b, double c, double d)
    {
        if (t < d / 2)
            return OutCube(t * 2, b, c / 2, d);

        return InCube((t * 2) - d, b + c / 2, c / 2, d);
    }
    /// <summary>
    /// Easing function for a quartic (t^4) easing out/in: 
    /// deceleration until halfway, then acceleration.
    /// </summary>
    /// <param name="t">Current time (in seconds)</param>
    /// <param name="b">Starting Value</param>
    /// <param name="c">Final Value</param>
    /// <param name="d">Duration (in seconds)</param>
    /// <returns>The value at time t.</returns>
    public static double OutInQuart(double t, double b, double c, double d)
    {
        if (t < d / 2)
            return OutQuart(t * 2, b, c / 2, d);

        return InQuart((t * 2) - d, b + c / 2, c / 2, d);
    }

    /// <summary>
    /// Easing function for a quintic (t^5) easing in/out: 
    /// acceleration until halfway, then deceleration.
    /// </summary>
    /// <param name="t">Current time (in seconds)</param>
    /// <param name="b">Starting Value</param>
    /// <param name="c">Final Value</param>
    /// <param name="d">Duration (in seconds)</param>
    /// <returns>The value at time t.</returns>
    public static double OutInQuint(double t, double b, double c, double d)
    {
        if (t < d / 2)
            return OutQuint(t * 2, b, c / 2, d);
        return InQuint((t * 2) - d, b + c / 2, c / 2, d);
    }
    /// <summary>
    /// Easing function for a back (overshooting cubic easing: (s+1)*t^3 - s*t^2) easing out/in: 
    /// deceleration until halfway, then acceleration.
    /// </summary>
    /// <param name="t">Current time (in seconds)</param>
    /// <param name="b">Starting Value</param>
    /// <param name="c">Final Value</param>
    /// <param name="d">Duration (in seconds)</param>
    /// <returns>The value at time t.</returns>
    public static double OutInBack(double t, double b, double c, double d)
    {
        if (t < d / 2)
            return OutBack(t * 2, b, c / 2, d);
        return InBack((t * 2) - d, b + c / 2, c / 2, d);
    }

    /// <summary>
    /// Easing function for an elastic (exponentially decaying sine wave) easing out/in: 
    /// deceleration until halfway, then acceleration.
    /// </summary>
    /// <param name="t">Current time (in seconds)</param>
    /// <param name="b">Starting Value</param>
    /// <param name="c">Final Value</param>
    /// <param name="d">Duration (in seconds)</param>
    /// <returns>The value at time t.</returns>
    public static double OutInElastic(double t, double b, double c, double d)
    {
        if (t < d / 2)
            return OutElastic(t * 2, b, c / 2, d);
        return InElastic((t * 2) - d, b + c / 2, c / 2, d);
    }

    /// <summary>
    /// Easing function for a bounce (exponentially decaying parabolic bounce) easing out/in: 
    /// deceleration until halfway, then acceleration.
    /// </summary>
    /// <param name="t">Current time (in seconds)</param>
    /// <param name="b">Starting Value</param>
    /// <param name="c">Final Value</param>
    /// <param name="d">Duration (in seconds)</param>
    /// <returns>The value at time t.</returns>
    public static double OutInBounce(double t, double b, double c, double d)
    {
        if (t < d / 2)
            return OutBounce(t * 2, b, c / 2, d);
        return InBounce((t * 2) - d, b + c / 2, c / 2, d);
    }
    #endregion
    #endregion
}