using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static AnimatorStateChangeCallback;

public static class AnimatorExtensions
{
    public static bool TryRegisterStateChangeCallback(this Animator animator, string stateName, StateChangeType stateChangeType, Action callback)
    {
        bool registered = false;

        int callbackIdx = (int)stateChangeType;

        var stateCallbacks = animator.GetBehaviours<AnimatorStateChangeCallback>();
        if (stateCallbacks.Length > 0)
        {
            foreach (var state in stateCallbacks)
            {
                if (state.name == stateName)
                {
                    state.callbacks[callbackIdx] = callback;
                    registered = true;
                }
            }
        }

        return registered;
    }
}