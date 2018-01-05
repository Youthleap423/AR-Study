using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetSchoolReg : MonoBehaviour {

	// Use this for initialization
	void Start () {
		PlayerPrefs.SetString("SchoolLogin","true");
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
