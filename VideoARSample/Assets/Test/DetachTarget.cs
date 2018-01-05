using UnityEngine;
using System.Collections;
using Vuforia;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class DetachTarget : MonoBehaviour {
	private GameObject gameObj;
	public GameObject panel1;
	public static bool track;
	//public RawImage mUISpinner;
	//private Transform ts;
	// Use this for initialization
	void Start () {
	//	track = true;
	}
	
	// Update is called once per frame
	void Update () {


	}

	public void detach (){
	    track = false;
		Debug.Log("AFter clcik"+track);
		TrackerManager.Instance.GetTracker<ObjectTracker>().Stop();

	}
	public void loadScene()
	{		
				panel1.SetActive (true);
		//mUISpinner.rectTransform.Rotate(Vector3.forward, 90.0f * Time.deltaTime);
		SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex);
	}

}
