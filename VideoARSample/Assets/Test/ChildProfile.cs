using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.IO;

public class ChildProfile : MonoBehaviour {
	public InputField firstNameInput;
	public InputField lastNameInput;
	public InputField usertNameInput;
	public InputField childClassInput;
	public Dropdown childClassdropdown;
	public RawImage UserRawImage;
	public Texture defaultTexture;
	public GameObject StudyARBtn;
	public GameObject ARDrawingBtn;
	public GameObject DownloadingBtn;
	public GameObject UpdateProfileBtn;
	public GameObject EditBtn;
	public static bool IsUpdateProfile;
	private string fileContents;
	public GameObject Loading;
	public static int CurrentClass;
	public static string fileName;

	// Use this for initialization
	void OnEnable () {
		UpdateProfileBtn.SetActive (false);
		EditBtn.SetActive (true);
		IsUpdateProfile = true;
		UserItemScript itemscript = Camera.main.GetComponent<ProfileScript> ().childrenlist [Camera.main.GetComponent<ProfileScript> ().SelectedItemNo].GetComponent<UserItemScript> ();
		firstNameInput.text = itemscript.firstName;
		lastNameInput.text = itemscript.lastName;
		usertNameInput.text = itemscript.userName;
		childClassInput.text = itemscript.childclass;
		fileContents = itemscript.imageUrl;
		for (int i = 0; i < childClassdropdown.options.Count; i++) {
			if (childClassdropdown.options [i].text == itemscript.childclass)
				childClassdropdown.value = i;
		}
		if (itemscript.userTexture == null) {
			UserRawImage.texture = defaultTexture;
			UserRawImage.transform.parent.parent.GetComponent<Text> ().text = itemscript.firstName;
		} else {
			UserRawImage.texture = itemscript.userTexture;
			UserRawImage.transform.parent.parent.GetComponent<Text> ().text = "";
		}
		AllInactive ();
	}

	public void EditClick(){
		UpdateProfileBtn.SetActive (true);
		EditBtn.SetActive (false);
		firstNameInput.enabled = true;
		lastNameInput.enabled = true;
		usertNameInput.enabled = true;
		childClassInput.enabled = true;
		childClassdropdown.enabled = true;
		StudyARBtn.SetActive (false);
		ARDrawingBtn.SetActive (false);
		DownloadingBtn.SetActive (false);
		UserRawImage.GetComponentInParent<Button> ().enabled = true;
	}

	public void imageHandle(string imagepath)
	{
		fileContents = imagepath;
		StartCoroutine (SetImage());
	}

	public IEnumerator SetImage(){
		UserRawImage.transform.GetChild (0).gameObject.SetActive (true);
		byte[] byteArray = File.ReadAllBytes (fileContents);
		Texture2D texture = new Texture2D (8, 8);
		texture.LoadImage (byteArray);
		yield return 0;
		UserRawImage.texture = texture;
		UserRawImage.transform.parent.parent.GetComponent<Text> ().text = "";
		UserRawImage.transform.GetChild (0).gameObject.SetActive (false);
	}

	public void UserImageClick(){
		SAGAndroidPlugin.getInstance().openGallery("Image");
	}

	public void BackClick(){
	}

	public void UpdateClick(){
		UserItemScript itemscript = Camera.main.GetComponent<ProfileScript> ().childrenlist [Camera.main.GetComponent<ProfileScript> ().SelectedItemNo].GetComponent<UserItemScript> ();
		StartCoroutine(Camera.main.GetComponent<ProfileScript> ().UpdateUploadToFirebaseStorage (firstNameInput.text,lastNameInput.text,usertNameInput.text,childClassdropdown.options[childClassdropdown.value].text,fileContents,itemscript.pushkey));
	}

	public void AllInactive(){
		UpdateProfileBtn.SetActive (false);
		EditBtn.SetActive (true);
		firstNameInput.enabled = false;
		lastNameInput.enabled = false;
		usertNameInput.enabled = false;
		childClassInput.enabled = false;
		childClassdropdown.enabled = false;
		StudyARBtn.SetActive (true);
		DownloadingBtn.SetActive (true);
		ARDrawingBtn.SetActive (true);
		UserRawImage.GetComponentInParent<Button> ().enabled = false;
	}

	public void StudyARClick(){
		Loading.SetActive(true);
		SceneManager.LoadScene ("Index");
	}


		
	// Update is called once per frame
	void Update () {
		
	}

	public void OnDisable(){
		IsUpdateProfile = false;
	}
}
