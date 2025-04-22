using System;
using UnityEngine;
using BehaviorDesigner.Runtime.Tasks;
using BehaviorDesigner.Runtime;

[TaskCategory("Bot")]
public class GoToTargetPoint: BehaviorDesigner.Runtime.Tasks.Action
{
    private RobotController bot;
    public SharedVector3 recordTargetPos;
     public ControllerState controllerState;
    
    private bool isExecute;
    public override void OnAwake()
    {
        base.OnAwake();
        bot = GetComponent<ActorComponent>().GetActorComponent<RobotController>();
    }
    public override void OnStart()
    {
        base.OnStart();
        bot.StartPath(bot.transform.position, recordTargetPos.Value,controllerState);
    }
    public override TaskStatus OnUpdate()
    {
        if (recordTargetPos.Value == Vector3.zero)
        {
            return TaskStatus.Failure;
        }
        if (bot.moveAgent.reachedEndOfPath)
        {
            bot.ResetAnimMoveParameter();
            return TaskStatus.Success;
        }
        bot.animatorMachine.animator.SetFloat(Const_Animation.Forward, bot.GetAnimMoveParameterByState(controllerState), 0.2f, Time.deltaTime);
        return TaskStatus.Running;
    }
    public override void OnConditionalAbort()
    {
        base.OnConditionalAbort();
        bot.StopSeeker();
    }
}

