using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

[UpdateInGroup(typeof(InitializationSystemGroup))]
public class FxSystem : ComponentSystem
{
    protected override void OnUpdate()
    {
        Entities.ForEach((Entity entity, ref Translation pos) =>
        {
            if (EntityManager.HasComponent(entity, typeof(Fxtag)))
            {
                BulletImpactPool.PlayBulletImpact(pos.Value);
                EntityManager.RemoveComponent(entity, typeof(Fxtag));
                Debug.Log("Fxtag!");
            }
        });
    }
}
