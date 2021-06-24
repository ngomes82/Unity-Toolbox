using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class TimeSlicedCoroutine : IEnumerator
{
    private float maxExecutionTime;
    private IEnumerator coroutine;
    private Stopwatch stopwatch;
    public TimeSlicedCoroutine(float argMaxExecutionTime, IEnumerator argCoroutine)
    {
        maxExecutionTime = argMaxExecutionTime;
        coroutine = argCoroutine;
        stopwatch = new Stopwatch();
    }

    public object Current => null;

    public bool MoveNext()
    {
        bool isRunning = true;
        stopwatch.Start();
        while (isRunning & stopwatch.Elapsed.TotalSeconds < maxExecutionTime)
        {
            isRunning = coroutine.MoveNext();
        }

        stopwatch.Reset();

        return isRunning;
    }

    public void Reset()
    {
        stopwatch.Reset();
    }
}
