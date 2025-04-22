using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnAnimatorUpdate : StateMachineBehaviour
{
    public AnimatorUpdateArgSaver[] updateMethodName;
    public float startNormalizeTime = 0;
    public float endNormalizeTime = 1;
    public bool isPlayer = true;
    public bool isOnlyOneFrame = false;
    private bool isExcute = false;
    [Header("允许在层没运行的时候执行")]
    public bool allowLayerNoRunExecute = true;
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    //override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}

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
        if (!isExcute&&isOnlyOneFrame )
        {
            foreach (var item in updateMethodName)
            {
                item.SetAnimatorInfo(animator, stateInfo, layerIndex);
                //if (isPlayer)
                //{
                //    PlayerFacade.Instance.SendMessage("AnimatorMonoUpdate", item, SendMessageOptions.DontRequireReceiver);
                //}
                //else
                //{
                //    animator.SendMessageUpwards("AnimatorMonoUpdate", item, SendMessageOptions.DontRequireReceiver);
                //}
                animator.SendMessageUpwards("AnimatorMonoUpdate", item, SendMessageOptions.DontRequireReceiver);
            }
            isExcute = true;
            return;
        }

        if (isOnlyOneFrame)
        {
            return;
        }
        float normalizedTime = stateInfo.normalizedTime;
       
        normalizedTime = Mathf.Repeat(normalizedTime,1);
        if (normalizedTime >= startNormalizeTime && normalizedTime <= endNormalizeTime)
        {
            if (updateMethodName != null)
            {
                isExcute = true;
                foreach (var item in updateMethodName)
                {
                    item.SetAnimatorInfo(animator, stateInfo, layerIndex);
                    if (isPlayer)
                    {
                        PlayerFacade.Instance.SendMessage("AnimatorMonoUpdate", item, SendMessageOptions.DontRequireReceiver);
                    }
                    else
                    {
                        animator.SendMessageUpwards("AnimatorMonoUpdate", item, SendMessageOptions.DontRequireReceiver);
                    }
                }
            }
        }
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        isExcute = false;
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
