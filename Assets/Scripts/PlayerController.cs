using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.Networking;

public class PlayerController : NetworkBehaviour {

    private float currentPlayerSpeed = GameController.GetDefaultPlayerSpeed();
    private bool gravInv = false;           // gravity inverted or not
    private bool lookingLeft = false;       // animation for left/right is simply flipped by localScale.x
    private float timeSinceLastShot = 0.0f;

    private Animator playerAnimator;
	private Rigidbody2D playerRigidBody;
    private Transform gun;
	private Vector3 mousePos;				// mouse position of the client
	private GameObject wall1;
	private GameObject wall2;

    public GameObject bulletPrefab;
	public Transform bulletSpawn;
	public GameObject gameControllerObj;
	public GameController gameController;

    private GameObject gameEndPanel;

    // Use this for initialization
    void Awake()
    {
		gameEndPanel = GameObject.Find("GameEndPanel");
		CmdGameEndPanel(false);
		playerAnimator = this.GetComponent<Animator>();
		playerRigidBody = this.GetComponent<Rigidbody2D>();
        gun = transform.Find("Gun");
		gameController = GameObject.Find ("GameController").GetComponent<GameController> ();
		wall1 = GameObject.Find ("wall1");
		wall2 = GameObject.Find ("wall2");
    }

	void FixedUpdate()
    {
		if (!isLocalPlayer)
        {
			return;
		}

        // DO NOT ENTER CONTROLS INSIDE FIXED_UPDATE!
        // FixedUpdate() is only called on fixed times and might therefore not register the input at all!

		// stop animation speed if no input is received
		if (!Input.GetKey(KeyCode.A) && !Input.GetKey(KeyCode.D))
		{
			CmdStopAnimation();
		}
	}

	void Update()
    {
        // only control my own player, not all of them
        if (!isLocalPlayer)
        {
			return;
		}

        // if game ended, disable player control and display game over message
        if (isServer)
        {
            if (GameController.GetGameEnded())
            {
                CmdGameEndPanel(true);
                return;
            }
            else
            {
                CmdGameEndPanel(false);
            }
        }
        else
        {
            if (gameEndPanel.activeSelf)
            {
                return;
            }

        }

        // stop animation speed if no input is received
        if (!Input.GetKey(KeyCode.A) && !Input.GetKey(KeyCode.D))
        {
            CmdStopAnimation();
        }

        // update mouse position for shooting in direction of the mouse
        this.mousePos = Input.mousePosition;
        this.mousePos.z = transform.position.z - Camera.main.transform.position.z;
        CmdMousePosition(mousePos);

        // walk left
        if (Input.GetKey(KeyCode.A))
        {
            CmdMoveLeft();
        }

        // walk right
        if (Input.GetKey(KeyCode.D))
        {
            CmdMoveRight();
        }

        // change gravity
        if (Input.GetKeyDown(KeyCode.W))
        {
            CmdChangeGrav("up");
			if (isServer) {
				wall1.transform.localPosition = new Vector3 (4.7f, 0f, 0f);
				RpcChangeWall1 ("up");
			} else {
				wall2.transform.localPosition = new Vector3 (-4f, 1f, 0f);
				CmdChangeWall2 ("up");
			}
        }
        else if (Input.GetKeyDown(KeyCode.S))
        {
            CmdChangeGrav("down");
			if (isServer) {
				wall1.transform.localPosition = new Vector3 (4.7f, -2f, 0f);
				RpcChangeWall1 ("down");
			} else {
				wall2.transform.localPosition = new Vector3 (-4f, -0.5f, 0f);
				CmdChangeWall2 ("down");
			}
        }

        //showSpeed ();

        // shoot
        timeSinceLastShot += Time.deltaTime;
        if (Input.GetKey(KeyCode.Mouse0))
        {
            if (timeSinceLastShot >= GameController.GetShotDelay())
            {
                CmdFire();
                timeSinceLastShot = 0.0f;
            }
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

		if (dir == "up" && !gravInv) {
			gravInv = true;
            playerRigidBody.gravityScale = -1 * GameController.GetGravity();
			transform.localScale = new Vector3(scale.x, -1 * scale.y, scale.z);
		} else if (dir == "down" && gravInv){
			gravInv = false;
			playerRigidBody.gravityScale = GameController.GetGravity();
            transform.localScale = new Vector3(scale.x, Mathf.Abs(scale.y), scale.z);
		}
	}

	[Command]
	void CmdChangeWall2(string dir) {
		if (dir == "up") {
			wall2.transform.localPosition = new Vector3 (-4f, 1f, 0f);
		} else {
			wall2.transform.localPosition = new Vector3 (-4f, -0.5f, 0f);
		}
	}

	[ClientRpc]
	void RpcChangeWall1(string dir) {
		if (isServer)
			return;
		if (dir == "up") {
			wall1.transform.localPosition = new Vector3 (4.7f, 0f, 0f);
		} else {
			wall1.transform.localPosition = new Vector3 (4.7f, -2f, 0f);
		}
	}

    [Command]
	void CmdFire()
    {
        // create bullet from prefab
        Vector3 bulletSpawnPosition = bulletSpawn.position;
        GameObject bulletGameObject = (GameObject)Instantiate(bulletPrefab, bulletSpawnPosition, Quaternion.identity);
        
		Bullet bullet = bulletGameObject.GetComponent<Bullet>();
        bullet.setShootingPlayer(playerRigidBody);
        
        // bullet velocity in direction of the mouse
		Vector3 direction = Camera.main.ScreenToWorldPoint(mousePos) - bulletSpawnPosition;
        direction.Normalize();
        float bulletSpeed = this.currentPlayerSpeed + GameController.GetBulletSpeed();
		bulletGameObject.GetComponent<Rigidbody2D>().velocity = new Vector2(direction.x * bulletSpeed, direction.y * bulletSpeed);

        // spawn the bullet on the clients
        NetworkServer.Spawn(bulletGameObject);
		RpcIgnoreCollision(bulletGameObject);
	}

	// update server with mouse position of the client
	[Command]
	void CmdMousePosition(Vector3 pos)
    {
		this.mousePos = pos;
	}

	// prevent bullet from colliding with player that fires it
	[ClientRpc]
	void RpcIgnoreCollision(GameObject bulletGameObject)
    {
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
	void RpcLogMessage(string m)
    {
		Debug.Log(m);
	}

	//[ClientRpc]
	void showMessage(string message)
    {
		if (!isLocalPlayer)
        {
			return;
		}

		GameObject.Find ("DebugMessage").GetComponent<Text>().text = message;
	}

	[Command]
	void CmdMoveRight()
    {
        RpcMoveRight();
	}

	[ClientRpc]
	void RpcMoveRight()
    {
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
		if (isLocalPlayer) {
			playerAnimator.Play ("player1_walk_east");
		} else {
			playerAnimator.Play("player2_walk_east");
		}
		transform.Translate(Vector2.right * currentPlayerSpeed * Time.deltaTime);
	}

	[Command]
	void CmdMoveLeft()
    {
        RpcMoveLeft();
	}

	[ClientRpc]
	void RpcMoveLeft()
    {
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
		if (isLocalPlayer) {
			playerAnimator.Play ("player1_walk_west");
		} else {
			playerAnimator.Play("player2_walk_west");
		}

		transform.Translate(Vector2.left * currentPlayerSpeed * Time.deltaTime);
	}

    [Command]
    void CmdGameEndPanel(bool active)
    {
        RpcGameEndPanel(active);
    }

    [ClientRpc]
    void RpcGameEndPanel(bool active)
    {
        gameEndPanel.SetActive(active);
    }

    [ClientRpc]
    public void RpcUpdateSpeed(float playerHealth)
    {
        float defaultSpeed = GameController.GetDefaultPlayerSpeed();
        float maxSpeed = GameController.GetMaximumPlayerSpeed();

        float speedRange = maxSpeed - defaultSpeed;
        float percentage = 1 - (playerHealth / GameController.GetMaximumPlayerHealth());

        this.currentPlayerSpeed = defaultSpeed + (percentage * speedRange);
    }

    [ClientRpc]
	public void RpcResetPlayerSpeed()
    {
		this.currentPlayerSpeed = GameController.GetDefaultPlayerSpeed();
	}

	void OnCollisionEnter2D(Collision2D col)
    {
		bool chaseMode = GameController.GetDieOnPlayerCollision();
        // only react on collision if it is enabled
        // if enabled, only react on collision once (because the collision is registered at host and client and therefore called 2 times)
		if (chaseMode == false || !isLocalPlayer)
        {
			return;
		}

		GameObject hitObject = col.gameObject;
		if (hitObject.tag == "Player")
        {
            hitObject.GetComponent<Health>().PlayerCollision();
            this.gameObject.GetComponent<Health>().PlayerCollision();
			this.gameObject.GetComponent<Health>().RpcSubstractScoreFromAll();
		}
	}

	public override void OnStartLocalPlayer()
    {
		Animator animator = this.GetComponent<Animator>();
		animator.runtimeAnimatorController = Resources.Load("player1_controller") as RuntimeAnimatorController;

		SpriteRenderer spriteRenderer = this.GetComponent<SpriteRenderer>();
		spriteRenderer.sprite = Resources.Load("player1_15") as Sprite;

//		if (isServer) {
//			wall = GameObject.Find ("wall1");
//		} else {
//			wall = GameObject.Find ("wall2");
//		}
	}
}
