using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorSettings : MonoBehaviour {

    public Texture2D targetCursorTexture;

	// Use this for initialization
	void Start ()
    {
        Cursor.SetCursor(targetCursorTexture, Vector2.zero, CursorMode.Auto);
    }
	
	void Awake ()
    {
        Cursor.SetCursor(targetCursorTexture, Vector2.zero, CursorMode.Auto);
    }
}
