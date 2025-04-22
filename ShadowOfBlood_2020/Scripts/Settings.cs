using UnityEngine;

public class Settings : MonoBehaviour
{
	static Settings instance;//单例

	[Header("Game Object References")]
	public Transform player;
	public GameObject player2;
	[Header("bullettDamage")]
	public float Bdamage;
	[Header("Collision Info")]
	public float playerCollisionRadius = .5f;
    
    public static float PlayerCollisionRadius
	{
		get { return instance.playerCollisionRadius; }
	}
	public static float GetBulletdamage
    {
		get { return instance.Bdamage; }
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
		if (instance != null && instance != this)
			Destroy(gameObject);
		else
			instance = this;
	}//单例
    private void Start()
    {
		GetBulletDamage();

	}
    public void GetBulletDamage()
    {
		Bdamage = player2.GetComponent<PlayerShooting>().bulletPrefab.GetComponent<ProjectileBehaviour>().damage;
    }//获取现在子弹的攻击力
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

	public static bool IsPlayerDead()
	{
		return instance.player == null;
	}
}
