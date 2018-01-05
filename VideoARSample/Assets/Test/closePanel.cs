using UnityEngine;
using System.Collections;

public class closePanel : MonoBehaviour {
	public GameObject gs;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void close(){
		gs.SetActive (false);
	}
}
