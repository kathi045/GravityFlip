using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour {

    private static float gravity = 2.0f;            // default gravity
    private static float defaultPlayerSpeed = 4.5f; // default player walk speed
    private static float maxPlayerSpeed = 10.0f;    // default maximum player walk speed
    private static float bulletSpeed = 6.0f;        // default bullet speed
    private static int bulletDamage = 15;           // default damage of a single bullet
    private static int maximumPlayerHealth = 100;   // default player health

    public Text scoreTextPlayer1;
    public Text scoreTextPlayer2;
    public int scorePlayer1;
    public int scorePlayer2;

	// Use this for initialization
	void Start () {
        scorePlayer1 = 0;
        scorePlayer2 = 0;
        UpdateScore();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void UpdateScore()
    {
        scoreTextPlayer1.text = "Player 1:   " + scorePlayer1;
        scoreTextPlayer2.text = "Player 2:   " + scorePlayer2;
    }

    public void AddScore(int newScoreValuePlayer1, int newScoreValuePlayer2)
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
}
