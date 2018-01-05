using Firebase;
using Firebase.Database;
using Firebase.Storage;
using Firebase.Unity.Editor;
using System;
using System.Text;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ProfileScript : MonoBehaviour {

	public GameObject ChildDetail;
	public GameObject ChildDetailProfile;
	public GameObject AddUserBtn;
	public InputField parentEmailInput;
	public InputField parentPasswordInput;
	public InputField parentPhoneInput;
	public InputField firstNameInput;
	public InputField lastNameInput;
	public InputField usertNameInput;
	public InputField childClassInput;
	public Dropdown childClassdropdown;
	public RawImage UserImageInput;
	public GameObject UserItem;
	public GameObject UserItemParent;
	public GameObject Loading;
	public GameObject NoInternetPanel;
	public GameObject DownloadingBtn;
	public GameObject DownloadingPanel;
	public List<GameObject> childrenlist = new List<GameObject>();
	string imageURL;
	DependencyStatus dependencyStatus = DependencyStatus.UnavailableOther;
	DatabaseReference reference;
	string Pushid ;
	public int SelectedItemNo;
	public Texture defaultusertexture; 
	private string MyStorageBucket = "gs://lbf-ar-books.appspot.com/";
	private string firebaseStorageLocation;
	private string fileContents;
	public static bool IsGalleryFrom;

	void Start() {
		PlayerPrefs.SetString("SchoolLogin","false");
		TouchScreenKeyboard.hideInput = true;
		IsGalleryFrom = false;
		if (PlayerPrefs.GetInt (Constants.IsLogin) == 0) {
			PlayerPrefs.SetInt (Constants.IsLogin, 1);
			PlayerPrefs.SetString (Constants.IsLoginEmailorPhone, Constants.ParentEmail);
			PlayerPrefs.SetString (Constants.IsLoginpassword, Constants.ParentPassword);
		} else {
			Constants.ParentEmail = PlayerPrefs.GetString (Constants.IsLoginEmailorPhone);
			Constants.ParentPassword = PlayerPrefs.GetString (Constants.IsLoginpassword);
		}
		parentEmailInput.text = Constants.ParentEmail;
		parentPasswordInput.text = Constants.ParentPassword;
		SAGAndroidPlugin.getInstance().imageDelegate += imageHandle;
		parentEmailInput.enabled = false;
		parentPasswordInput.enabled = false;
		parentPhoneInput.enabled = false;
		dependencyStatus = FirebaseApp.CheckDependencies();
		if (dependencyStatus != DependencyStatus.Available) {
			FirebaseApp.FixDependenciesAsync().ContinueWith(task => {
				dependencyStatus = FirebaseApp.CheckDependencies();
				if (dependencyStatus == DependencyStatus.Available) {
					InitializeFirebase();
				} else {
					Debug.LogError(
						"Could not resolve all Firebase dependencies: " + dependencyStatus);
				}
			});
		} else {
			InitializeFirebase();
		}
	}

	public void imageHandle(/*string message, byte[] data,*/string imagepath)
	{
		if (!IsGalleryFrom) {
			fileContents = imagepath;
			StartCoroutine (SetImage ());
		} else {
			ChildDetailProfile.GetComponent<ChildProfile> ().imageHandle (imagepath);
		}
	}

	public void NoInternetRetryClick(){
		if (Constants.isInternetConnected ()) {
			NoInternetPanel.SetActive (false);
			SceneManager.LoadScene ("ProfileView");
		}
	}

	public void SetGalleryDelegate(){
//		SAGAndroidPlugin.getInstance().imageDelegate -= imageHandle;
	}

	public IEnumerator SetImage(){
		UserImageInput.transform.GetChild (0).gameObject.SetActive (true);
		byte[] byteArray = File.ReadAllBytes (fileContents);
		Texture2D texture = new Texture2D (8, 8);
		texture.LoadImage (byteArray);
		yield return 0;
		UserImageInput.texture = texture;
		UserImageInput.transform.parent.parent.GetComponent<Text> ().text = "";
		UserImageInput.transform.GetChild (0).gameObject.SetActive (false);
	}

	// Initialize the Firebase database:
	void InitializeFirebase() {
		FirebaseApp app = FirebaseApp.DefaultInstance;
		app.SetEditorDatabaseUrl("https://lbf-ar-books.firebaseio.com/");
		if (app.Options.DatabaseUrl != null) app.SetEditorDatabaseUrl(app.Options.DatabaseUrl);
		GetChildUser ();


		var appBucket = FirebaseApp.DefaultInstance.Options.StorageBucket;
		if (!String.IsNullOrEmpty(appBucket)) {
			MyStorageBucket = String.Format("gs://{0}/", appBucket);
		}
	}

	public void DownloadedChapterClick(){
		DownloadingPanel.SetActive (true);
	}

	public void SignOutClick(){
		PlayerPrefs.SetInt (Constants.IsLogin,0);
		PlayerPrefs.DeleteAll ();
		SAGAndroidPlugin.getInstance().imageDelegate -= imageHandle;
		SceneManager.LoadScene ("Login");
	}

	public void GetChildUser() {
		Loading.SetActive (true);
		AddUserBtn.SetActive(false);
		for(int i=0;i < childrenlist.Count;i++){
			Destroy (childrenlist[i]);
		}
		childrenlist.Clear ();
		reference = FirebaseDatabase.DefaultInstance.GetReference ("Child").Child(Constants.ParentUDID);
		DataSnapshot snapshot = null;
		reference.GetValueAsync().ContinueWith(task => {
			if (task.IsFaulted) {
				// Handle the error...
				Debug.Log(task.Exception.ToString());
				Loading.SetActive (false);
			}
			else if (task.IsCompleted) {
				snapshot = task.Result;
				Debug.Log("Transaction complete."+snapshot.ChildrenCount);
				int no = -1;
				foreach(DataSnapshot datasnap in snapshot.Children){
					Debug.Log("Child:"+datasnap.Child(Constants.firstName).Value);
					GameObject item = Instantiate(UserItem);
					item.transform.parent = UserItemParent.transform;
					item.transform.localPosition = new Vector3(no*220f,-500,0f);
					item.transform.localScale = Vector3.one;
					item.GetComponent<UserItemScript>().firstName =datasnap.Child(Constants.firstName).Value.ToString();
					item.GetComponent<UserItemScript>().lastName =datasnap.Child(Constants.lastName).Value.ToString();
					item.GetComponent<UserItemScript>().userName =datasnap.Child(Constants.userName).Value.ToString();
					item.GetComponent<UserItemScript>().childclass =datasnap.Child(Constants.childclass).Value.ToString();
					item.GetComponent<UserItemScript>().imageUrl =datasnap.Child(Constants.imageUrl).Value.ToString();
					item.GetComponent<UserItemScript>().pushkey =datasnap.Child(Constants.pushkey).Value.ToString();

					childrenlist.Add(item);
					if(!string.IsNullOrEmpty(datasnap.Child(Constants.imageUrl).Value.ToString()))
						StartCoroutine(SetImagefromServer(datasnap.Child(Constants.imageUrl).Value.ToString(),item.GetComponent<UserItemScript>()));
					no++;
					item.name = no.ToString();
				}
				print("No:"+no);
				if(no == 2)
					AddUserBtn.SetActive(false);
				else
					AddUserBtn.SetActive(true);
				Loading.SetActive (false);
			}
		});
	}

	IEnumerator SetImagefromServer(string URL,UserItemScript itemscript){
//		itemscript.gameObject.SetActive (true);
		WWW www = new WWW (URL);
		yield return www;
		itemscript.rawImg.texture = www.texture;
		itemscript.userTexture = www.texture;
//		itemscript.rawImg.GetComponentInChildren<Text> ().text = "";
//		itemscript.gameObject.SetActive (false);
	}

	IEnumerator UploadToFirebaseStorage() {
		Loading.SetActive (true);
		firebaseStorageLocation = MyStorageBucket + fileContents.Substring(fileContents.LastIndexOf('/')+1);
		StorageReference reference = FirebaseStorage.DefaultInstance
			.GetReferenceFromUrl(firebaseStorageLocation);
		if (string.IsNullOrEmpty (fileContents)) {
			AddChildUser ();
		} else {
			byte[] byteArray = File.ReadAllBytes (fileContents);
			var task = reference.PutBytesAsync (byteArray);
			yield return new WaitUntil (() => task.IsCompleted);
			if (task.IsFaulted) {
				Debug.Log (task.Exception.ToString ());
				Loading.SetActive (false);
			} else {
				imageURL = task.Result.DownloadUrl.ToString ();
				AddChildUser ();
				Debug.Log ("Finished uploading... Download Url: " + task.Result.DownloadUrl.ToString ());
				Debug.Log ("Press the Download button to download text from Cloud Storage");
			}
		}
	}


	public void AddChildUser() {
		reference = FirebaseDatabase.DefaultInstance.GetReference ("Child").Child(Constants.ParentUDID);
		Pushid = reference.Push().Key;
		writeNewUser(firstNameInput.text,lastNameInput.text,usertNameInput.text,childClassdropdown.options[childClassdropdown.value].text,imageURL,Pushid);
	}

	public void UserImageClick(){
		SAGAndroidPlugin.getInstance().openGallery("Image");
	}

	private void writeNewUser(string firstName, string lastName,string userName,string childclass,string imageUrl,string id) {
		ChildInfo user = new ChildInfo( firstName,  lastName, userName, childclass, imageUrl, id);
		string json = JsonUtility.ToJson(user);
		reference.Child(id).SetRawJsonValueAsync(json).ContinueWith(task => {
			if (task.Exception != null) {
				Debug.Log(task.Exception.ToString());
				Loading.SetActive (false);
			} else if (task.IsCompleted) {
				Debug.Log("Transaction complete.");
				GetChildUser ();
				ChildDetail.SetActive (false);
//				SAGAndroidPlugin.getInstance().imageDelegate += imageHandle;
			}
		});;
	}

	public IEnumerator UpdateUploadToFirebaseStorage(string firstName, string lastName,string userName,string childclass,string contentfile,string id) {
		Loading.SetActive (true);
		StorageReference reference = null;
		if (!contentfile.Contains ("https://")) {
			firebaseStorageLocation = MyStorageBucket + contentfile.Substring (contentfile.LastIndexOf ('/') + 1);
			reference = FirebaseStorage.DefaultInstance.GetReferenceFromUrl (firebaseStorageLocation);
		}
		if (contentfile.Contains("https://") || string.IsNullOrEmpty (contentfile)) {
			UpdateChildUser (firstName,lastName,userName,childclass,contentfile,id);
		} else {
			byte[] byteArray = File.ReadAllBytes (contentfile);
			var task = reference.PutBytesAsync (byteArray);
			yield return new WaitUntil (() => task.IsCompleted);
			if (task.IsFaulted) {
				Debug.Log (task.Exception.ToString ());
				Loading.SetActive (false);
			} else {
				contentfile = task.Result.DownloadUrl.ToString ();
				UpdateChildUser (firstName,lastName,userName,childclass,contentfile,id);
				Debug.Log ("Finished uploading... Download Url: " + task.Result.DownloadUrl.ToString ());
				Debug.Log ("Press the Download button to download text from Cloud Storage");
			}
		}
	}


	public void UpdateChildUser(string firstName, string lastName,string userName,string childclass,string contentfile,string id) {
		reference = FirebaseDatabase.DefaultInstance.GetReference ("Child").Child(Constants.ParentUDID);
		writeUpdateUser(firstName,lastName,userName,childclass,contentfile,id);
	}

	public void writeUpdateUser(string firstName, string lastName,string userName,string childclass,string contentfile,string id) {
		ChildInfo user = new ChildInfo( firstName,  lastName, userName, childclass, contentfile, id);
		string json = JsonUtility.ToJson(user);
		reference.Child(id).SetRawJsonValueAsync(json).ContinueWith(task => {
			if (task.Exception != null) {
				Debug.Log(task.Exception.ToString());
				Loading.SetActive (false);
			} else if (task.IsCompleted) {
				Debug.Log("Update complete.");
				ChildDetailProfile.GetComponent<ChildProfile>().AllInactive();
				GetChildUser ();
				Loading.SetActive (false);
			}
		});;
	}


	public void AddUserClick(){
		firstNameInput.text = "";
		lastNameInput.text = "";
		usertNameInput.text = "";
		childClassInput.text = "";
		childClassdropdown.value = 0;
		imageURL = "";
		fileContents = "";
		ChildDetail.SetActive (true);
		SetGalleryDelegate ();
		IsGalleryFrom = false;
		UserImageInput.texture = defaultusertexture;
		UserImageInput.transform.parent.parent.GetComponent<Text> ().text = "User";
	}

	public void SaveClick(){
//		AddChildUser ();
		StartCoroutine(UploadToFirebaseStorage());
	}

	// Update is called once per frame
	void Update () {
		if (Input.GetKeyUp (KeyCode.Escape)) {
			if (Loading.activeSelf == false) {
				if (DownloadingPanel.activeSelf) {
					DownloadingPanel.SetActive (false);
				} else if (ChildDetailProfile.activeSelf) {
					ChildDetail.SetActive (false);
					ChildDetailProfile.SetActive (false);
//					SAGAndroidPlugin.getInstance().imageDelegate += imageHandle;
				} else if (ChildDetail.activeSelf) {
					ChildDetail.SetActive (false);
					ChildDetailProfile.SetActive (false);
//					SAGAndroidPlugin.getInstance().imageDelegate += imageHandle;
				} else {
					Application.Quit ();
				}
			}
		}

		if (!Constants.isInternetConnected ()) {
			NoInternetPanel.SetActive (true);
		}
	}

	void OnDestroy() {
		SAGAndroidPlugin.getInstance().imageDelegate -= imageHandle;
	}
}

public class ChildInfo {
	public string firstName;
	public string lastName;
	public string userName;
	public string childclass;
	public string imageUrl;
	public string id;

	public ChildInfo() {
	}

	public ChildInfo(string firstName, string lastName,string userName,string childclass,string imageUrl,string id) {
		this.firstName = firstName;
		this.lastName = lastName;
		this.userName = userName;
		this.childclass = childclass;
		this.imageUrl = imageUrl;
		this.id = id;
	}
}