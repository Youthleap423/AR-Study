using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class ToastScript : MonoBehaviour {

	public static ToastScript instance;

	public GameObject ToastObj;

	// Use this for initialization
	void Awake () {
		instance = this;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void ToastShow (string message,float sec = 2f){
		ToastObj.GetComponentInChildren<Text> ().text = message;
		StopCoroutine (ToastVisible(sec));
		StartCoroutine (ToastVisible(sec));
	}

	public IEnumerator ToastVisible(float sec){
		ToastObj.SetActive (true);
		yield return new WaitForSeconds (2f);
		ToastObj.SetActive (false);
	}
}
