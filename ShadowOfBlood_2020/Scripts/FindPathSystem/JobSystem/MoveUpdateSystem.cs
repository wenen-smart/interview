using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public class MoveUpdateSystem : ComponentSystem
{

    protected override void OnUpdate()
    {

        Entities.ForEach((ref Translation player, ref PlayerTag playerTag,ref RecordPlayerLastPosData recordPlayerLastPosData ) =>
        {
            int2 targetEnd = new int2(int.MaxValue, int.MaxValue);
            /*float3 roundPos = Settings.GetPositionAroundPlayer(1.5f);*/
            int2 target = (int2)math.round(new float2(player.Value.x, player.Value.z));
            if (PathManager.Instance.IsPositionInsideGrid(target))
            {
                if(math.distance(player.Value, recordPlayerLastPosData.lastPos)>5f)
                {
                    recordPlayerLastPosData.lastPos = player.Value;
                }
                else
                {
                    return;
                }
                targetEnd = target;
                Entities.WithAll<ZombieTag>().ForEach((Entity entity, ref Translation translation) =>
                {
                    if (targetEnd.x != int.MaxValue && targetEnd.y != int.MaxValue)
                    {
                        int2 currentPos = new int2(Mathf.RoundToInt(translation.Value.x), Mathf.RoundToInt(translation.Value.z));
                        int2 offset = targetEnd - currentPos;
                        int startX = currentPos.x, startY = currentPos.y;
                        if (offset.x != 0)
                        {
                            startX += offset.x / math.abs(offset.x);
                        }
                        if (offset.y != 0)
                        {
                            startX += offset.y / math.abs(offset.y);
                        }
                        EntityManager.AddComponentData<TargetPointData>(entity, new TargetPointData
                        {
                            startPosition = new int2 { x = startX, y = startY },

                            endPosition = targetEnd

                        });
                    }

                });
            }

        });
//         if (Input.GetKeyDown(KeyCode.X))
//         {
// 
// 
//             int2 targetEnd = new int2(int.MaxValue, int.MaxValue);
//             Entities.WithAll<ZombieTag>().ForEach((Entity entity, ref Translation translation) =>
//             {
// 
//                 Entities.ForEach((ref Translation player, ref PlayerTag playerTag) =>
//                 {
//                     /*float3 roundPos = Settings.GetPositionAroundPlayer(1.5f);*/
//                     int2 target = (int2)math.round(new float2(player.Value.x, player.Value.z));
//                     if (PathManager.Instance.IsPositionInsideGrid(target))
//                     {
//                       
//                         targetEnd = target;
//                     }
//                    
//                 }
//                 );
//                 if (targetEnd.x != int.MaxValue && targetEnd.y != int.MaxValue)
//                 {
//                     int2 currentPos = new int2(Mathf.RoundToInt(translation.Value.x), Mathf.RoundToInt(translation.Value.z));
//                     int2 offset = targetEnd - currentPos;
//                     int startX= currentPos.x, startY= currentPos.y;
//                     if (offset.x!=0)
//                     {
//                         startX += offset.x / math.abs(offset.x);
//                     }
//                     if (offset.y!=0)
//                     {
//                         startX += offset.y / math.abs(offset.y);
//                     }
//                     EntityManager.AddComponentData<TargetPointData>(entity, new TargetPointData
//                     {
//                         startPosition = new int2 { x=startX,y=startY},
// 
//                         endPosition = targetEnd
// 
//                     });
//                 }
// 
//             });
//         }
        
    }
//     public struct SetPathTargerJob : IJob
//     {
//         public void Execute()
//         {
//             throw new System.NotImplementedException();
//         }
//     }
}
