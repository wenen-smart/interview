using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public class PathFollowSystem : JobComponentSystem
{
    //     public struct FollowJob : IJob
    //     {
    //         /* public int pathIndex;*/
    //         public PathIndex followPathIndex;
    //         /*public float3 target;*/
    //         /*public int2 nextPath;*/
    //         public Entity entity;
    //         public Rotation rotation;
    //         public Translation translation;
    //         //         public int moveSpeed;
    //         //         public int angleSpeed;
    //         public DynamicBuffer<PathBuffPosition> buffPosition;
    //         public float deltaTime;
    //         public void Execute()
    //         {
    //             
    //             if (followPathIndex.pathIndex >= 0)
    //             {
    //                 int2 targetPath = buffPosition[followPathIndex.pathIndex].pathPos;
    //                 float3 targetPos = new float3 { x = targetPath.x, y = 0, z = targetPath.y };
    //                 /*float3 moveDir = math.normalize(targetPos - translation.Value);*/
    //                 /*Debug.Log("dir :" + moveDir);*/
    //                 float3 lookdir = math.normalizesafe(targetPos - translation.Value);
    //                 float agularSpeed = 80f;
    //                 rotation.Value = math.slerp(rotation.Value, quaternion.LookRotation(lookdir, Vector3.up), deltaTime * agularSpeed);
    //                 /*Debug.Log("safeDir:" + lookdir);*/
    //                 float moveSpeed = 3f;
    //                 /*translation.Value += moveDir * moveSpeed * Time.DeltaTime;*/
    //                 translation.Value += lookdir * moveSpeed * deltaTime;
    //                 float distance = math.distance(translation.Value, targetPos);
    //                 if (distance < .1f)
    //                 {
    // 
    //                     Debug.Log(targetPos);
    //                     followPathIndex.pathIndex--;
    //                 }
    //             }
    //         }
    //     }

    protected override void OnCreate()
    {

    }

    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        float deltaTime = Time.DeltaTime;
        int2 mapSize = PathManager.Instance.MapSize;
        JobHandle jobHandle = Entities.WithAll<ZombieTag>().ForEach((DynamicBuffer<PathBuffPosition> buffPos, ref Translation translation, ref Rotation rotation, ref PointIndexData pathFollow) =>
          {
              if (pathFollow.pathIndex >= 0)
              {
                  int2 targetPath = buffPos[pathFollow.pathIndex].pathPos;
                  float3 targetPos = new float3 { x = targetPath.x, y = 0, z = targetPath.y };
                //                 float3 moveDir = math.normalize(targetPos - translation.Value);
                //                 Debug.Log("dir :" + moveDir);
                float3 lookdir = math.normalizesafe(targetPos - translation.Value);
                  float agularSpeed = 20f;

                /* Debug.Log("safeDir:" + lookdir);*/
                  float moveSpeed = 3f;
                /*translation.Value += moveDir * moveSpeed * Time.DeltaTime;*/
                  translation.Value += lookdir * moveSpeed * deltaTime;
                  rotation.Value = math.slerp(rotation.Value, quaternion.LookRotationSafe(lookdir, (float3)Vector3.up), deltaTime * agularSpeed);
                  float distance = math.distance(translation.Value, targetPos);
                  
                      
                      if (distance < .1f)
                      {
                          /* Debug.Log(targetPos);*/
                         
                        
                              pathFollow.pathIndex--;
                       
                      }


              }

          }).Schedule(inputDeps);

        return jobHandle;


    }
   

    //     Entities.WithAll<ZombieTag>().ForEach((Entity entity, DynamicBuffer<PathBuffPosition> buffPos, ref Translation translation, ref Rotation rotation, ref PathIndex pathFollow) =>
    //         {
    //             FollowJob followJob = new FollowJob
    //             {
    //                 followPathIndex = pathFollow,
    //                 rotation = rotation,
    //                 translation = translation,
    //                 buffPosition = buffPos,
    //                 deltaTime = Time.DeltaTime
    //             };
    //             followJob.Run();
    //         });
    //     }
    //     

}
