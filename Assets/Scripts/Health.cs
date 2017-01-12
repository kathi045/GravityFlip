using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class Health : NetworkBehaviour {

    public static int maxHealth = GameController.GetMaximumPlayerHealth();

	[SyncVar (hook = "OnChangeHealth")] public int currentHealth = maxHealth;
	public RectTransform healthbar;

	public void TakeDamage(int amount, bool victory) {
		if (!isServer) {
			return;
		}
        
		currentHealth -= amount;
		if (currentHealth <= 0) {
			currentHealth = maxHealth;
            GetComponentInParent<PlayerController>().RpcIncreaseSpeed(0.5f);
			if (victory == true) {
				RpcAddScore ();
			}
            RpcRespawn();
		}
	}

	// is called when SyncVar is being changed
	void OnChangeHealth(int health) {
		healthbar.sizeDelta = new Vector2(health, healthbar.sizeDelta.y);
	}

	[ClientRpc]
	void RpcRespawn() {
		if (isLocalPlayer) {
			Vector3 newSpawnPosition = Vector3.zero;
            GameObject[] platforms = GameObject.FindGameObjectsWithTag("Platform");
            int spawnIndex = Random.Range(0, platforms.Length);
            newSpawnPosition = platforms[spawnIndex].GetComponent<Transform>().position;
            //newSpawnPosition.y -= 1;
            transform.position = newSpawnPosition;
<<<<<<< HEAD
			GetComponentInParent<PlayerController> ().ResetPlayerSpeed ();
            //GameObject.Find("DebugMessage").GetComponent<Text>().text = "Debug: Position: " + transform.position + " | PlatformIndex: " + spawnIndex + " | Platform length: " + platforms.Length;
=======

            GameObject.Find("DebugMessage").GetComponent<Text>().text = "Debug: Position: " + transform.position + " | PlatformIndex: " + spawnIndex + " | Platform length: " + platforms.Length;
>>>>>>> 0e42a6d63e3e6c62c3e7722b016b75e0114e27eb
		}
	}

    [ClientRpc]
	void RpcAddScore()
    {
		if (isLocalPlayer) {
			GameController.AddScore (0, 100);
		} else {
			GameController.AddScore (100, 0);
		}

    }
}
