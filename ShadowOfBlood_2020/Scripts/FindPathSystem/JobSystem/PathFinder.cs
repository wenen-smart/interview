using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public class PathFinder : ComponentSystem
{
    private const int MOVE_DIAGONAL_COST = 14;
    private const int MOVE_STRAIGHT_COST = 10;
    
    public int pathIndex;
    public float headY;
    
//     private void Start()
//     {
//         headY= transform.position.y;
//     }
//     private void Update()
//     {
//         if (path.Length>0)
//         {
//             if (pathIndex >= path.Length - 1)
//             {
//                 pathIndex = path.Length - 2;
//             }
//             else if (pathIndex < 0)
//             {
//                 return;
//             }
//             float3 targetPos = new float3(path[pathIndex].x, headY, path[pathIndex].y);
//             transform.position = Vector3.MoveTowards(transform.position, targetPos, Time.deltaTime*2);
//             if (math.distance(transform.position,(Vector3)targetPos)<=0.15f)
//             {
//                 transform.position = targetPos;
//                 pathIndex--;
//                 Debug.Log("222");
//             }
//         }
//     }
    protected  override void OnUpdate()
    {
        int mapHeight = PathManager.Instance.MapHeight;
        int mapWidth = PathManager.Instance.MapWidth;
        int2 girdSize = new int2(mapHeight, mapWidth);
        NativeArray<Point> pathNodeArray=GetGirdArray();
        NativeList<JobHandle> jobHandleList = new NativeList<JobHandle>(Allocator.Temp);
        List<FindPathJob> findJobList = new List<FindPathJob>();
        Entities.ForEach((Entity entity,ref TargetPointData findingParams) =>
        {
            
            /*Debug.Log("find");*/
            NativeArray<Point> tmpPathNodeArray = new NativeArray<Point>(pathNodeArray, Allocator.TempJob);
            FindPathJob findPathJob = new FindPathJob
            {
                start = findingParams.startPosition,
                end = findingParams.endPosition,
                mapSize = girdSize,
                pathNodeList = tmpPathNodeArray,
                
                
                entity = entity


            };
            findJobList.Add(findPathJob);
            jobHandleList.Add(findPathJob.Schedule());




            PostUpdateCommands.RemoveComponent<TargetPointData>(entity);
        }
        ) ;
        JobHandle.CompleteAll(jobHandleList);

        for (int i = 0; i < findJobList.Count; i++)
        {
            FindPathJob findJob = findJobList[i];


            SetBuffJob bufferJob = new SetBuffJob()
            {
                entity = findJob.entity,
                pathBuffChunk = GetBufferFromEntity<PathBuffPosition>(),
                pathIndexComponent = GetComponentDataFromEntity<PointIndexData>(),
                pathNodeList = findJob.pathNodeList,
                pathfindingParams = GetComponentDataFromEntity<TargetPointData>(),
                mapSize = girdSize
            };
            bufferJob.Run();
        }

        
        pathNodeArray.Dispose();
    }
    [BurstCompile]
    public struct SetBuffJob : IJob
    {
        [DeallocateOnJobCompletion]
        public NativeArray<Point> pathNodeList;
        public ComponentDataFromEntity<TargetPointData> pathfindingParams;
        public ComponentDataFromEntity<PointIndexData> pathIndexComponent;
        public Entity entity;
        /*public int mapWidth;*/
        public int2 mapSize;
        public BufferFromEntity<PathBuffPosition> pathBuffChunk;
        public void Execute()
        {
           DynamicBuffer<PathBuffPosition> pathBuffPosition = pathBuffChunk[entity];
            TargetPointData param = pathfindingParams[entity];
            int endIndex = CalculatePosToAarryIndex(param.endPosition.x, param.endPosition.y, mapSize);
            Point endNode = pathNodeList[endIndex];
            if (endNode.parentIndex == -1)
            {
                // Didn't find a path!
                Debug.Log("Didn't find a path!");
                pathIndexComponent[entity] = new PointIndexData { pathIndex = -1 };
            }
            else
            {
                // Found a path
                /*Debug.Log("Found a path");*/

                CalculatePath(pathNodeList, endNode, pathBuffPosition, mapSize);
                pathIndexComponent[entity] = new PointIndexData { pathIndex = pathBuffPosition.Length - 1 };

                //                 foreach (int2 pathPosition in pathBuffPosition)
                //                 {
                //                     Debug.Log(pathPosition);
                //                 }


            }
           
        }
    }
    private NativeArray<Point> GetGirdArray()
    {
        int mapHeight = PathManager.Instance.MapHeight;
        int mapWidth = PathManager.Instance.MapWidth;
        int2 mapSize = new int2(mapHeight, mapWidth);

        NativeArray<Point> pathNodeList = new NativeArray<Point>(mapHeight * mapWidth, Allocator.Temp);

        for (int x = 0; x < mapWidth; x++)
        {

            for (int y = 0; y < mapHeight; y++)
            {

                Point point = new Point
                {

                    g = int.MaxValue,
                    index = posIndexConvertArrayIndex(x, y, mapSize.x),
                    
                    parentIndex = -1
                };
                point.x = x-mapSize.x/2;
                point.y = y-mapSize.y/2;
                point.isObstacle = PathManager.Instance.map.GetISObstacleInPoint(new int2(x, y));
                point.CalculateF();
                pathNodeList[point.index] = point;

            }
        }
        return pathNodeList;
    }
    [BurstCompile]
    public struct FindPathJob : IJob
    {
        public int2 start;
        public int2 end;
        
        public NativeArray<Point> pathNodeList;
        public int2 mapSize;
//         [NativeDisableUnsafePtrRestriction]
//         public BufferFromEntity<PathBuffPosition> pathBuffPosition;
        public Entity entity;
        
       

        public void Execute()
        {
            for (int i = 0; i < pathNodeList.Length; i++)
            {
                Point pathNode = pathNodeList[i];
                pathNode.h = CalculateDistanceCost(new int2(pathNode.x, pathNode.y), end);
                pathNodeList[i] = pathNode;
            }
            
            //         NativeArray<int2> neighbourOffsetArray = new NativeArray<int2>(8, Allocator.Temp);
            //         neighbourOffsetArray[0] = new int2(-1, 0); // Left
            //         neighbourOffsetArray[1] = new int2(+1, 0); // Right
            //         neighbourOffsetArray[2] = new int2(0, +1); // Up
            //         neighbourOffsetArray[3] = new int2(0, -1); // Down
            //         neighbourOffsetArray[4] = new int2(-1, -1); // Left Down
            //         neighbourOffsetArray[5] = new int2(-1, +1); // Left Up
            //         neighbourOffsetArray[6] = new int2(+1, -1); // Right Down
            //         neighbourOffsetArray[7] = new int2(+1, +1); // Right Up

            Point startNode = pathNodeList[CalculatePosToAarryIndex(start.x, start.y, mapSize.x)];
           
            startNode.g = 0;
            startNode.CalculateF();
            pathNodeList[startNode.index] = startNode;
            NativeList<int> openList = new NativeList<int>(Allocator.Temp);
            NativeList<int> closeList = new NativeList<int>(Allocator.Temp);
            openList.Add(startNode.index);


            int endIndex = CalculatePosToAarryIndex(end.x, end.y, mapSize.x);
            while (openList.Length > 0)
            {

                int minPathNodeIndex = GetLowestCostFNodeIndex(openList, pathNodeList);
                Point currentNode = pathNodeList[minPathNodeIndex];
                int2 currentNodePos = new int2(currentNode.x, currentNode.y);
                if (minPathNodeIndex == endIndex)
                {
                    break;
                }

                for (int i = 0; i < openList.Length; i++)
                {
                    if (openList[i] == minPathNodeIndex)
                    {
                        openList.RemoveAtSwapBack(i);
                        break;
                    }
                }
                closeList.Add(minPathNodeIndex);
                NativeList<int> aroundIndexList = new NativeList<int>(Allocator.Temp);
                aroundIndexList= GetRoundList(minPathNodeIndex, mapSize, pathNodeList);
                for (int i = 0; i < aroundIndexList.Length; i++)
                {
                    if (closeList.Contains(aroundIndexList[i]))
                    {
                        aroundIndexList.RemoveAtSwapBack(i);
                    }
                }
                for (int i = 0; i < aroundIndexList.Length; i++)
                {
                    /*Debug.Log(aroundIndexList[i] + "index");*/
                    if (closeList.Contains(aroundIndexList[i]))
                    {
                        /* Debug.Log("666");*/
                        continue;
                    }

                    Point neighbourNode = pathNodeList[aroundIndexList[i]];
                    int2 neighbourPosition = new int2(neighbourNode.x, neighbourNode.y);

                    /*                Debug.Log(777);*/
                    float nowG = currentNode.g + CalculateDistanceCost(currentNodePos, neighbourPosition);
                    if (nowG < neighbourNode.g)
                    {

                        neighbourNode.parentIndex = currentNode.index;
                        neighbourNode.g = nowG;
                        neighbourNode.CalculateF();
                        pathNodeList[aroundIndexList[i]] = neighbourNode;
                        if (!openList.Contains(neighbourNode.index))
                        {
                            openList.Add(neighbourNode.index);
                        }
                    }


                }
                //             for (int i = 0; i < neighbourOffsetArray.Length; i++)
                //             {
                //                 int2 neighbourOffset = neighbourOffsetArray[i];
                //                 int2 neighbourPosition = new int2(currentNode.x + neighbourOffset.x, currentNode.y + neighbourOffset.y);
                // 
                //                 if (!IsPositionInsideGrid(neighbourPosition, mapSize))
                //                 {
                //                     // Neighbour not valid position
                //                     continue;
                //                 }
                // 
                //                 int neighbourNodeIndex = posIndexConvertArrayIndex(neighbourPosition.x, neighbourPosition.y, mapSize.x);
                // 
                //                 if (closeList.Contains(neighbourNodeIndex))
                //                 {
                //                     // Already searched this node
                //                     continue;
                //                 }
                // 
                //                 Point neighbourNode = pathNodeList[neighbourNodeIndex];
                //                 if (neighbourNode.isObstacle)
                //                 {
                //                     // Not walkable
                //                     continue;
                //                 }
                // 
                //                 int2 currentNodePosition = new int2(currentNode.x, currentNode.y);
                // 
                //                 int tentativeGCost = currentNode.g + CalculateDistanceCost(currentNodePosition, neighbourPosition);
                //                 if (tentativeGCost < neighbourNode.g)
                //                 {
                //                     neighbourNode.parentIndex = currentNode.index;
                //                     neighbourNode.g = tentativeGCost;
                //                     neighbourNode.CalculateF();
                //                     pathNodeList[neighbourNodeIndex] = neighbourNode;
                // 
                //                     if (!openList.Contains(neighbourNode.index))
                //                     {
                //                         openList.Add(neighbourNode.index);
                //                     }
                //                 }
                // 
                //             }
                aroundIndexList.Dispose();


            }
            

            
            openList.Dispose();
            /* neighbourOffsetArray.Dispose();*/
            closeList.Dispose();



        }

        
    }
    private bool IsPositionInsideGrid(int2 gridPosition, int2 gridSize)
    {
        return
             gridPosition.x >= -gridSize.x / 2 &&
            gridPosition.y >= -gridSize.y / 2 &&
            gridPosition.x < gridSize.x / 2 &&
            gridPosition.y < gridSize.y / 2;
    }
    private static int GetLowestCostFNodeIndex(NativeList<int> openList, NativeArray<Point> pathNodeArray)
    {
        Point lowestCostPathNode = pathNodeArray[openList[0]];
        for (int i = 1; i < openList.Length; i++)
        {
            Point testPathNode = pathNodeArray[openList[i]];
            if (testPathNode.f < lowestCostPathNode.f)
            {
                lowestCostPathNode = testPathNode;
            }
        }
        return lowestCostPathNode.index;
    }
    private NativeList<int2> CalculatePath(NativeArray<Point> pathNodeArray, Point endNode,int2 mapSize)
    {
        if (endNode.parentIndex == -1)
        {
            // Couldn't find a path!
            return new NativeList<int2>(Allocator.Temp);
        }
        else
        {
            // Found a path
            NativeList<int2> path = new NativeList<int2>(Allocator.Temp);
            path.Add(new int2(endNode.x, endNode.y)+ mapSize/2);

            Point currentNode = endNode;
            while (currentNode.parentIndex != -1)
            {
                Point cameFromNode = pathNodeArray[currentNode.parentIndex];
                path.Add(new int2(cameFromNode.x, cameFromNode.y));
                currentNode = cameFromNode;
            }

            return path;
        }
    }
    private static void CalculatePath(NativeArray<Point> pathNodeArray, Point endNode, DynamicBuffer<PathBuffPosition> pathBuff,int2 mapSize)
    {
        if (endNode.parentIndex == -1)
        {
            // Couldn't find a path!
            
        }
        else
        {
            // Found a path
            pathBuff.Clear();
            pathBuff.Add(new PathBuffPosition {pathPos= new int2(endNode.x, endNode.y) });
            Point currentNode = endNode;
            while (currentNode.parentIndex != -1)
            {
                Point cameFromNode = pathNodeArray[currentNode.parentIndex];
                pathBuff.Add(new PathBuffPosition { pathPos = new int2(cameFromNode.x, cameFromNode.y) });
                currentNode = cameFromNode;
            }

        }
    }
    private static int CalculatePosToAarryIndex(int posx,int posY,int2 mapSize)
    {
        return (posx + mapSize.x / 2) + (posY + mapSize.y / 2) * mapSize.x;
    }
    private static int posIndexConvertArrayIndex(int x, int y, int width)
    {
        return x + y * width;
    }
    private static int CalculateDistanceCost(int2 aPosition, int2 bPosition)
    {
        int xDistance = math.abs(aPosition.x - bPosition.x);
        int yDistance = math.abs(aPosition.y - bPosition.y);
        int remaining = math.abs(xDistance - yDistance);
        return MOVE_DIAGONAL_COST * math.min(xDistance, yDistance) + MOVE_STRAIGHT_COST * remaining;
    }
    private static int FindMinFofPoint(NativeList<int> openList, NativeArray<Point> pathpointList)
    {
        int minFPointIndex = 0;
        if (openList.Length > 0)
        {

            for (int i = 0; i < openList.Length; i++)
            {
                int tempmin = openList[i];
                if (pathpointList[tempmin].f < pathpointList[minFPointIndex].f)
                {
                    minFPointIndex = openList[i];
                }
            }
        }
        return minFPointIndex;
    }
    
    private static NativeList<int> GetRoundList(int point, int2 mapSize, NativeArray<Point> pathList)
    {
        int up = -1, down = -1, left = -1, right = -1;
        NativeList<int> aroundIndexList = new NativeList<int>(Allocator.Temp);
//         Action<int> addAroundList = (index) =>
//         {
// 
//             if (pathList[index].isObstacle == false)
//             {
// 
//                 aroundIndexList.Add(index);
//             }
//         };
        
        if (pathList[point].y < mapSize.y/2 - 1)
        {
            /*Debug.Log(pathList[point].y+"up");*/
            up = CalculatePosToAarryIndex(pathList[point].x, pathList[point].y + 1, mapSize);
            if (pathList[up].isObstacle == false)
            {

                aroundIndexList.Add(up);
            }
        }
        if (pathList[point].y > -mapSize.y/2)
        {
            down = CalculatePosToAarryIndex(pathList[point].x, pathList[point].y - 1, mapSize);

            if (pathList[down].isObstacle == false)
            {

                aroundIndexList.Add(down);
            }
        }
        if (pathList[point].x > -mapSize.x/2)
        {
            left = CalculatePosToAarryIndex(pathList[point].x - 1, pathList[point].y, mapSize);
            if (pathList[left].isObstacle == false)
            {

                aroundIndexList.Add(left);
            }
        }
        if (pathList[point].x < mapSize.x/2 - 1)
        {

            right = CalculatePosToAarryIndex(pathList[point].x + 1, pathList[point].y, mapSize);
            if (pathList[right].isObstacle == false)
            {

                aroundIndexList.Add(right);
            }
        }
        int upLeft = -1, upRight = -1, downLeft = -1, downRight = -1;
//         Func<int, int, bool> booleanIsHave = (x, y) =>
//         {
//             return (x != -1 && y != -1);
//         };
        if (up != -1 && left != -1)
        {
            upLeft = CalculatePosToAarryIndex(pathList[point].x - 1, pathList[point].y + 1, mapSize);
            if (pathList[upLeft].isObstacle == false)
            {

                aroundIndexList.Add(upLeft);
            }
        }
        if (up != -1 && right != -1)
        {
            upRight = CalculatePosToAarryIndex(pathList[point].x + 1, pathList[point].y + 1, mapSize);
            if (pathList[upRight].isObstacle == false)
            {

                aroundIndexList.Add(upRight);
            }
        }
        if (down != -1 && left != -1)
        {
            downLeft = CalculatePosToAarryIndex(pathList[point].x - 1, pathList[point].y - 1, mapSize);
            if (pathList[downLeft].isObstacle == false)
            {

                aroundIndexList.Add(downLeft);
            }
        }
        if (down != -1 && right != -1)
        {
            downRight = CalculatePosToAarryIndex(pathList[point].x + 1, pathList[point].y - 1, mapSize);
            if (pathList[downRight].isObstacle == false)
            {

                aroundIndexList.Add(downRight);
            }
        }
        return aroundIndexList;

    }


}
