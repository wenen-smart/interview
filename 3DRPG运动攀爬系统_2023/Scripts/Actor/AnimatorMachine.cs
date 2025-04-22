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
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatorMachine : ActorComponent
{
    Movement movement;
    [HideInInspector]public Animator animator;
    [MultSelectTags]public MatchIKGoal _IKGoal;
    public Transform t;
    public override void ActorComponentAwake()
    {
        transform.localEulerAngles=Vector3.zero;
    }
    public override void Start()
    {
        movement=GetComponentInParent<Movement>();
        animator=GetComponent<Animator>();
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
    public bool CheckInAnimationDistrict(int layer,string stateName,float start,float end)
    {
        if (start<0||start>1)
        {
            DebugTool.DebugWarning("动画区间有误，请检查");
        }
        if (end < 0 || end > 1)
        {
            DebugTool.DebugWarning("动画区间有误，请检查");
        }
         AnimatorStateInfo animatorStateInfo = animator.GetCurrentAnimatorStateInfo(layer);
         return animatorStateInfo.IsName(stateName)&&animatorStateInfo.normalizedTime>=start&&animatorStateInfo.normalizedTime<=end;
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