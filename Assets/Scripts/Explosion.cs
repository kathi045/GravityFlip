using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : MonoBehaviour {

	// Use this for initialization
	void Start () {
		ParticleSystem exp = GetComponent<ParticleSystem> ();
		exp.Play ();
		Destroy (gameObject, exp.main.duration);
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
