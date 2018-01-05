using System;
using UnityEngine;

    public class SAGAndroidPlugin
    {
	    public delegate void ImageDataReceived(string message/*, byte[] imageData,string filepath*/);
		public ImageDataReceived imageDelegate;
        public delegate void VideoDataReceived(string videoPath);
        public VideoDataReceived videoDelegate;

#if (UNITY_ANDROID && !UNITY_EDITOR)
        AndroidJavaObject androidObject;
        AndroidJavaObject unityObject;
#endif

        private static SAGAndroidPlugin instance;

        public void handleImage(string param)
        {
            if (imageDelegate == null)
            {
                Debug.Log("You must assign imageDelegate first");
            }
            else
            {
#if (UNITY_ANDROID && !UNITY_EDITOR)
			imageDelegate(param /*, androidObject.Call<byte[]>("getPluginData"),androidObject.Call<string>("getPluginPathData")*/);
                cleanUp();
#endif
            }

        }

	public void handleVideo(string param)
	{
		if (imageDelegate == null)
		{
			Debug.Log("You must assign imageDelegate first");
		}
		else
		{
			#if (UNITY_ANDROID && !UNITY_EDITOR)
			videoDelegate(param);
			cleanUp();
			#endif
		}

	}

        public static SAGAndroidPlugin getInstance()
        {

            if (instance == null)
            {

                instance = new SAGAndroidPlugin();

            }

            return instance;



        }

        private SAGAndroidPlugin()
        {

#if (UNITY_ANDROID && !UNITY_EDITOR)

            AndroidJavaClass unityClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            unityObject = unityClass.GetStatic<AndroidJavaObject>("currentActivity");

		AndroidJavaClass androidClass = new AndroidJavaClass("com.sag.unityandroidgalleryplugin.UnityBinder");
            androidObject = androidClass.GetStatic<AndroidJavaObject>("instance");
#endif

        }

        public void openCamera()
        {
#if (UNITY_ANDROID && !UNITY_EDITOR)
            androidObject.Call("launchCamera", unityObject);
#endif
        }

		public void openGallery(string resType)
        {
#if (UNITY_ANDROID && !UNITY_EDITOR)
		androidObject.Call("launchGallery",resType,unityObject);
#endif
        }



        public void cleanUp()
        {
#if (UNITY_ANDROID && !UNITY_EDITOR)
            androidObject.Call("cleanUp");
#endif
        }

    }
    