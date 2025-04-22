using Assets.Resolution.Scripts.Map;
using Pathfinding;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RoadEntity : MonoBehaviour
{
    public MapRoad[] road;
    [HideInInspector] public RoadPointNode root;
    [HideInInspector] public RoadPointNode last;
    public Dictionary<Transform, RoadPointNode> roadNodeDics=new Dictionary<Transform, RoadPointNode>(); //树中节点。
    public bool drawNextDir = true;
    public bool drawPriorDir = false;
    public bool DrawRoadInNoRuntimeMode=true;
    public Color drawNextsRoodPathColor=Color.yellow;
    public Color drawPriorsRoodPathColor=Color.gray;
    public bool isOnlySelectDraw=true;
    //public bool isLoop=false;


    public void OnValidate()
    {
        if (road==null||road.Length==0||DrawRoadInNoRuntimeMode==false)
        {
            return;
        }
        ConstructRoadTree();
    }
    public void OnDrawGizmos()
    {
        if (isOnlySelectDraw)
        {
            return;
        }
        if (DrawRoadInNoRuntimeMode==false&&Application.isPlaying==false)
        {
            return;
        }
        if (drawNextDir)
        {
            Gizmos.color = drawNextsRoodPathColor;
            DrawNextsRoodNode(root);
        }
        if (drawPriorDir)
        {
            Gizmos.color = drawPriorsRoodPathColor;
            DrawPriorsRoodNode(last);
        }
    }
    void OnDrawGizmosSelected()
    {
        if (!isOnlySelectDraw)
        {
            return;
        }
        if (DrawRoadInNoRuntimeMode == false && Application.isPlaying == false)
        {
            return;
        }
        if (drawNextDir)
        {
            Gizmos.color = drawNextsRoodPathColor;
            DrawNextsRoodNode(root);
        }
        if (drawPriorDir)
        {
            Gizmos.color = drawPriorsRoodPathColor;
            DrawPriorsRoodNode(last);
        }

    }
    public void DrawNextsRoodNode(RoadPointNode roadPointNode)
    {
        if (roadPointNode == null)
        {
            return;
        }
        if (roadPointNode.nexts == null || roadPointNode.nexts.Length == 0)
        {
            return;
        }
        var start = roadPointNode.point.position;
        for (int i = 0; i < roadPointNode.nexts.Length; i++)
        {
            Gizmos.DrawLine(start, roadPointNode.nexts[i].point.position);
            DrawNextsRoodNode(roadPointNode.nexts[i]);
        }
    }
    public void DrawPriorsRoodNode(RoadPointNode roadPointNode)
    {
        if (roadPointNode.priors == null || roadPointNode.priors.Count == 0)
        {
            return;
        }
        var start = roadPointNode.point.position;
        for (int i = 0; i < roadPointNode.priors.Count; i++)
        {
            Gizmos.DrawLine(start, roadPointNode.priors[i].point.position);
            DrawPriorsRoodNode(roadPointNode.priors[i]);
        }
    }
    public void ConstructRoadTree()
    {
        //->next

        roadNodeDics.Clear();
        for (int i = 0; i < road.Length; i++)
        {
            RoadPointNode roadPointNode = new RoadPointNode();
            if (i == 0)
            {
                root = roadPointNode;
            }
            else
            {
                if (road[i].toTargets.Length == 0)
                {

                    last = roadPointNode;
                }
            }
            roadNodeDics.Add(road[i].point, roadPointNode);//记录
        }
        for (int i = 0; i < road.Length; i++)
        {
            PreorderContruct(roadNodeDics[road[i].point], road[i]);
        }
    }
    public void PreorderContruct(RoadPointNode roadPointNode, MapRoad roadLeaf)
    {
        roadPointNode.point = roadLeaf.point;
        roadPointNode.nexts = new RoadPointNode[roadLeaf.toTargets.Length];
        RoadPointNode nextNode;

        for (int i = 0; i < roadLeaf.toTargets.Length; i++)
        {
            Transform point = roadLeaf.toTargets[i];

            if (roadNodeDics.ContainsKey(point))//
            {
                nextNode = roadNodeDics[point];//网存在一个结点被多个结点连接。
                roadPointNode.nexts[i] = nextNode;
                if (nextNode.priors == null)
                {
                    nextNode.priors = new List<RoadPointNode>();
                }
                nextNode.priors.Add(roadPointNode);
            }
            //else
            //{
            //    nextNode = new RoadPointNode();
            //    nextNode.point = point;
            //    last = nextNode;
            //    roadNodeDics.Add(point, nextNode);//记录
            //}
            
        }
    }
    public RoadPointNode FindNearbyPathPoint(RobotController robotController, float limitDis, SearchNearbyMethod searchNearbyMethod, bool randomOrMustNotMin = false, Transform ignoreTransform = null)
    {
        RoadPointNode minPoint = null;
        float minDis = -1;
        List<RoadPointNode> list = new List<RoadPointNode>();
        foreach (var item in roadNodeDics)
        {
            if (item.Key == ignoreTransform)
            {
                continue;
            }
            var dis = robotController.GetPathDistance(item.Key.position, searchNearbyMethod);


            if (limitDis >= dis)
            {
                list.Add(item.Value);
                if (minDis > dis || minPoint == null)
                {
                    minDis = dis;
                    minPoint = item.Value;
                }
            }

        }
        if (list.Count > 0)
        {
            if (list.Count == 1)
            {
                return list[0];
            }
            if (randomOrMustNotMin)
            {
                return list[UnityEngine.Random.Range(0, list.Count)];
            }
            else
            {
                return minPoint;
            }
        }
        return null;
    }

    public RoadPointNode FindBestClosetPathPoint(RobotController robotController, SearchNearbyMethod searchNearbyMethod, Transform ignoreTransform = null)
    {
        RoadPointNode point = null;
        float minDis = -1;
        foreach (var item in roadNodeDics)
        {
            if (item.Key == ignoreTransform)
            {
                continue;
            }
            var dis = robotController.GetPathDistance( item.Key.position, searchNearbyMethod);


            if (minDis < 0 || dis < minDis)
            {
                minDis = dis;
                point = item.Value;
            }
        }
        return point;
    }
    public RoadPointNode FindBestClosetPathPointInDir(RobotController robotController, RoadPointNode targetNode, SearchNearbyMethod searchNearbyMethod, Transform ignoreTransform = null)
    {
        RoadPointNode nearlyNode = FindBestClosetPathPoint(robotController, searchNearbyMethod, ignoreTransform);
        var targetPoint = targetNode.point.position;
        NextRoadTreeType roadTreeType;
        bool nextisLower = NextCostLowerThanCurrent(out roadTreeType, nearlyNode, targetNode);
        if (nextisLower)
        {
            IEnumerable<RoadPointNode> tree = null;
            switch (roadTreeType)
            {
                case NextRoadTreeType.NextTree:
                    tree = nearlyNode.nexts;
                    break;
                case NextRoadTreeType.PriorTree:
                    tree = nearlyNode.priors;
                    break;
                default:
                    break;
            }
            int treeLen = tree.Count();
            return tree.ElementAt(UnityEngine.Random.Range(0, treeLen));
        }
        return nearlyNode;
    }
    public NextRoadTreeType GetTreeDirection(RoadPointNode current, RoadPointNode targetNode)
    {
        if (current.IsExistTheNodeInNextTree(targetNode))
        {
            return NextRoadTreeType.NextTree;
        }
        if (current.IsExistTheNodeInPriorTree(targetNode))
        {
            return NextRoadTreeType.PriorTree;
        }
        return NextRoadTreeType.None;
    }
    public bool NextCostLowerThanCurrent(out NextRoadTreeType roadTreeType, RoadPointNode current, RoadPointNode targetNode)
    {
        RoadPointNode lowerPoint;
        float currentToTarget = Vector3.Distance(targetNode.point.position, current.point.position);
        roadTreeType = GetTreeDirection(current, targetNode);
        IEnumerable<RoadPointNode> tree = null;
        switch (roadTreeType)
        {
            case NextRoadTreeType.NextTree:
                tree = current.nexts;
                break;
            case NextRoadTreeType.PriorTree:
                tree = current.priors;
                break;
            default:
                break;
        }
        foreach (var node in tree)
        {
            var dis = Vector3.Distance(targetNode.point.position, node.point.position);
            if (dis < currentToTarget)
            {
                lowerPoint = node;
                return true;
            }
        }
        return false;
    }
}
