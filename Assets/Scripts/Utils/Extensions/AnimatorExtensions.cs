using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static AnimatorStateChangeCallback;

public static class AnimatorExtensions
{
    public static bool TryRegisterStateChangeCallback(this Animator animator, string stateName, StateChangeType stateChangeType, Action callback)
    {
        bool hasRegistered          = false;
        int callbackIdx             = (int)stateChangeType;
        var allStateChangeCallbacks = animator.GetBehaviours<AnimatorStateChangeCallback>();
        
        foreach (var stateCallbackScript in allStateChangeCallbacks)
        {
            if (stateCallbackScript.name == stateName)
            {
                stateCallbackScript.callbacks[callbackIdx] = callback;
                hasRegistered = true;
            }
        }

        if(!hasRegistered)
        {
            Debug.LogError($"Failed to register {stateChangeType} callback for {stateName} on {animator.gameObject.name}. Make sure to add an AnimatorStateChangeCallback script to the relevant animation state!", animator.gameObject);
        }

        return hasRegistered;
    }
}