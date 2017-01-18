using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class BackToMainMenuOnClick : MonoBehaviour {

	public void BackToMainMenu()
    {
        GameController.SetGameEnded(false);
        NetworkManager.singleton.StopHost();
    }
	
}
