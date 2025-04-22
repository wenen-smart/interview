using Unity.Transforms;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using Random = Unity.Mathematics.Random;

[DisableAutoCreation]
public class EntitySpawnerSystem : ComponentSystem
{
	
	private float Timer;
	private Random random;
	protected override void OnCreate()
	{
		random = new Random(56);
	}
	protected override void OnUpdate()
	{
        Timer -= Time.DeltaTime;
        if (Timer <= 0f)
        {
            Timer = 0.1f;
            GameObject enemy = GameObject.Instantiate(ConversionE.Instance.GameObject);
            enemy.transform.position = new float3(random.NextFloat(-5f, 5f), random.NextFloat(0f, 5f), random.NextFloat(-5f, 5f));

            //             Entity SpawnedEntity = EntityManager.Instantiate(ConversionE.prefabEntity);
            // 
            //             EntityManager.SetComponentData(SpawnedEntity,
            //                 new Translation { Value = new float3(random.NextFloat(-5f, 5f), random.NextFloat(0f, 5f), random.NextFloat(-5f, 5f)) }
            // 
            // 
            //         );
        }
    }
}
