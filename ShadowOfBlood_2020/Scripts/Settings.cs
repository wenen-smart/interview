using UnityEngine;
using Unity.Mathematics;
using UnityEngine.SceneManagement;

public class Settings : MonoBehaviour
{
	private static Unity.Mathematics.Random random;
	static Settings instance;//单例
	private float  Hp;
	[Header("Game Object References")]
	public Transform player;
	public GameObject player2;
	public static int enemyNumber=0;
	public static bool ISBoold=false;
	private   int enemyNumberup = 900;
	[Header("bullettDamage")]
	public float Bdamage;
	[Header("Collision Info")]

	public float playerCollisionRadius = .5f;
	[Header(" Player Power")]
	public static float power;

	public  GameObject[] EnemyDiedgame;
	public static GameObject[] GetEnemy
    {
		 get { return instance.EnemyDiedgame; }
    }
	public  static float  Sethp (float value)
    {
       return instance.Hp = value; 
    }
	public static int getenemyUP
    {
		get { return instance.enemyNumberup; }
    }
	
	public static float Gethp
    {
        get { return instance.Hp; }
    }
    public static float PlayerCollisionRadius
	{
		get { return instance.playerCollisionRadius; }
	}
	public static float GetBulletdamage
    {
		get { return instance.Bdamage+power; }
    }
	public float enemyCollisionRadius = .3f;
    

    public static float EnemyCollisionRadius
	{
		get { return instance.enemyCollisionRadius; }
	}

	public static Vector3 PlayerPosition
	{
		get { return instance.player.position; }
	}

   

    void Awake()
	{
		random = new Unity.Mathematics.Random(96);
		if (instance != null && instance != this)
			Destroy(gameObject);
		else
			instance = this;
	}//单例
    private void Start()
    {
		if (SceneManager.GetActiveScene().name!="0")//初始场景不加载
		GetBulletDamage();

	}
    public void GetBulletDamage()
    {
		Bdamage = player2.GetComponent<Gun>().gun.GetComponent<PlayerShooting>().bulletPrefab.GetComponent<ConversionB>().Damage;
    }//获取现在子弹的攻击力
	public static void  GetMax(float radius, ref float xMax , ref float xMin, ref float zMax , ref  float zMin )
    {
	  xMax=	instance.player.position.x + radius;
		  xMin = instance.player.position.x - radius;
	 zMax = instance.player.position.z + radius;
	 zMin = instance.player.position.z - radius;
	
	}
	public static Vector3 GetPositionAroundPlayer(float radius)
	{
		Vector3 playerPos = instance.player.position;

		float angle = UnityEngine.Random.Range(0f, 2 * Mathf.PI);
		float s = Mathf.Sin(angle);
		float c = Mathf.Cos(angle);
		
		return new Vector3(c * radius, 1.1f,/*高无所谓，在spawner重新赋值了*/ s * radius) + playerPos;
	}
	
	public static void PlayerDied()
	{
		if (instance.player == null)
			return;

		PlayerMovementAndLook playerMove = instance.player.GetComponent<PlayerMovementAndLook>();
		playerMove.PlayerDied();

		instance.player = null;
		
	}
	public static void EnemyDied(float3 pos3)
    {
		Vector3 shipPosition = new Vector3(pos3.x, 1, pos3.z);
		 
		float r = random.NextFloat(0, 100);
		if (r > 99)
		{
			
			Instantiate(GetEnemy[0], shipPosition,quaternion.identity);
		}
		else if  (r > 90)
        {
			Instantiate(GetEnemy[1], shipPosition, quaternion.identity);
		}
		else if (r > 80)
        {
			Debug.Log("enemyNumber");
			   Instantiate(GetEnemy[2], shipPosition, quaternion.identity);
		}
		else
        {
			return;
        }
	}
	public static bool IsPlayerDead()
	{
		return instance.player == null;
	}
}
