using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour {

	void OnCollisionEnter2D(Collision2D col) {
		GameObject hit = col.gameObject;
		Health health = hit.GetComponent<Health> ();

		if (health != null) {
			health.TakeDamage (10);
		}

		Destroy (gameObject);
	}
}
