using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClearIntAction : StateMachineBehaviour
{
    public ClearAnimAnimArgs[] actionStrs;
    private int frame;
    private List<ClearAnimAnimArgs> enterClearActQueue=new List<ClearAnimAnimArgs>();
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        frame = 0;
        enterClearActQueue.Clear();
        if (actionStrs != null)
        {
            foreach (var sign in actionStrs)
            {
                if (sign.isEnter)
                {
                    enterClearActQueue.Add(sign);
                    //animator.SetInteger(sign.actionStr, 0);
                }
            }
        }
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        for (int i = 0; i < enterClearActQueue.Count; i++)
        {
            if (enterClearActQueue[i].delay<= frame)
            {
                animator.SetInteger(enterClearActQueue[i].actionStr, 0);
                //Debug.Log("clear "+ enterClearActQueue[i].actionStr);
                enterClearActQueue.Remove(enterClearActQueue[i]);
                i--;
            }
        }
        frame++;
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (actionStrs != null)
        {
            foreach (var sign in actionStrs)
            {
                if (sign.isEnter==false)
                {
                    animator.SetInteger(sign.actionStr, 0);
                }
            }
        }
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
}
[Serializable]
public class ClearAnimAnimArgs
{
    public string actionStr;
    public bool isEnter;
    public int delay = 1;
}
