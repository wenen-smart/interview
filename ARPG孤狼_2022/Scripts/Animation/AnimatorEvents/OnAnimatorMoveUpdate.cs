using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnAnimatorMoveUpdate : StateMachineBehaviour
{
    public AnimatorMoveTickType moveTickType;
    private RoleController controller;
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        controller = animator.GetComponentInParent<RoleController>();
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Vector3 velocity = animator.transform.TransformDirection(new Vector3(animator.GetFloat("CurveX"), animator.GetFloat("CurveY"), animator.GetFloat("CurveZ")));
        if (controller==null)
        {
            controller=animator.GetComponentInParent<RoleController>();
        }
        switch (moveTickType)
        {
            case AnimatorMoveTickType.IncreaseVelo:
                controller.IncreaseVelo +=velocity;
                break;
            case AnimatorMoveTickType.DeltaPosition:
                controller.IncreaseVelo +=velocity;
                break;
            default:
                break;
        }
        
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    //override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{

    //}

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
public enum AnimatorMoveTickType
{
    IncreaseVelo=0,
    DeltaPosition=1
}