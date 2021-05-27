using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatorStateChangeCallback : StateMachineBehaviour
{
    public enum StateChangeType
    {
        OnEnter = 0,
        OnExit = 1
    }

    public Action[] callbacks = { null, null };

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        callbacks[0]?.Invoke();
        base.OnStateEnter(animator, stateInfo, layerIndex);
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        callbacks[1]?.Invoke();
        base.OnStateExit(animator, stateInfo, layerIndex);
    }
}
