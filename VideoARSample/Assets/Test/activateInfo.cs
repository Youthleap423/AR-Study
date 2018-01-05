using UnityEngine;
using System.Collections;

public class activateInfo : MonoBehaviour {
	public GameObject infoPanel;
	// Use this for initialization
	void Start () {
		//infoPanel = GameObject.Find ("infoPanel");
	}

	public void showInfo () {
	
		infoPanel.SetActive (true);
	}

	public void hideInfo(){
		infoPanel.SetActive (false);
	}

	// Update is called once per frame
	void Update () {
	
	}
}
