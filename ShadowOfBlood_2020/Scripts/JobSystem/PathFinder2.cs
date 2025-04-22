using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
//using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.Rendering;

[DisableAutoCreation]
public class PathFinder : JobComponentSystem
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
    private EndSimulationEntityCommandBufferSystem endSimulationEntityCommand;
    EntityQuery enemyGroup;
   
    protected override void OnCreate()
    {
        endSimulationEntityCommand = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
        /* enemyGroup = GetEntityQuery(ComponentType.ReadOnly<TargetPointData>());*/
         enemyGroup = GetEntityQuery(ComponentType.ReadOnly<TargetPointData>(),typeof(PointIndexData));
        
    }
    protected  override JobHandle OnUpdate(JobHandle inputDependencies)
    {
       
        if (enemyGroup.CalculateEntityCount() > 0)
        {


            int mapHeight = PathManager.Instance2.MapHeight;
            int mapWidth = PathManager.Instance2.MapWidth;
          int  girdSize = PathManager.Instance2.girdSize;
            int2 mapSize = new int2( mapWidth, mapHeight);
            int2 arraySize = new int2(mapWidth/girdSize,mapHeight/girdSize);
            NativeArray<Point> pathNodeArray = GetGirdArray(girdSize,arraySize);
            /*NativeList<JobHandle> jobHandleList = new NativeList<JobHandle>(Allocator.Temp);*/
           /* List<FindPathJob> findJobList = new List<FindPathJob>();*/

            /*Debug.Log("find");*/
            NativeArray<Point> tmpPathNodeArray = new NativeArray<Point>(pathNodeArray, Allocator.TempJob);
            /*            var targetPointData = enemyGroup.ToComponentDataArray<TargetPointData>(Allocator.TempJob);*/
            //             var entityGroup = enemyGroup.ToEntityArray(Allocator.TempJob);
            //             var tempEntityGroup = new NativeArray<Entity>(entityGroup, Allocator.TempJob);
            /*  JobHandle tempJobHandle;*/
            /* NativeArray<Entity> entityGroup =  enemyGroup.ToEntityArrayAsync(Allocator.TempJob, out tempJobHandle);*/
            /* Dictionary<Entity, NativeArray<Point>> allPathInfo = new Dictionary<Entity, NativeArray<Point>>();*/
            /* tempJobHandle.Complete();*/
            var pointIndexDataChunk = GetArchetypeChunkComponentType<PointIndexData>();
            var targetPointDataChunk = GetArchetypeChunkComponentType<TargetPointData>();
            var entityChunk = GetArchetypeChunkEntityType();
            /*var pathBuffChunk = GetArchetypeChunkBufferType<PathBuffPosition>(); */
            FindPathJob findPathJob = new FindPathJob
            {
                //                 start = targetPointData[2].startPosition,
                //                 end = targetPointData[2].endPosition,
                mapSize = mapSize,
                globallist = tmpPathNodeArray,
                /* entityArray = entityGroup,*/
                commandBuffer = endSimulationEntityCommand.CreateCommandBuffer().ToConcurrent(),
                /*allJobPathInfo = allPathInfo,*/
                pathBuffChunk = GetBufferFromEntity<PathBuffPosition>(),
                //                 pathIndexComponent = GetComponentDataFromEntity<PointIndexData>(),
                //                 targetComponentArray=GetComponentDataFromEntity<TargetPointData>(),
                entityChunkType = GetArchetypeChunkEntityType(),
                pathIndexChunkType = pointIndexDataChunk,
                targetChunkType = targetPointDataChunk,
                girdSize = girdSize,
                arraySize= arraySize
                /* bufferFromEntity = GetBufferFromEntity<NodePointBuffData>(),*/
                /* translationBuffer = GetComponentDataFromEntity<Translation>()*/
                /*entityChunkType = GetArchetypeChunkEntityType()*/



            };
           /* Debug.Log(allPathInfo.Count);*/
//             SetBuffJob bufferJob = new SetBuffJob()
//             {
//                 pathBuffChunk = GetBufferFromEntity<PathBuffPosition>(),
//                 pathIndexComponent = GetComponentDataFromEntity<PointIndexData>(),
//                 pathNodeList = tmpPathNodeArray,
//                 pathfindingParams = GetComponentDataFromEntity<TargetPointData>(),
//                 tranlation = GetComponentDataFromEntity<Translation>(),
//                 mapSize = girdSize,
//                 entityChunkType = GetArchetypeChunkEntityType(),
//                /* entityArray = tempEntityGroup,*/
//                 commandBuffer = endSimulationEntityCommand.CreateCommandBuffer()
//             };


            JobHandle findJob = findPathJob.Schedule(enemyGroup, inputDependencies);
            findJob.Complete();
            endSimulationEntityCommand.AddJobHandleForProducer(findJob);
            /* ArchetypeChunkIterator aci = enemyGroup.GetArchetypeChunkIterator();*/
            /*bufferJob.RunWithoutJobs<SetBuffJob>(ref aci);*/


            /*  targetPointData.Dispose();*/
            /*entityGroup.Dispose();*/
           
            return findJob;

        }
        return new JobHandle();



    }
    /*[BurstCompile]*/
//     public struct SetBuffJob : IJobChunk
//     {
//         [DeallocateOnJobCompletion]
//         public NativeArray<Point> pathNodeList;
//         [ReadOnly]public ComponentDataFromEntity<TargetPointData> pathfindingParams;
//         [NativeDisableContainerSafetyRestriction]
//         public ComponentDataFromEntity<PointIndexData> pathIndexComponent;
//         /*public ComponentDataFromEntity<Translation> tranlation;*/
//         [ReadOnly]public ComponentDataFromEntity<Translation> tranlation;
// /*        public NativeArray<Entity> entityArray;*/
//         [ReadOnly] public ArchetypeChunkEntityType entityChunkType;
//         /*public NativeArray<Entity> entityArray;*/
//         public EntityCommandBuffer commandBuffer;
// 
//         /*public int mapWidth;*/
//         [ReadOnly]public int2 mapSize;
//       public BufferFromEntity<PathBuffPosition> pathBuffChunk;
//         public void Execute(ArchetypeChunk chunk, int chunkIndex, int firstEntityIndex)
//         {
//             //             var chunkEntityArray = chunk.GetNativeArray(entityChunkType);
//             // 
//             //             for (int e = 0; e < chunkEntityArray.Length; e++)
//             //             {
//             //                 Entity entity = chunkEntityArray[e];
//             //                
//             //                 DynamicBuffer <PathBuffPosition> pathBuffPosition = pathBuffChunk[entity];
//             //                 TargetPointData param = pathfindingParams[entity];
//             //                 int endIndex = CalculatePosToAarryIndex(param.endPosition.x, param.endPosition.y, mapSize);
//             //                 /*DynamicBuffer<PathBuffPosition> pathBuffPosition = pathBuffChunk[entity];*/
//             //                 Point endNode = pathNodeList[endIndex];
//             //                 Translation transform = tranlation[entity];
//             //                 if (endNode.parentIndex == -1)
//             //                 {
//             //                     // Didn't find a path!
//             //                     Debug.Log("Didn't find a path!");
//             //                     pathIndexComponent[entity] = new PointIndexData { pathIndex = -1 };
//             //                 }
//             //                 else
//             //                 {
//             //                     // Found a path
//             //                     /*Debug.Log("Found a path");*/
//             // 
//             //                     CalculatePath(pathNodeList, endNode, pathBuffPosition, mapSize, ref transform,commandBuffer,entity);
//             //                     pathIndexComponent[entity] = new PointIndexData { pathIndex = pathBuffPosition.Length - 1 };
//             // 
//             //                     //                 foreach (int2 pathPosition in pathBuffPosition)
//             //                     //                 {
//             //                     //                     Debug.Log(pathPosition);
//             //                     //                 }
//             // 
//             // 
//             //                 }
//             //                 
//             //             }
//             //           
//             //            
//         }
// 
// 
//     }
    private static NativeArray<Point> GetGirdArray(int girdSize,int2 arraySize)
    {
        int mapHeight = PathManager.Instance2.MapHeight;
        int mapWidth = PathManager.Instance2.MapWidth;
        int2 mapSize = new int2(mapHeight, mapWidth);

        NativeArray<Point> pathNodeList = new NativeArray<Point>(arraySize.x * arraySize.y, Allocator.Temp);

        for (int x = 0; x < arraySize.x; x++)
        {

            for (int y = 0; y < arraySize.y; y++)
            {

                Point point = new Point
                {

                    g = int.MaxValue,
                    index = posIndexConvertArrayIndex(x, y, arraySize.x),
                    
                    parentIndex = -1
                };
                point.x = (x*girdSize-mapSize.x/2);
                point.y = (y*girdSize-mapSize.y/2);
                point.isObstacle = PathManager.Instance2.map.GetISObstacleInPoint(new int2(x, y));
                
                point.CalculateF();
                pathNodeList[point.index] = point;

            }
        }
        return pathNodeList;
    }
    [BurstCompile]
    [RequireComponentTag(typeof(TargetPointData))]
    public struct FindPathJob : IJobChunk
    {
//         [ReadOnly]public int2 start;
//         [ReadOnly]public int2 end;
        /*public NativeArray<TargetPointData> targetParamsArray;*/
//         [NativeDisableContainerSafetyRestriction]
//         public BufferFromEntity<NodePointBuffData> bufferFromEntity;
[DeallocateOnJobCompletion]
        public NativeArray<Point> globallist;  // 无用，但是又必须要有，鬼知道她妈的。多线程应该有锁，访问不了，元素不为空却报空
        [ReadOnly]public int2 mapSize;
        /*[ReadOnly] public NativeArray<Entity> entityArray;*/
        public EntityCommandBuffer.Concurrent commandBuffer;

        /*public ComponentDataFromEntity<Translation> translationBuffer;*/
        [NativeDisableContainerSafetyRestriction]
        public BufferFromEntity<PathBuffPosition> pathBuffChunk;
        /* [NativeDisableContainerSafetyRestriction]*/
        /*public ComponentDataFromEntity<PointIndexData> pathIndexComponent;*/
        /*[ReadOnly] public ComponentDataFromEntity<TargetPointData> targetComponentArray;*/
        [ReadOnly] public ArchetypeChunkComponentType<TargetPointData> targetChunkType;
        public ArchetypeChunkComponentType<PointIndexData> pathIndexChunkType;
/*        public ArchetypeChunkBufferType<PathBuffPosition> pathBuffChunkType;*/
        /*public ComponentDataFromEntity<Translation> tranlation;*/
        /*        [ReadOnly] public ComponentDataFromEntity<Translation> tranlation;*/

        /* public NativeArray<NativeArray<Point>> saveEachJobPath;*/
        [ReadOnly] public ArchetypeChunkEntityType entityChunkType;
        //         [NativeDisableUnsafePtrRestriction]
        //         public BufferFromEntity<PathBuffPosition> pathBuffPosition;
        /* public Entity entity;*/
        [ReadOnly] public int girdSize;
        [ReadOnly] public int2 arraySize;



        public void Execute(ArchetypeChunk chunk, int chunkIndex, int firstEntityIndex)
        {
            var entityChunk = chunk.GetNativeArray(entityChunkType);
            var targetArry = chunk.GetNativeArray(targetChunkType);
            var pathIndexArray = chunk.GetNativeArray(pathIndexChunkType);
            NativeArray<Point> global = GetGirdArray(girdSize,arraySize);

            for (int c = 0; c < chunk.Count; c++)
            {

                NativeArray<Point> pathNodeList = new NativeArray<Point>(global, Allocator.Temp);
                //             for (int e = 0; e < entityChunk.Length; e++)
                //             {
                //             for (int e = 0; e < entityArray.Length; e++)
                //             {
                /* Entity entity = entityChunk[e];*/

                int2 start = targetArry[c].startPosition;
                int2 end= targetArry[c].endPosition;
                
                for (int i = 0; i < pathNodeList.Length; i++)
                {
                    Point pathNode = pathNodeList[i];
                    pathNode.h = CalculateDistanceCost(new int2(pathNode.x, pathNode.y), end);
                    pathNodeList[i] = pathNode;
                }

                NativeArray<int2> neighbourOffsetArray = new NativeArray<int2>(8, Allocator.Temp);
                neighbourOffsetArray[0] = new int2(-girdSize, 0); // Left
                neighbourOffsetArray[1] = new int2(+girdSize, 0); // Right
                neighbourOffsetArray[2] = new int2(0, +girdSize); // Up
                neighbourOffsetArray[3] = new int2(0, -girdSize); // Down
                neighbourOffsetArray[4] = new int2(-girdSize, -girdSize); // Left Down
                neighbourOffsetArray[5] = new int2(-girdSize, +girdSize); // Left Up
                neighbourOffsetArray[6] = new int2(+girdSize, -girdSize); // Right Down
                neighbourOffsetArray[7] = new int2(+girdSize, +girdSize); // Right Up

                //                 Debug.Log(start);
                //                 Debug.Log(mapSize);

                Point startNode = pathNodeList[CalculatePosToAarryIndex(start.x, start.y, mapSize,girdSize,arraySize)];
               
                startNode.g = 0;
                startNode.CalculateF();
                pathNodeList[startNode.index] = startNode;
                NativeList<int> openList = new NativeList<int>(Allocator.Temp);
                NativeList<int> closeList = new NativeList<int>(Allocator.Temp);
                openList.Add(startNode.index);


                int endIndex = CalculatePosToAarryIndex(end.x, end.y, mapSize,girdSize,arraySize);
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
                    //                     NativeList<int> aroundIndexList = new NativeList<int>(Allocator.Temp);
                    //                     aroundIndexList = GetRoundList(minPathNodeIndex, mapSize, pathNodeList);
                    //                     for (int i = 0; i < aroundIndexList.Length; i++)
                    //                     {
                    //                         if (closeList.Contains(aroundIndexList[i]))
                    //                         {
                    //                             aroundIndexList.RemoveAtSwapBack(i);
                    //                         }
                    //                     }
                    //                     for (int i = 0; i < aroundIndexList.Length; i++)
                    //                     {
                    //                         /*Debug.Log(aroundIndexList[i] + "index");*/
                    //                         if (closeList.Contains(aroundIndexList[i]))
                    //                         {
                    //                             /* Debug.Log("666");*/
                    //                             continue;
                    //                         }
                    // 
                    //                         Point neighbourNode = pathNodeList[aroundIndexList[i]];
                    //                         int2 neighbourPosition = new int2(neighbourNode.x, neighbourNode.y);
                    // 
                    //                         /*                Debug.Log(777);*/
                    //                         float nowG = currentNode.g + CalculateDistanceCost(currentNodePos, neighbourPosition);
                    //                         if (nowG < neighbourNode.g)
                    //                         {
                    // 
                    //                             neighbourNode.parentIndex = currentNode.index;
                    //                             neighbourNode.g = nowG;
                    //                             neighbourNode.CalculateF();
                    //                             pathNodeList[aroundIndexList[i]] = neighbourNode;
                    //                             if (!openList.Contains(neighbourNode.index))
                    //                             {
                    //                                 openList.Add(neighbourNode.index);
                    //                             }
                    //                         }
                    // 
                    // 
                    //                     }
                    for (int i = 0; i < neighbourOffsetArray.Length; i++)
                    {
                        int2 neighbourOffset = neighbourOffsetArray[i];
                        int2 neighbourPosition = new int2(currentNode.x + neighbourOffset.x, currentNode.y + neighbourOffset.y);

                        if (!IsPositionInsideGrid(neighbourPosition, mapSize))
                        {
                            // Neighbour not valid position
                            continue;
                        }

                        int neighbourNodeIndex = CalculatePosToAarryIndex(neighbourPosition.x, neighbourPosition.y, mapSize,girdSize,arraySize);

                        if (closeList.Contains(neighbourNodeIndex))
                        {
                            // Already searched this node
                            continue;
                        }

                        Point neighbourNode = pathNodeList[neighbourNodeIndex];
                        if (neighbourNode.isObstacle)
                        {
                            // Not walkable
                            continue;
                        }

                        int2 currentNodePosition = new int2(currentNode.x, currentNode.y);

                        float tentativeGCost = currentNode.g + CalculateDistanceCost(currentNodePosition, neighbourPosition);
                        if (tentativeGCost < neighbourNode.g)
                        {
                            neighbourNode.parentIndex = currentNode.index;
                            neighbourNode.g = tentativeGCost;
                            neighbourNode.CalculateF();
                            pathNodeList[neighbourNodeIndex] = neighbourNode;

                            if (!openList.Contains(neighbourNode.index))
                            {
                                openList.Add(neighbourNode.index);
                            }
                        }

                    }
                    /* aroundIndexList.Dispose();*/
                    if (openList.IndexOf(endIndex)>-1)
                    {
                        break;
                    }

                }


                commandBuffer.RemoveComponent<TargetPointData>(chunkIndex, entityChunk[c]);
                openList.Dispose();
                neighbourOffsetArray.Dispose();
                closeList.Dispose();


                //                 DynamicBuffer<NodePointBuffData> db = bufferFromEntity[entity];
                //                 db.Clear();
                //                 for (int i = 0; i < pathNodeList.Length; i++)
                //                 {
                //                     NodePointBuffData nodePointBuffData = new NodePointBuffData { point = pathNodeList[i] };
                //                     db.Add(nodePointBuffData);
                //                 }
                /* saveEachJobPath[e] = new NativeArray<Point>(pathNodeList,Allocator.TempJob);*/

                DynamicBuffer<PathBuffPosition> pathBuffPosition = pathBuffChunk[entityChunk[c]];
                TargetPointData param = targetArry[c];
                /*int endIndex = CalculatePosToAarryIndex(param.endPosition.x, param.endPosition.y, mapSize);*/
                /*DynamicBuffer<PathBuffPosition> pathBuffPosition = pathBuffChunk[entity];*/
                Point endNode = pathNodeList[endIndex];
                /*Translation transform = tranlation[entity];*/
                if (endNode.parentIndex == -1)
                {
                    // Didn't find a path!
                    Debug.Log("Didn't find a path!");
                    pathIndexArray[c] = new PointIndexData { pathIndex = -1 };
                }
                else
                {
                    // Found a path
                    /*Debug.Log("Found a path");*/

                    CalculatePath(pathNodeList, endNode, pathBuffPosition, mapSize);
                    pathIndexArray[c] = new PointIndexData { pathIndex = pathBuffPosition.Length - 1 };

                    //                 foreach (int2 pathPosition in pathBuffPosition)
                    //                 {
                    //                     Debug.Log(pathPosition);
                    //                 }


                }
            }



        }
    }
    private static bool IsPositionInsideGrid(int2 gridPosition, int2 gridSize)
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
            path.Add(new int2(endNode.x, endNode.y));

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
            /*float2 currentPos = new float2(translation.Value.x, translation.Value.z);*/
            while (currentNode.parentIndex != -1)
            {
                Point cameFromNode = pathNodeArray[currentNode.parentIndex];
                int2 camerfromPos = new int2(cameFromNode.x, cameFromNode.y);
                currentNode = cameFromNode;
                if (cameFromNode.parentIndex == -1)
                {
                    continue;
                }
                pathBuff.Add(new PathBuffPosition { pathPos = camerfromPos });
               
                
            }

        }
    }

//     private static void CalculatePath(NativeArray<Point> pathNodeArray, Point endNode, PathPosList pathBuff, int2 mapSize, [ReadOnly] ref Translation translation)
//     {
//         if (endNode.parentIndex == -1)
//         {
//             // Couldn't find a path!
// 
//         }
//         else
//         {
//             // Found a path
//             pathBuff.buffPosition.Clear();
// 
//              pathBuff.buffPosition.Add(new PathBuffPosition { pathPos = new int2(endNode.x, endNode.y) });
//             Point currentNode = endNode;
//             float2 currentPos = new float2(translation.Value.x, translation.Value.z);
//             while (currentNode.parentIndex != -1)
//             {
//                 Point cameFromNode = pathNodeArray[currentNode.parentIndex];
//                 int2 camerfromPos = new int2(cameFromNode.x, cameFromNode.y);
//                 currentNode = cameFromNode;
//                 if (cameFromNode.parentIndex == -1)
//                 {
//                     continue;
//                 }
//                 pathBuff.Add(new PathBuffPosition { pathPos = camerfromPos });
// 
// 
//             }
// 
//         }
//     }
//     private static int CalculatePosToAarryIndex(int posx,int posY,int2 mapSize)
//     {
//         return  (posx + mapSize.x / 2) + (posY + mapSize.y / 2) * mapSize.x;
//  
//     }
    private static int CalculatePosToAarryIndex(int posx, int posY, int2 mapSize, int girdSize, int2 arraySize)
    {
        return ((posx + mapSize.x / 2) / girdSize) + ((posY + mapSize.y / 2) / girdSize) * arraySize.x;
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
    
    private static NativeList<int> GetRoundList(int point, int2 mapSize, NativeArray<Point> pathList,int girdSize,int2 arraySize)
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
            up = CalculatePosToAarryIndex(pathList[point].x, pathList[point].y + 1, mapSize,girdSize,arraySize);
            if (pathList[up].isObstacle == false)
            {

                aroundIndexList.Add(up);
            }
        }
        if (pathList[point].y > -mapSize.y/2)
        {
            down = CalculatePosToAarryIndex(pathList[point].x, pathList[point].y - 1, mapSize, girdSize,arraySize);

            if (pathList[down].isObstacle == false)
            {

                aroundIndexList.Add(down);
            }
        }
        if (pathList[point].x > -mapSize.x/2)
        {
            left = CalculatePosToAarryIndex(pathList[point].x - 1, pathList[point].y, mapSize,girdSize,arraySize);
            if (pathList[left].isObstacle == false)
            {

                aroundIndexList.Add(left);
            }
        }
        if (pathList[point].x < mapSize.x/2 - 1)
        {

            right = CalculatePosToAarryIndex(pathList[point].x + 1, pathList[point].y, mapSize,girdSize,arraySize);
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
            upLeft = CalculatePosToAarryIndex(pathList[point].x - 1, pathList[point].y + 1, mapSize,girdSize,arraySize);
            if (pathList[upLeft].isObstacle == false)
            {

                aroundIndexList.Add(upLeft);
            }
        }
        if (up != -1 && right != -1)
        {
            upRight = CalculatePosToAarryIndex(pathList[point].x + 1, pathList[point].y + 1, mapSize,girdSize,arraySize);
            if (pathList[upRight].isObstacle == false)
            {

                aroundIndexList.Add(upRight);
            }
        }
        if (down != -1 && left != -1)
        {
            downLeft = CalculatePosToAarryIndex(pathList[point].x - 1, pathList[point].y - 1, mapSize,girdSize,arraySize);
            if (pathList[downLeft].isObstacle == false)
            {

                aroundIndexList.Add(downLeft);
            }
        }
        if (down != -1 && right != -1)
        {
            downRight = CalculatePosToAarryIndex(pathList[point].x + 1, pathList[point].y - 1, mapSize,girdSize,arraySize);
            if (pathList[downRight].isObstacle == false)
            {

                aroundIndexList.Add(downRight);
            }
        }
        return aroundIndexList;

    }


}
