using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Bullet : NetworkBehaviour {

	void OnCollisionEnter2D(Collision2D col) {
		GameObject hit = col.gameObject;			// gameobject that was hit by the bullet
		Health health = hit.GetComponent<Health> ();

		if (health != null) {
			health.TakeDamage (10);
		}

		Destroy (gameObject);
	}
}
