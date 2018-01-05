using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine;

public class ForgetPasswordScript : MonoBehaviour {


	protected Firebase.Auth.FirebaseAuth auth;
	private Firebase.Auth.FirebaseAuth otherAuth;
	protected Dictionary<string, Firebase.Auth.FirebaseUser> userByAuth = new Dictionary<string, Firebase.Auth.FirebaseUser>();
	// Set the phone authentication timeout to a minute.
	private uint phoneAuthTimeoutMs = 180 * 1000;
	// The verification id needed along with the sent code for phone authentication.
	private string phoneAuthVerificationId;
	bool sentcode;
	// Options used to setup secondary authentication object.
	private Firebase.AppOptions otherAuthOptions = new Firebase.AppOptions {
		ApiKey = "",
		AppId = "",
		ProjectId = ""
	};

	const int kMaxLogSize = 16382;
	Firebase.DependencyStatus dependencyStatus = Firebase.DependencyStatus.UnavailableOther;

	private Firebase.Auth.ForceResendingToken resendtoken = null;
	public InputField parentEmailInput;
	public GameObject Loading;

	// When the app starts, check to make sure that we have
	// the required dependencies to use Firebase, and if not,
	// add them if possible.
	public void Start() {
		TouchScreenKeyboard.hideInput = true;
		if(!Constants.isInternetConnected())
			ToastScript.instance.ToastShow("No Internet Connection",4f);
		dependencyStatus = Firebase.FirebaseApp.CheckDependencies();
		if (dependencyStatus != Firebase.DependencyStatus.Available) {
			Firebase.FirebaseApp.FixDependenciesAsync().ContinueWith(task => {
				dependencyStatus = Firebase.FirebaseApp.CheckDependencies();
				if (dependencyStatus == Firebase.DependencyStatus.Available) {
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

	// Handle initialization of the necessary firebase modules:
	void InitializeFirebase() {
		Debug.Log("Setting up Firebase Auth");
		auth = Firebase.Auth.FirebaseAuth.DefaultInstance;
		auth.StateChanged += AuthStateChanged;
		auth.IdTokenChanged += IdTokenChanged;
		// Specify valid options to construct a secondary authentication object.
		if (otherAuthOptions != null &&
			!(string.IsNullOrEmpty(otherAuthOptions.ApiKey) ||
				string.IsNullOrEmpty(otherAuthOptions.AppId) ||
				string.IsNullOrEmpty(otherAuthOptions.ProjectId))) {
			try {
				otherAuth = Firebase.Auth.FirebaseAuth.GetAuth(Firebase.FirebaseApp.Create(
					otherAuthOptions, "Secondary"));
				otherAuth.StateChanged += AuthStateChanged;
				otherAuth.IdTokenChanged += IdTokenChanged;

			} catch (Exception e) {
				Debug.Log("ERROR: Failed to initialize secondary authentication object.");
			}
		}
		AuthStateChanged(this, null);
	}

	// Update is called once per frame
	void Update () {

		if (Input.GetKeyUp (KeyCode.Escape)) {
			SceneManager.LoadScene ("Login");
		}

	}


	public void ResetPasswordClick(){
		sentcode = false;
		if (Constants.isInternetConnected ()) {
			if ( Constants.IsValidEmailAddress(parentEmailInput.text)) {
				Loading.SetActive (true);
				SendPasswordResetEmail ();
			} else
				ToastScript.instance.ToastShow ("Enter In-valid Password", 4f);
		}else
			ToastScript.instance.ToastShow ("No Internet Connection", 4f);

	}

	// Send a password reset email to the current email address.
	public void SendPasswordResetEmail() {
		auth.SendPasswordResetEmailAsync(parentEmailInput.text).ContinueWith((authTask) => {

			if (authTask.IsCanceled) {
				Debug.Log("Send Password Reset Email canceled.");
				Loading.SetActive (false);
				ToastScript.instance.ToastShow("Send Password Reset Email canceled.",4f);
			} else if (authTask.IsFaulted) {
				Loading.SetActive (false);
				ToastScript.instance.ToastShow("Enter Email-Id is Not Registered",4f);
			} else if (authTask.IsCompleted) {
				Loading.SetActive (false);
				ToastScript.instance.ToastShow ("Password reset email sent to " + parentEmailInput.text,4f);
				Debug.Log("Password reset email sent to " + parentEmailInput.text);
			}

		});
	}

	// Log the result of the specified task, returning true if the task
	// completed successfully, false otherwise.
	bool LogTaskCompletion(Task task, string operation) {
		bool complete = false;
		if (task.IsCanceled) {
			Debug.Log(operation + " canceled.");
		} else if (task.IsFaulted) {
			Debug.Log(operation + " encounted an error.");
			Debug.Log(task.Exception.ToString());
		} else if (task.IsCompleted) {
			Debug.Log(operation + " completed");
			complete = true;
		}
		return complete;
	}

	// Track state changes of the auth object.
	void AuthStateChanged(object sender, System.EventArgs eventArgs) {
		print ("fsd");
		Firebase.Auth.FirebaseAuth senderAuth = sender as Firebase.Auth.FirebaseAuth;
		Firebase.Auth.FirebaseUser user = null;
		if (senderAuth != null) userByAuth.TryGetValue(senderAuth.App.Name, out user);
		if (senderAuth == auth && senderAuth.CurrentUser != user) {
			bool signedIn = user != senderAuth.CurrentUser && senderAuth.CurrentUser != null;
			if (!signedIn && user != null) {
				Debug.Log("Signed out " + user.UserId);
			}
			user = senderAuth.CurrentUser;
			userByAuth[senderAuth.App.Name] = user;
			if (signedIn) {
				Debug.Log("Signed in " + user.UserId);
				//				displayName = user.DisplayName ?? "";
				//				DisplayDetailedUserInfo(user, 1);
			}

		}
	}

	// Track ID token changes.
	void IdTokenChanged(object sender, System.EventArgs eventArgs) {
		Firebase.Auth.FirebaseAuth senderAuth = sender as Firebase.Auth.FirebaseAuth;
		if (senderAuth == auth && senderAuth.CurrentUser != null /*&& !fetchingToken*/) {
			senderAuth.CurrentUser.TokenAsync(false).ContinueWith(
				task => Debug.Log(string.Format("Token[0:8] = {0}", task.Result.Substring(0, 8))));
		}
	}
}
