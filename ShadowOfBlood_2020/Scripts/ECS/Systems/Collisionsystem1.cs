using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;
using Unity.Collections;
using Unity.Physics;
using Unity.Physics.Systems;
using Unity.Entities;
using Unity.Jobs;
using Unity.Transforms;

public class CollisionBE : JobComponentSystem
{
    private BeginInitializationEntityCommandBufferSystem bufferSystem;
    private BuildPhysicsWorld buildPhysicsWorld;
    private StepPhysicsWorld stepPhysicsWorld;
    protected override void OnCreate()
    {
        base.OnCreate();
        bufferSystem = World.GetOrCreateSystem<BeginInitializationEntityCommandBufferSystem>();
        buildPhysicsWorld = World.GetOrCreateSystem<BuildPhysicsWorld>();
        stepPhysicsWorld = World.GetOrCreateSystem<StepPhysicsWorld>();
    }
    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        

       TriggerJob triggerJob = new TriggerJob
        {
            Bullet = GetComponentDataFromEntity<MoveForward>(),
            Enemy = GetComponentDataFromEntity<EnemyTag>(),
            entityCommandBuffer = bufferSystem.CreateCommandBuffer()

        };
        JobHandle jobHandle = triggerJob.Schedule(stepPhysicsWorld.Simulation, ref buildPhysicsWorld.PhysicsWorld, inputDeps);
        jobHandle.Complete();
        return jobHandle;
    }

    private struct TriggerJob : ITriggerEventsJob
    {
        [ReadOnly] public ComponentDataFromEntity<MoveForward> Bullet;
       
        [ReadOnly] public ComponentDataFromEntity<EnemyTag> Enemy;
        public EntityCommandBuffer entityCommandBuffer;
        public void Execute(TriggerEvent triggerEvent)
        {
            
            if (Bullet.HasComponent(triggerEvent.EntityA))
            {
                if (Enemy.HasComponent(triggerEvent.EntityB))
                {
                    Debug.Log("poooooA!");
                    entityCommandBuffer.AddComponent(triggerEvent.EntityB, new Fxtag());
                    }
            }
            if (Bullet.HasComponent(triggerEvent.EntityB))
            {
                if (Enemy.HasComponent(triggerEvent.EntityA))
                {
                    Debug.Log("poooooB!");
                }
            }
        }
    }

}
