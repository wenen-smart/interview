using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

using Unity.Mathematics;




public class EnemySpawner : MonoBehaviour
{
	[Header("Enemy Spawn Info")]
	public bool spawnEnemies = true;
	
	public float enemySpawnRadius = 10f;
	public GameObject enemyPrefab;

	[Header("Enemy Spawn Timing")]
	[Range(1, 100)] public int spawnsPerInterval = 1;
	[Range(.1f, 2f)] public float spawnInterval = 1f;
	
	EntityManager manager;
	Entity enemyEntityPrefab;

	float cooldown;


	void Start()
	{
		BlobAssetStore blobAssetStore = new BlobAssetStore();
			var settings = GameObjectConversionSettings.FromWorld(World.DefaultGameObjectInjectionWorld, blobAssetStore);
			manager = World.DefaultGameObjectInjectionWorld.EntityManager;
		enemyEntityPrefab = GameObjectConversionUtility.ConvertGameObjectHierarchy(enemyPrefab, settings);
		//enemyEntityPrefab = ;
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
			float3 pos = Settings. GetPositionAroundPlayer(enemySpawnRadius);
			pos.y = enemyPrefab.GetComponent<EnemyBehaviour>().posY;




			/*Entity enemy = manager.Instantiate(enemyEntityPrefab);*/
			GameObject enemy= Instantiate(enemyPrefab);
			enemy.transform.position = pos;
			/*manager.SetComponentData(enemy, new Translation { Value = pos });*/

		}
	}
}



