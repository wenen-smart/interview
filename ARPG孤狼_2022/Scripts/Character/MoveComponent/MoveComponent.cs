using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public abstract class MoveComponent : ActorComponent
{
    [HideInInspector] public Vector3 deltaPosition;
    [HideInInspector] public Vector3 increaseVelo;
    public float baseMoveSpeed;
    [HideInInspector] public float speedMult = 0;
    [HideInInspector] public float lastSpeedMult = 0;
    protected bool disableMove = false;
    protected bool ignoreGravity = false;
    public bool useCustomGravity;
    [HideInInspector] public float _gravity = 9.8f;
    protected MotionModifyClip _CurrentRunTimeMotionClip;
    protected abstract Vector3 _mVelocity { get; set; }
    public bool IsPlayMotionClip { get { return _CurrentRunTimeMotionClip != null; } }
    protected Vector3 lastInGroundPosition { get;  set; }
    protected Func<bool> JudgeOnGroundHandler;
    public virtual Vector3 POSITION { get { return transform.position; }set { transform.position = value;} }
    protected float allDirectionsSpeedMult = 1;
    [HideInInspector]
    public NavMeshAgent Agent;
    public CapsuleCollider capsuleCollider;
    public bool _isInGrounded = true;
    private float fallMultiply=10;//tmep;

    public bool IsInGrounded
    {
        get
        {
            return _isInGrounded;
        }
        protected set
        {
            if (_isInGrounded != value)
            {
                // change and maybe apply event
                _isInGrounded = value;
                //changeGroundStateEventHandler.Invoke(value);
            }
        }
    }
    public override void Init()
    {
        base.Init();
        Agent = GetComponent<NavMeshAgent>();
        capsuleCollider = GetComponent<CapsuleCollider>();
    }
    public void SetBaseMoveSpeed(float baseSpeed)
    {
        baseMoveSpeed = baseSpeed;
    }
    public void SetSpeedMult(float mult)
    {
        lastSpeedMult = speedMult;
        speedMult = mult;
    }
    public void SetAllDirSpeedMult(float mult)
    {
        allDirectionsSpeedMult = mult;
    }
    public void RenewAllDirSpeedMult()
    {
        allDirectionsSpeedMult = 1;
    }
    //所有方向上的速度缩放。
    public float GetAllDirSpeedMult()
    {
        return allDirectionsSpeedMult;
    }
    public abstract void CalcuateMove();
    public void ClearIncreaseVelo()
    {
        increaseVelo = Vector3.zero;
    }
    public void ClearMoveSpeed()
    {
        SetSpeedMult(0);
    }
    public float GetGravityABS()
    {
        if (useCustomGravity == false)
        {
            return -Physics.gravity.y * Time.deltaTime;
        }
        return _gravity;
    }
    public abstract Vector3 CalcuateGravityVelocity();
    public Vector3 CalcuateCustomGravityVelocity()
    {
        if (useCustomGravity==false||ignoreGravity)
        {
            return Vector3.zero;
        }
        return _gravity*Vector3.up*Time.deltaTime;//V=FT/m
    }
    public virtual void AddImpluseForce(Vector3 force)
    {

    }
    public virtual void EnableGravity(bool isGravity)
    {
        ignoreGravity = !isGravity;
    }
    public void UpdateMotionClip(MotionModifyClip motionModifyClip)
    {
        _CurrentRunTimeMotionClip?.OnFinish();//TODO
        _CurrentRunTimeMotionClip = motionModifyClip;
        _CurrentRunTimeMotionClip.finishAction += () => {FinishRunTimeMotionClip(_CurrentRunTimeMotionClip.data.MID); };
    }
    public void FinishRunTimeMotionClip(int mid)
    {
        if (_CurrentRunTimeMotionClip==null)
        {
            return;
        }
        if (_CurrentRunTimeMotionClip.data.MID!=mid)
        {
            MyDebug.DebugError("当前要结束的运动片段与当前运动片段不一样，会发生严重错误");
            return;
        }
        _CurrentRunTimeMotionClip = null;
    }
    public abstract void MotionClipTick();
    public virtual void ApplyMotionClipVelocity()
    {
        if (_CurrentRunTimeMotionClip==null||_CurrentRunTimeMotionClip.IsCanApplyMotion==false)
        {
            _CurrentRunTimeMotionClip.OnTickClip(Time.deltaTime*allDirectionsSpeedMult);
            return;
        }
        _CurrentRunTimeMotionClip.OnTickClip(Time.deltaTime*allDirectionsSpeedMult);
        if (_CurrentRunTimeMotionClip!=null)
        {
            CalculateClip();
        }
        return;
    }
    public void CalculateClip()
    {
        if (_CurrentRunTimeMotionClip.IsVelocityMode)
        {
            VelocityMode();
        }
        else if(_CurrentRunTimeMotionClip.IsMoveToPositionMode)
        {
            MoveToPositionMode();
        }
    }
    public virtual void VelocityMode()
    {
        //速度覆盖
        if (_CurrentRunTimeMotionClip.data.isMotionOverride)
        {
            _mVelocity = _CurrentRunTimeMotionClip.GetVelocity();
        }
        else
        {
            _mVelocity += _CurrentRunTimeMotionClip.GetVelocity();
        }
    }

    public virtual void MoveToPositionMode()
    {

    }
    public abstract bool IsOnGround();
    public void RegisterIsOnGroundHandler(Func<bool> func)
    {
        JudgeOnGroundHandler += func;
    }
    //控制一切作用力是否有效(推力)  重力和AddForce除外
    public virtual void SetAffectForceToggle(bool unlock)
    {

    }
    public virtual bool IsInputVelocity()
    {
        return false;
    }
    public bool TargetPositionIsCanArrive(Vector3 targetPos)
    {
        return CalculatePath(targetPos) != null;
    }
    public NavMeshPath CalculatePath(Vector3 targetPos)
    {
        NavMeshPath navMeshPath=new NavMeshPath();
        if (NavMesh.CalculatePath(POSITION, targetPos, NavMesh.AllAreas, navMeshPath))
        {
            return navMeshPath;
        }
        return null;
    }
    public virtual void SetNavMeshTarget(Vector3 target)
    {
        if (Agent==null)
        {
            return;
        }
        if (Agent.enabled==false)
        {
            Agent.enabled = true;
            Agent.updatePosition = true;
            Agent.updateRotation = true;
            Agent.isStopped = true;
            Agent.isStopped = false;
        }
        float speed = baseMoveSpeed* speedMult;
        Agent.speed = speed;
        if (TargetPositionIsCanArrive(target))
        {
            Agent.SetDestination(target);
        }
    }
    public float GetNavRemainingDistance()
    {
        if (Agent.enabled==false||Agent.isOnNavMesh==false)
        {
            return 0;
        }
        return Agent.remainingDistance;
    }
    public virtual void StopNavAgent()
    {
        if (Agent == null)
        {
            return;
        }
        if (Agent.enabled==true)
        {
            Agent.isStopped = true;
            Agent.enabled = false;
        }
    }
    public virtual float GetIntoAirMinUpVelocity()
    {
        return 0;
    }
}
