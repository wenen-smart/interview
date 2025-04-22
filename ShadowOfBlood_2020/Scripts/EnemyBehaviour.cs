using System;
using Unity.Burst;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Authoring;
using Unity.Transforms;
using UnityEngine;
using Collider = Unity.Physics.Collider;

/*[RequireComponent(typeof(Rigidbody))]*/
public class EnemyBehaviour : MonoBehaviour
{
	[Header("Movement")]
	public float speed = 2f;

	[Header("Life Settings")]
	public float enemyHealth = 1f;
	[Header("y")]
	public float posY = 0f;
    //Rigidbody rigidBody;


    //void Start()
    //{
    //	rigidBody = GetComponent<Rigidbody>();
    //}

    //void Update()
    //{
    //	if (!Settings.IsPlayerDead())
    //	{
    //		Vector3 heading = Settings.PlayerPosition - transform.position;
    //		heading.y = 0f;
    //		transform.rotation = Quaternion.LookRotation(heading);
    //	}

    //	Vector3 movement = transform.forward * speed * Time.deltaTime;
    //	rigidBody.MovePosition(transform.position + movement);
    //}

    //Enemy Collision
    //void OnTriggerEnter(Collider theCollider)
    //{
    //	if (!theCollider.CompareTag("Bullet"))
    //		return;

    //	enemyHealth--;

    //	if(enemyHealth <= 0)
    //	{
    //		Destroy(gameObject);
    //		BulletImpactPool.PlayBulletImpact(transform.position);
    //	}
    //}

    /// <summary>
    /// convert
    /// </summary>

    // 	public void Convert(Entity entity, EntityManager manager, GameObjectConversionSystem conversionSystem)
    // 	{
    //         EntityArchetype entityArchetype = manager.CreateArchetype(typeof(Translation), typeof(Rotation), typeof(ZombieTag));
    // 		entityZoombie = manager.CreateEntity(entityArchetype);
    // 		manager.AddComponent(entity, typeof(EnemyTag));
    // 		manager.AddComponent(entity, typeof(MoveForward));
    // 		MoveSpeed moveSpeed = new MoveSpeed { Value = speed };
    // 		manager.AddComponentData(entity, moveSpeed);
    // 
    // 		Health health = new Health { Value = enemyHealth };
    // 		manager.AddComponentData(entity, health);
    // 	}

    //update 


    private Entity entityZoombie;
    private EntityManager entityManager;
    public ZombieToEntity zombieToEntity;

    // Start is called before the first frame update
    private void Start()
    {
       

       
    }
    // Update is called once per frame
    void Update()
    {

        Entity entity = zombieToEntity.GetEntity();
        EntityManager manager = zombieToEntity.GetEntityManager();
        transform.position = manager.GetComponentData<Translation>(entity).Value;
        transform.rotation = manager.GetComponentData<Rotation>(entity).Value;

        /* jobToMove job = new jobToMove*/
        //         {
        //             trans = transform,
        //             tempentity = entity
        //         };
        //         JobHandle jobHandle = job.Schedule();
        //         jobHandle.Complete();

    }

    [BurstCompile]
    public struct jobToMove : IJob
    {
        public Transform trans;
        public Entity tempentity;

        public void Execute()
        {
            trans.position = World.DefaultGameObjectInjectionWorld.EntityManager.GetComponentData<Translation>(tempentity).Value;
        }
    }
}
