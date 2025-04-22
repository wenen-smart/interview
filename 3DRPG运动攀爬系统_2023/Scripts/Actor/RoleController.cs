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
using Cinemachine;
using System;
using UnityEditor;
using UnityEngine.Animations.Rigging;

public delegate void StateChangeDelegate(bool state);
[DefaultExecutionOrder(200)]
public class RoleController : ActorComponent
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
    public float movement_baseSpeed=0f;
    public float movement_angularSpeed = 15;
    public bool IsInGround
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
    public override void ActorComponentAwake()
    {
        base.ActorComponentAwake();
        animatorMotionDriver = GetActorComponent<AnimatorMotionDriver>();
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
}

