using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using UnityEngine.SceneManagement;
public class DownloadVideoFromUrl : MonoBehaviour {


	public GameObject DownloadBar;
	public UnityEngine.UI.Image ProgressBar;
	public string[] url;
	public Text ratioTxt;
	public Text DownloadingTxt;
	//	public Text downloadstatustxt;
	public Text FilenameTxt;
	string JasonFileName ;
	DirectoryInfo dir;
	FileInfo[] info;
	int CurrentUrlNo;
	public GameObject panel1;
	public static string currentItemPath;
	public static string IOSpath;
	// Use this for initialization
	void Start () {
		Screen.sleepTimeout = SleepTimeout.NeverSleep;
		#if UNITY_IPHONE
		IOSpath = GetiPhoneDocumentsPath();
		#else
		Directory.CreateDirectory (Application.persistentDataPath + "/");
		#endif
		Download ();
	}


	void Update(){
	}

	string GetiPhoneDocumentsPath()
	{
		string path = Application.persistentDataPath + "/" + "localPet";
		DirectoryInfo dirInf = new DirectoryInfo(path); 
		if(!dirInf.Exists){
			Debug.Log ("Creating subdirectory");
			dirInf.Create(); 
		}

		if(dirInf.Exists){
			Debug.Log ("Its Exists subdirectory");
		}

		return path;
	}

	public void Download () {
		if (!string.IsNullOrEmpty (gameObject.GetComponent<DownloadVideoFromUrl>().url[CurrentUrlNo])) {
			StartCoroutine (DownloadFromServer ());
		} else {
			Debug.Log ("Plz Enter URL");
		}
	}

	IEnumerator DownloadFromServer() 
	{
		string FileName = gameObject.GetComponent<DownloadVideoFromUrl>().url[CurrentUrlNo].Substring (gameObject.GetComponent<DownloadVideoFromUrl>().url[CurrentUrlNo].LastIndexOf ('/') + 1);
		FilenameTxt.text = FileName;
		WWW www = new WWW (gameObject.GetComponent<DownloadVideoFromUrl>().url[CurrentUrlNo].Trim());	
		StartCoroutine (ShowProgress (www));
		Debug.Log ("URL:"+gameObject.GetComponent<DownloadVideoFromUrl>().url[CurrentUrlNo].Trim());
		yield return www;
		Debug.Log ("Byte:"+ www.bytes.Length);
		//Debug.Log("8888888888888888888888888888888888888888888"+mtrack.TrackableName);

		if (www.bytes.Length > 1000 && www.error == "" ) {
			byte[] Bytes = www.bytes;
			#if UNITY_IPHONE
			File.WriteAllBytes (IOSpath + "/" + FileName, Bytes);
			#else
			File.WriteAllBytes (Application.persistentDataPath + "/" + FileName, Bytes);
			#endif
			//	Camera.main.GetComponent<GameController> ().Targets [CurrentUrlNo].SetActive(true);
			if (CurrentUrlNo < gameObject.GetComponent<DownloadVideoFromUrl>().url.Length-1) {
				CurrentUrlNo++;
				Download ();
				yield break;
			}
			yield return new WaitForSeconds (2f);
			gameObject.SetActive (false);
			panel1.SetActive (true);

			string currentTarget = PlayerPrefs.GetString("currentDownloadTarget");
			SceneManager.LoadScene(currentTarget);
		} else {
			DownloadBar.SetActive (false);
			Debug.Log ("error in url:"+www.error);
		}
	}

	private IEnumerator ShowProgress(WWW www) {
		StartCoroutine (Downloding());
		float tem = 0;
		while (!www.isDone) {
			DownloadBar.SetActive (true);
			tem = (www.progress * 100);
			//				Debug.Log (tem);
			float ratio = (www.progress);
			ProgressBar.gameObject.SetActive (true);
			ProgressBar.rectTransform.localScale = new Vector3 (ratio, 1, 1);
			ratioTxt.text = ((int)tem).ToString () + "%";
			yield return new WaitForSeconds (.1f);
		}
		if (www.error == "") {
			ProgressBar.rectTransform.localScale = new Vector3 (1, 1, 1);
			ratioTxt.text = "100%";
			//			if (CurrentUrlNo < url.Length-1) {
			//				CurrentUrlNo++;
			//				Download ();
			//				yield break;
			//			}
			if (CurrentUrlNo == gameObject.GetComponent<DownloadVideoFromUrl>().url.Length) {
				FilenameTxt.text = "";
				DownloadingTxt.text = "Downloaded Successfully.";
			}
			//			yield return new WaitForSeconds (2f);
			//			gameObject.SetActive (false);
		}
		//		ProgressBar.gameObject.SetActive (false);
		//		DownloadBar.gameObject.SetActive (false);
		//		ratioTxt.text = "";
		//		url.text = "";
	}

	IEnumerator Downloding (){
		DownloadingTxt.gameObject.SetActive(true);
		DownloadingTxt.text = "Downloading :  "+(CurrentUrlNo+1).ToString()+"/"+gameObject.GetComponent<DownloadVideoFromUrl>().url.Length;
		yield return new WaitForSeconds (0.5f);
		DownloadingTxt.text = "Downloading :- "+(CurrentUrlNo+1).ToString()+"/"+gameObject.GetComponent<DownloadVideoFromUrl>().url.Length;
		yield return new WaitForSeconds (0.5f);
		DownloadingTxt.text = "Downloading :-) "+(CurrentUrlNo+1).ToString()+"/"+gameObject.GetComponent<DownloadVideoFromUrl>().url.Length;
		yield return new WaitForSeconds (0.5f);
		StartCoroutine (Downloding ());
	}

	//	public void myDownloads () {
	//		downloadstatustxt.gameObject.SetActive (false);
	//		ProgressBar.gameObject.SetActive (false);
	//		DownloadBar.gameObject.SetActive (false);
	//		ratioTxt.text = "";
	//		url = "";
	//
	//		Debug.Log ("IOSPath:"+IOSpath);
	//		#if UNITY_IPHONE
	//		dir = new DirectoryInfo (IOSpath+"/");
	//		#else
	//		dir = new DirectoryInfo (Application.persistentDataPath + "/Downloads/");
	//		#endif
	//
	//		info = dir.GetFiles("*.*");
	//		Debug.Log ("No of files in Directory:"+info.Length);
	//		foreach (FileInfo f in info) {
	//			Debug.Log ("file Name in Directory:"+f.FullName);
	//			GameObject item = Instantiate (itemPrfb);
	//			item.gameObject.transform.GetChild (0).GetComponent<Text> ().text = f.Name;
	//			item.transform.parent = DownloadItemParent.transform;
	//			item.transform.localScale = Vector3.one;
	//			Debug.Log(f.Name);
	//		}
	//	}


	//	public void CancelYesClick(){
	//		StopAllCoroutines ();
	//		#if UNITY_IPHONE 
	//		#if !UNITY_EDITOR_WIN
	//		System.IO.File.Delete("/private" + DownloadFromUrl.IOSpath+"/"+FilenameTxt.text);
	//		#else
	//		System.IO.File.Delete(DownloadFromUrl.IOSpath+"/"+FilenameTxt.text);
	//		#endif
	//		#else
	//		File.Delete (Application.persistentDataPath + "/Downloads/" + FilenameTxt.text);
	//		#endif
	//		ProgressBar.gameObject.SetActive (false);
	//		DownloadBar.gameObject.SetActive (false);
	//		ratioTxt.text = "";
	//		url = "";
	//		Time.timeScale = 1;
	//	}


}
