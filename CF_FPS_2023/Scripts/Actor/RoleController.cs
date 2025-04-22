/**
 $ @Author       : unity-mircale 9944586+unity-mircale@user.noreply.gitee.com
 $ @Date         : 2023-01-09 16:10:57
 $ @LastEditors  : unity-mircale 9944586+unity-mircale@user.noreply.gitee.com
 $ @LastEditTime : 2023-02-11 15:57:33
 $ @FilePath     : \MovementProject\Assets\Resolution\Scripts\RoleController.cs
 $ @Description  : 
 $ @
 $ @Copyright (c) 2023 by unity-mircale 9944586+unity-mircale@user.noreply.gitee.com, All Rights Reserved. 
 **/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEditor;
using Assets.Resolution.Scripts.Inventory;


public enum UnitCamp
{
    A=1,
    B=1<<1
}
public delegate void StateChangeDelegate(bool state);
[DefaultExecutionOrder(200)]
public abstract partial class RoleController : ActorComponent
{
    /// <summary>
    /// 为防止错误修改身体的坐标信息，覆盖基类属性，直接返回人物根对象的transform。
    /// 同时公开一个bodyTransform的属性，供需要调整body坐标信息时访问。
    /// </summary>
    new public Transform transform { get { return actorSystem.transform; } }
    public Transform bodyTransfrom { get { return base.transform; } }
    [HideInInspector]
    public AnimatorMachine animatorMachine;
    [HideInInspector]
    public Movement movement;
    private AnimatorMotionDriver animatorMotionDriver;
    public Vector3 deltaPosition { get { return movement.deltaPosition; } set { movement.deltaPosition = value; } }
    public Quaternion deltaRotation { get { return movement.deltaRotation; } set { movement.deltaRotation = value; } }
    protected Vector3 baseDir_CoordinateAxis;//基准方向坐标系

    protected Vector3 lastMovementAxis;
    protected Vector3 lastContinueMovementPosition;//上一次满足持续移动记录的值  触发记录：第一帧按下或急停转向。
    public bool isCanCtrlRotate;
    public bool isCanCtrlMove;
    [SerializeField]protected float movement_Speed=0f;
    [SerializeField]private float walkSpeed;
    [SerializeField]private float runSpeed;
    [SerializeField]protected float crouchSpeedRate=1;
    public float WalkSpeed { get { return (isCrouch ? crouchSpeedRate : 1) * walkSpeed; } }
    public float RunSpeed { get { return (isCrouch ? crouchSpeedRate : 1) * runSpeed; } }
    public float movement_angularSpeed = 15;
    public virtual bool IsInGround
    { 
        get{
            return movement.IsInGrounded;
        }
    }
    public float air_Friction=0.5f;
    //public float ground_Friction = 10f;
    public float jump_BaseMovenumInPanel=4;
    protected float currentHorizontalMovenum;
    protected Vector3 movenum;
    //protected Vector3 lastMovenum;
    public float invaild_loseGroundDelta=0.1f;
    protected float lastTime_loseGround;
    public LayerMask EnvironmentLayer;
    [HideInInspector]public bool isCrouch;
    [HideInInspector]public bool isJump;
    public Collider[] bodyColliders;
    [HideInInspector]public ActorHealth actorHealth;
    public abstract float characterRadius { get; }
    [HideInInspector]public CapsuleCollider characterCollider;
    public virtual Vector3 EyePosition
	{
		get
		{
            return head.position;
		}
	}
    public override void ActorComponentAwake()
    {
        base.ActorComponentAwake();
        actorHealth = actorSystem.GetComponentInChildren<ActorHealth>();
        actorHealth.Init();
        actorHealth.OnDamage += OnDamage;
        actorHealth.OnDie += OnDie;
        animatorMotionDriver = GetActorComponent<AnimatorMotionDriver>();
        animatorMachine = GetActorComponent<AnimatorMachine>();
        animatorMachine.Init();
        FindHead();
        RuntimeInventory = GetActorComponent<MyRuntimeInventory>();
        Debug.Log("AnimatorMachine"+animatorMachine);
        movement = transform.GetComponent<Movement>();
        characterCollider = transform.GetComponent<CapsuleCollider>();
        movement_Speed = RunSpeed;
        RuntimeInventory.ThrowWeaponHandler += ThrowOutWeapon;
        actorHealth.attachAsChildHandler += EffectAttachToBone;
        MapManager.Instance.RegisterCamp(unit.ToString(), this);
    }
    protected virtual void OnAnimatorMove()
    {
        if (animatorMotionDriver!=null&&animatorMotionDriver.enabled)
        {
            animatorMotionDriver.AnimatorMove(animatorMachine.animator,animatorMachine.animator.deltaPosition,animatorMachine.animator.deltaRotation);
        }
        
    }


    public void OpenKinematic()
    {
        movement.EnableKinematic(true);
    }
    public void CloseKinematic()
    {
        movement.EnableKinematic(false);
    }
   
   
   
    public void OpenRotateSwitch()
    {
        isCanCtrlRotate = true;
    }
    public void OpenMoveSwitch()
    {
        isCanCtrlMove = true;
    }
    public void OpenMoveAndDirectionSwitch()
    {
        OpenRotateSwitch();
        OpenMoveSwitch();
    }
    public void CloseMoveAndDirectionSwitch()
    {
        CloseRotateSwitch();
        CloseMoveSwitch();
    }
    public void CloseRotateSwitch()
    {
        isCanCtrlRotate = false;
    }
    public void CloseMoveSwitch()
    {
        isCanCtrlMove = false;
    }
    
    public float GetFixedMovementInputValueMagnitude(Vector2 _movementAxis)
    {
        Vector3 movementAxis = new Vector3(_movementAxis.x, 0, _movementAxis.y);
        //修正斜边单位化
        float angle = ConvertTolower90(movementAxis);
        float fixedMagnitude = angle == 90 ? movementAxis.magnitude : Mathf.Cos(angle * Mathf.Deg2Rad) * movementAxis.magnitude;
        return fixedMagnitude;
    }

    /** 
     * @Description: 将角度转换为锐角
     * @param       axis [Vector3]:
     * @return      [*]
     * @author     : Miracle
     */
    private float ConvertTolower90(Vector3 axis)
    {
        float angle = Vector3.Angle(axis, Vector3.forward);
        if (angle > 90) angle = 180 - angle;
        return angle;
    }

    private void OnDrawGizmos()
    {
        
    }
    public Vector3 ProjectOnSelfXZPlane(Vector3 normal)
    {
        return Vector3.ProjectOnPlane(normal, Vector3.up);
    }


    public abstract bool IsInMove();

    public void CalculateControllerState()
    {
        //在地面上运动
        if (IsInMove() && IsInGround)
        {
            //isn't idle and air
            controllerState &= ~ControllerState.Idle;
            controllerState &= ~ControllerState.InAir;
            if (movement_Speed == WalkSpeed)
            {
                controllerState |= ControllerState.Walking;
            }
            else
            {
                controllerState &= ~ControllerState.Walking;
            }
            if (movement_Speed == RunSpeed)
            {
                controllerState |= ControllerState.Running;
            }
            else
            {
                controllerState &= ~ControllerState.Running;
            }
        }
        else if (IsInGround)
        {
            //idle
            controllerState = ControllerState.Idle;
        }
        else
        {
            controllerState = ControllerState.InAir;
        }
        if (isCrouch)
        {
            controllerState |= ControllerState.Crouched;
        }
        else
        {
            controllerState &= ~ControllerState.Crouched;
        }
        if (isJump)
        {
            controllerState |= ControllerState.Jumped;
        }
        else
        {
            controllerState &= ~ControllerState.Jumped;
        }
        //TODO
    }
    public bool IsBodyCollider(Collider _collider)
    {
        foreach (var item in bodyColliders)
        {
            if (item == _collider)
            {
                return true;
            }
        }
        return false;
    }

    public abstract void SetMoveMode(ControllerState controllerState);

    public virtual  void ReSpawn()
    {
        isJump = false;
        isCrouch = false;
        movement_Speed = RunSpeed;
        actorHealth.Init();
        animatorMachine.animator.Rebind();
        RuntimeInventory.InitializeRuntimeInventory();
        RuntimeInventory.ExchangeWeapon(0);
    }
    protected virtual void OnDamage(DamageInfo damageInfo) { }
    protected virtual void OnDie(DamageInfo damageInfo) { }
    /// <summary>
    /// 获取距离最近的骨头： 默认是获取Unity人形骨架，勾选Humanoid才能使用
    /// </summary>
    /// <param name="pos"></param>
    /// <returns></returns>
    public virtual Transform GetClosetBoneTransByHumanoid(Vector3 pos)
    {
        Animator anim = animatorMachine.animator;
        Transform targetBone = null;
        float closetDis = 0;
        for (int i = 1; i <= 54; i++)
        {
            Transform bone = anim.GetBoneTransform((HumanBodyBones)i);
            if (bone == null)
            {
                continue;
            }
            if (targetBone == null)
            {
                targetBone = bone;
                closetDis = (pos - bone.position).sqrMagnitude;
            }
            else
            {
                var dis = (pos - bone.position).sqrMagnitude;
                if (closetDis > dis)
                {
                    closetDis = dis;
                    targetBone = bone;
                }
            }
        }
        return targetBone;
    }
    /// <summary>
    /// 获取距离最近的骨头：使用与所有有骨骼模型 通用
    /// </summary>
    /// <param name="pos"></param>
    /// <returns></returns>
    public virtual Transform GetClosetBoneTransGeneric(Vector3 pos)
    {
        //暂时可能没法实现
        SkinnedMeshRenderer[] skinnedMeshRenderer = GetComponentsInChildren<SkinnedMeshRenderer>();

        Transform targetBone = null;
        float closetDis = 0;
        for (int i = 0; i < skinnedMeshRenderer.Length; i++)
        {
            Transform bone = skinnedMeshRenderer[i].rootBone;
            if (bone == null)
            {
                continue;
            }
            if (targetBone == null)
            {
                targetBone = bone;
                closetDis = (pos - bone.position).sqrMagnitude;
            }
            else
            {
                var dis = (pos - bone.position).sqrMagnitude;
                if (closetDis > dis)
                {
                    closetDis = dis;
                    targetBone = bone;
                }
            }
        }
        return targetBone;
    }

    public virtual Transform GetClosetBoneTrans(Vector3 pos)
    {
        Transform boneTrans = null;
        if (animatorMachine.animator.isHuman)
        {
            boneTrans = GetClosetBoneTransByHumanoid(pos);
        }
        else
        {
            boneTrans = GetClosetBoneTransGeneric(pos);
        }
        return boneTrans;
    }

    public virtual void EffectAttachToBone(GameObject go)
    {
        Transform bone =  GetClosetBoneTrans(go.transform.position);
        go.transform.SetParent(bone,true);
    }

    public virtual Transform GetHeadTransform()
	{
        return head;
	}
}
[System.Serializable]
public struct RoleHeightInfo
{
    public Transform viewerPoint;
    public float movement_ColliderHeight;
}
