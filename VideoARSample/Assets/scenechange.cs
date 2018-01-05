using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class scenechange : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyUp (KeyCode.Escape)) {
			if(SceneManager.GetActiveScene().name == "ui-1")
				 SceneManager.LoadScene ("ProfileView");
			else if(SceneManager.GetActiveScene().name != "Index")
				SceneManager.LoadScene ("ui-1");
		}
	}
	public void scene1()
	{
		SceneManager.LoadScene ("Tiger");
	}
	public void scene2()
	{
		SceneManager.LoadScene ("Hippo");
	}
	public void scene3()
	{
		SceneManager.LoadScene ("Lion");
	}
	public void scene4()
	{
		SceneManager.LoadScene ("Giraffe");
	}
	public void scene5()
	{
		SceneManager.LoadScene ("Elephant");
	}
	public void scene0()
	{
		SceneManager.LoadScene ("ui-1");
	}
	public void scen()
	{
		if(PlayerPrefs.GetString("SchoolLogin") == "true") {
		SceneManager.LoadScene ("SchoolProfile");
		} else
		SceneManager.LoadScene ("ProfileView");
	}
	public void loadIndex()
	{
		SceneManager.LoadScene ("Index");
	}
	public void schooolRegistration(){
		SceneManager.LoadScene ("SchoolRegistration");
	}
	public void login(){
		SceneManager.LoadScene ("Login");
	}
	public void Registration(){
		SceneManager.LoadScene ("Registration");
	}
	public void schoolLogin(){
		SceneManager.LoadScene ("SchoolLogin");
	}

}
