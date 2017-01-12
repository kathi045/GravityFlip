using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class Health : NetworkBehaviour {

    public static int maxHealth = GameController.GetMaximumPlayerHealth();

	[SyncVar (hook = "OnChangeHealth")] public int currentHealth = maxHealth;
	public RectTransform healthBar;

	public void TakeDamage(int amount, bool victory)
    {
		if (!isServer)
        {
			return;
		}
        
		currentHealth -= amount;
        GetComponentInParent<PlayerController>().RpcUpdateSpeed(currentHealth);

        if (currentHealth <= 0)
        {
			currentHealth = maxHealth;
            GetComponentInParent<PlayerController>().RpcResetPlayerSpeed();
			if (victory == true)
            {
				RpcAddScore();
			}

            RpcRespawn();
		}
	}

	// is called when SyncVar is being changed
	void OnChangeHealth(int health)
    {
		healthBar.sizeDelta = new Vector2(health, healthBar.sizeDelta.y);
	}

	[ClientRpc]
	void RpcRespawn()
    {
		if (isLocalPlayer)
        {
			Vector3 newSpawnPosition = Vector3.zero;
            GameObject[] platforms = GameObject.FindGameObjectsWithTag("Platform");
            int spawnIndex = Random.Range(0, platforms.Length);
            newSpawnPosition = platforms[spawnIndex].GetComponent<Transform>().position;
            //newSpawnPosition.y -= 1;
            transform.position = newSpawnPosition;
            //GameObject.Find("DebugMessage").GetComponent<Text>().text = "Debug: Position: " + transform.position + " | PlatformIndex: " + spawnIndex + " | Platform length: " + platforms.Length;
		}
	}

    [ClientRpc]
	void RpcAddScore()
    {
		if (isLocalPlayer)
        {
			GameController.AddScore(0, GameController.GetPointsForKill());
		} else
        {
			GameController.AddScore(GameController.GetPointsForKill(), 0);
		}

    }
}
