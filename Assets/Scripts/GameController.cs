using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class GameController : NetworkBehaviour {

    private static float gravity = 2.0f;                // default gravity
    private static float defaultPlayerSpeed = 6.0f;     // default player walk speed
    private static float maxPlayerSpeed = 12.0f;        // default maximum player walk speed
    private static float bulletSpeed = 6.0f;            // default bullet speed
    private static int bulletDamage = 15;               // default damage of a single bullet
    private static int maximumPlayerHealth = 100;       // default player health
    private bool dieOnPlayerCollision = true;           // default for chase mode

    public static Text scoreTextPlayer1;
    public static Text scoreTextPlayer2;
    public static int scorePlayer1;
    public static int scorePlayer2;

	// Use this for initialization
	void Start () {
        scorePlayer1 = 0;
        scorePlayer2 = 0;
        scoreTextPlayer1 = GameObject.Find("ScorePlayer1").GetComponent<Text>();
        scoreTextPlayer2 = GameObject.Find("ScorePlayer2").GetComponent<Text>();
        UpdateScore();
	}
	
	// Update is called once per frame
	void Update () {

	}

	[Command]
	public void CmdUpdateScore() {
		scoreTextPlayer1.text = "You:   " + scorePlayer1;
		scoreTextPlayer2.text = "Enemy:   " + scorePlayer2;
	}

    public static void UpdateScore()
    {
        scoreTextPlayer1.text = "You:   " + scorePlayer1;
        scoreTextPlayer2.text = "Enemy:   " + scorePlayer2;
    }

    public static void AddScore(int newScoreValuePlayer1, int newScoreValuePlayer2)
    {
        scorePlayer1 += newScoreValuePlayer1;
        scorePlayer2 += newScoreValuePlayer2;
        UpdateScore();
    }

    public static float GetGravity()
    {
        return gravity;
    }

    public static float GetMaximumPlayerSpeed()
    {
        return maxPlayerSpeed;
    }

    public static float GetDefaultPlayerSpeed()
    {
        return defaultPlayerSpeed;
    }

    public static float GetBulletSpeed()
    {
        return bulletSpeed;
    }

    public static int GetBulletDamage()
    {
        return bulletDamage;
    }

    public static int GetMaximumPlayerHealth()
    {
        return maximumPlayerHealth;
    }

    public bool GetDieOnPlayerCollision()
    {
        return dieOnPlayerCollision;
    }

    public void SetDieOnPlayerCollision(bool chaseModeActivated)
    {
		dieOnPlayerCollision = chaseModeActivated;
    }
}
