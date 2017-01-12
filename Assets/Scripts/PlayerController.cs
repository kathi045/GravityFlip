using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.Networking;

public class PlayerController : NetworkBehaviour {

    private float currentPlayerSpeed = GameController.GetDefaultPlayerSpeed();
    private bool gravInv = false;           // gravity inverted or not
    private bool lookingLeft = false;       // animation for left/right is simply flipped by localScale.x

    private Animator playerAnimator;
	private Rigidbody2D playerRigidBody;
    private Transform gun;
	private Vector3 mousePos;				// mouse position of the client

    public GameObject bulletPrefab;
	public Transform bulletSpawn;
	public GameObject gameControllerObj;
	public GameController gameController;

	// Use this for initialization
	void Awake() {
		gameControllerObj = GameObject.Find ("GameController");
		gameController = gameControllerObj.GetComponent<GameController> ();
		playerAnimator = this.GetComponent<Animator>();
		playerRigidBody = this.GetComponent<Rigidbody2D>();
        gun = transform.Find("Gun");
    }

	void FixedUpdate() {
		if (!isLocalPlayer) {
			return;
		}

		// stop animation speed if no input is received
		if (Input.anyKey == false)
		{
			CmdStopAnimation();
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
	}

	void Update()
    {
        // only control my own player, not all of them
        if (!isLocalPlayer) {
			return;
		}

        // stop animation speed if no input is received
        if (Input.anyKey == false)
        {
            CmdStopAnimation();
        }

		RpcShowSpeed ();

        // shoot
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            CmdFire();
        }

        

        // exit
        if (Input.GetKey(KeyCode.Escape))
        {
            NetworkManager.singleton.StopHost();
        }
	}

    [Command]
    void CmdStopAnimation()
    {
        RpcStopAnimation();
    }

    [ClientRpc]
    void RpcStopAnimation()
    {
        playerAnimator.speed = 0;
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
                playerRigidBody.gravityScale = -1 * GameController.GetGravity();
				transform.localScale = new Vector3(scale.x, -1 * scale.y, scale.z);
			}
		} else if (dir == "down") {
			if (gravInv) {
				gravInv = false;
				playerRigidBody.gravityScale = GameController.GetGravity();
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
        float bulletSpeed = GameController.GetBulletSpeed();
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
        if (bulletGameObject != null)
        {
            Collider2D bulletCollider = bulletGameObject.GetComponent<Collider2D>();
            if (bulletCollider != null)
            {
                Physics2D.IgnoreCollision(bulletCollider, GetComponent<Collider2D>());
            }
        }
	}

	[ClientRpc]
	void RpcLogMessage(string m) {
		Debug.Log(m);
	}

	[ClientRpc]
	void RpcShowSpeed() {
		if (!isLocalPlayer) {
			return;
		}
		GameObject.Find ("DebugMessage").GetComponent<Text> ().text = "PlayerSpeed: " + currentPlayerSpeed;
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
		transform.Translate(Vector2.right * currentPlayerSpeed * Time.deltaTime);
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
		transform.Translate(Vector2.left * currentPlayerSpeed * Time.deltaTime);
	}

    [ClientRpc]
    public void RpcIncreaseSpeed(float increaseSpeed)
    {
        float maxSpeed = GameController.GetMaximumPlayerSpeed();

        if (currentPlayerSpeed + increaseSpeed < maxSpeed)
        {
            currentPlayerSpeed += increaseSpeed;
        }
        else
        {
            currentPlayerSpeed = maxSpeed;
        }
    }

	public void ResetPlayerSpeed() {
		this.currentPlayerSpeed = GameController.GetDefaultPlayerSpeed ();
	}

	void OnCollisionEnter2D(Collision2D col) {
		bool chaseMode = gameController.GetDieOnPlayerCollision(); 
		Debug.Log("chaseMode: " + chaseMode.ToString());
		if (chaseMode == false) {
			return;
		}
		GameObject hitObject = col.gameObject;
		if (hitObject.tag == "Player") {
			hitObject.GetComponent<Health> ().TakeDamage (100, false);
			this.gameObject.GetComponent<Health> ().TakeDamage (100, false);
		}
	}
}
