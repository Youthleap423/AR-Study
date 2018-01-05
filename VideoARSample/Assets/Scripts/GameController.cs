using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Vuforia;

public class GameController : MonoBehaviour {

	public GameObject[] Targets;
	public static int CurrentImageTarget;

	// Use this for initialization
	void Start () {
		CurrentImageTarget = 0;
		for (int i = 0; i < Targets.Length; i++) {
			string FileName = Targets[i].GetComponent<VideoTrackableEventHandler>().fileName;
			print ("Filename:"+FileName);
			if (File.Exists (Application.persistentDataPath + "/" + FileName)) {
				Targets [i].SetActive (true);
			} else {
				Targets [i].SetActive (false);
			}
		}
	}
	
	// Update is called once per frame
	void Update () {
		
	}


	public void StopClick(){
		for(int i =0;i<Targets.Length;i++){
			if (CurrentImageTarget == i+1) {
				Targets [i].GetComponent<VideoTrackableEventHandler> ().IsFixed = true;
			}
		}
	}

	public void PlayFullScreen(){
		for(int i =0;i<Targets.Length;i++){
			if (CurrentImageTarget == i+1) {
				VideoPlaybackBehaviour video = Targets [i].GetComponentInChildren<VideoPlaybackBehaviour>();
				if (video != null && video.VideoPlayer != null)
				{
//						VideoPlayerHelper.MediaState state = video.VideoPlayer.GetStatus();
//						if (state == VideoPlayerHelper.MediaState.PAUSED ||
//							state == VideoPlayerHelper.MediaState.READY ||
//							state == VideoPlayerHelper.MediaState.STOPPED)
//						{
//							// Pause other videos before playing this one
//							PauseOtherVideos(video);
//							video.VideoPlayer.Pause();
//							// Play this video on texture where it left off
//							video.VideoPlayer.Play(true, video.VideoPlayer.GetCurrentPosition());
//						}
//						else if (state == VideoPlayerHelper.MediaState.REACHED_END)
//						{
//							// Pause other videos before playing this one
//							PauseOtherVideos(video);
//							video.VideoPlayer.Pause();
//							// Play this video from the beginning
//							video.VideoPlayer.Play(true, 0);
//						}

					// Pause the video if it is currently playing
					video.VideoPlayer.Pause();

					// Seek the video to the beginning();
					video.VideoPlayer.SeekTo(0.0f);

					// Display the busy icon
					video.ShowBusyIcon();
					StartCoroutine( PlayFullscreenVideoAtEndOfFrame(video) );
					// Play the video full screen
				}
			}
		}
	}


	public static IEnumerator PlayFullscreenVideoAtEndOfFrame(VideoPlaybackBehaviour video)
	{
		#if !UNITY_EDITOR
		#if UNITY_ANDROID
		Screen.orientation = ScreenOrientation.LandscapeLeft;
		#else // iOS or UWP
		Screen.orientation = ScreenOrientation.AutoRotation;
		Screen.autorotateToPortrait = true;
		Screen.autorotateToPortraitUpsideDown = true;
		Screen.autorotateToLandscapeLeft = true;
		Screen.autorotateToLandscapeRight = true;
		#endif
		#endif //!UNITY_EDITOR

		yield return new WaitForEndOfFrame ();

		// we wait a bit to allow the ScreenOrientation.AutoRotation to become effective
		yield return new WaitForSeconds (0.3f);

		video.VideoPlayer.Play(true, 0);

		// We call WaitForEndOfFrame twice, so to ensure that
		// we intercept the time when the fullscreen video player stops.
		yield return new WaitForEndOfFrame();
		yield return new WaitForEndOfFrame();

		// When we reach this point, we know that the fullscreen player terminated.
		Debug.Log("Fullscreen playback exited.");

		// We restore the Play icon
		video.ShowPlayIcon();

		#if !UNITY_EDITOR
		// We now restore the Portrait orientation
		// as the sample UI requires so.
		Screen.autorotateToPortrait = true;
		Screen.autorotateToLandscapeLeft = false;
		Screen.autorotateToLandscapeRight = false;
		Screen.autorotateToPortraitUpsideDown = false;

		// We need to act in 2 steps, i.e. first we change to landscape 
		// and then to Portrait; this ensures that Vuforia can acknowlegde an orientation change.
		// First we set it temporarily to landscape
		Screen.orientation = ScreenOrientation.LandscapeLeft;

		// We wait for about half a second to be sure the 
		// screen orientation has switched to landscape
		yield return new WaitForSeconds(0.7f);

		// Finally we set to Portrait
		Screen.orientation = ScreenOrientation.Portrait;
		#endif //!UNITY_EDITOR
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
