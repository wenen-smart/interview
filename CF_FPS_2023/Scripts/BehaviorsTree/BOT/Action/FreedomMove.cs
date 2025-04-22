using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorDesigner.Runtime.Tasks;
using Pathfinding;
using Assets.Resolution.Scripts.Map;

[TaskCategory("Bot")]
public class FreedomMove : Action
{
    private RobotController bot;
    private RichAI moveAgent;
    private Seeker seeker;
    public ControllerState controllerState;
    public override void OnAwake()
    {
        base.OnAwake();
        bot = GetComponent<ActorComponent>().GetActorComponent<RobotController>();
        moveAgent = bot.moveAgent;
        seeker = bot.seeker;
    }
    public override void OnStart()
    {
        bot.FindClosetRoadPointAndToMove();
        bot.SetMoveMode(controllerState);
    }
    public override TaskStatus OnUpdate()
    {
        if (Vector3.Distance(transform.position, bot.targetPointNode.point.position) <= (moveAgent.endReachedDistance + 0.1f))
        {
            bot.ArriveRoadPointNode(bot.targetPointNode);
            switch (MapManager.Instance.ToEnemyHomeDir(bot.unit))
            {
                case Assets.Resolution.Scripts.Map.NextRoadTreeType.None:
                    break;
                case Assets.Resolution.Scripts.Map.NextRoadTreeType.NextTree:
                    if (bot.targetPointNode.nexts != null && bot.targetPointNode.nexts.Length != 0)
                    {
                        bot.targetPointNode = bot.targetPointNode.NextRandom(NextRoadTreeType.NextTree);
                        seeker.StartPath(transform.position, bot.targetPointNode.point.position);
                    }
                    break;
                case Assets.Resolution.Scripts.Map.NextRoadTreeType.PriorTree:
                    if (bot.targetPointNode.priors != null && bot.targetPointNode.priors.Count != 0)
                    {
                        bot.targetPointNode = bot.targetPointNode.NextRandom(NextRoadTreeType.PriorTree);
                        seeker.StartPath(transform.position, bot.targetPointNode.point.position);
                    }
                    break;
                default:
                    break;
            }

        }
        else
        {
            //TODO
            if (bot.waitTimer_whenVeryNearAnotherAgent.timerState != MyTimer.TimerState.Run)
            {
                //if (bot.waitTimer_whenVeryNearAnotherAgent.timerState == MyTimer.TimerState.Finish)
                //{
                //    //continue
                //    bot.SetMoveMode(controllerState);
                //}
                bot.animatorMachine.animator.SetFloat(Const_Animation.Forward, bot.GetAnimMoveParameterByState(controllerState), 0.2f, Time.deltaTime);
            }
        }
        return TaskStatus.Running;
    }
    public override void OnConditionalAbort()
    {
        base.OnConditionalAbort();
        bot.StopSeeker();
    }
}
