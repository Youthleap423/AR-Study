using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vuforia;

public class VideoTrackableEventHandler : MonoBehaviour, ITrackableEventHandler
{
	#region PRIVATE_MEMBERS
	private TrackableBehaviour mTrackableBehaviour;
	private bool mHasBeenFound = false;
	private bool mLostTracking;
	private float mSecondsSinceLost;
	#endregion // PRIVATE_MEMBERS

	public GameObject classroom;
	public GameObject bedroom;
	public string fileName;
	public GameObject StopBtn;
	public GameObject fullScreenBtn;
	public bool IsFixed;
//	VideoPlaybackBehaviour video;
	private GameObject videoObj;
	private GameObject currentEnvObj;
	#region MONOBEHAVIOUR_METHODS
	void Start()
	{
		DetachTarget.track = true;

		mTrackableBehaviour = GetComponent<TrackableBehaviour>();
		if (mTrackableBehaviour)
		{
			mTrackableBehaviour.RegisterTrackableEventHandler(this);
		}

		OnTrackingLost();

		videoObj = transform.Find("Video").gameObject;
		//
		//		//PlayerPrefs.DeleteAll ();

		string currentEnv = PlayerPrefs.GetString("env");
		Debug.Log ("currentEnv" + currentEnv);
		if (currentEnv == "") {
			PlayerPrefs.SetString ("env", "bedroom");
			currentEnv = PlayerPrefs.GetString("env");
			Debug.Log ("asjdlaskjdlka"+currentEnv);
		}
		if(currentEnv == "bedroom"){
			bedroom.SetActive(true);
			classroom.SetActive(false);
			videoObj.transform.parent = bedroom.transform;	
		}
		else if(currentEnv == "classroom")
		{
			bedroom.SetActive(false);
			classroom.SetActive(true);
			videoObj.transform.parent = classroom.transform;	
		}
//		currentEnvObj = GameObject.Find(currentEnv);
//		videoObj.transform.parent = currentEnvObj.transform;	
		//
		//		if(currentEnv == "bedroom"){
		////			bedroom.GetComponentInChildren<VideoPlaybackBehaviour> ().fileName = fileName;
		//			bedroom.SetActive(true);
		//			classroom.SetActive(false);
		//			video = bedroom.GetComponentInChildren<VideoPlaybackBehaviour> ();
		//		}
		//		else if(currentEnv == "classroom")
		//		{
		////			classroom.GetComponentInChildren<VideoPlaybackBehaviour> ().fileName = fileName;
		//			bedroom.SetActive(false);
		//			classroom.SetActive(true);
		//			video = classroom.GetComponentInChildren<VideoPlaybackBehaviour> ();
		//		}
		//

	}

	void Update()
	{
		// Pause the video if tracking is lost for more than two seconds
		if (mHasBeenFound && mLostTracking)
		{
			if (mSecondsSinceLost > 2.0f)
			{
				VideoPlaybackBehaviour video = GetComponentInChildren<VideoPlaybackBehaviour>();
				if (video != null &&
					video.CurrentState == VideoPlayerHelper.MediaState.PLAYING)
				{
					video.VideoPlayer.Pause();
				}

				mLostTracking = false;
			}

			mSecondsSinceLost += Time.deltaTime;
		}
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


	void OnTrackingFound()
	{
		/* for (int i = 0; i < Camera.main.GetComponent<GameController> ().Targets.Length; i++) {
			Camera.main.GetComponent<GameController> ().Targets [i].GetComponent<VideoTrackableEventHandler> ().IsFixed = false;
			Renderer[] rendererComponentstarget = Camera.main.GetComponent<GameController> ().Targets[i].GetComponentsInChildren<Renderer>();
			Collider[] colliderComponentstarget = Camera.main.GetComponent<GameController> ().Targets[i].GetComponentsInChildren<Collider>();
			// Disable rendering:
			foreach (Renderer component in rendererComponentstarget)
			{
				component.enabled = false;
			}

			// Disable colliders:
			foreach (Collider component in colliderComponentstarget)
			{
				component.enabled = false;
			}
		}
		GameController.CurrentImageTarget = int.Parse(gameObject.name.Substring(gameObject.name.Length-1));
*/
        Renderer[] rendererComponents = GetComponentsInChildren<Renderer>();
		Collider[] colliderComponents = GetComponentsInChildren<Collider>(); 

		// Enable rendering:
		foreach (Renderer component in rendererComponents)
		{
			component.enabled = true;
		}

		// Enable colliders:
		foreach (Collider component in colliderComponents)
		{
			component.enabled = true;
		}
		StopBtn.SetActive (true);
		fullScreenBtn.SetActive (true);
		Debug.Log("Trackable " + mTrackableBehaviour.TrackableName + " found");

		// Optionally play the video automatically when the target is found

		VideoPlaybackBehaviour video = GetComponentInChildren<VideoPlaybackBehaviour>();
		if (video != null && video.AutoPlay)
		{
			if (video.VideoPlayer.IsPlayableOnTexture())
			{
				VideoPlayerHelper.MediaState state = video.VideoPlayer.GetStatus();
				if (state == VideoPlayerHelper.MediaState.PAUSED ||
					state == VideoPlayerHelper.MediaState.READY ||
					state == VideoPlayerHelper.MediaState.STOPPED)
				{
					// Pause other videos before playing this one
					PauseOtherVideos(video);

					// Play this video on texture where it left off
					video.VideoPlayer.Play(false, video.VideoPlayer.GetCurrentPosition());
				}
				else if (state == VideoPlayerHelper.MediaState.REACHED_END)
				{
					// Pause other videos before playing this one
					PauseOtherVideos(video);

					// Play this video from the beginning
					video.VideoPlayer.Play(false, 0);
				}
			}
		}

		mHasBeenFound = true;
		mLostTracking = false;
	}

	 void OnTrackingLost()
	{
		//GameController.CurrentImageTarget = 0;
		if(fullScreenBtn != null)
		fullScreenBtn.SetActive (false);
		if(StopBtn != null)
		StopBtn.SetActive (false);
		Debug.Log("Track value in VideoBehavior"+DetachTarget.track);

		if(DetachTarget.track){
		Renderer[] rendererComponents = GetComponentsInChildren<Renderer>();
		Collider[] colliderComponents = GetComponentsInChildren<Collider>();

		// Disable rendering:
		foreach (Renderer component in rendererComponents)
		{
			component.enabled = false;
		}

		// Disable colliders:
		foreach (Collider component in colliderComponents)
		{
			component.enabled = false;
		}

		Debug.Log("Trackable " + mTrackableBehaviour.TrackableName + " lost");


			VideoPlaybackBehaviour video = GetComponentInChildren<VideoPlaybackBehaviour>();
			if (video != null && video.VideoPlayer != null )
			{
					VideoPlayerHelper.MediaState state = video.VideoPlayer.GetStatus();
					if (state == VideoPlayerHelper.MediaState.PLAYING || 
						state == VideoPlayerHelper.MediaState.PAUSED )
					{
						video.VideoPlayer.Stop();
					}
			}

		mLostTracking = true;
		mSecondsSinceLost = 0;
		}

	}

	// Pause all videos except this one
	 void PauseOtherVideos(VideoPlaybackBehaviour currentVideo)
	{
		VideoPlaybackBehaviour[] videos = (VideoPlaybackBehaviour[])
			FindObjectsOfType(typeof(VideoPlaybackBehaviour));

		foreach (VideoPlaybackBehaviour video in videos)
		{
			if (video != currentVideo)
			{
				if (video.CurrentState == VideoPlayerHelper.MediaState.PLAYING)
				{
					video.VideoPlayer.Pause();
				}
			}
		}
	}
}

