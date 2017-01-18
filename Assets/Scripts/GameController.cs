using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class GameController : NetworkBehaviour {

    private static float gravity = 2.0f;                // default gravity
    private static float defaultPlayerSpeed = 6.0f;     // default player walk speed
    private static float maxPlayerSpeed = 12.0f;        // default maximum player walk speed
    private static float bulletSpeed = 2.0f;            // default bullet speed (added to current player speed)
    private static int bulletDamage = 15;               // default damage of a single bullet
    private static int maximumPlayerHealth = 100;       // default player health
    private static bool dieOnPlayerCollision = true;    // default for chase mode
    private static int pointsForKill = 100;             // default points for a player kill
    private static int pointsForWin = 1000;             // default points for winning the game
    private static float shotDelay = 0.3f;              // default waiting time between shots
    private static bool gameEnded = false;              // flag for game end

    public static Text scoreTextPlayer1;
    public static Text scoreTextPlayer2;
    public static int scorePlayer1;
    public static int scorePlayer2;

    // Use this for initialization
    void Start ()
    {
        scorePlayer1 = 0;
        scorePlayer2 = 0;
        scoreTextPlayer1 = GameObject.Find("ScorePlayer1").GetComponent<Text>();
        scoreTextPlayer2 = GameObject.Find("ScorePlayer2").GetComponent<Text>();
        UpdateScore();
    }
	
	// Update is called once per frame
	void Update ()
    {

	}

    public static void UpdateScore()
    {
        scoreTextPlayer1.text = "You:   " + scorePlayer1;
        scoreTextPlayer2.text = "Enemy:   " + scorePlayer2;

        if (scorePlayer1 >= pointsForWin || scorePlayer2 >= pointsForWin)
        {
            gameEnded = true;
        }
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

    public static void SetBulletDamage(int damage)
    {
        bulletDamage = damage;
    }

    public static int GetBulletDamage()
    {
        return bulletDamage;
    }

    public static int GetMaximumPlayerHealth()
    {
        return maximumPlayerHealth;
    }

    public static bool GetDieOnPlayerCollision()
    {
        return dieOnPlayerCollision;
    }

    public static void SetDieOnPlayerCollision(bool activated)
    {
        dieOnPlayerCollision = activated;
    }

    public static int GetPointsForKill()
    {
        return pointsForKill;
    }

    public static int GetPointsForWin()
    {
        return pointsForWin;
    }

    public static void SetPointsForWin(int points)
    {
        pointsForWin = points;
    }

    public static float GetShotDelay()
    {
        return shotDelay;
    }

    public static void SetShotDelay(float delay)
    {
        shotDelay = delay;
    }

    public static bool GetGameEnded()
    {
        return gameEnded;
    }

    public static void SetGameEnded(bool end)
    {
        gameEnded = end;
    }
}
