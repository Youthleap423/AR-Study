using UnityEngine;
using System.Collections;
public class CallBack : MonoBehaviour {

    

    // Use this for initialization
    void Awake () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void Camera_PickDone(string param)
    {
		SAGAndroidPlugin.getInstance().handleImage(param);
    }

    void Gallery_PickDone(string param)
    {

		SAGAndroidPlugin.getInstance().handleImage(param);
    }

	void Gallery_Video_PickDone(string param)
	{

		SAGAndroidPlugin.getInstance().handleVideo(param);
	}

   
}
