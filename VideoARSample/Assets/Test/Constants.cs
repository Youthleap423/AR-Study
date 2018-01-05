using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text.RegularExpressions;
using System.Net;
using System.IO;

public class Constants : MonoBehaviour {

	public static string ParentUDID = "8veVOhBPwROGLYuTQIi9yrkcGZ23";
	public static string ParentEmail = "";
	public static string ParentPassword = "";

	//-------------keys-----------------
	public const string firstName = "firstName";
	public const string lastName = "lastName";
	public const string userName = "userName";
	public const string childclass = "childclass";
	public const string imageUrl = "imageUrl";
	public const string pushkey = "id";


	//----------------Player Prefab keys-------------
	public const string IsLogin = "IsLogin";
	public const string IsLoginEmailorPhone = "IsLoginEmailorPhone";
	public const string IsLoginpassword = "IsLoginpassword";

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public static bool IsValidEmailAddress(string s)
	{
		var regex = new Regex(@"[a-z0-9!#$%&amp;'*+/=?^_`{|}~-]+(?:.[a-z0-9!#$%&amp;'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?");
		return regex.IsMatch(s);
	}

	public static bool IsValidPhoneNumber(string s)
	{
		var regex = new Regex(@"^\+?\d{0,2}\-?\d{4,5}\-?\d{5,6}");
		return regex.IsMatch(s);
	}

	public static bool isInternetConnected(){

//				string HtmlText = GetHtmlFromUri("http://google.com");
//				if(HtmlText == "")
//				{
//					//No connection
//					return false;
//				}
//				else if(!HtmlText.Contains("schema.org/WebPage"))
//				{
//					//Redirecting since the beginning of googles html contains that 
//					//phrase and it was not found
//		
//					return false;
//				}
//				else
//				{
//					//success
//		
//					return true;
//				}
//
//				if (Application.internetReachability == NetworkReachability.ReachableViaLocalAreaNetwork) {
//					return true;
//				}
//				else
//				{
//					//success
//					return false;
//				}

		bool isConnectedToInternet = false;
		if(Application.internetReachability == NetworkReachability.ReachableViaCarrierDataNetwork ||
			Application.internetReachability == NetworkReachability.ReachableViaLocalAreaNetwork)
		{
			isConnectedToInternet = true;
		}
		return isConnectedToInternet;

	}

	public static string GetHtmlFromUri(string resource)
	{
		string html = string.Empty;
		HttpWebRequest req = (HttpWebRequest)WebRequest.Create(resource);
		try
		{
			using (HttpWebResponse resp = (HttpWebResponse)req.GetResponse())
			{
				bool isSuccess = (int)resp.StatusCode < 299 && (int)resp.StatusCode >= 200;
				if (isSuccess)
				{
					using (StreamReader reader = new StreamReader(resp.GetResponseStream()))
					{
						//We are limiting the array to 80 so we don't have
						//to parse the entire html document feel free to 
						//adjust (probably stay under 300)
						char[] cs = new char[80];
						reader.Read(cs, 0, cs.Length);
						foreach(char ch in cs)
						{
							html +=ch;
						}
					}
				}
			}
		}
		catch
		{
			return "";
		}
		return html;
	}

}
