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

	private bool toGame;
	private float anticipationDuration;
	private float setupStart;
	public bool setup = false;
	public bool playVideo = false;

	public float biofeedbackDuration = 30f;
	private float biofeedbackStart;
	public bool biofeedback = false;

	private int numberOfTrials = 3;
	private int currentTrial = 0;

	public bool end = false;
	public float baselineAverage = 0.0f;
	public float feedbackAverage = 0.0f;
	public int score = 0;

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
			float now = Time.time;
			Debug.ClearDeveloperConsole ();
			Debug.Log ("Console cleared");
			if (now - biofeedbackStart >= biofeedbackDuration) {
				biofeedback = false;
				currentTrial++;
				end = true;
				SwitchToEnd ();
			}
		}
	}

	public void SwitchToSetupForGame(){
		toGame = true;
		setup = true;
		SceneManager.LoadScene ("setup");
		setupStart = Time.time;

	}

	public void SwitchToSetupForGraphs(){
		toGame = false;
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
			SwitchToGame();
		} else {
			SwitchToGraphs();
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

	public void SwitchToStart(){
		SceneManager.LoadScene ("start");
		SkinProcessor.instance.Restart ();
	}

	private void SwitchToEnd(){
		SceneManager.LoadScene ("end");
	}

}