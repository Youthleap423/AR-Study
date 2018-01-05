using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SplashScript : MonoBehaviour {

	// Use this for initialization
	IEnumerator Start () {
			yield return new WaitForSeconds (2f);
			SceneManager.LoadScene ("Login");
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
