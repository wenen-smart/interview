using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;
using Unity.Mathematics;


public class ConversionE : MonoBehaviour, IConvertGameObjectToEntity,IDeclareReferencedPrefabs
{
    public  Entity  prefabEntity;
    public GameObject GameObject;
    public float speed;
    public float enemyHealth;

    public void DeclareReferencedPrefabs(List<GameObject> referencedPrefabs) => referencedPrefabs.Add(GameObject);
    
    public void Convert(Entity entity, EntityManager manager, GameObjectConversionSystem conversionSystem)
    {
        Entity prefabEntity1 = conversionSystem.GetPrimaryEntity(GameObject);
        prefabEntity = prefabEntity1;
       Unity.Mathematics. Random r = new Unity.Mathematics.Random(51);
        manager.AddComponent(prefabEntity, typeof(EnemyTag));
        manager.AddComponent(prefabEntity, typeof(AutoPartolTagComponent));
        //manager.AddComponent(prefabEntity, typeof(MoveForward));
        MoveSpeed moveSpeed = new MoveSpeed { Value = speed };
        manager.AddComponentData(prefabEntity, moveSpeed);
        Health health = new Health { Value = enemyHealth };
        manager.AddComponentData(prefabEntity, health);
        manager.AddBuffer<PathBuffPosition>(prefabEntity);
        manager.AddComponentData(prefabEntity, new PointIndexData() { pathIndex = -1 });
        manager.AddComponentData(prefabEntity, new RecordRotateData() { angle = r.NextFloat(1, 360) });

      



    }


}
