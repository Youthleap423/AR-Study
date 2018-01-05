using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.SceneManagement;

public class DownloadedChapters : MonoBehaviour {

	public string[] BookVideos;
	public GameObject BookName;
	public GameObject loading;
	bool IsCount = false;
	// Use this for initialization
	void Start () {
		
		for (int i = 0; i < BookVideos.Length; i++) {
			if (!File.Exists (Application.persistentDataPath + "/" + BookVideos[i])) {
				IsCount = true;
				Debug.Log(Application.persistentDataPath + "/" + BookVideos[i]);
			}
		}
		Debug.Log(IsCount);
		if (IsCount){
			BookName.SetActive (false);
		}else
			BookName.SetActive (true);

		//IsCount = false;
	}

	public void DeleteVideo(){
		loading.SetActive(true);
		for (int i = 0; i < BookVideos.Length; i++) {
			File.Delete(Application.persistentDataPath+"/"+BookVideos[i]);
		}
		loading.SetActive(false);

		SceneManager.LoadScene("ProfileView");
		}                                                                                                                                                                                                                                       
	public void CloseClick(){
		gameObject.SetActive (false);
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
