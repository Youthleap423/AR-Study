using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
public class ClssromBedrom : MonoBehaviour {
	public PlayerPrefs env;

	public GameObject classroom;
	public GameObject bedroom;
	public GameObject panel1;
	private GameObject video;
	private GameObject currentEnvObj;
	private GameObject vid;
	public GameObject downloadingpanel;

	public void setEnvironment(){
		string name = EventSystem.current.currentSelectedGameObject.name; 
		Debug.Log (name);
		PlayerPrefs.SetString ("env", name);
		SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
		panel1.SetActive (true);
	}


	// Use this for initialization
	void Start () {
//		video = GameObject.Find("Video");

		//PlayerPrefs.DeleteAll ();
//		string currentEnv = PlayerPrefs.GetString("env");
//		Debug.Log ("currentEnv" + currentEnv);
//		if (currentEnv == "") {
//			PlayerPrefs.SetString ("env", "bedroom");
//			currentEnv = PlayerPrefs.GetString("env");
//			Debug.Log ("asjdlaskjdlka"+currentEnv);
//		}
//		if(currentEnv == "bedroom"){
//
//			bedroom.SetActive(true);
//			classroom.SetActive(false);
//		}
//		else if(currentEnv == "classroom")
//		{
//			bedroom.SetActive(false);
//			classroom.SetActive(true);
//		}
//		currentEnvObj = GameObject.Find(currentEnv);

	
//		video.transform.parent = currentEnvObj.transform;			
//		Debug.Log (currentEnvObj);



	}
	// Update is called once per frame
	void Update () {

		if (Input.GetKeyUp (KeyCode.Escape) && !downloadingpanel.activeSelf)
			SceneManager.LoadScene ("ProfileView");
	}
}
