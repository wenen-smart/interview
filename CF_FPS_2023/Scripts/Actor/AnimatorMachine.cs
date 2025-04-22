/**
 $ @Author       : unity-mircale 9944586+unity-mircale@user.noreply.gitee.com
 $ @Date         : 2023-01-09 18:33:57
 $ @LastEditors  : unity-mircale 9944586+unity-mircale@user.noreply.gitee.com
 $ @LastEditTime : 2023-02-01 17:16:38
 $ @FilePath     : \MovementProject\Assets\Resolution\Scripts\AnimatorMachine.cs
 $ @Description  : 
 $ @
 $ @Copyright (c) 2023 by unity-mircale 9944586+unity-mircale@user.noreply.gitee.com, All Rights Reserved. 
 **/
/**
 $ @Author       : unity-mircale 9944586+unity-mircale@user.noreply.gitee.com
 $ @Date         : 2023-01-09 18:33:57
 $ @LastEditors  : unity-mircale 9944586+unity-mircale@user.noreply.gitee.com
 $ @LastEditTime : 2023-01-10 18:33:23
 $ @FilePath     : \MovementProject\Assets\Resolution\Scripts\AnimatorMachine.cs
 $ @Description  : 
 $ @
 $ @Copyright (c) 2023 by unity-mircale 9944586+unity-mircale@user.noreply.gitee.com, All Rights Reserved. 
 **/
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatorMachine : ActorComponent
{
    Movement movement;
    [HideInInspector]public Animator animator;
    [MultSelectTags]public MatchIKGoal _IKGoal;
    public Transform t;
    public AnimatorInfoConfig AnimatorInfoSO;

    public override void Init()
    {
        base.Init();

		transform.localEulerAngles = Vector3.zero;
        movement = GetComponentInParent<Movement>();
        animator = GetComponent<Animator>();
    }

    public void ControlIKGoal(int target)
    {
        if (target<0)
        {
            if (((int)_IKGoal).IsSelectThisEnumInMult(Mathf.Abs(target)))
            {
                _IKGoal += target;
            }
            return;
        }
        if (!((int)_IKGoal).IsSelectThisEnumInMult(target))
        {
            _IKGoal += target;
        }
    }
    public void ClearIKGoal()
    {
        _IKGoal = 0;
    }

    #region Module
    /// <summary>
    /// 
    /// </summary>
    /// <param name="layer"></param>
    /// <param name="stateName"></param>
    /// <param name="start"></param>
    /// <param name="end"></param>
    /// <param name="ignoreTransitionToOther"> 当这个动画切换到其他动画的情况是否要忽略，忽略则认为这种情况不在区间里。</param>
    /// <returns></returns>
    public bool CheckInAnimationDistrict(int layer,string stateName,float start,float end,bool ignoreTransitionToOther=false)
    {
        if (start<0||start>1)
        {
            DebugTool.DebugWarning("动画区间有误，请检查");
        }
        if (end < 0 || end > 1)
        {
            DebugTool.DebugWarning("动画区间有误，请检查");
        }
		if (ignoreTransitionToOther)
		{
			//A->B过渡，检测过渡的目标状态是不是 要检测的状态：stateName。如果是可以检测区间，否则说明要么状态A不是stateName
			//要么状态A是stateName状态，但正在过渡。所以不可检测区间，认为不在区间。这里排除 Self To Self 情况。
			if (animator.IsInTransition(layer) && animator.GetNextAnimatorStateInfo(layer).IsName(stateName) == false)
			{
				return false;
			}
		}
		AnimatorStateInfo animatorStateInfo = animator.GetCurrentAnimatorStateInfo(layer);
         return animatorStateInfo.IsName(stateName)&&animatorStateInfo.normalizedTime>=start&&animatorStateInfo.normalizedTime<=end;
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="_animator"></param>
    /// <param name="layer"></param>
    /// <param name="stateName"></param>
    /// <param name="start"></param>
    /// <param name="end"></param>
    /// <param name="ignoreTransitionToOther"> 当这个动画切换到其他动画的情况是否要忽略，忽略则认为这种情况不在区间里。</param>
    /// <returns></returns>
    public static bool CheckInAnimationDistrict(Animator _animator,int layer, string stateName, float start, float end,bool ignoreTransitionToOther=false)
    {
        if (start < 0 || start > 1)
        {
            DebugTool.DebugWarning("动画区间有误，请检查");
        }
        if (end < 0 || end > 1)
        {
            DebugTool.DebugWarning("动画区间有误，请检查");
        }
		if (ignoreTransitionToOther)
		{
            //A->B过渡，检测过渡的目标状态是不是 要检测的状态：stateName。如果是可以检测区间，否则说明要么状态A不是stateName
            //要么状态A是stateName状态，但正在过渡。所以不可检测区间，认为不在区间。这里排除 Self To Self 情况。
			if (_animator.IsInTransition(layer)&&_animator.GetNextAnimatorStateInfo(layer).IsName(stateName)==false)
			{
                return false;
			}
		}
        AnimatorStateInfo animatorStateInfo = _animator.GetCurrentAnimatorStateInfo(layer);
        return animatorStateInfo.IsName(stateName) && animatorStateInfo.normalizedTime >= start && animatorStateInfo.normalizedTime <= end;
    }
    public void PauseAnimation(int state)
    {
        if (state==0)
        {
            CtrlSpeed(0);
        }
        else
        {
            CtrlSpeed(1);
        }
    }
    public void CtrlSpeed(float speed)
    {
         animator.speed = speed;
    }
    public bool CheckAnimationStateByName(int layer,string stateName)
    {
        return animator.GetCurrentAnimatorStateInfo(layer).IsName(stateName);
    }
    public bool CheckAnimationStateByName(int layer,params string[] stateNames)
    {
        foreach (var item in stateNames)
        {
            if (animator.GetCurrentAnimatorStateInfo(layer).IsName(item))
            {
                return true;
            }
        }
        return false;
    }  
    public bool CheckAnimationStateByTag(int layer,string stateTag)
    {
        return animator.GetCurrentAnimatorStateInfo(layer).IsTag(stateTag);
    }
    public bool CheckNextAnimationStateByName(int layer, params string[] stateNames)
    {
        foreach (var item in stateNames)
        {
            if (animator.GetNextAnimatorStateInfo(layer).IsName(item))
            {
                return true;
            }
        }
        return false;
    }
    public bool CheckNextAnimationStateByName(int layer,string stateName)
    {
        return animator.GetNextAnimatorStateInfo(layer).IsName(stateName);
    }
    public bool CheckNextAnimationStateByTag(int layer, string stateTag)
    {
        return animator.GetNextAnimatorStateInfo(layer).IsTag(stateTag);
    }


    public bool CheckNextOrCurrentAnimationStateByName(int layer, params string[] stateNames)
    {
        return  CheckNextAnimationStateByName(layer,stateNames)||CheckAnimationStateByName(layer,stateNames);
    }
    public bool CheckNextOrCurrentAnimationStateByName(int layer, string stateName)
    {
        return  CheckNextAnimationStateByName(layer,stateName)||CheckAnimationStateByName(layer,stateName);
    }
    public bool CheckNextOrCurrentAnimationStateByTag(int layer, string stateTag)
    {
        return  CheckNextAnimationStateByTag(layer,stateTag)||CheckAnimationStateByTag(layer,stateTag);
    }
    #region Static
    public static bool CheckAnimationStateByName(Animator animator,int layer, string stateName)
    {
        return animator.GetCurrentAnimatorStateInfo(layer).IsName(stateName);
    }
    public static bool CheckAnimationStateByName(Animator animator,int layer, params string[] stateNames)
    {
        foreach (var item in stateNames)
        {
            if (animator.GetCurrentAnimatorStateInfo(layer).IsName(item))
            {
                return true;
            }
        }
        return false;
    }
    public static bool CheckAnimationStateByTag(Animator animator,int layer, string stateTag)
    {
        return animator.GetCurrentAnimatorStateInfo(layer).IsTag(stateTag);
    }
    public static bool CheckNextAnimationStateByName(Animator animator,int layer, params string[] stateNames)
    {
        foreach (var item in stateNames)
        {
            if (animator.GetNextAnimatorStateInfo(layer).IsName(item))
            {
                return true;
            }
        }
        return false;
    }
    public static bool CheckNextAnimationStateByName(Animator animator,int layer, string stateName)
    {
        return animator.GetNextAnimatorStateInfo(layer).IsName(stateName);
    }
    public static bool CheckNextAnimationStateByTag(Animator animator,int layer, string stateTag)
    {
        return animator.GetNextAnimatorStateInfo(layer).IsTag(stateTag);
    }


    public static bool CheckNextOrCurrentAnimationStateByName(Animator animator,int layer, params string[] stateNames)
    {
        return CheckNextAnimationStateByName(animator,layer, stateNames) || CheckAnimationStateByName(animator,layer, stateNames);
    }
    public static bool CheckNextOrCurrentAnimationStateByName(Animator animator,int layer, string stateName)
    {
        return CheckNextAnimationStateByName(animator,layer, stateName) || CheckAnimationStateByName(animator,layer, stateName);
    }
    public static bool CheckNextOrCurrentAnimationStateByTag(Animator animator,int layer, string stateTag)
    {
        return CheckNextAnimationStateByTag(animator,layer, stateTag) || CheckAnimationStateByTag(animator,layer, stateTag);
    }
    public static void ListenerAnimation(Animator animator, int layer, string animationName, float startNormalizeTime, Action callback)
    {
        ListenerAnimation(animator,layer,Animator.StringToHash(animationName),startNormalizeTime,callback);
    }
	public static void ListenerAnimation(Animator animator, int layer, int shortNameHash, float startNormalizeTime, Action callback)
	{
		ListenAnimationClip listenAnimationClip = animator.gameObject.GetComponent<ListenAnimationClip>();
		if (listenAnimationClip == null)
		{
			listenAnimationClip = animator.gameObject.AddComponent<ListenAnimationClip>();
		}
		ListenAnimationDistinct listenAnimationDistinct = new ListenAnimationDistinct(layer, shortNameHash, startNormalizeTime, startNormalizeTime, true, callback);
		listenAnimationClip.Listen(listenAnimationDistinct);
	}
	#endregion
	public void SetMirrorParameter(bool isMirror)
    {
        animator.SetBool("Mirror", isMirror);
    }
    public  Vector3 GetRootPosition(RootMotionProcessClear posClearPriorityInProcess)
    {
        var localPoint = transform.InverseTransformPoint(animator.rootPosition);//DeltaPosition 是世界空间下
        if (posClearPriorityInProcess.HasFlag(RootMotionProcessClear.ClearX))
        {
            localPoint.x = (0);
        }
        if (posClearPriorityInProcess.HasFlag(RootMotionProcessClear.ClearY))
        {
            localPoint.y = (0);
        }
        if (posClearPriorityInProcess.HasFlag(RootMotionProcessClear.ClearZ))
        {
            localPoint.z = (0);
        }
        return transform.TransformPoint(localPoint);
    }
    public void LerpLayer(string layerName,float weight,float speed)
    {


    }

    public void CrossFade(string destinationStateName,float transitionDuration,int layer,float transitionOffset,bool isAutoAccordingReadTransitionSo=false)
	{
		if (isAutoAccordingReadTransitionSo&&AnimatorInfoSO)
		{
            var stateinfo = animator.GetCurrentAnimatorStateInfo(layer);
             StateTransition stateTransition = AnimatorInfoSO.GetStateTransition(layer,stateinfo.shortNameHash,Animator.StringToHash(destinationStateName));
			if (stateTransition != null)
			{
				if (stateinfo.normalizedTime>=stateTransition.exitTime)
				{
					if (stateTransition.exitTime<0)
					{
                        animator.CrossFade(stateTransition.destStateShortNameHash, stateTransition.duration, stateTransition.layer, stateTransition.offset,0);
					}
                    else
					{
						float alreadyPassNormalieTime = Mathf.Clamp01((stateinfo.normalizedTime - stateTransition.exitTime) / stateTransition.duration);
                        Debug.Log("alreadyPassNormalieTime:"+alreadyPassNormalieTime);
						animator.CrossFade(stateTransition.destStateShortNameHash, stateTransition.duration, stateTransition.layer, stateTransition.offset, alreadyPassNormalieTime);
					}
                    
				}
                else
				{
                    ListenerAnimation(animator,layer,stateTransition.curStateShortNameHash,stateTransition.exitTime,()=> { 
                    animator.CrossFade          (stateTransition.destStateShortNameHash,stateTransition.duration,stateTransition.layer,stateTransition.offset);
                    });
				}
                
			}
            else
			{
                //Debug.LogWarning($"找不到 到{destinationStateName}的过渡");
                animator.CrossFade(destinationStateName,transitionDuration,layer,transitionOffset);
			}
		}
        else
		{
            animator.CrossFade(destinationStateName,transitionDuration,layer,transitionOffset);
		}
	}
    #endregion
}
public enum MatchIKGoal
{
    LeftHand=1,
    RightHand=1<<1,
    LeftFoot=1<<2,
    RightFoot=1<<3,
    Root=1<<4
}
