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

        var allStateChangeCallbacks = animator.GetBehaviours<AnimatorStateChangeCallback>();
        
        foreach (var stateCallbackScript in allStateChangeCallbacks)
        {
            if (stateCallbackScript.name == stateName)
            {
                stateCallbackScript.callbacks[callbackIdx] = callback;
                registered = true;
            }
        }

        return registered;
    }
}