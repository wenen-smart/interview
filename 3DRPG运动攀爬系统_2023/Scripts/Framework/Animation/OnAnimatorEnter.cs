using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnAnimatorEnter : StateMachineBehaviour
{
    //private string stateName;
    public string[] enterMethodName;
    private int frame=0;
    public int delayFrame = 0;
    public bool isTransitionAfter=false;
    public  Action action;
    [Header("允许在层没运行的时候执行")]
    public bool allowLayerNoRunExecute=false;
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        frame = -1;
        if (enterMethodName != null)
        {
            action = () => {
                foreach (var item in enterMethodName)
                {
                    animator.SendMessageUpwards(item, SendMessageOptions.RequireReceiver);
                }
            };

        }
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (allowLayerNoRunExecute == false)
        {
            if (animator.GetLayerWeight(layerIndex)<=0)
            {
                return;
            }
        }

        if (isTransitionAfter)
        {
            if (animator.IsInTransition(layerIndex)==false)
            {
                frame += 1;
            }
        }
        else
        {
            frame += 1;
        }
        if (frame == delayFrame)
        {
            action.Invoke();
            Debug.Log("Enter");
        }
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        frame = 0;
        
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
    void Awake()
    {
        
    }
}
