using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonSetup : MonoBehaviour {

	public Button beeButton;
	public Button graphButton;

	// Use this for initialization
	void Start () {
		SetupButtons ();
	}
	
	// Update is called once per frame
	private void SetupButtons(){
		if (EventManager.instance) {
			beeButton.onClick.AddListener (EventManager.instance.SwitchToSetupForGame);
			graphButton.onClick.AddListener (EventManager.instance.SwitchToSetupForGraphs);
		} else {
			Debug.Log ("EventManager not yet active");
			Invoke ("SetupButtons", 1f);
		}
		
	}
}
