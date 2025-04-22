using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AnimatorTransArgType
{
    None,
    Float,
    Object,
    AnimatorCurve,
    Bool,
    String,
    Int,
}
[Serializable]
public class AnimatorUpdateArgSaver
{
    public string methodName;
    public AnimatorTransArgType argType;
    public string arg;
    [HideInInspector]
    public Animator animator;
    [HideInInspector]
    public AnimatorStateInfo stateInfo;
    [HideInInspector]
    public int layerIndex;


    public AnimatorUpdateArgSaver(string methodName, AnimatorTransArgType argType, string arg)
    {
        this.methodName = methodName;
        this.argType = argType;
        this.arg = arg;
    }

    public void SetAnimatorInfo(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        this.animator = animator;
        this.stateInfo = stateInfo;
        this.layerIndex = layerIndex;
    }
}
