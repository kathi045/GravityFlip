using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class Health : NetworkBehaviour {

    public static int maxHealth = GameController.GetMaximumPlayerHealth();

	[SyncVar (hook = "OnChangeHealth")] public int currentHealth = maxHealth;
	public RectTransform healthBar;

    public void PlayerCollision()
    {
        currentHealth = maxHealth;
        GetComponentInParent<PlayerController>().RpcResetPlayerSpeed();
        RpcRespawn(true);
    }

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

            RpcRespawn(false);
		}
	}

	// is called when SyncVar is being changed
	void OnChangeHealth(int health)
    {
		healthBar.sizeDelta = new Vector2(health, healthBar.sizeDelta.y);
	}

	Vector3[] getPlayerPositions()
    {
		GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
		int count = players.Length;
		Debug.Log("count players: " + count);
		Vector3[] positions = new Vector3[count];

		int i = 0;
		foreach (GameObject player in players)
        {
			Debug.Log("player position: " + player.transform.position);
			positions[i++] = player.transform.position;
		}

		return positions;
	}

	[ClientRpc]
	void RpcRespawn(bool playerCollision)
    {
		Vector3[] playerPositions = getPlayerPositions();				// positions of all players
		Vector3 playerPosition = Vector3.zero;							// position of the other player
		foreach (Vector3 pos in playerPositions)
        {
			if (pos != this.transform.position)
            {
				playerPosition = pos;
			}
		}

		if (isLocalPlayer)
        {
			GameObject[] spawnPoints = GameObject.FindGameObjectsWithTag("SpawnPoint");
			float distance;
			float maxDistance = 0.0f;
			Vector3 newSpawnPosition = Vector3.zero;

            // if player position could not be found or players collided, select a random spawn
			if (playerPosition == Vector3.zero || playerCollision)
            {
				int spawnIndex = Random.Range(0, spawnPoints.Length);
				newSpawnPosition = spawnPoints[spawnIndex].GetComponent<Transform>().position;
			}
            // otherwise choose the spawn that is furthest away
            else
            {
				for (int i = 0; i < spawnPoints.Length; i++)
                {
					distance = Vector3.Distance(spawnPoints[i].transform.position, playerPosition);
					if (distance > maxDistance)
                    {
						maxDistance = distance;
						newSpawnPosition = spawnPoints[i].transform.position;
					}
				}
			}
           
            transform.position = newSpawnPosition;
        }
	}

    [ClientRpc]
	void RpcAddScore()
    {
		if (isLocalPlayer)
        {
			GameController.AddScore(0, GameController.GetPointsForKill());
		}
        else
        {
			GameController.AddScore(GameController.GetPointsForKill(), 0);
		}

    }

	[ClientRpc]
	public void RpcSubstractScoreFromAll()
    {
		GameController.AddScore(-GameController.GetPointsForKill(), -GameController.GetPointsForKill());
	}
}
