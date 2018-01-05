using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateLoading : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		transform.Rotate(Vector3.back *6f, Space.World);
	}
}
