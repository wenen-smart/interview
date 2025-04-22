using Unity.Entities;
using UnityEngine;

public class ConversionE : MonoBehaviour, IConvertGameObjectToEntity
{
    public static Entity prefabEntity;
    public  GameObject GameObject;
    public  float speed;
    private static ConversionE instance;
    public  float enemyHealth;

    public static ConversionE Instance
    {
        get
        {
            if (instance == null)
            {
                instance = GameObject.FindObjectOfType<ConversionE>();
            }
            return instance;
        }
    }
    private void Start()
    {
        if (instance==null)
        {
            instance = this;
        }
    }
    public void Convert(Entity entity, EntityManager manager, GameObjectConversionSystem conversionSystem)
    {
        using (BlobAssetStore blobAssetStore = new BlobAssetStore())
        {
            Entity prefabEntity = GameObjectConversionUtility.ConvertGameObjectHierarchy(GameObject, GameObjectConversionSettings.FromWorld(manager.World, blobAssetStore));
            ConversionE.prefabEntity = prefabEntity;
           manager.AddComponent(prefabEntity, typeof(EnemyTag));
         //   manager.AddComponent(prefabEntity, typeof(MoveForward));


          //  MoveSpeed moveSpeed = new MoveSpeed { Value = speed };
          //  manager.AddComponentData(prefabEntity, moveSpeed);
          //这几个有问题开了直接穿
            Health health = new Health { Value = enemyHealth };
            manager.AddComponentData(prefabEntity, health);
        }
        
    }
}
