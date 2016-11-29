using UnityEngine;
using System.Collections;

public class playerController : MonoBehaviour {

	private Animator animator;
	private GameObject floor;
	private Rigidbody2D rb;

	private bool canJump;
	public int speed;
	public int jumpSpeed;

	// Use this for initialization
	void Start () {
		animator = this.GetComponent<Animator> ();
		rb = this.GetComponent<Rigidbody2D> ();
		floor = GameObject.Find ("Floor");
		canJump = false;
	}
	
	// Update is called once per frame
	void FixedUpdate () {

		transform.rotation = Quaternion.identity;

		if (Input.anyKey == false) {
			animator.speed = 0;
		} else {
			animator.speed = 1;
		}
		
		if (Input.GetKey (KeyCode.W)) {
			// play animation
			animator.SetInteger ("direction", 2);
			animator.Play ("player_walk_north");
		} else if (Input.GetKey (KeyCode.S)) {
			// play animation
			animator.SetInteger ("direction", 0);
			animator.Play ("player_walk_south");
		} else if (Input.GetKey (KeyCode.A)) {
			// play animation
			animator.SetInteger ("direction", 1);
			animator.Play ("player_walk_west");
			// walk left
			transform.Translate(Vector2.left * speed * Time.deltaTime);
		} else if(Input.GetKey(KeyCode.D)) {
			// play animation
			animator.SetInteger ("direction", 3);
			animator.Play ("player_walk_east");
			// walk right
			transform.Translate(Vector2.right * speed * Time.deltaTime);
		}

		if (Input.GetKeyDown (KeyCode.Space)) {
			if (canJump) {
				rb.AddForce(Vector2.up * jumpSpeed);
			}
		}
			

	}

	void OnCollisionEnter2D(Collision2D col) {
		if (col.gameObject == floor) {
			canJump = true;
		} else {
			canJump = false;
		}
	}
}
