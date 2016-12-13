using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Bullet : NetworkBehaviour {

    private Rigidbody2D shootingPlayer = null;

	void OnCollisionEnter2D(Collision2D col) {
		GameObject hit = col.gameObject;			// gameobject that was hit by the bullet
		Health health = hit.GetComponent<Health>();
        Rigidbody2D hitPlayer = hit.GetComponent<Rigidbody2D>();

        if (health != null)
        {
            if (hitPlayer != shootingPlayer)
            {
                health.TakeDamage(10);
            }
		}

        Destroy(gameObject);
	}

    public void setShootingPlayer(Rigidbody2D shootingPlayer)
    {
        this.shootingPlayer = shootingPlayer;
    }
}
