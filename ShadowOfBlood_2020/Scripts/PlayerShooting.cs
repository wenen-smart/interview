using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;
using Unity.Mathematics;
using Unity.Core;

public class PlayerShooting : MonoBehaviour
{
	//public bool useECS = false;
	//public bool spreadShot = false;
	public  Unity.Mathematics.Random R;
	[Header("General")]
	public Transform gunBarrel;
	public ParticleSystem shotVFX;
	public AudioSource shotAudio;
	public float fireRate = .1f;
	public float fireRatepuls = .5f;
	public int spreadAmount = 20;

	[Header("Bullets")]
	public GameObject bulletPrefab;
	public GameObject bulletL;
	//float timer;
	public bool IsL = false;
	EntityManager manager;



	void Start()
	{
		R = new Unity.Mathematics.Random(17);
		var settings = GameObjectConversionSettings.FromWorld(World.DefaultGameObjectInjectionWorld, null);
		manager = World.DefaultGameObjectInjectionWorld.EntityManager;
		//bulletEntityPrefab = GameObjectConversionUtility.ConvertGameObjectHierarchy(bulletPrefab, settings);

	}

	void Update()
	{
		
	//	timer += Time.deltaTime;

		//if (Input.GetButton("Fire1") && timer >= fireRate)
		//{
		//	Vector3 rotation = gunBarrel.rotation.eulerAngles;
		//	//rotation.x = 0f;
		//	SpawnBulletECS(rotation);

		//	timer = 0f;

		//	if (shotVFX)
		//		shotVFX.Play();

		//	if (shotAudio)
		//		shotAudio.Play();

		//}
		//else if (Input.GetButton("Fire2") && timer >= fireRatepuls)
		//{
		//	Vector3 rotation = gunBarrel.rotation.eulerAngles;
		//	SpawnBulletSpreadECS(rotation);
		//	timer = 0f;

		//	if (shotVFX)
		//		shotVFX.Play();

		//	if (shotAudio)
		//		shotAudio.Play();
		//}
	




	}

	//void SpawnBullet(Vector3 rotation)
	//{
	//	GameObject bullet = Instantiate(bulletPrefab) as GameObject;

	//	bullet.transform.position = gunBarrel.position;
	//	bullet.transform.rotation = Quaternion.Euler(rotation);
	//}

	//void SpawnBulletSpread(Vector3 rotation)
	//{
	//	int max = spreadAmount / 2;
	//	int min = -max;

	//	Vector3 tempRot = rotation;
	//	for (int x = min; x < max; x++)
	//	{
	//		tempRot.x = (rotation.x + 3 * x) % 360;

	//		for (int y = min; y < max; y++)
	//		{
	//			tempRot.y = (rotation.y + 3 * y) % 360;

	//			GameObject bullet = Instantiate(bulletPrefab) as GameObject;

	//			bullet.transform.position = gunBarrel.position;
	//			bullet.transform.rotation = Quaternion.Euler(tempRot);
	//		}
	//	}
	//}

	void SpawnBulletECS(Vector3 rotation)
	{
		if (!IsL)
		{
			Entity bullet = manager.Instantiate(bulletPrefab.GetComponent<ConversionB>().prefabEntity);
			var r = R.NextFloat(-10, 10);
			Debug.Log(r);
			manager.SetComponentData(bullet, new Translation { Value = gunBarrel.position });
			manager.SetComponentData(bullet, new Rotation { Value = Quaternion.Euler(/*rotation*/ new Vector3(rotation.x, rotation.y + r, rotation.z)) });
		}
        else
        {
			
			Entity bullet = manager.Instantiate(bulletPrefab.GetComponent<ConversionB>().prefabEntity);
			manager.SetComponentData(bullet, new Translation { Value = gunBarrel.position });
			manager.SetComponentData(bullet, new Rotation { Value = Quaternion.Euler(rotation) });
			//manager.SetComponentData(bullet, new Rotation { Value = Quaternion.Euler(/*rotation*/ new Vector3(rotation.x, rotation.y + r, rotation.z)) });
		}
	}

	void SpawnBulletSpreadECS(Vector3 rotation)
	{if (!IsL)
		{
			int max = spreadAmount / 2;
			int min = -max;
			int totalAmount = spreadAmount;

			Vector3 tempRot = rotation;
			int index = 0;

			NativeArray<Entity> bullets = new NativeArray<Entity>(totalAmount, Allocator.TempJob);
			manager.Instantiate(bulletPrefab.GetComponent<ConversionB>().prefabEntity, bullets);

			//for (int x = min; x < max; x++)
			//{
			//	tempRot.x = (rotation.x + 3 * x) % 180;

			for (int y = min; y < max; y++)
			{
				var r = R.NextFloat(-10, 10);
				tempRot.y = (rotation.y + 3 * y) % 360;

				manager.SetComponentData(bullets[index], new Translation { Value = gunBarrel.position });
				manager.SetComponentData(bullets[index], new Rotation { Value = Quaternion.Euler(new Vector3(tempRot.x + r, tempRot.y + r, tempRot.z)) });

				index++;
			}
			//}
			bullets.Dispose();
		}
        else
        {
			Entity bullet = manager.Instantiate(bulletL.GetComponent<ConversionB>().prefabEntity);
			manager.SetComponentData(bullet, new Translation { Value = gunBarrel.position });
			manager.SetComponentData(bullet, new Rotation { Value = Quaternion.Euler(rotation) });
		}
	}
	public void  shootLeft()
    {

			Vector3 rotation = gunBarrel.rotation.eulerAngles;
			//rotation.x = 0f;
			SpawnBulletECS(rotation);

			//timer = 0f;

			if (shotVFX)
				shotVFX.Play();

			if (shotAudio)
				shotAudio.Play();

		
	} 
	public void shootRight()
    {
		
			Vector3 rotation = gunBarrel.rotation.eulerAngles;
			SpawnBulletSpreadECS(rotation);
			//timer = 0f;

			if (shotVFX)
				shotVFX.Play();

			if (shotAudio)
				shotAudio.Play();
		
	}
}

