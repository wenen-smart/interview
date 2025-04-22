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
    [Header("�����ڲ�û���е�ʱ��ִ��")]
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
                //ע�⣺A��B���ɣ��ڹ������� Aһֱ�Ƕ������ĵ�ǰ״̬�� AB����ִ��StateUpdate�������ɽ�����״̬��ֵ��B�л���A
                //����StateUpdate�е�stateinfo ��ȡ���ǵ�ǰ����ű����ҵ�״̬��stateinfo��������״̬����ǰ״̬��
                //Ҫ���ȡ������״̬�Ƿ�Ҫ������״̬���ɡ�ֻҪ���㵱ǰ�������ҵ�ǰ�ű���stateinfo���ڵ�ǰ״̬����stateinfo
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

    //���ɽ������˳�
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
