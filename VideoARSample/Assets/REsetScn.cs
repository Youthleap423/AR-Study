using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class REsetScn : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void rstSCen()
	{
		for (int i = 0; i < Camera.main.GetComponent<GameController> ().Targets.Length; i++) {
			Camera.main.GetComponent<GameController>().Targets[i].SetActive(false);
			Camera.main.GetComponent<GameController>().Targets[i].SetActive(true);
		}
			
	}
}
