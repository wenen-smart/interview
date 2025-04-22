using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class ZombieToEntity : MonoBehaviour,IConvertGameObjectToEntity
{
    private Entity entity;
    private EntityManager entityManager;
    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        this.entity = entity;
        entityManager = dstManager;
        dstManager.AddComponent<ZombieTag>(entity);
        dstManager.AddBuffer<PathBuffPosition>(entity);
        dstManager.AddComponentData(entity,new PointIndexData() { pathIndex=-1});
    }
    public Entity GetEntity()
    {
        return entity;
    }
    public EntityManager GetEntityManager()
    {
        return entityManager;
    }
}
