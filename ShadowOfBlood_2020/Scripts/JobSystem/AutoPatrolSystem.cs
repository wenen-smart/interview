using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
[DisableAutoCreation]
public class AutoPatrolSystem : JobComponentSystem
{                       
    EntityQuery entityQuery;
    
    protected override void OnCreate()
    {
        entityQuery = GetEntityQuery(ComponentType.ReadOnly<EnemyTag>());
    }
    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        float T = 190f,w=0,v=0,radius=50;
        if (T > 0)
        {
            w = 2 * math.PI / T;
        }
        v = w * radius;
        float dt = Time.DeltaTime;
     JobHandle jobHandle=   Entities.WithAll<AutoPartolTagComponent>().ForEach((Entity entity, ref Translation translation, ref Rotation rotation,ref RecordRotateData rrd) =>
        {
            float angle = w * (180 / math.PI);



            translation.Value +=math.forward(rotation.Value)*dt*v;

            //v=wr=2pair n
            rotation.Value = quaternion.RotateY(rrd.angle);
            rrd.angle += dt * angle;
        }
        ).Schedule(inputDeps);
        return jobHandle;
    }

	//    public struct PartolJob : IJobChunk
	//    {
	//        [Unity.Collections.ReadOnly] public ArchetypeChunkEntityType entityChunkType;
	//        //         public ArchetypeChunkComponentType<Translation> translationChunkType;
	//        //         public ArchetypeChunkComponentType<Rotation> rotationChunkType;
	//        [Unity.Collections.ReadOnly] public float deltaTime;
	//        [Unity.Collections.ReadOnly] public float radius;
	//        [NativeDisableContainerSafetyRestriction]
	//        public ComponentDataFromEntity<Translation> translationArray;
	//        [NativeDisableContainerSafetyRestriction]
	//        public ComponentDataFromEntity<Rotation> rotationArray;
	//        /*[Unity.Collections.ReadOnly] public float toForwardSpeed;*/
	//        [Unity.Collections.ReadOnly] public float T;
	//            public void Execute(ArchetypeChunk chunk, int chunkIndex, int firstEntityIndex)
	//            {
	//                var variable = chunk.GetNativeArray(entityChunkType);

	//                unsafe { 
	//                for (int i = 0; i < variable.Length; i++)
	//                {
	//                    var enemyentity = variable[i];
	//                    Translation translationEnemy = translationArray[enemyentity];
	//                        Translation* translation = &translationEnemy;

	//                    var rotationEnemy = rotationArray[enemyentity];
	//                        Rotation* rotation = &rotationArray[enemyentity];
	//                        float w = 0;
	//                    if (T>0)
	//                    {
	//                        w = 2 * math.PI / T;
	//                    }
	//                    float v = w * radius;

	//                    float angle = w * (180 / math.PI);

	//                        translation->Value  = math.lerp(translationEnemy.Value, translationEnemy.Value+math.forward(rotationEnemy.Value)*10,deltaTime* v);

	//                        //v=wr=2pair n
	//                        rotation->Value = math.slerp(rotationEnemy.Value,rotationEnemy.Value.value+quaternion.RotateY(angle).value,deltaTime*2);

	//                }
	//                }
	////             var translationArray = chunk.GetNativeArray<Translation>(translationChunkType);
	////             var rotationArray = chunk.GetNativeArray<Rotation>(rotationChunkType);
	////             for (int i = 0; i < translationArray.Length; i++)
	////             {
	////                var enemyTranslation= translationArray[i];
	////                 enemyTranslation.Value += 
	////                 
	////             }
	////             for (int i = 0; i < rotationArray.Length; i++)
	////             {
	////                 var enemyRotation = rotationArray[i];
	////             }
	//        }
	//    }
}
