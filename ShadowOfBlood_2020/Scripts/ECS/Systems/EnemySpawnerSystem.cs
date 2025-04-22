using Unity.Transforms;
using Unity.Entities;
using Unity.Mathematics;

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
// 		Timer -= Time.DeltaTime;
// 		if (Timer <= 0f)
// 		{
// 			Timer = 0.1f;
// 		
// 				Entity SpawnedEntity = EntityManager.Instantiate(ConversionE.prefabEntity);
// 
// 				EntityManager.SetComponentData(SpawnedEntity,
// 					new Translation { Value = new float3(random.NextFloat(-5f, 5f), random.NextFloat(0f, 5f), random.NextFloat(-5f, 5f)) }
// 
// 				
// 			);
// 		}
	}
}
