using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.Networking;

public class playerController : NetworkBehaviour {

    private float speed = 4.5f;				// player walk speed
	private float gravity = 2.0f;
	private bool gravInv = false;           // gravity inverted or not
    private bool lookingLeft = false;       // animation for left/right is simply flipped by localScale.x
    //private bool canJump = false;			// jumping is possible
    //private bool grounded = false;		// is player on the ground or in the air (jumping)
	//private float jumpForce = 500f;       // jumping disabled
	//private Transform groundCheck;		// position at the player's feet (for ground detection)

    private Animator playerAnimator;
	private Rigidbody2D playerRigidBody;
    private Transform gun;
	private Vector3 mousePos;				// mouse position of the client 
	private Text text;						// GUI Text field for displaying messages during runtime
	private float bulletSpeed = 6.0f;

    public GameObject bulletPrefab;
	public Transform bulletSpawn;

	// Use this for initialization
	void Awake() {
		playerAnimator = this.GetComponent<Animator>();
		playerRigidBody = this.GetComponent<Rigidbody2D>();
        gun = transform.Find("Gun");
		text = FindObjectOfType<Text>();
		//groundCheck = transform.Find("groundCheck");
    }

	void Update() {
		// only control my own player, not all of them
		if (!isLocalPlayer) {
			return;
		}

		// update mouse position for shooting in direction of the mouse
		this.mousePos = Input.mousePosition;
		this.mousePos.z = transform.position.z - Camera.main.transform.position.z;
		CmdMousePosition(mousePos);

        // walk left
        if (Input.GetKey(KeyCode.A)) {
			CmdMoveLeft();
		}

		// walk right
		if (Input.GetKey(KeyCode.D)) {
			CmdMoveRight();
		}

		// change gravity
		if (Input.GetKeyDown(KeyCode.W)) {
			CmdChangeGrav("up");
		} else if (Input.GetKeyDown(KeyCode.S)) {
			CmdChangeGrav("down");
		}

		//grounded = Physics2D.Linecast (transform.position, groundCheck.position, 1 << LayerMask.NameToLayer("Ground"));
//		// jump
//		if (Input.GetKeyDown(KeyCode.Space) && grounded) {
//			//playerRigidBody.AddForce(Vector2.up * jumpForce);
//			canJump = true;
//		}
	}
	
	// Update is called once per frame
	void FixedUpdate() {
        // cancel all animation if no input is received
		if(Input.anyKey == false) {
			playerAnimator.speed = 0;
		}

        if (!isLocalPlayer)
        {
            return;
        }

		// shoot
		if (Input.GetKeyDown (KeyCode.Mouse0)) {
			CmdFire();
		}

        //		if (canJump) {
        //			if (!gravInv) {
        //				playerRigidBody.AddForce(Vector2.up * jumpForce);
        //			} else {
        //				playerRigidBody.AddForce(Vector2.down * jumpForce);
        //			}
        //			canJump = false;
        //		}
    }

	[Command]
	void CmdChangeGrav(string dir) {
		RpcChangeGrav(dir);
	}

	[ClientRpc]
	void RpcChangeGrav(string dir) {
		Vector3 scale = transform.localScale;
		if (dir == "up") {
			if (!gravInv) {
				gravInv = true;
				playerRigidBody.gravityScale = -1 * gravity;
				transform.localScale = new Vector3(scale.x, -1 * scale.y, scale.z);
			}
		} else if (dir == "down") {
			if (gravInv) {
				gravInv = false;
				playerRigidBody.gravityScale = gravity;
				transform.localScale = new Vector3(scale.x, Mathf.Abs(scale.y), scale.z);
			}
		}
	}

    [Command]
	void CmdFire() {
        // create bullet from prefab
        float bulletOffset = lookingLeft ? -0.1f : 0.1f;
        Vector3 bulletSpawnPosition = bulletSpawn.position + new Vector3(bulletOffset, 0, 0);
        GameObject bulletGameObject = (GameObject)Instantiate(bulletPrefab, bulletSpawnPosition, Quaternion.identity);
        
		Bullet bullet = bulletGameObject.GetComponent<Bullet>();
        bullet.setShootingPlayer(playerRigidBody);
        
        // bullet velocity in direction of the mouse
		Vector3 direction = Camera.main.ScreenToWorldPoint(mousePos) - bulletSpawnPosition;
        direction.Normalize();
		bulletGameObject.GetComponent<Rigidbody2D>().velocity = new Vector2(direction.x * bulletSpeed, direction.y * bulletSpeed);

        // spawn the bullet on the clients
        NetworkServer.Spawn(bulletGameObject);
		RpcIgnoreCollision(bulletGameObject);
	}

	// update server with mouse position of the client
	[Command]
	void CmdMousePosition(Vector3 pos) {
		this.mousePos = pos;
	}

	// prevent bullet from colliding with player that fires it
	[ClientRpc]
	void RpcIgnoreCollision(GameObject bulletGameObject) {
		Physics2D.IgnoreCollision (bulletGameObject.GetComponent<Collider2D>(), GetComponent<Collider2D>());
	}

	[ClientRpc]
	void RpcLogMessage(string m) {
		Debug.Log(m);
	}

	[Command]
	void CmdMoveRight()
    {
        RpcMoveRight();
	}

	[ClientRpc]
	void RpcMoveRight() {
		Vector3 gunPos = gun.transform.localPosition;
		Vector3 gunScale = gun.transform.localScale;

		if (lookingLeft)
		{
			lookingLeft = false;
			gunPos.x = -gunPos.x;
			gun.transform.localPosition = gunPos;
			gunScale.x = -gunScale.x;
			gun.transform.localScale = gunScale;
		}

		playerAnimator.speed = 1;
		playerAnimator.SetInteger("direction", 3);
		playerAnimator.Play("player_walk_east");
		transform.Translate(Vector2.right * speed * Time.deltaTime);
	}

	[Command]
	void CmdMoveLeft()
    {
        RpcMoveLeft();
	}

	[ClientRpc]
	void RpcMoveLeft() {
		Vector3 gunPos = gun.transform.localPosition;
		Vector3 gunScale = gun.transform.localScale;

		if (!lookingLeft)
		{
			lookingLeft = true;
			gunPos.x = -gunPos.x;
			gun.transform.localPosition = gunPos;
			gunScale.x = -gunScale.x;
			gun.transform.localScale = gunScale;
		}

		playerAnimator.speed = 1;
		playerAnimator.SetInteger("direction", 1);
		playerAnimator.Play("player_walk_west");
		transform.Translate(Vector2.left * speed * Time.deltaTime);
	}
}