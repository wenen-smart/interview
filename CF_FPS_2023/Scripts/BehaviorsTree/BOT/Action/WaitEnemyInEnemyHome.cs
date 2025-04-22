using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorDesigner.Runtime.Tasks;
using Assets.Resolution.Scripts.Map;
using Pathfinding;

[TaskCategory("Bot")]
public class WaitEnemyInEnemyHome : Action
{
    RobotController bot;
    private TeamHome enemyHome;
    private NextRoadTreeType nextRoadTreeType;
    private RoadPointNode startNode;
    private RichAI moveAgent;
    private Seeker seeker;
    public ControllerState controllerState;
    public int maxWaiter=2;
    public override void OnAwake()
    {
        bot = GetComponent<ActorComponent>().GetActorComponent<RobotController>();
        moveAgent = bot.moveAgent;
        seeker = bot.seeker;
        enemyHome = MapManager.Instance.GetTeamHomeInfo(bot.GetEnemyUnitTag());
        base.OnAwake();
    }
    public override void OnStart()
    {
        base.OnStart();
        nextRoadTreeType = Random.Range(0,1.0f)>=0.5f?NextRoadTreeType.NextTree:NextRoadTreeType.PriorTree;
        if (nextRoadTreeType==NextRoadTreeType.NextTree)
        {
            startNode = enemyHome.homeNearlyPath.root;
        }
        else if (nextRoadTreeType == NextRoadTreeType.PriorTree)
        {
            startNode = enemyHome.homeNearlyPath.last;
        }
        bot.GoToTargetRoadPoint(startNode);
        bot.SetMoveMode(controllerState);
        bot.team.WaiterEnemyHome(bot);
    }

    public override TaskStatus OnUpdate()
    {
        if (Vector3.Distance(transform.position, bot.targetPointNode.point.position) <= (moveAgent.endReachedDistance + 0.1f))
        {
            switch (nextRoadTreeType)
            {
                case Assets.Resolution.Scripts.Map.NextRoadTreeType.None:
                    break;
                case Assets.Resolution.Scripts.Map.NextRoadTreeType.NextTree:
                    if (bot.targetPointNode.nexts != null && bot.targetPointNode.nexts.Length != 0)
                    {
                        bot.targetPointNode = bot.targetPointNode.NextRandom(NextRoadTreeType.NextTree);
                        seeker.StartPath(transform.position, bot.targetPointNode.point.position);
                    }
                    else
                    {
                        bot.targetPointNode = null;
                    }
                    break;
                case Assets.Resolution.Scripts.Map.NextRoadTreeType.PriorTree:
                    if (bot.targetPointNode.priors != null && bot.targetPointNode.priors.Count != 0)
                    {
                        bot.targetPointNode = bot.targetPointNode.NextRandom(NextRoadTreeType.PriorTree);
                        seeker.StartPath(transform.position, bot.targetPointNode.point.position);
                    }
                    else
                    {
                        bot.targetPointNode = null;
                    }
                    break;
                default:
                    break;
            }
            if (bot.targetPointNode==null)
            {
                //return home
                Owner.SendEvent("ReturnHome");
                return TaskStatus.Failure;
            }
        }
        else
        {
            bot.animatorMachine.animator.SetFloat(Const_Animation.Forward, bot.GetAnimMoveParameterByState(controllerState), 0.2f, Time.deltaTime);
        }
        return TaskStatus.Running;
    }
    public override void OnEnd()
    {
        base.OnEnd();
        bot.team.LeaveWaitEnemyHome(bot);
    }
    public override void OnConditionalAbort()
    {
        base.OnConditionalAbort();
        bot.team.LeaveWaitEnemyHome(bot);
    }
}
