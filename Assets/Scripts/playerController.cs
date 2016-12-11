using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class playerController : NetworkBehaviour {

    public float speed = 7f;
    public float jumpForce = 500f;
	public int gravity = 2;
	public float bulletSpeed = 6.0f;

	private bool gravInv = false;			// gravity inverted or not
	//private bool canJump = false;			// jumping is possible
	//private bool grounded = false;			// is player on the ground or in the air (jumping)
	private bool flipped = false;			// animation for left/right is simply flipped by localScale.x

    private Animator playerAnimator;
	private Rigidbody2D playerRigidBody;
	private Transform groundCheck;			// position at the player's feet (for ground detection)
	private Transform gun;

	public GameObject bulletPrefab;
	public Transform bulletSpawn;

	// Use this for initialization
	void Awake() {
		playerAnimator = this.GetComponent<Animator>();
		playerRigidBody = this.GetComponent<Rigidbody2D>();
		groundCheck = transform.Find ("groundCheck");
		gun = transform.Find ("Gun");
	}

	void Update() {
		// only control my own player, not all of them
		if (!isLocalPlayer) {
			return;
		}

		//grounded = Physics2D.Linecast (transform.position, groundCheck.position, 1 << LayerMask.NameToLayer ("Ground"));

//		// jump
//		if (Input.GetKeyDown(KeyCode.Space) && grounded) {
//			//playerRigidBody.AddForce(Vector2.up * jumpForce);
//			canJump = true;
//		}
	}
	
	// Update is called once per frame
	void FixedUpdate() {
        // cancel all animation if no input is received
		if (Input.anyKey == false) {
			playerAnimator.speed = 0;
		}

		if (!isLocalPlayer) {
			return;
		}

		//transform.rotation = Quaternion.identity;
		Vector3 gunPos = gun.transform.localPosition;
		Vector3 eulerAngles = gun.transform.eulerAngles;

        // walk left
		if (Input.GetKey (KeyCode.A)) {
			if (!flipped) {
				flipped = true;
				eulerAngles.y = 180.0f;
				gunPos.x = -gunPos.x;
				gun.transform.localPosition = gunPos;
				gun.transform.eulerAngles = eulerAngles;

			}
			playerAnimator.speed = 1;
			playerAnimator.SetInteger("direction", 1);
			playerAnimator.Play("player_walk_west");
			transform.Translate(Vector2.left * speed * Time.deltaTime);
		}

        // walk right
		if (Input.GetKey (KeyCode.D)) {
			if (flipped) {
				flipped = false;
				eulerAngles.y = 0.0f;
				gunPos.x = -gunPos.x;
				gun.transform.localPosition = gunPos;
				gun.transform.eulerAngles = eulerAngles;
			}
            playerAnimator.speed = 1;
            playerAnimator.SetInteger("direction", 3);
			playerAnimator.Play("player_walk_east");
			transform.Translate(Vector2.right * speed * Time.deltaTime);
		}

		Vector3 scale = transform.localScale;

		if (Input.GetKeyDown (KeyCode.W)) {
			if (!gravInv) {
				gravInv = true;
				playerRigidBody.gravityScale = -1 * gravity;
				transform.localScale = new Vector3 (scale.x, -1 * scale.y, scale.z);
			}
		} else if (Input.GetKeyDown (KeyCode.S)) {
			if (gravInv) {
				gravInv = false;
				playerRigidBody.gravityScale = gravity;
				transform.localScale = new Vector3 (scale.x, Mathf.Abs (scale.y), scale.z);
			}
		}

		// shoot
		if (Input.GetKeyDown (KeyCode.Mouse0)) {
			CmdFire ();
		}

//		if (canJump) {
//			if (!gravInv) {
//				playerRigidBody.AddForce (Vector2.up * jumpForce);
//			} else {
//				playerRigidBody.AddForce (Vector2.down * jumpForce);
//			}
//			canJump = false;
//		}
	}

//	void Flip() {
//		flipped = !flipped;
//		Vector3 scale = transform.localScale;
//		scale.x *= -1;
//		transform.localScale = scale;
//	}

	[Command]
	void CmdFire() {
		// create bullet from prefab
		GameObject bullet = (GameObject)Instantiate(bulletPrefab, bulletSpawn.position, bulletSpawn.rotation);

		// add velocity to the bullet
		if (!flipped) {
			bullet.GetComponent<Rigidbody2D>().velocity = bullet.transform.right * bulletSpeed;
		} else {
			//bulletSpeed = -bulletSpeed;
			bullet.GetComponent<Rigidbody2D>().velocity = bullet.transform.right * bulletSpeed;
		}

		// spawn the bullet on the clients
		NetworkServer.Spawn(bullet);
	}

	public override void OnStartLocalPlayer() {
		// only local player (my own) has different color
		//GetComponent<MeshRenderer> ().material.color = Color.blue;
	}
}
