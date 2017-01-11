using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;
using UnityEngine.UI;

public class CustomNetworkManager : MonoBehaviour {

    public void StartHost()
    {
        NetworkManager.singleton.StartHost();
    }

    public void StartClient()
    {
        string ipAddress = GameObject.Find("IpAddressText").GetComponent<Text>().text;
        if (ipAddress != null && ipAddress.Length > 0)
        {
            NetworkManager.singleton.networkAddress = ipAddress;
            NetworkManager.singleton.StartClient();
        }
    }

}
