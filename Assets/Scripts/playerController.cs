using UnityEngine;
using System.Collections;

public class playerController : MonoBehaviour {

    public int speed;
    public int jumpSpeed;

    private Animator playerAnimator;
	private GameObject floor;
    private ArrayList platforms;
	private Rigidbody2D playerRigidBody;
	private bool canJump;

	// Use this for initialization
	void Start() {
		playerAnimator = this.GetComponent<Animator>();
		playerRigidBody = this.GetComponent<Rigidbody2D>();
		floor = GameObject.Find("Floor");
        platforms = new ArrayList(GameObject.FindGameObjectsWithTag("Platform"));
		canJump = false;
	}
	
	// Update is called once per frame
	void FixedUpdate() {

		transform.rotation = Quaternion.identity;

        // cancel all animation if no input is received
		if (Input.anyKey == false) {
			playerAnimator.speed = 0;
		}
		
        // walk left
		if (Input.GetKey(KeyCode.A)) {
            playerAnimator.speed = 1;
            playerAnimator.SetInteger("direction", 1);
			playerAnimator.Play("player_walk_west");
			transform.Translate(Vector2.left * speed * Time.deltaTime);
		}

        // walk right
        if (Input.GetKey(KeyCode.D)) {
            playerAnimator.speed = 1;
            playerAnimator.SetInteger("direction", 3);
			playerAnimator.Play("player_walk_east");
			transform.Translate(Vector2.right * speed * Time.deltaTime);
		}

        // jump
		if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.W)) {
			if (canJump) {
				playerRigidBody.AddForce(Vector2.up * jumpSpeed);
                canJump = false;
			}
		}
	}

	void OnCollisionEnter2D(Collision2D col) {
        GameObject collisionObject = col.gameObject;
		if (collisionObject == floor || platforms.Contains(collisionObject)) {
			canJump = true;
		} else {
			canJump = false;
		}
	}
}
