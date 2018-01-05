using UnityEngine;
using System.Collections;

public class showHide : MonoBehaviour {
	public GameObject g1;
	public GameObject g2;



	// Use this for initialization
	void Awake () {

	}


	public void toggle(){
		if (g1.activeSelf == isActiveAndEnabled) {
		
			g1.SetActive (false);
			g2.SetActive (true);

		} else {
			g1.SetActive (true);
			g2.SetActive (false);
		}
	
	}


	// Update is called once per frame
	void Update () {
	
	}
}
