using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Bullet : NetworkBehaviour {

    private Rigidbody2D shootingPlayer = null;
	public GameObject explosionPrefab;

	void OnCollisionEnter2D(Collision2D col)
    {
		GameObject hitObject = col.gameObject;			// gameobject that was hit by the bullet
		Health health = hitObject.GetComponent<Health>();
		Rigidbody2D hitObjectRb = hitObject.GetComponent<Rigidbody2D>();

        if (health != null)
        {
			if (hitObjectRb != shootingPlayer)
            {
				health.TakeDamage(GameController.GetBulletDamage(), true);
			}
		}
		playExplosion ();
		Destroy (gameObject);
	}

    public void setShootingPlayer(Rigidbody2D shootingPlayer)
    {
        this.shootingPlayer = shootingPlayer;
    }

	void playExplosion() {
		Instantiate (explosionPrefab, transform.position, Quaternion.identity);
	}
}
