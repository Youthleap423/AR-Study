using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine;

public class RegistraionScript : MonoBehaviour {


	protected Firebase.Auth.FirebaseAuth auth;
	private Firebase.Auth.FirebaseAuth otherAuth;
	protected Dictionary<string, Firebase.Auth.FirebaseUser> userByAuth = new Dictionary<string, Firebase.Auth.FirebaseUser>();
	// Set the phone authentication timeout to a minute.
	private uint phoneAuthTimeoutMs = 180 * 1000;
	// The verification id needed along with the sent code for phone authentication.
	private string phoneAuthVerificationId;
	// Options used to setup secondary authentication object.
	private Firebase.AppOptions otherAuthOptions = new Firebase.AppOptions {
		ApiKey = "",
		AppId = "",
		ProjectId = ""
	};
	private Firebase.Auth.ForceResendingToken resendtoken = null;

	const int kMaxLogSize = 16382;
	Firebase.DependencyStatus dependencyStatus = Firebase.DependencyStatus.UnavailableOther;

	public InputField parentEmailInput;
	public InputField phoneNumberInput;
	public InputField passwordInput;
	public InputField confirmpasswordInput;
	public GameObject Loading;

	public GameObject PhoneOTPVerifyUIObj;
	public InputField OTPVerifyInput;



	// When the app starts, check to make sure that we have
	// the required dependencies to use Firebase, and if not,
	// add them if possible.
	public virtual void Start() {
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
			if (PhoneOTPVerifyUIObj.activeSelf) {
				PhoneOTPVerifyUIObj.SetActive (false);
				parentEmailInput.text = "";
				phoneNumberInput.text = "";
				passwordInput.text = "";
			}
			else
				SceneManager.LoadScene ("Login");
		}
	}

	public void RegisterClick(){
		if (Constants.isInternetConnected ()) {
			if (Constants.IsValidEmailAddress (parentEmailInput.text) && !string.IsNullOrEmpty (phoneNumberInput.text) && phoneNumberInput.text.Length == 10 && !string.IsNullOrEmpty (phoneNumberInput.text) ) {
				if (!string.IsNullOrEmpty (passwordInput.text)) {				
					if (passwordInput.text == confirmpasswordInput.text) {
						CreateUserAsync ();
					}else
						ToastScript.instance.ToastShow("Enter a valid Confirm Password",4f);
				}else
					ToastScript.instance.ToastShow("Enter a valid Password",4f);
			}
			else
				ToastScript.instance.ToastShow("Invalid Email-Id/Phone Number",4f);
		}else
			ToastScript.instance.ToastShow ("No Internet Connection", 4f);
	}

	public void AlreayLoginClick(){
		SceneManager.LoadScene ("Login");
	}

	public Task CreateUserAsync() {
		Loading.SetActive (true);
		Debug.Log(String.Format("Attempting to create user {0}...", parentEmailInput.text));
		// This passes the current displayName through to HandleCreateUserAsync
		// so that it can be passed to UpdateUserProfile().  displayName will be
		// reset by AuthStateChanged() when the new user is created and signed in.
		return auth.CreateUserWithEmailAndPasswordAsync(parentEmailInput.text, passwordInput.text)
			.ContinueWith((task) => {
				return HandleCreateUserAsync(task);
			}).Unwrap();
	}

	public Task EmailVerficationAsync() {
		Debug.Log(String.Format("Attempting to Verify Email {0}...", parentEmailInput.text));

		// This passes the current displayName through to HandleCreateUserAsync
		// so that it can be passed to UpdateUserProfile().  displayName will be
		// reset by AuthStateChanged() when the new user is created and signed in.
		if (auth.CurrentUser != null) {
			Debug.Log (String.Format ("User Info: {0}  {1}", auth.CurrentUser.Email,
				auth.CurrentUser.ProviderId));
			Loading.SetActive (false);
			VerifyPhoneNumber ();
			return auth.CurrentUser.SendEmailVerificationAsync();
		}
		Loading.SetActive (false);
		// Nothing to update, so just return a completed Task.
		return Task.FromResult(0);
	}


	Task HandleCreateUserAsync(Task<Firebase.Auth.FirebaseUser> authTask,
		string newDisplayName = null) {
		if (authTask.IsCanceled) {
			Loading.SetActive (false);
			ToastScript.instance.ToastShow ("Cancel:",4f);
		} else if (authTask.IsFaulted) {
			Loading.SetActive (false);
			ToastScript.instance.ToastShow ("Contact info@lbf o acivate your login",4f);
		} else if (authTask.IsCompleted) {
			if (LogTaskCompletion(authTask, "User Creation")) {
				if (auth.CurrentUser != null) {
					//				parentEmailInput.text = "";
					//				phoneNumberInput.text = "";
					//				passwordInput.text = "";
					ToastScript.instance.ToastShow("Please Verifiy Your Email to login :)",4f);
					Debug.Log(String.Format("User Info: {0}  {1}", auth.CurrentUser.Email,
						auth.CurrentUser.ProviderId));
					return UpdateUserProfileAsync(phoneNumberInput.text);

				}else
					Loading.SetActive (false);
			}else
				Loading.SetActive (false);
		}else
			Loading.SetActive (false);

		// Nothing to update, so just return a completed Task.
		return Task.FromResult(0);
	}

	public Task UpdateUserProfileAsync(string newDisplayName = null) {
		if (auth.CurrentUser == null) {
			Debug.Log("Not signed in, unable to update user profile");
			return Task.FromResult(0);
		}
		Debug.Log("Updating user profile");
		return auth.CurrentUser.UpdateUserProfileAsync(new Firebase.Auth.UserProfile {
			DisplayName = newDisplayName,
		}).ContinueWith((task) => {
			return HandleUpdateUserProfile(task);
		}).Unwrap();
	}

	Task HandleUpdateUserProfile(Task authTask) {
		if (authTask.IsFaulted) {
			ToastScript.instance.ToastShow ("UpdateFault:",4f);
		} else if (authTask.IsCompleted) {
			return EmailVerficationAsync();
			Debug.Log (" Update display PhoneNo Verified: " + auth.CurrentUser.PhoneNumber);
		}else
			Loading.SetActive (false);

		// Nothing to update, so just return a completed Task.
		return Task.FromResult(0);
	}


	// Begin authentication with the phone number.
	public void VerifyPhoneNumber() {
		//		ToastScript.instance.ToastShow ("VerifyPhoneNumberEnter");
		var phoneAuthProvider = Firebase.Auth.PhoneAuthProvider.GetInstance(auth);
		phoneAuthProvider.VerifyPhoneNumber(phoneNumberInput.text, phoneAuthTimeoutMs, resendtoken,
			verificationCompleted: (cred) => {
				//				UpdateUserProfileAsync(phoneNumberInput.text);
				//				Debug.Log("Phone Auth, auto-verification completed");
				//				ToastScript.instance.ToastShow ("Phone Auth, auto-verification completed");
				//				auth.SignInWithCredentialAsync(cred).ContinueWith(HandleSigninResult);
			},
			verificationFailed: (error) => {
				Loading.SetActive (false);
				Debug.Log("Phone Auth, verification failed");
				ToastScript.instance.ToastShow ("Enter In-Valid Phone Number");
			},
			codeSent: (id, token) => {
				phoneAuthVerificationId = id;
				resendtoken = token;
				Debug.Log("Phone Auth, code sent And Verify Email");
				ToastScript.instance.ToastShow ("Phone Auth, code sent");
				Loading.SetActive (false);
				PhoneOTPVerifyUIObj.SetActive(true);
			},
			codeAutoRetrievalTimeOut: (id) => {
				Debug.Log("Phone Auth, auto-verification timed out");
				ToastScript.instance.ToastShow ("Phone Auth, auto-verification timed out");
				Loading.SetActive (false);
			});
	}

	void HandleSigninResult(Task<Firebase.Auth.FirebaseUser> authTask) {
		LogTaskCompletion(authTask, "Sign-in");
		Debug.Log("  Email Verified: " + auth.CurrentUser.IsEmailVerified);
		if (authTask.IsFaulted) {
			Loading.SetActive (false);
			ToastScript.instance.ToastShow (authTask.Exception.Message);
		} else if (authTask.IsCompleted) {
			Loading.SetActive (false);
			AlreayLoginClick();
			Debug.Log ("  PhoneNo Verified: " + auth.CurrentUser.PhoneNumber);
		}
	}

	public void VerifyClick(){
		if(!string.IsNullOrEmpty(OTPVerifyInput.text) && OTPVerifyInput.text.Length == 6 ){
			VerifyReceivedPhoneCode();
		}else
			ToastScript.instance.ToastShow ("Enter Valid OTP Number");

	}

	public void ResendClick(){
		VerifyPhoneNumber ();
	}

	// Sign in using phone number authentication using code input by the user.
	public void VerifyReceivedPhoneCode() {
		//		UpdateUserProfileAsync(phoneNumberInput.text);
		var phoneAuthProvider = Firebase.Auth.PhoneAuthProvider.GetInstance(auth);
		// receivedCode should have been input by the user.
		var cred = phoneAuthProvider.GetCredential(phoneAuthVerificationId, OTPVerifyInput.text);
		auth.SignInWithCredentialAsync(cred).ContinueWith(HandleSigninResult);
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
