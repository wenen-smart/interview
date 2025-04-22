using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
[UpdateAfter(typeof(PathFinderSingleCareSystem))]
public class PathFollowSystem : ComponentSystem
{
  
   protected override void OnUpdate()
    {
        float deltaTime = Time.DeltaTime;
        int2 mapSize = PathManager.Instance2.MapSize;
        Entities.WithAll<EnemyTag>().ForEach((DynamicBuffer<PathBuffPosition> buffPos, ref Translation translation, ref Rotation rotation, ref PointIndexData pathFollow) =>
          {
              if (pathFollow.pathIndex >= 0)
              {
                  int2 targetPath = buffPos[pathFollow.pathIndex].pathPos;
                  float3 targetPos = new float3 { x = targetPath.x, y = 0, z = targetPath.y };

                  float distance = math.distance(translation.Value, targetPos);
                  if (distance < .15f)
                  {
                    /* Debug.Log(targetPos);*/
                    if (pathFollow.pathIndex <= 1)
                    {
                        pathFollow.pathIndex = -1;
                    }
                    else
                    {
                        pathFollow.pathIndex--;
                      }

                      if (pathFollow.pathIndex <= -1)
                      {
                          buffPos.Clear();
                          return;
                      }
                  }
                  float3 lookdir = math.normalizesafe(targetPos - translation.Value);
                  lookdir.y = 0;
                  float agularSpeed = 20f;  
                  float moveSpeed = 3f;  
                  translation.Value += lookdir * moveSpeed * deltaTime;
                  rotation.Value = math.slerp(rotation.Value, quaternion.LookRotationSafe(lookdir, (float3)Vector3.up), deltaTime * agularSpeed);

              }

          });

      


    }


  

}
