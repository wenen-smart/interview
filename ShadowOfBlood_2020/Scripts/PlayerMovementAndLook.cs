using System.Configuration;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class PlayerMovementAndLook : MonoBehaviour
{
	private Cinemachine.CinemachineCollisionImpulseSource MyInpulse;
	public  Image hp;
	 float fix;
	  Renderer renderer;
	  Material red;

	[Header("Camera")]
	public Camera mainCamera;

	[Header("Movement")]
	public float speed = 4.5f;
	public LayerMask whatIsGround;

	[Header("Life Settings")]
	//public float playerHealth = 1f;

	[Header("Animation")]
	public Animator playerAnimator;

	Rigidbody playerRigidbody;
	bool isDead;
	//Texture textureRed;
	void Awake()

	{
		//	textureRed= AssetDatabase.LoadAssetAtPath("Assets/UI/Icons/StyleSprites/AllCorners_Dark_OnActive_Underline 1.png", typeof(Texture2D)) as Texture2D;
		MyInpulse =GameObject.Find("Gun"). GetComponent<Cinemachine.CinemachineCollisionImpulseSource>();
		playerRigidbody = GetComponent<Rigidbody>();
		renderer = GameObject.FindGameObjectWithTag("playerbaes").GetComponent<Renderer>();
		red = renderer.material;
        //red = new Material(Shader.Find("Standard"));
        //red.EnableKeyword("_EMISSION");
        //red.SetTexture("_EMISSION", textureRed);
        //red.SetTextureScale("_EMISSION", new Vector2(0, 555));
        //renderer.material = red;
        //red.SetColor("_Color", Color.red);
     
	}

	void FixedUpdate()
	{
		fix = 5 - Settings.Gethp / 200;
		red.mainTextureScale = new Vector2(0, fix);
		// life ui
		hp.fillAmount = Settings.Gethp / 1000;
		if (Settings.ISBoold==true)
        {
			MyInpulse.GenerateImpulse();
		//	Debug.Log("Yes");
			Gui.OnBlood();
			Settings.ISBoold = false;

		}
		//red.SetColor("_EmissionColor", new Color(0, 44, 444));
		if (isDead)
			return;

	
		float h = Input.GetAxis("Horizontal");
		float v = Input.GetAxis("Vertical");

		Vector3 inputDirection = new Vector3(h, 0, v);

		//Camera Direction
		var cameraForward = mainCamera.transform.forward;
		var cameraRight = mainCamera.transform.right;

		cameraForward.y = 0f;
		cameraRight.y = 0f;

		//Try not to use var for roadshows or learning code
		Vector3 desiredDirection = cameraForward * inputDirection.z + cameraRight * inputDirection.x;
		
		//Why not just pass the vector instead of breaking it up only to remake it on the other side?
		MoveThePlayer(desiredDirection);
		TurnThePlayer();
		AnimateThePlayer(desiredDirection);
		
	}

	void MoveThePlayer(Vector3 desiredDirection)
	{
		Vector3 movement = new Vector3(desiredDirection.x, 0f, desiredDirection.z);
		movement = movement.normalized * speed * Time.deltaTime;

		playerRigidbody.MovePosition(transform.position + movement);
	}

	void TurnThePlayer()
	{
		Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
		RaycastHit hit;

		if (Physics.Raycast(ray, out hit, whatIsGround))
		{
			Vector3 playerToMouse = hit.point - transform.position;
			playerToMouse.y = 0f;
			playerToMouse.Normalize();

			Quaternion newRotation = Quaternion.LookRotation(playerToMouse);
			playerRigidbody.MoveRotation(newRotation);
		}
	}

	void AnimateThePlayer(Vector3 desiredDirection)
	{
		if(!playerAnimator)
			return;

		Vector3 movement = new Vector3(desiredDirection.x, 0f, desiredDirection.z);
		float forw = Vector3.Dot(movement, transform.forward);
		float stra = Vector3.Dot(movement, transform.right);

		playerAnimator.SetFloat("Forward", forw);
		playerAnimator.SetFloat("Strafe", stra);
	}



	public void PlayerDied()
	{
		if (isDead)
			return;

		isDead = true;

		playerAnimator.SetTrigger("Died");
		playerRigidbody.isKinematic = true;
		GetComponent<Collider>().enabled = false;
		gameObject.GetComponent<Gun>().enabled = false;
		Invoke("Del", 5.0f);
	}
	public void Del()
    {
		
		Destroy(gameObject);
		Gui.OnLose();

    }
}
