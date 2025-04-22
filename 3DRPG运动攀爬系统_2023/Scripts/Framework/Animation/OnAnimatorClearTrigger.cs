using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnAnimatorClearTrigger : StateMachineBehaviour
{

    public string[] enterClear;
    public string[] exitClear;
    private int frame;
    public int delayFrame = 0;
    private List<string> enterClearActQueue = new List<string>();
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (enterClear!=null)
        {
            foreach (var sign in enterClear)
            {
                enterClearActQueue.Add(sign);
            }
        }
        frame = 0;
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        for (int i = 0; i < enterClearActQueue.Count; i++)
        {
            if (delayFrame <= frame)
            {
                animator.ResetTrigger(enterClearActQueue[i]);
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
        if (exitClear != null)
        {
            foreach (var sign in exitClear)
            {
                animator.ResetTrigger(sign);
            }
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
