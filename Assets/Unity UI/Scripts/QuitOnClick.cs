using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuitOnClick : MonoBehaviour {

	public void Quit() {
#if UNITY_EDITOR
        // if in Unity's editor play mode
        UnityEditor.EditorApplication.isPlaying = false;
#else
        // if in a built application
        Application.Quit();
#endif
    }
	
}
