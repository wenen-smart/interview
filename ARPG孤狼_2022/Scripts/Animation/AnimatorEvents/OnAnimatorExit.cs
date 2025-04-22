using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnAnimatorExit : StateMachineBehaviour
{

    public string[] exitMethodName;
    public  Action action;
    public string stateName;
    [Header("允许在层没运行的时候执行")]
    public bool allowLayerNoRunExecute = true;
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (exitMethodName != null)
        {
            action = () =>
            {
                foreach (var item in exitMethodName)
                {
                    animator.SendMessageUpwards(item, SendMessageOptions.DontRequireReceiver);
                }
            };
        }
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (allowLayerNoRunExecute == false)
        {
            if (animator.GetLayerWeight(layerIndex) <= 0)
            {
                return;
            }
        }
        if (action!=null)
        {
            if (animator.GetNextAnimatorStateInfo(0).IsName(stateName)==false &&animator.GetAnimatorTransitionInfo(0).normalizedTime>=0.02f)
            {
                    action?.Invoke();
                    action = null;
                
            }
        }
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {

        if (action!=null)
        {
            action?.Invoke();
            action = null;
        }
    }

    // OnStateMove is called right after Animator.OnAnimatorMove()
    //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that processes and affects root motion
    //}

    // OnStateIK is called right after Animator.OnAnimatorIK()
    //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that sets up animation IK (inverse kinematics)
    //}
}
