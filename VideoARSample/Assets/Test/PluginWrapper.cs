using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PluginWrapper : MonoBehaviour {

	enum ResourceType{
		Default,
		Image,
		Video
	}

	public  string filepath;
	public Text Loading;

	void Start () {
		filepath = "";
		print (ResourceType.Image.ToString());
		SAGAndroidPlugin.getInstance().imageDelegate = imageHandle;
		SAGAndroidPlugin.getInstance().videoDelegate = videoHandle;
	}

	void imageHandle(/*string message, byte[] data,*/string imagepath)
	{

//		JSONArray jsa = (JSONArray)JSON.Parse(message);
//		JSONNode jsn = jsa[0];
//		int w = jsn["width"].AsInt;
//		int h = jsn["height"].AsInt;
//
//		Texture2D xx = new Texture2D(w, h, TextureFormat.BGRA32, false);
//		xx.LoadImage(data);
//		//        Sprite newSprite = Sprite.Create(xx as Texture2D, new Rect(0f, 0f, xx.width, xx.height), Vector2.zero);
//		img.texture = xx;
		filepath = imagepath;
		Loading.text = filepath;
//		DownloadFromUrl.currentItemPath = filepath.Substring(filepath.LastIndexOf('/')+1);
//		testtxt.text = filepath;
//		SceneManager.LoadScene ("PlaySelection");
//		Camera.main.GetComponent<DownloadFromUrl>().viewModePanel.SetActive(true);
	}

	void videoHandle(string videoPath)
	{
		filepath = videoPath;
//		DownloadFromUrl.currentItemPath = filepath.Substring(filepath.LastIndexOf('/')+1);
//		testtxt.text = filepath;
//		SceneManager.LoadScene ("PlaySelection");
//		Camera.main.GetComponent<DownloadFromUrl>().viewModePanel.SetActive(true);
	}
	
	// Update is called once per frame
	void Update () {

	}

	public void GalleryImagesOpen(){
		SAGAndroidPlugin.getInstance().openGallery(ResourceType.Image.ToString());
	}

	public void GalleryVideosOpen(){
		SAGAndroidPlugin.getInstance().openGallery(ResourceType.Video.ToString());
	}

	public void BackClick(){
		gameObject.SetActive (false);
	}

}
