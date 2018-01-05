using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class activateRegister : MonoBehaviour {
	public Button btn;
	public Toggle toggle;
	// Use this for initialization
	void Start () {
		toggle.onValueChanged.AddListener((value) =>
			{
				activateReg(value);
			});//Do this in Start() for example
		
	}
	public void activateReg(bool value){
		if(value)
		{
			btn.interactable = true;
		}else {
			btn.interactable = false;
		}
	}

	public void openPrivacy(){
		Application.OpenURL("https://docs.google.com/document/d/1tT2cj-F1an0rrxZLA3yzkdBJoQDe4NPGD8K_YCGbOP0/edit?usp=sharing");
	}
	// Update is called once per frame
	void Update () {
		
	}
}
