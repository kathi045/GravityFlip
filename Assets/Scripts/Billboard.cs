using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Billboard : MonoBehaviour {

	void Update () {
		Vector3 scale = transform.localScale;
		scale.x = 0.0035f;
		transform.localScale = scale;
	}
}
