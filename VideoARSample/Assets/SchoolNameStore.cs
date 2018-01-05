using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase;
using Firebase.Database;
using Firebase.Unity.Editor;
using System;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SchoolNameStore : MonoBehaviour {
	public InputField schoolName;
	DependencyStatus dependencyStatus = DependencyStatus.UnavailableOther;
	DatabaseReference reference;
	private string MyStorageBucket = "gs://lbf-ar-books.appspot.com/";
	string Pushid ;

	// Use this for initialization
	void Start () {
		if (PlayerPrefs.GetInt (Constants.IsLogin) == 0) {
			PlayerPrefs.SetInt (Constants.IsLogin, 1);
			PlayerPrefs.SetString (Constants.IsLoginEmailorPhone, Constants.ParentEmail);
			PlayerPrefs.SetString (Constants.IsLoginpassword, Constants.ParentPassword);
		} else {
			Constants.ParentEmail = PlayerPrefs.GetString (Constants.IsLoginEmailorPhone);
			Constants.ParentPassword = PlayerPrefs.GetString (Constants.IsLoginpassword);
		}
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

	void InitializeFirebase() {
		FirebaseApp app = FirebaseApp.DefaultInstance;
		app.SetEditorDatabaseUrl("https://lbf-ar-books.firebaseio.com/");
		if (app.Options.DatabaseUrl != null) app.SetEditorDatabaseUrl(app.Options.DatabaseUrl);
		//GetChildUser ();


		var appBucket = FirebaseApp.DefaultInstance.Options.StorageBucket;
		if (!String.IsNullOrEmpty(appBucket)) {
			MyStorageBucket = String.Format("gs://{0}/", appBucket);
		}
	}


	public void AddChildUser() {
		reference = FirebaseDatabase.DefaultInstance.GetReference ("Child").Child(Constants.ParentUDID);
		Pushid = reference.Push().Key;
		writeNewUser(schoolName.text,Pushid);
	}

	private void writeNewUser(string schoolName,string id) {
		SchoolInfo user = new SchoolInfo( schoolName, id);
		string json = JsonUtility.ToJson(user);
		reference.Child(id).SetRawJsonValueAsync(json).ContinueWith(task => {
			if (task.Exception != null) {
				Debug.Log(task.Exception.ToString());
				//Loading.SetActive (false);
			} else if (task.IsCompleted) {
				Debug.Log("Transaction complete.");
				//GetChildUser ();
				//ChildDetail.SetActive (false);
				//				SAGAndroidPlugin.getInstance().imageDelegate += imageHandle;
			}
		});;
	}
	// Update is called once per frame
	void Update () {
		
	}


	public class SchoolInfo {
		public string schoolName;

		public string id;

		public SchoolInfo() {
		}

		public SchoolInfo(string schoolName,string id) {
			this.schoolName = schoolName;
			this.id = id;
		}
	}

}
