using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using Pathfinding;
using UnityEditor;
using UnityEngine;

[TaskCategory("Bot")]
public class SeekToTarget: BehaviorDesigner.Runtime.Tasks.Action
{
    private RobotController bot;
    public SharedTransform target;
    private RoleController role;
    public float minDisWhenReachTargetNearby=-1;//负数为以碰撞体radius作限制。
    private bool isRandomTargetNearby=>randomRange_Angle!=0;
    public int randomRange_Angle=60;
    public SharedVector3 recordTargetPoint;
    private Vector3 targetTransPos;
    private float lastTime;
    private float resetTransPosThrehold=0.1f;
    public ControllerState contollerState=ControllerState.Walking;

    public override void OnAwake()
    {
        base.OnAwake();
        bot = GetComponent<ActorComponent>().GetActorComponent<RobotController>();
    }
    public override void OnStart()
    {
        if (target.Value==null)
        {
            return;
        }
        try
        {
            role = target.Value.GetComponent<ActorComponent>().GetActorComponent<RoleController>();
        }
        catch (Exception e)
        {
            Debug.Log(e);
            
        }
        if (Time.time-lastTime>resetTransPosThrehold)
        {
            targetTransPos = Vector3.zero;
        }
        lastTime = Time.time;
    }
    public override TaskStatus OnUpdate()
    {
        if (target.Value==null)
        {
            return TaskStatus.Failure;
        }
        var targetPos = target.Value.position;
        if (Vector3.Distance(targetPos,targetTransPos)>0.05f)//性能优化
        {
            if (minDisWhenReachTargetNearby != 0)
            {
                targetTransPos = targetPos;
                var var_minDisWhenReachTargetNearby = minDisWhenReachTargetNearby;
                if (var_minDisWhenReachTargetNearby < 0)
                {
                    var_minDisWhenReachTargetNearby = bot.characterRadius + role.characterRadius + 0.01f;
                }
                var offset = (targetPos - bot.transform.position);
                var dis = offset.magnitude;
                var dir = offset.normalized;
                if (isRandomTargetNearby)
                {
                    var targetOffestDir = Quaternion.Euler(0, UnityEngine.Random.Range(-randomRange_Angle, randomRange_Angle * 1.0f), 0) * -dir;
                    targetPos = targetPos + (targetOffestDir * var_minDisWhenReachTargetNearby);
                }
                else
                {
                    targetPos = transform.position + dir * (dis - var_minDisWhenReachTargetNearby);
                }
            }
            DebugTool.DrawWireSphere(targetPos, 0.1f, Color.blue, 5);
            recordTargetPoint.Value =  targetPos;
            bot.StartPath(bot.transform.position,targetPos,contollerState);
        }
        if (bot.moveAgent.reachedEndOfPath)
        {
            bot.ResetAnimMoveParameter();
            return TaskStatus.Success;
        }
        bot.animatorMachine.animator.SetFloat(Const_Animation.Forward, bot.GetAnimMoveParameterByState(contollerState), 0.2f, Time.deltaTime);
        return TaskStatus.Running;
    }

    public override void OnConditionalAbort()
    {
        base.OnConditionalAbort();
        bot.StopSeeker();
    }

#if UNITY_EDITOR
    public override void OnDrawGizmos()
    {
        if (target.Value!=null)
        {
            if (Selection.transforms.Contains(target.Value) && Selection.transforms.Contains(Owner.transform))
            {
                Handles.color = new Color(0, 0, 0, 0.1f);
                Handles.DrawSolidArc(target.Value.position, target.Value.up,  target.Value.position-transform.position , -randomRange_Angle, minDisWhenReachTargetNearby);
                Handles.DrawSolidArc(target.Value.position, target.Value.up,  target.Value.position-transform.position, randomRange_Angle, minDisWhenReachTargetNearby);
            }
        }
    }
    #endif
}

