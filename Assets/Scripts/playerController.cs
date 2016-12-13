using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class playerController : NetworkBehaviour {

    private float speed = 4.5f;
    //private float jumpForce = 500f;       // jumping disabled
	private float gravity = 2.0f;
	private float bulletSpeed = 6.0f;

	private bool gravInv = false;           // gravity inverted or not
    private bool lookingLeft = false;       // animation for left/right is simply flipped by localScale.x
    //private bool canJump = false;			// jumping is possible
    //private bool grounded = false;		// is player on the ground or in the air (jumping)

    private Animator playerAnimator;
	private Rigidbody2D playerRigidBody;
    private Transform gun;
    //private Transform groundCheck;		// position at the player's feet (for ground detection)

    public GameObject bulletPrefab;
	public Transform bulletSpawn;

	// Use this for initialization
	void Awake() {
		playerAnimator = this.GetComponent<Animator>();
		playerRigidBody = this.GetComponent<Rigidbody2D>();
        gun = transform.Find("Gun");
        //groundCheck = transform.Find("groundCheck");
    }

	void Update() {
		// only control my own player, not all of them
		if (!isLocalPlayer) {
			return;
		}

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

		// shoot
		if (Input.GetKeyDown (KeyCode.Mouse0)) {
			CmdFire();
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

		if(!isLocalPlayer) {
			return;
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
        //bulletSpawnPosition = gun.
        GameObject bulletGameObject = (GameObject)Instantiate(bulletPrefab, bulletSpawnPosition, bulletSpawn.rotation);
        Bullet bullet = bulletGameObject.GetComponent<Bullet>();
        bullet.setShootingPlayer(playerRigidBody);

        // add velocity to the bullet
        float bulletVelocity = lookingLeft ? bulletSpeed : bulletSpeed;
        bulletGameObject.GetComponent<Rigidbody2D>().velocity = bulletGameObject.transform.right * bulletVelocity;

        // logging
        string gunAngleLog = "Gun euler angle: " + gun.eulerAngles.x + ", " + gun.eulerAngles.y + ", " + gun.eulerAngles.z;
        RpcLogMessage(gunAngleLog);
        string gunPositionLog = "Gun position: " + gun.localPosition.x + ", " + gun.localPosition.y + ", " + gun.localPosition.z;
        RpcLogMessage(gunPositionLog);

        // spawn the bullet on the clients
        NetworkServer.Spawn(bulletGameObject);
	}

	[ClientRpc]
	void RpcLogMessage(string m) {
		Debug.Log(m);
	}

	[Command]
	void CmdMoveRight()
    {
        Vector3 gunPos = gun.transform.localPosition;
        Vector3 eulerAngles = gun.transform.localEulerAngles;

        if (lookingLeft)
        {
            lookingLeft = false;
            eulerAngles.y = 0.0f;
            gunPos.x = -gunPos.x;
            gun.transform.localPosition = gunPos;
            gun.transform.localEulerAngles = eulerAngles;
        }

        RpcMoveRight();
	}

	[ClientRpc]
	void RpcMoveRight() {
		playerAnimator.speed = 1;
		playerAnimator.SetInteger("direction", 3);
		playerAnimator.Play("player_walk_east");
		transform.Translate(Vector2.right * speed * Time.deltaTime);
	}

	[Command]
	void CmdMoveLeft()
    {
        Vector3 gunPos = gun.transform.localPosition;
        Vector3 eulerAngles = gun.transform.localEulerAngles;

        if (!lookingLeft)
        {
            lookingLeft = true;
            eulerAngles.y = 180.0f;
            gunPos.x = -gunPos.x;
            gun.transform.localPosition = gunPos;
            gun.transform.localEulerAngles = eulerAngles;
        }

        RpcMoveLeft();
	}

	[ClientRpc]
	void RpcMoveLeft() {
		playerAnimator.speed = 1;
		playerAnimator.SetInteger("direction", 1);
		playerAnimator.Play("player_walk_west");
		transform.Translate(Vector2.left * speed * Time.deltaTime);
	}

	public override void OnStartLocalPlayer() {
		// only local player (my own) has different color
		//GetComponent<MeshRenderer>().material.color = Color.blue;
	}
}
