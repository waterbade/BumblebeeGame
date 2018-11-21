using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Video;
public class EventManager : MonoBehaviour {

	public static EventManager instance;
	private static EventManager eventManager;

	private bool toGame = false;
	private bool toGraphs = false;
	private float anticipationDuration;
	private float setupStart;
	[HideInInspector] public bool setup = false;
	[HideInInspector] public bool playVideo = false;

	public float biofeedbackDuration = 120f;
	private float biofeedbackStart;
	[HideInInspector] public bool biofeedback = false;

	private int currentTrial = 0;

	[HideInInspector] public bool end = false;
	[HideInInspector] public float baselineAverage = 0.0f;
	[HideInInspector] public float feedbackAverage = 0.0f;
	[HideInInspector] public int score = 0;

	void Awake()
	{
		//SingletonPattern for EventManager
		if (instance == null)
			instance = this;
		else if(instance != this)
			Destroy (gameObject);
		DontDestroyOnLoad (this.gameObject);
	}

	void Start (){
		SetAnticipationDuration ();
	}

	void Update() {
		if (setup) {
			float now = Time.time;
			if (now - setupStart >= anticipationDuration) {
				playVideo = true;
				Debug.Log ("Play video");
				setup = false;
			}
		}

		if (biofeedback) {
			SkinProcessor.instance.SetupSkinZones ();
			float now = Time.time;
			Debug.ClearDeveloperConsole ();
			//Debug.Log ("Console cleared");
			if (now - biofeedbackStart >= biofeedbackDuration) {
				biofeedback = false;
				currentTrial++;
				end = true;
				SwitchToEnd ();
			}
		}
	}

	public void SwitchToSetupForGame(){
		AudioManager.instance.StopMusic ();
		toGame = true;
		toGraphs = false;
		setup = true;
		SceneManager.LoadScene ("setup");
		setupStart = Time.time;
	}

	public void SwitchToSetupForGraphs(){
		AudioManager.instance.StopMusic ();
		toGame = false;
		toGraphs = true;
		setup = true;
		SceneManager.LoadScene ("setup");
		setupStart = Time.time;
	}
		
	public void SwitchToSetupForControl(){
		AudioManager.instance.StopMusic ();
		toGame = false;
		toGraphs = false;
		setup = true;
		SceneManager.LoadScene ("setup");
		setupStart = Time.time;
	}

	void SetAnticipationDuration (){
		anticipationDuration = Random.Range (15.0f, 20.0f); 
	}

	public float GetGameSessionDuration(){
		return biofeedbackDuration;
	}

	public void StartBiofeedback(){
		if (toGame) {
			SwitchToGame ();
		} else if (toGraphs) {
			SwitchToGraphs ();
		} else {
			SwitchToControl ();
		}
		biofeedback = true;
		biofeedbackStart = Time.time;
	}

	private void SwitchToGame(){
		SceneManager.LoadScene ("game");
	}

	private void SwitchToGraphs(){
		SceneManager.LoadScene ("graphs");
	}

	private void SwitchToControl(){
		SceneManager.LoadScene ("control");
	}

	public void SwitchToStart(){
		SceneManager.LoadScene ("start");
		SkinProcessor.instance.Restart ();

		setup = false;
		playVideo = false;
		biofeedback = false;
		end = false;
		baselineAverage = 0.0f;
		feedbackAverage = 0.0f;
		score = 0;
	}

	private void SwitchToEnd(){
		BitalinoObject.instance.GetComponentInChildren<BitalinoReader> ().OnApplicationQuit ();
		SceneManager.LoadScene ("end");
	}

}