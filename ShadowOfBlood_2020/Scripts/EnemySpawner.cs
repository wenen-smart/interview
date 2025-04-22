using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

using Unity.Mathematics;
using UnityEngine.UIElements;

public class EnemySpawner : MonoBehaviour
{
	private float x, z;
	private float xmax;
	private float xmin;
	private float zmax; 
	private float zmin;

	private Unity.Mathematics.Random random;
	[Header("Enemy Spawn Info")]
	public bool spawnEnemies = true;
	
	public float enemySpawnRadius = 15f;
	public GameObject enemyPrefab;

	[Header("Enemy Spawn Timing")]
	[Range(1, 100)] public int spawnsPerInterval = 1;
	[Range(.1f, 2f)] public float spawnInterval = 1f;
	
	EntityManager manager;
	//Entity enemyEntityPrefab;

	float cooldown;


	void Start()
	{
		
			var settings = GameObjectConversionSettings.FromWorld(World.DefaultGameObjectInjectionWorld, null);
			manager = World.DefaultGameObjectInjectionWorld.EntityManager;
		random = new Unity.Mathematics.Random(45);
	}

	void Update()
    {
		if (!spawnEnemies || Settings.IsPlayerDead())
			return;

		cooldown -= Time.deltaTime;

		if (cooldown <= 0f)
		{
			
				cooldown += spawnInterval;
				Spawn();
				
			
		}
    }

	void Spawn()
	{
		
			for (int i = 0; i < spawnsPerInterval; i++)
			{
				
					Vector3 pos2=new float3(0,0,0);
					//Vector3 pos = Settings.GetPositionAroundPlayer(enemySpawnRadius);
				Settings.GetMax(enemySpawnRadius, ref xmax, ref xmin, ref zmax, ref zmin);

					
				//if (xmax > 50)
    //            {
				//	 x = random.NextFloat(-50, xmin);

				//}
				//if (xmin < -50)
				//{
				//	 x = random.NextFloat(xmax, 50);

				//}
				//if (zmax > 50)
				//{
				//	 z = random.NextFloat(-50, zmin);

				//}
				//if (zmin < -50)
				//{
				//	 z = random.NextFloat(zmax, 50);

				//}
				pos2 = new float3(x, random.NextFloat(0.2f, 1), z);
				//if (xmin > -50&& zmax < 50&& xmin > -50&& xmax < 50)
    //            {
				//	do { pos2 = new float3(random.NextFloat(-50, 50), random.NextFloat(0, 0.8f), random.NextFloat(-50, 50)); }
				//	while ((pos2.x < xmax && pos2.x > xmin) || (pos2.z < zmax && pos2.z > zmin));

    //            }



				


				// ConversionE.prefabEntity

				Entity enemy = manager.Instantiate(enemyPrefab.GetComponent<ConversionE>().prefabEntity);
					manager.SetComponentData(enemy, new Translation { Value = pos2 });
					Settings.enemyNumber+=1;
				Debug.Log(Settings.enemyNumber);
				
          
            
			}
		
	}
}



