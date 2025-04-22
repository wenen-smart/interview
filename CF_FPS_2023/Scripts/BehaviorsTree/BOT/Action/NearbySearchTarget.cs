using System;
using Assets.Resolution.Scripts.Map;
using Assets.Resolution.Scripts.Region;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using Pathfinding;
using UnityEngine;

[TaskCategory("Bot")]
public class NearbySearchTarget: BehaviorDesigner.Runtime.Tasks.Action
{
    private RobotController bot;
    public SharedVector3 recordTargetPos;
    public float searchMinRange=3;
    public float searchMaxRange=6;
    public float searchHighAreaDis=10;
    public SearchNearbyMethod searchNearbyMethod;
    private RoadPointNode bestNearbyNode;
    private RoadPointNode Nearby_targetNode;
    private AIRegion targetRegion;
    private IntoRegionPath intoRegionPath;
    private Vector3 targetPosInRegion;
    private bool isArriveRegionOrEntry;
    public ControllerState controllerState;
    public override void OnAwake()
    {
        base.OnAwake();
        bot = GetComponent<ActorComponent>().GetActorComponent<RobotController>();
    }
    public override void OnStart()
    {
        base.OnStart();
        //bestNearbyNode = MapManager.Instance.FindBestClosetPathPoint(recordTargetPos.Value);
        //Nearby_targetNode = MapManager.Instance.FindNearbyPathPoint(bestNearbyNode.point.position, UnityEngine.Random.Range(searchMinRange,searchMaxRange),true,bestNearbyNode.point);
        //if (Nearby_targetNode != null)
        //{
        //    bot.seeker.StartPath(transform.position,Nearby_targetNode.point.position);
        //    DebugTool.DrawWireSphere(Nearby_targetNode.point.position,0.2f,Color.red,5,"SearchTarget");
        //}

        //寻找高台

        AIRegion region = bot.FindNearbyHighArea(searchHighAreaDis,searchNearbyMethod);
        intoRegionPath = null;
        if (region)
        {
            if (region.pointNodes.Length>0)
            {
                intoRegionPath = region.intoRegion[0];//TODO Random
                targetPosInRegion=region.pointNodes[region.pointNodes.Length==1?0:UnityEngine.Random.Range(0,region.pointNodes.Length)].position;
                //bot.seeker.StartPath(transform.position, intoRegionPath.RegionEntry.position);
                bot.StartPath(transform.position, targetPosInRegion,controllerState);
            }
        }
    }
    public override TaskStatus OnUpdate()
    {
        //if (isArriveRegionOrEntry == false  && Vector3.Distance(transform.position, intoRegionPath.RegionEntry.position) <= (bot.moveAgent.endReachedDistance + 0.01f))
        //{
        //    isArriveRegionOrEntry = true;
        //    bot.moveAgent.destination = targetPosInRegion;
        //}
        //else
        //{
        //    if (isArriveRegionOrEntry)
        //    {

        //    }
        //}
        if (intoRegionPath==null)
        {
            return TaskStatus.Failure;
        }
        if (Vector3.Distance(transform.position, targetPosInRegion) <= (bot.moveAgent.endReachedDistance + 0.01f))
        {
            return TaskStatus.Success;
        }
        bot.animatorMachine.animator.SetFloat(Const_Animation.Forward, bot.GetAnimMoveParameterByState(controllerState), 0.2f, Time.deltaTime);
        return TaskStatus.Running;
    }
}

