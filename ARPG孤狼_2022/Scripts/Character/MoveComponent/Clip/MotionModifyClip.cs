using Buff.Base;
using Buff.Enum;
using System;
using UnityEngine;

public struct MotionModifyClipData
{
    public int MID;
    public MotionModifyType motionModifyType;
    public bool isMotionOverride;
    public float targetTime;
    public float motionSpeed;
    public bool IsUnInterruptedCalcuateVelocity;//是否不间断的计算速度
    public MotionVariableType variableType;//是已知时间  还是已知速度 。
}

public class MotionModifyClip
{
    public MotionModifyClipData data;
    public MotionModifyBuff buff;
    protected Vector3 targetDis;
    public Vector3 _goalPos;
    public Vector3 TargetDis { get => targetDis;protected set => targetDis = value; }
    protected int tickedCount = 0;
    public float currentTime;
    public bool IsCanApplyMotion { get { return !(data.IsUnInterruptedCalcuateVelocity == false &&tickedCount>0); } }
    public Action finishAction;
    protected Vector3 currentPos;
    public bool IsVelocityMode { get { return MotionClipFactory.IsVelocityMode(data.motionModifyType); } }
    public bool IsMoveToPositionMode { get { return MotionClipFactory.IsMoveToPositionMode(data.motionModifyType); } }
    public bool isFinish = false;
    public Func<float> GetSpeedMultHandler;
    public virtual void Init()
    {
        currentPos = buff.AffectedPerson.transform.position;
        isFinish = false;
    }
    public virtual void OnTickClip(float deltaTime)
    {
        if (isFinish)
        {
            Finish();
        }
    }
    public void OnFinish()
    {
        isFinish = true;
    }
    protected void Finish()
    {
        isFinish = false;
        currentTime = 0;
        finishAction?.Invoke();
        finishAction = null;
    }
    public Vector3 GetVelocity()
    {
       return targetDis / data.targetTime;
    }
    public Vector3 GetCurrentFramePos()
    {
        return currentPos;
    }
    public float GetAllDirSpeedMult()
    {
        if (GetSpeedMultHandler==null)
        {
            return 1;
        }
        return GetSpeedMultHandler.Invoke();
    }
}
