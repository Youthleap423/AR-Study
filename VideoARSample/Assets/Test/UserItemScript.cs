using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UserItemScript : MonoBehaviour {

	public string firstName = "";
	public string lastName = "";
	public string userName = "";
	public string childclass = "";
	public string imageUrl = "";
	public string pushkey = "";
	public Texture userTexture;
	public RawImage rawImg;
	// Use this for initialization
	void Start () {
		GetComponent<Text> ().text = firstName;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void ItemClick(){
		ProfileScript.IsGalleryFrom = true;
		Camera.main.GetComponent<ProfileScript> ().SetGalleryDelegate ();
		Camera.main.GetComponent<ProfileScript> ().SelectedItemNo = int.Parse (gameObject.name);
		Camera.main.GetComponent<ProfileScript> ().ChildDetail.SetActive (false);
		Camera.main.GetComponent<ProfileScript> ().ChildDetailProfile.SetActive (true);
	}
}
