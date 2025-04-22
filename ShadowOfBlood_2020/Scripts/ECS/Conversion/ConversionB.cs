using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;
using Unity.Mathematics;


public class ConversionB : MonoBehaviour, IConvertGameObjectToEntity, IDeclareReferencedPrefabs
{
    public Entity prefabEntity;
    public GameObject GameObject;
    public float speed;
    public float lifeTime;
    public float Damage;


    public void DeclareReferencedPrefabs(List<GameObject> referencedPrefabs) => referencedPrefabs.Add(GameObject);

    public void Convert(Entity entity, EntityManager manager, GameObjectConversionSystem conversionSystem)
    {
        Entity prefabEntity2 = conversionSystem.GetPrimaryEntity(GameObject);
        prefabEntity = prefabEntity2;

        manager.AddComponent(prefabEntity, typeof(MoveForward));


        MoveSpeed moveSpeed = new MoveSpeed { Value = speed };
        manager.AddComponentData(prefabEntity, moveSpeed);

        TimeToLive timeToLive = new TimeToLive { Value = lifeTime };
        manager.AddComponentData(prefabEntity, timeToLive);


    }
}