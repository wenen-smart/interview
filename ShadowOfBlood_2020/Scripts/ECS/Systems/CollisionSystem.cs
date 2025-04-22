using System.Threading;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

[UpdateAfter(typeof(MoveForwardSystem))]
[UpdateBefore(typeof(TimedDestroySystem))]
public class CollisionSystem : JobComponentSystem
{
	
	EntityQuery enemyGroup;
	EntityQuery bulletGroup;
	EntityQuery playerGroup;
	public static  float timer1 = 3;// 怪物攻击频率
	protected override void OnCreate()
	{
		playerGroup = GetEntityQuery(typeof(Health), ComponentType.ReadOnly<Translation>(), ComponentType.ReadOnly<PlayerTag>());
		enemyGroup = GetEntityQuery(typeof(Health), ComponentType.ReadOnly<Translation>(), ComponentType.ReadOnly<EnemyTag>());
		bulletGroup = GetEntityQuery(typeof(TimeToLive), ComponentType.ReadOnly<Translation>());
	}
	protected override JobHandle OnUpdate(JobHandle inputDependencies)
	{
		
		timer1 -= Time.DeltaTime;
		var healthType = GetArchetypeChunkComponentType<Health>(false);
		var translationType = GetArchetypeChunkComponentType<Translation>(true);
		//	var bulletsDamageType = GetArchetypeChunkComponentType<Damage>(true);
		float d = Settings.GetBulletdamage;
		float enemyRadius = Settings.EnemyCollisionRadius;
		float playerRadius = Settings.PlayerCollisionRadius;

		var jobEvB = new CollisionJob()
		{
			D = d,
			radius = enemyRadius * enemyRadius,
			healthType = healthType,
			translationType = translationType,
			//damagesType=bulletsDamageType,
			transToTestAgainst = bulletGroup.ToComponentDataArray<Translation>(Allocator.TempJob),
			//damagesAgainst = bulletGroup.ToComponentDataArray<Damage>(Allocator.TempJob)
		};

		JobHandle jobHandle = jobEvB.Schedule(enemyGroup, inputDependencies);

		if  (Settings.IsPlayerDead()){return jobHandle; }
		//返回Job句柄


		CollisionJobPVE jobPvE = new CollisionJobPVE()
		{
			//D = d,
			newtime = timer1,
			    //timer = 3,
				radius = playerRadius * playerRadius,
				healthType = healthType,
				translationType = translationType,
				//damagesType = bulletsDamageType,
				transToTestAgainst = enemyGroup.ToComponentDataArray<Translation>(Allocator.TempJob),
				//damagesAgainst = bulletGroup.ToComponentDataArray<Damage>(Allocator.TempJob)
			};
	
		return  jobPvE.Schedule(playerGroup, jobHandle);
		


	}

	[BurstCompile]
	struct CollisionJob : IJobChunk
	{
		public float radius;
		public float D;
		public ArchetypeChunkComponentType<Health> healthType;
		[ReadOnly] public ArchetypeChunkComponentType<Translation> translationType;
		//[ReadOnly] public ArchetypeChunkComponentType<Damage> damagesType;
		[DeallocateOnJobCompletion]
		[ReadOnly] public NativeArray<Translation> transToTestAgainst;
		//[ReadOnly] public NativeArray<Damage> damagesAgainst;

		public void Execute(ArchetypeChunk chunk, int chunkIndex, int firstEntityIndex)
		{
			var chunkHealths = chunk.GetNativeArray(healthType);
			var chunkTranslations = chunk.GetNativeArray(translationType);
			//var chunkBulletdamage = chunk.GetNativeArray(damagesType);

			for (int i = 0; i < chunk.Count; i++)
			{
				float damage = 0f;
				Health health = chunkHealths[i];
				Translation pos = chunkTranslations[i];
				

				for (int j = 0; j < transToTestAgainst.Length; j++)
				{
					Translation pos2 = transToTestAgainst[j];

					if (CheckCollision(pos.Value, pos2.Value, radius))
					{
						
				//			Damage dam = chunkBulletdamage[0];
							damage += D;

						
					}
				}

				if (damage > 0)
				{
					health.Value -= damage;
					chunkHealths[i] = health;

				}
			}
			
		}
	}
    struct CollisionJobPVE : IJobChunk
    {
		public float  newtime;
        public float radius;
	    public float   timer;
        public ArchetypeChunkComponentType<Health> healthType;
        [ReadOnly] public ArchetypeChunkComponentType<Translation> translationType;
        //[ReadOnly] public ArchetypeChunkComponentType<Damage> damagesType;
        [DeallocateOnJobCompletion]
        [ReadOnly] public NativeArray<Translation> transToTestAgainst;
        //[ReadOnly] public NativeArray<Damage> damagesAgainst;

        public void Execute(ArchetypeChunk chunk, int chunkIndex, int firstEntityIndex)
        {
            var chunkHealths = chunk.GetNativeArray(healthType);
            var chunkTranslations = chunk.GetNativeArray(translationType);
			//var chunkBulletdamage = chunk.GetNativeArray(damagesType);
			timer = newtime;
			for (int i = 0; i < chunk.Count; i++)
            {
                float damage = 0f;
                Health health = chunkHealths[i];
                Translation pos = chunkTranslations[i];


                for (int j = 0; j < transToTestAgainst.Length; j++)
                {
                    Translation pos2 = transToTestAgainst[j];

                    if (CheckCollision(pos.Value, pos2.Value, radius))
                    {
						
							damage += 1;// 怪物伤害都为1
						

                    }
                }

                if (damage > 0&&timer<=0)
                {
                    health.Value -= damage;
                    chunkHealths[i] = health;
					timer1 = 3;
				}
            }
			
        }
    }
    

	static bool CheckCollision(float3 posA, float3 posB, float radiusSqr)
	{
		float3 delta = posA - posB;
		float distanceSquare = delta.x * delta.x + delta.z * delta.z;

		return distanceSquare <= radiusSqr;
	}
}
