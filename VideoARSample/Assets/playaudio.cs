using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playaudio : MonoBehaviour {
	int counter=0;
	public AudioSource sound;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetMouseButtonDown(0))
			counter ++; 
	}
	public void audPLA()
	{
		if (counter % 2 != 0) {
		
			sound.Play ();
		} else {
		
			sound.Stop ();
		}

	}
}
