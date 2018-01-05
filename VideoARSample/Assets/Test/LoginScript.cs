using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine;

public class LoginScript : MonoBehaviour {


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
	public InputField passwordInput;
	public GameObject Loading;
//	private string schoolLogin;
	public GameObject PhoneOTPVerifyUIObj;
	public InputField OTPVerifyInput;

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
			if (PhoneOTPVerifyUIObj.activeSelf) {
				PhoneOTPVerifyUIObj.SetActive (false);
				parentEmailInput.text = "";
				passwordInput.text = "";
			} else
				Application.Quit ();
		}

//		if(parentEmailInput.text.Length >= 10 && int.Parse(parentEmailInput.text.Substring(parentEmailInput.text.Length-10)) != null){
//			Debug.Log ("PhoneNumberValid"+parentEmailInput.text.Substring(parentEmailInput.text.Length-10));
//		}else
//			Debug.Log ("PhoneNumberNotValid");
	}

	public void SignUpClick(){
		SceneManager.LoadScene ("Registration");
	}

	public void ForgetPasswordClick(){
		SceneManager.LoadScene ("ForgetPassword");
	}

	public void LoginClick(){
		sentcode = false;
		if (Constants.isInternetConnected ()) {
			if (parentEmailInput.text.Contains ("@")) {
				if (!string.IsNullOrEmpty (passwordInput.text))
					SigninAsync ();
				else
					ToastScript.instance.ToastShow ("Enter valid Password", 4f);
			} else if (parentEmailInput.text.Length >= 10 && int.Parse (parentEmailInput.text.Substring (parentEmailInput.text.Length - 10)) != null) {
				if (!string.IsNullOrEmpty (passwordInput.text))
					VerifyPhoneNumber ();
				else
					ToastScript.instance.ToastShow ("Enter valid Password", 4f);
			} else
				ToastScript.instance.ToastShow ("Enter valid Email-Id/Phone Number", 4f);
		}else
			ToastScript.instance.ToastShow ("No Internet Connection", 4f);
		
	}

	public Task SigninAsync() {
		Loading.SetActive (true);
		Debug.Log(String.Format("Attempting to sign in as {0}...", parentEmailInput.text));
		return auth.SignInWithEmailAndPasswordAsync(parentEmailInput.text, passwordInput.text)
			.ContinueWith(HandleSigninResult);
	}

	void HandleSigninResult(Task<Firebase.Auth.FirebaseUser> authTask) {
//		LogTaskCompletion (authTask, "Sign-in");
		if (authTask.IsFaulted) {
			Loading.SetActive (false);
			ToastScript.instance.ToastShow("Enter In-valid Email-Id",4f);
		} else if (authTask.IsCompleted) {
		Loading.SetActive (false);
		if (auth.CurrentUser.IsEmailVerified) {
				ToastScript.instance.ToastShow ("You are Logged in");
			Constants.ParentUDID = auth.CurrentUser.UserId;
			Constants.ParentEmail = auth.CurrentUser.Email;
			Constants.ParentPassword = passwordInput.text;

				if(PlayerPrefs.GetString("SchoolLogin") == "false")
				{
			SceneManager.LoadScene ("ProfileView");
				} else {
					SceneManager.LoadScene ("SchoolProfile");
									
				}
//			VerifyPhoneNumber() ;
		} else
			ToastScript.instance.ToastShow ("User Email Not Verified");
		Debug.Log ("  Email Verified: " + auth.CurrentUser.IsEmailVerified);
	}
	}


	// Begin authentication with the phone number.
	public void VerifyPhoneNumber() {
		var phoneAuthProvider = Firebase.Auth.PhoneAuthProvider.GetInstance(auth);
		phoneAuthProvider.VerifyPhoneNumber(parentEmailInput.text, phoneAuthTimeoutMs, resendtoken,
			verificationCompleted: (cred) => {
				Debug.Log("Phone Auth, auto-verification completed");
//				Constants.ParentUDID = auth.CurrentUser.UserId;
//				Constants.ParentEmail = auth.CurrentUser.Email;
//				Constants.ParentPassword = passwordInput.text;
				ToastScript.instance.ToastShow ("Login in progress...");
//				SceneManager.LoadScene ("ProfileView");
				if(!sentcode)
				auth.SignInWithCredentialAsync(cred).ContinueWith(HandleSigninPhoneResult);
			},
			verificationFailed: (error) => {
				Loading.SetActive (false);
				Debug.Log("Phone Auth, verification failed");
				ToastScript.instance.ToastShow ("Enter In-Valid Phone Number",4f);
			},
			codeSent: (id, token) => {
				phoneAuthVerificationId = id;
				resendtoken = token;
				ToastScript.instance.ToastShow ("Phone Auth, code sent");
				sentcode = true;
				Debug.Log("Phone Auth, code sent");
				Loading.SetActive (false);
				PhoneOTPVerifyUIObj.SetActive(true);
			},
			codeAutoRetrievalTimeOut: (id) => {
				Debug.Log("Phone Auth, auto-verification timed out");
				ToastScript.instance.ToastShow ("Phone Auth, auto-verification timed out");
				Loading.SetActive (false);
			});
	}

	void HandleSigninPhoneResult(Task<Firebase.Auth.FirebaseUser> authTask) {
		LogTaskCompletion(authTask, "Sign-in");
		Debug.Log("  Email Verified: " + auth.CurrentUser.IsEmailVerified);
		if (authTask.IsFaulted) {
			Loading.SetActive (false);
			ToastScript.instance.ToastShow ("Enter In-Valid Phone Number",4f);
		} else if (authTask.IsCompleted) {
			Loading.SetActive (false);
			Constants.ParentUDID = auth.CurrentUser.UserId;
			Constants.ParentEmail = parentEmailInput.text;
			Constants.ParentPassword = passwordInput.text;
			SceneManager.LoadScene ("ProfileView");
			Debug.Log ("  PhoneNo Verified: " + auth.CurrentUser.PhoneNumber);
		}
	}

	public void VerifyClick(){
		if(!string.IsNullOrEmpty(OTPVerifyInput.text) && OTPVerifyInput.text.Length == 6 ){
			VerifyReceivedPhoneCode();
		}else
			ToastScript.instance.ToastShow ("Enter Valid OTP Number");
		
	}


	// Sign in using phone number authentication using code input by the user.
	public void VerifyReceivedPhoneCode() {
		var phoneAuthProvider = Firebase.Auth.PhoneAuthProvider.GetInstance(auth);
		// receivedCode should have been input by the user.
		var cred = phoneAuthProvider.GetCredential(phoneAuthVerificationId, OTPVerifyInput.text);
		auth.SignInWithCredentialAsync(cred).ContinueWith(HandleSigninPhoneResult);
	}

	public void ResendClick(){
		VerifyPhoneNumber ();
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
		if(PlayerPrefs.GetInt(Constants.IsLogin) == 1){
			parentEmailInput.text = PlayerPrefs.GetString(Constants.IsLoginEmailorPhone);
			passwordInput.text = PlayerPrefs.GetString(Constants.IsLoginpassword);
			LoginClick();
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
