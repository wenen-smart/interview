using Unity.Entities;
using Unity.Transforms;

[UpdateInGroup(typeof(InitializationSystemGroup))]
public class RemoveDeadSystem : ComponentSystem
{
	protected override void OnUpdate()
	{
		Entities.ForEach((Entity entity, ref Health health, ref Translation pos) =>
		{
		if	(EntityManager.HasComponent(entity, typeof(PlayerTag)))
			{
				Settings.Sethp(health.Value);//获取实时生命
            }
			if (health.Value <= 0)
			{
				if (EntityManager.HasComponent(entity, typeof(PlayerTag)))
				{
					Settings.PlayerDied();
				}

				else if (EntityManager.HasComponent(entity, typeof(EnemyTag)))
				{
					PostUpdateCommands.DestroyEntity(entity);
					BulletImpactPool.PlayBulletImpact(pos.Value);
					Settings.EnemyDied(pos.Value);
					CollisionGet.money++;
					Settings.enemyNumber--;
				}
			}
		});
	}
}