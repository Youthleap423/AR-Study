using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class showPanel : MonoBehaviour {
	public GameObject panel;
	// Use this for initialization
	void Start () {

	}

	// Update is called once per frame
	void Update () {

	}
	public void show(){
		panel.SetActive(true);
	}
}