﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OptionsMenu : MonoBehaviour {

    public void ChangePointsToWin(float receivedIndex)
    {
        int[] points = { 500, 1000, 1500, 2000, 2500, 3000 };

        int index = (int)receivedIndex;
        if (index >= 0 && index < points.Length)
        {
            GameController.SetPointsForWin(points[index]);
        }

        Debug.Log("Points to win: " + GameController.GetPointsForWin());

    }

    public void ChangeBulletDamage(float receivedIndex)
    {
        int[] damage = { 10, 15, 20, 25, 50, 100 };

        int index = (int)receivedIndex;
        if (index >= 0 && index < damage.Length)
        {
            GameController.SetBulletDamage(damage[index]);
        }

        Debug.Log("Bullet damage: " + GameController.GetBulletDamage());

    }

    public void ChangeChaseMode(bool activated)
    {
        GameController.SetDieOnPlayerCollision(activated);

        Debug.Log("Chase mode: " + GameController.GetDieOnPlayerCollision());
    }
}
