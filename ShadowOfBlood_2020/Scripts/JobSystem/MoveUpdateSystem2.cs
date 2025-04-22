using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.Burst;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public class MoveUpdateSystem : ComponentSystem
{
    EntityQuery entityQuery;
    int gridSize;

    protected override void OnCreate()
    {
        entityQuery = GetEntityQuery(ComponentType.ReadOnly<EnemyTag>());
        gridSize = PathManager.Instance2.girdSize;
    }
    protected override void OnUpdate()
    {
        //if (gridSize <= 6 && entityQuery.CalculateEntityCount() > (3 * gridSize - 2) * 500)
        //{
        //    PathManager.Instance2.girdSize += 1;
        //    this.gridSize = PathManager.Instance2.girdSize;
        //    PathManager.Instance2.DynamicWriteMap();

        //}
        Entities.WithNone<PlayerTag>().WithAll<EnemyTag>().ForEach((Entity entity, ref Translation translation) =>
        {

            int2 targetEnd = new int2(int.MaxValue, int.MaxValue);
            int2 currentPos = new int2(Mathf.RoundToInt(translation.Value.x), Mathf.RoundToInt(translation.Value.z));

            

            //                 if(math.distance(player.Value, recordPlayerLastPosData.lastPos)>2f)
            //                 {
            //                     recordPlayerLastPosData.lastPos = player.Value;
            //                     if (gridSize<=6&&entityQuery.CalculateEntityCount()>(3*gridSize-2)*500)
            //                     {
            //                         PathManager.Instance.girdSize += 1;
            //                         this.gridSize = PathManager.Instance.girdSize;
            //                         PathManager.Instance.DynamicWriteMap();
            //                        
            //                     } 
            //                 }
            //                 else
            //                 {
            //                     return;
            //                 }

            Entities.WithNone<EnemyTag>().WithAnyReadOnly<PlayerTag>().ForEach((ref Translation player, ref RecordPlayerLastPosData recordPlayerLastPosData) =>
            {
                int2 target = (int2)math.round(new float2(player.Value.x, player.Value.z));
                /*targetEnd = target;*/
                if (PathManager.Instance2.IsPositionInsideGrid(target))
                {
                    //                     if (targetEnd.x != int.MaxValue && targetEnd.y != int.MaxValue)
                    //                     {

                    int2 offset = target - currentPos;
                    int startX = currentPos.x, startY = currentPos.y;

                    if (math.length(offset) < 16f)//控制怪在什么范围会追击玩家
                    {
                        if (EntityManager.HasComponent<AutoPartolTagComponent>(entity))
                        {
                        EntityManager.RemoveComponent<AutoPartolTagComponent>(entity);
                        }
                        EntityManager.AddComponentData<TargetPointData>(entity, new TargetPointData
                        {
                            startPosition = new int2 { x = startX, y = startY },

                            endPosition = target

                        });
                    }
                    else
                    {
                        if (!EntityManager.HasComponent<AutoPartolTagComponent>(entity))
                        {
                            EntityManager.AddComponent<AutoPartolTagComponent>(entity);
                        }
                    }
                    /* }*/

                }
                else
                {
                    //TODo
                }
            });



        });

    }

}
