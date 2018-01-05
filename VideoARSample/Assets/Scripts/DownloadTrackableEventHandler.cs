using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vuforia;
using System.IO;
using UnityEngine.SceneManagement;

public class DownloadTrackableEventHandler : MonoBehaviour, ITrackableEventHandler
{
	#region PRIVATE_MEMBERS
	private TrackableBehaviour mTrackableBehaviour;
	private bool mHasBeenFound = false;
	private bool mLostTracking;
	private float mSecondsSinceLost;
	private GameObject currentTracked;
	public GameObject Loading;
	public bool isFileLeftToDownload;
	#endregion // PRIVATE_MEMBERS

	public GameObject DownloadPanel;
	#region MONOBEHAVIOUR_METHODS
	void Start()
	{
		isFileLeftToDownload = false;
		mTrackableBehaviour = GetComponent<TrackableBehaviour>();
		if (mTrackableBehaviour)
		{
			mTrackableBehaviour.RegisterTrackableEventHandler(this);
		}

		OnTrackingLost();
	}

	void Update()
	{

	}

	#endregion //MONOBEHAVIOUR_METHODS


	#region PUBLIC_METHODS
	/// <summary>
	/// Implementation of the ITrackableEventHandler function called when the
	/// tracking state changes.
	/// </summary>
	public void OnTrackableStateChanged(
		TrackableBehaviour.Status previousStatus,
		TrackableBehaviour.Status newStatus)
	{
		if (newStatus == TrackableBehaviour.Status.DETECTED ||
			newStatus == TrackableBehaviour.Status.TRACKED ||
			newStatus == TrackableBehaviour.Status.EXTENDED_TRACKED)
		{
			OnTrackingFound();
		}
		else
		{
			OnTrackingLost();
		}
	}
	#endregion //PUBLIC_METHODS


	#region PRIVATE_METHODS
	private void OnTrackingFound()
	{
		currentTracked = GameObject.Find(mTrackableBehaviour.TrackableName.ToString());
		PlayerPrefs.SetString("currentDownloadTarget",mTrackableBehaviour.TrackableName);
		Debug.Log("********************************"+mTrackableBehaviour.TrackableName);

		for (int i = 0; i < currentTracked.GetComponent<DownloadTrackableEventHandler>().DownloadPanel.GetComponent<DownloadVideoFromUrl> ().url.Length; i++) {
			string url =  currentTracked.GetComponent<DownloadTrackableEventHandler>().DownloadPanel.GetComponent<DownloadVideoFromUrl> ().url[i];
			string FileName = url.Substring(url.LastIndexOf ('/') + 1);

			Debug.Log("********^^^^^^^^^^^******"+Application.persistentDataPath + "/" + FileName);

			if (!File.Exists (Application.persistentDataPath + "/" + FileName)) {
				Debug.Log("*****%%%%%*****");
				isFileLeftToDownload = true;
				currentTracked.GetComponent<DownloadTrackableEventHandler>().DownloadPanel.SetActive (true);
				break;
			}
		}
		if(isFileLeftToDownload == false){
			Loading.SetActive(true);
			SceneManager.LoadScene(mTrackableBehaviour.TrackableName);
		}
		mHasBeenFound = true;
		mLostTracking = false;
	}

	private void OnTrackingLost()
	{
		Debug.Log("Trackable " + mTrackableBehaviour.TrackableName + " lost");
		mLostTracking = true;
		mSecondsSinceLost = 0;
	}

	#endregion //PRIVATE_METHODS
}
