using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnAnimatorExit : StateMachineBehaviour
{
        //public string stateName;
    public string[] exitMethodName;
    public  Action action;
    private float shortHash;
    public bool isTransitionAfter = true;
    [Header("允许在层没运行的时候执行")]
    public bool allowLayerNoRunExecute = false;
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (exitMethodName != null)
        {
            action = () =>
            {
                foreach (var item in exitMethodName)
                {
                    animator.SendMessageUpwards(item, SendMessageOptions.RequireReceiver);
                }
            };
        }
        shortHash = stateInfo.shortNameHash;
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
        if (animator.IsInTransition(0))
        {
            DebugTool.DebugPrint(string.Format("Transition-stateInfoInupdate:{0}-currentInAnimator:{1}", stateInfo.shortNameHash,animator.GetCurrentAnimatorStateInfo(0).shortNameHash));
        }
        if (isTransitionAfter==false)
        {
            if (animator.IsInTransition(0))
            {
                //注意：A向B过渡，在过渡区间 A一直是动画机的当前状态。 AB都会执行StateUpdate，当过渡结束，状态的值由B切换到A
                //这里StateUpdate中的stateinfo 获取的是当前这个脚本所挂的状态的stateinfo，而不是状态机当前状态。
                //要想获取到动画状态是否要向其他状态过渡。只要满足当前过渡中且当前脚本的stateinfo等于当前状态机的stateinfo
                if ((String.Equals(animator.GetCurrentAnimatorStateInfo(0).fullPathHash,stateInfo.fullPathHash)))
                {
                    if (action != null)
                    {
                        Callback();
                    }
                }
            }
        }
    }

    //过渡结束才退出
    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {

        if (action!=null)
        {
            Callback();
            //Debug.Log("Exit_isTransitionAfter_"+stateName);
        }
    }
    public void Callback()
    {
        action?.Invoke();
        action = null;
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
